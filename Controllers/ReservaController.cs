using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Data.SqlClient;
using System.Globalization;
using System.Security.Claims;
using GestaoReservas.Models;

[Authorize(AuthenticationSchemes = "ClienteScheme")]
public class ReservaController : Controller
{
    private readonly IReservaRepository repository;
    private readonly IMesaRepository mesaRepository;
    private readonly IAmbienteRepository ambienteRepository;
    private readonly IClienteRepository clienteRepository;

    private static readonly string[] NomesAmbientesMapa = ["Interno", "Externo", "Sacada"];

    // Construtor: recebe as dependências por injeção
    public ReservaController(
        IReservaRepository repository,
        IMesaRepository mesaRepository,
        IAmbienteRepository ambienteRepository,
        IClienteRepository clienteRepository)
    {
        this.repository = repository;
        this.mesaRepository = mesaRepository;
        this.ambienteRepository = ambienteRepository;
        this.clienteRepository = clienteRepository;
    }

    private int ClienteLogadoId() => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    private void PreencherClienteLogado(Reserva model)
    {
        var cliente = clienteRepository.Read(ClienteLogadoId());
        if (cliente == null) return;
        model.Cliente = cliente;
        model.ClienteId = cliente.Id;
        foreach (var key in ModelState.Keys.Where(k => k.StartsWith("Cliente.")).ToList())
            ModelState.Remove(key);
    }

    private IActionResult? GarantirReservaDoCliente(Reserva? reserva)
    {
        if (reserva == null) return NotFound();
        if (reserva.ClienteId != ClienteLogadoId()) return Forbid();
        return null;
    }

    private Dictionary<string, int> IdsAmbientesMapa()
    {
        var cadastrados = ambienteRepository.Read();
        return NomesAmbientesMapa.ToDictionary(
            nome => nome,
            nome => cadastrados
                .FirstOrDefault(a => string.Equals(a.Nome.Trim(), nome, StringComparison.OrdinalIgnoreCase))
                ?.Id ?? 0);
    }

    // GET /Reserva/MesasDisponiveis?dataHora=2027-02-02&ambienteId=3
    [HttpGet]
    public ActionResult MesasDisponiveis([FromQuery] string? dataHora, [FromQuery] int ambienteId = 0)
    {
        var dia = ParseDiaConsulta(dataHora);
        var ocupadas = MesasReservadasNoDia(dia);

        var mesas = mesaRepository.Read();
        if (ambienteId > 0)
            mesas = mesas.Where(m => m.AmbienteId == ambienteId).ToList();

        var resultado = mesas.Select(m => new
        {
            id = m.Id,
            numero = m.Numero,
            lugares = m.QtdLugares,
            ambienteId = m.AmbienteId,
            ambiente = m.AmbienteNome,
            situacao = SituacaoMesaNoDia(m, ocupadas),
            dataConsulta = dia?.ToString("yyyy-MM-dd")
        }).ToList();

        return Json(resultado);
    }

    private static DateOnly? ParseDiaConsulta(string? data)
    {
        if (string.IsNullOrWhiteSpace(data)) return null;
        if (DateOnly.TryParse(data, CultureInfo.InvariantCulture, DateTimeStyles.None, out var dia))
            return dia;
        if (DateTime.TryParse(data, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind, out var dt))
            return DateOnly.FromDateTime(dt.Date);
        return null;
    }

    private static DateOnly ToDiaReserva(DateTime dataHora) => DateOnly.FromDateTime(dataHora.Date);

    private HashSet<int> MesasReservadasNoDia(DateOnly? dia, int ignorarReservaId = 0)
    {
        if (dia == null) return new HashSet<int>();
        return repository.Read()
            .Where(r => r.Id != ignorarReservaId
                && r.Status != "Cancelada"
                && ToDiaReserva(r.DataHora) == dia.Value)
            // N:N: junta todas as mesas de cada reserva do dia (tabela Reservas_Mesas)
            .SelectMany(r => r.MesaIds)
            .ToHashSet();
    }

    private static string SituacaoMesaNoDia(Mesa m, HashSet<int> reservadasNaData)
    {
        if (m.Status == "Ocupada") return "Ocupada";
        if (reservadasNaData.Contains(m.Id)) return "Reservada";
        return "Livre";
    }

    private static bool MesaLivreParaReservaNaData(Mesa mesa, HashSet<int> reservadasNaData) =>
        mesa.Status != "Ocupada" && !reservadasNaData.Contains(mesa.Id);

    private static bool DataReservaAnteriorAHoje(DateTime data) =>
        ToDiaReserva(data) < DateOnly.FromDateTime(DateTime.Today);

    public ActionResult Index()
    {
        var clienteId = ClienteLogadoId();
        var lista = repository.Read().Where(r => r.ClienteId == clienteId).ToList();
        ViewData["Title"] = "Minhas reservas";
        return View(lista);
    }

    // Ação atendida por requisições GET (abrir a tela)
    [HttpGet]
    // Ação Create (GET): abre o formulário vazio
    public ActionResult Create()
    {
        ViewBag.MapaAmbienteIds = IdsAmbientesMapa();
        var model = new Reserva { QtdPessoas = 2, Status = "Pendente" };
        PreencherClienteLogado(model);
        return View(model);
    }

    // Ação atendida por requisições POST (envio do formulário)
    [HttpPost]
    // Ação Create (POST): valida e grava o cadastro
    public ActionResult Create(Reserva model)
    {
        model.Status = "Pendente";
        ModelState.Remove(nameof(model.Status));
        PreencherClienteLogado(model);
        if (model.DataHora != default) model.DataHora = model.DataHora.Date;
        // Condição
        if (model.DataHora == default)
            ModelState.AddModelError(nameof(model.DataHora), "Informe a data da reserva.");
        else if (DataReservaAnteriorAHoje(model.DataHora))
            ModelState.AddModelError(nameof(model.DataHora), "A data da reserva não pode ser anterior à data de hoje.");

        // N:N: o cliente pode escolher uma ou mais mesas (repetidas são ignoradas)
        model.MesaIds = (model.MesaIds ?? new List<int>()).Where(id => id > 0).Distinct().ToList();
        if (model.MesaIds.Count == 0)
            ModelState.AddModelError(nameof(model.MesaIds), "Escolha ao menos uma mesa no mapa.");

        // MAPA DE MESAS: confere no servidor se TODAS as mesas escolhidas existem e estão livres
        // (status da mesa e reservas ativas no mesmo dia).
        if (ModelState.IsValid)
        {
            var reservadasNoDia = MesasReservadasNoDia(ToDiaReserva(model.DataHora));
            foreach (var mesaId in model.MesaIds)
            {
                var mesa = mesaRepository.Read(mesaId);
                if (mesa == null)
                    ModelState.AddModelError(nameof(model.MesaIds), $"Não foi possível cadastrar. A mesa {mesaId} não está cadastrada.");
                else if (!MesaLivreParaReservaNaData(mesa, reservadasNoDia))
                    ModelState.AddModelError(nameof(model.MesaIds), $"Não foi possível cadastrar. A mesa {mesa.Numero} ({mesa.AmbienteNome}) não está livre nesta data.");
            }
        }
        if (!ModelState.IsValid)
        {
            ViewBag.MapaAmbienteIds = IdsAmbientesMapa();
            return View(model);
        }
        try
        {
            repository.Create(model);
            // ALTERAÇÃO: após o cadastro mostra a reserva criada, onde o status (Pendente) aparece.
            return RedirectToAction(nameof(Details), new { id = model.Id });
        }
        // Erro vindo do banco de dados
        catch (SqlException ex)
        {
            // Variável motivo
            string motivo = ex.Number == 547 ? "A mesa informada não está cadastrada."
                : (ex.Number == 2627 || ex.Number == 2601) ? "Telefone ou e-mail já pertence a outro cliente com CPF diferente."
                : "Verifique CPF, telefone, e mesa cadastrados e a conexão com o banco.";
            // Registra a mensagem de erro para a tela
            ModelState.AddModelError("", "Não foi possível cadastrar. " + motivo);
            ViewBag.MapaAmbienteIds = IdsAmbientesMapa();
            return View(model);
        }
        // Mesa reservada por outra pessoa no mesmo instante (conferido de novo na gravação)
        catch (InvalidOperationException ex)
        {
            ModelState.AddModelError(nameof(model.MesaIds), "Não foi possível cadastrar. " + ex.Message);
            ViewBag.MapaAmbienteIds = IdsAmbientesMapa();
            return View(model);
        }
    }

    [HttpGet]
    public IActionResult Edit([FromRoute] int id)
    {
        // Busca o registro no banco
        var reserva = repository.Read(id);
        var acesso = GarantirReservaDoCliente(reserva);
        if (acesso != null) return acesso;
        if (reserva!.Status == "Cancelada") return BadRequest("Reserva cancelada não pode ser editada.");
        return View(reserva);
    }

    [HttpPost]
    public IActionResult Edit([FromRoute] int id, Reserva model)
    {
        var original = repository.Read(id);
        var acesso = GarantirReservaDoCliente(original);
        if (acesso != null) return acesso;
        if (original!.Status == "Cancelada") return BadRequest("Reserva cancelada não pode ser editada.");

        // Ajusta Id do modelo
        model.Id = id;
        // Ajusta Cliente do modelo
        model.Cliente = original.Cliente;
        // Ajusta ClienteId do modelo
        model.ClienteId = original.ClienteId;
        // Ajusta Status do modelo
        model.Status = original.Status;
        // CORREÇÃO: o cliente não troca de mesa; mantém sempre as mesas originais da reserva (N:N).
        model.MesaIds = original.MesaIds;
        model.MesasDescricao = original.MesasDescricao;
        // RESERVA POR DIA: na edição também guarda só a data (sem horário).
        if (model.DataHora != default) model.DataHora = model.DataHora.Date;
        // Esses dados não fazem parte da edição pelo cliente.
        foreach (var key in ModelState.Keys.Where(k =>
            // Atribui k
            k == "Id" || k == "Cliente" || k.StartsWith("Cliente.") ||
            // Atribui k
            k == "ClienteId" || k == "Status" || k.StartsWith("MesaIds")).ToList())
            // Ignora a validação deste campo
            ModelState.Remove(key);
        // Condição
        if (model.DataHora == default)
            ModelState.AddModelError(nameof(model.DataHora), "Informe a data da reserva.");
        else if (DataReservaAnteriorAHoje(model.DataHora))
            ModelState.AddModelError(nameof(model.DataHora), "A data da reserva não pode ser anterior à data de hoje.");

        // CORREÇÃO: na edição também confere se TODAS as mesas estão livres no novo dia (ignorando esta própria reserva).
        if (ModelState.IsValid)
        {
            var reservadasNoDia = MesasReservadasNoDia(ToDiaReserva(model.DataHora), id);
            foreach (var mesaId in model.MesaIds)
            {
                // Busca a mesa da reserva no banco
                var mesa = mesaRepository.Read(mesaId);
                // Mesa ocupada/reservada por outra reserva no mesmo dia: mostra o erro
                if (mesa != null && !MesaLivreParaReservaNaData(mesa, reservadasNoDia))
                    ModelState.AddModelError(nameof(model.MesaIds), $"Não foi possível salvar. A mesa {mesa.Numero} ({mesa.AmbienteNome}) não está livre nesta data.");
            }
        }
        // Se houver erro de validação, volta para o formulário
        if (!ModelState.IsValid) return View(model);
        // Tenta executar; erros são tratados abaixo
        try
        {
            // Chama o repositório: Update
            repository.Update(model);
            // Redireciona para outra tela
            return RedirectToAction(nameof(Index));
        }
        // Erro vindo do banco de dados
        catch (SqlException)
        {
            // Registra a mensagem de erro para a tela
            ModelState.AddModelError("", "Não foi possível salvar. Verifique a mesa informada e a conexão com o banco.");
            // Mostra a view com o modelo
            return View(model);
        }
    }

    // Ação atendida por requisições GET (abrir a tela)
    [HttpGet]
    // Ação Details: mostra os detalhes do registro
    public IActionResult Details(int id)
    {
        // Busca o registro no banco
        var model = repository.Read(id);
        var acesso = GarantirReservaDoCliente(model);
        if (acesso != null) return acesso;
        return View(model);
    }

    [HttpGet]
    public IActionResult Delete(int id)
    {
        var model = repository.Read(id);
        var acesso = GarantirReservaDoCliente(model);
        if (acesso != null) return acesso;
        return View(model);
    }

    [HttpPost, ActionName("Delete")]
    public IActionResult DeleteConfirmed([FromRoute] int id)
    {
        var model = repository.Read(id);
        var acesso = GarantirReservaDoCliente(model);
        if (acesso != null) return acesso;
        // Tenta executar; erros são tratados abaixo
        try
        {
            repository.Delete(id);
            MesaReservaSync.AplicarStatusReserva(repository, mesaRepository, model, "Cancelada");
            return RedirectToAction(nameof(Index));
        }
        // Erro vindo do banco de dados
        catch (SqlException)
        {
            // Registra a mensagem de erro para a tela
            ModelState.AddModelError("", "Não foi possível cancelar. Verifique a conexão com o banco.");
            // Mostra a view com o modelo
            return View("Delete", model);
        }
    }
}
