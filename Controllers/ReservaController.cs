// Importa as classes do ASP.NET MVC (Controller, ActionResult...)
using Microsoft.AspNetCore.Mvc;
// Importa as classes de acesso ao SQL Server
using Microsoft.Data.SqlClient;
// Importa os modelos do projeto
using System.Globalization;
using GestaoReservas.Models;

// Controller responsável pelas telas de Reserva
public class ReservaController : Controller
{
    // Dependência IReservaRepository recebida pelo construtor
    private readonly IReservaRepository repository;
    // MAPA DE MESAS: repositório de mesas usado para montar o mapa e validar a mesa escolhida.
    private readonly IMesaRepository mesaRepository;
    private readonly IAmbienteRepository ambienteRepository;

    private static readonly string[] NomesAmbientesMapa = ["Interno", "Externo", "Sacada"];

    // Construtor: recebe as dependências por injeção
    public ReservaController(
        IReservaRepository repository,
        IMesaRepository mesaRepository,
        IAmbienteRepository ambienteRepository)
    {
        this.repository = repository;
        this.mesaRepository = mesaRepository;
        this.ambienteRepository = ambienteRepository;
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
                && r.MesaId != null
                && ToDiaReserva(r.DataHora) == dia.Value)
            .Select(r => r.MesaId!.Value)
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

    // Ação Index: mostra a listagem
    public ActionResult Index() => View(repository.Read());

    // Ação atendida por requisições GET (abrir a tela)
    [HttpGet]
    // Ação Create (GET): abre o formulário vazio
    public ActionResult Create()
    {
        ViewBag.MapaAmbienteIds = IdsAmbientesMapa();
        return View(new Reserva { QtdPessoas = 2, Status = "Pendente" });
    }

    // Ação atendida por requisições POST (envio do formulário)
    [HttpPost]
    // Ação Create (POST): valida e grava o cadastro
    public ActionResult Create(Reserva model)
    {
        // Ajusta Status do modelo
        model.Status = "Pendente";
        // Ignora a validação deste campo
        ModelState.Remove(nameof(model.Status));
        // RESERVA POR DIA: guarda só a data (sem horário).
        if (model.DataHora != default) model.DataHora = model.DataHora.Date;
        // Condição
        if (model.Cliente == null)
            // Registra a mensagem de erro para a tela
            ModelState.AddModelError(nameof(model.Cliente), "Informe os dados do cliente.");
        // Condição
        if (model.DataHora == default)
            // Registra a mensagem de erro para a tela
            ModelState.AddModelError(nameof(model.DataHora), "Informe a data e hora.");
        // MAPA DE MESAS: confere no servidor se a mesa escolhida está livre
        // (status da mesa e reservas ativas no mesmo dia). Mesa não cadastrada segue o tratamento já existente do banco.
        if (ModelState.IsValid && model.MesaId != null)
        {
            // Variável mesa
            var mesa = mesaRepository.Read(model.MesaId.Value);
            // Condição
            if (mesa != null && !MesaLivreParaReservaNaData(mesa, MesasReservadasNoDia(ToDiaReserva(model.DataHora))))
                ModelState.AddModelError(nameof(model.MesaId), "Não foi possível cadastrar. A mesa escolhida não está livre nesta data.");
        }
        // Se houver erro de validação, volta para o formulário
        if (!ModelState.IsValid) return View(model);
        // Tenta executar; erros são tratados abaixo
        try
        {
            // Chama o repositório: Create
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
            // Mostra a view com o modelo
            return View(model);
        }
    }

    // Ação atendida por requisições GET (abrir a tela)
    [HttpGet]
    // Ação Edit (GET): abre o formulário de edição
    public ActionResult Edit([FromRoute] int id)
    {
        // Busca o registro no banco
        var reserva = repository.Read(id);
        // Se não encontrou o registro...
        if (reserva == null) return NotFound();
        // Condição
        if (reserva.Status == "Cancelada") return BadRequest("Reserva cancelada não pode ser editada.");
        // Mostra a view com o modelo
        return View(reserva);
    }

    // Ação atendida por requisições POST (envio do formulário)
    [HttpPost]
    // Ação Edit (POST): valida e salva as alterações
    public ActionResult Edit([FromRoute] int id, Reserva model)
    {
        // Busca o registro no banco
        var original = repository.Read(id);
        // Se não encontrou o registro...
        if (original == null) return NotFound();
        // Condição
        if (original.Status == "Cancelada") return BadRequest("Reserva cancelada não pode ser editada.");

        // Ajusta Id do modelo
        model.Id = id;
        // Ajusta Cliente do modelo
        model.Cliente = original.Cliente;
        // Ajusta ClienteId do modelo
        model.ClienteId = original.ClienteId;
        // Ajusta Status do modelo
        model.Status = original.Status;
        // CORREÇÃO: o cliente não troca de mesa; mantém sempre a mesa original da reserva.
        model.MesaId = original.MesaId;
        // RESERVA POR DIA: na edição também guarda só a data (sem horário).
        if (model.DataHora != default) model.DataHora = model.DataHora.Date;
        // Esses dados não fazem parte da edição pelo cliente.
        foreach (var key in ModelState.Keys.Where(k =>
            // Atribui k
            k == "Id" || k == "Cliente" || k.StartsWith("Cliente.") ||
            // Atribui k
            k == "ClienteId" || k == "Status").ToList())
            // Ignora a validação deste campo
            ModelState.Remove(key);
        // Condição
        if (model.DataHora == default)
            // Registra a mensagem de erro para a tela
            ModelState.AddModelError(nameof(model.DataHora), "Informe a data e hora.");
        // CORREÇÃO: na edição também confere se a mesa está livre no dia (ignorando esta própria reserva).
        if (ModelState.IsValid && model.MesaId != null)
        {
            // Busca a mesa da reserva no banco
            var mesa = mesaRepository.Read(model.MesaId.Value);
            // Mesa ocupada/reservada por outra reserva no mesmo dia: mostra o erro
            if (mesa != null && !MesaLivreParaReservaNaData(mesa, MesasReservadasNoDia(ToDiaReserva(model.DataHora), id)))
                ModelState.AddModelError(nameof(model.MesaId), "Não foi possível salvar. A mesa não está livre nesta data.");
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
    public ActionResult Details(int id)
    {
        // Busca o registro no banco
        var model = repository.Read(id);
        // Se não encontrou o registro...
        if (model == null) return NotFound();
        // Mostra a view com o modelo
        return View(model);
    }

    // Ação atendida por requisições GET (abrir a tela)
    [HttpGet]
    // Ação Delete: confirmação de exclusão
    public ActionResult Delete(int id)
    {
        // Busca o registro no banco
        var model = repository.Read(id);
        // Se não encontrou o registro...
        if (model == null) return NotFound();
        // Mostra a view com o modelo
        return View(model);
    }

    // Ação POST que responde pela rota Delete
    [HttpPost, ActionName("Delete")]
    // Ação que executa a exclusão confirmada
    public ActionResult DeleteConfirmed([FromRoute] int id)
    {
        // Busca o registro no banco
        var model = repository.Read(id);
        // Se não encontrou o registro...
        if (model == null) return NotFound();
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
