// Importa as classes de acesso ao SQL Server
using Microsoft.Data.SqlClient;
// Importa as classes do ASP.NET MVC (Controller, ActionResult...)
using Microsoft.AspNetCore.Mvc;
// Importa os modelos do projeto
using GestaoReservas.Models;

// Controller responsável pelas telas de AreaRestaurante
public class AreaRestauranteController : Controller
{
    // Dependência IReservaRepository recebida pelo construtor
    private readonly IReservaRepository repository;

    // CORREÇÃO: injeta a mesma interface já registrada no Program.cs.
    public AreaRestauranteController(IReservaRepository repository)
    {
        // Guarda a dependência repository recebida
        this.repository = repository;
    }

    // Ação Index: mostra a listagem
    public ActionResult Index()
    {
        // Mostra a view com o modelo
        return View();
    }

    // CORREÇÃO: organiza as ações do funcionário sem replicar o CRUD de mesas e ambientes.
    public ActionResult CadastrarFuncionario()
    {
        // Redireciona para outra tela
        return RedirectToAction("Create", "Funcionario");
    }

    // Ação GerenciarMesas
    public ActionResult GerenciarMesas()
    {
        // Redireciona para outra tela
        return RedirectToAction("Index", "Mesa");
    }

    // Ação GerenciarAmbientes
    public ActionResult GerenciarAmbientes()
    {
        // Redireciona para outra tela
        return RedirectToAction("Index", "Ambiente");
    }

    // Ação Reservas
    public ActionResult Reservas()
    {
        // CORREÇÃO: reutiliza a View existente; links Edit levam ao fluxo do restaurante.
        return View("~/Views/Reserva/Index.cshtml", repository.Read());
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
        if (reserva.Status == "Cancelada") return BadRequest("Reserva cancelada não pode ser alterada.");

        // Mostra a view com o modelo
        return View("~/Views/Reserva/Edit.cshtml", reserva);
    }

    // Ação atendida por requisições POST (envio do formulário)
    [HttpPost]
    // Ação Edit (POST): valida e salva as alterações
    public ActionResult Edit([FromRoute] int id, string status)
    {
        // Variável reservaId
        int reservaId = id;
        // Busca o registro no banco
        var reserva = repository.Read(reservaId);
        // Se não encontrou o registro...
        if (reserva == null) return NotFound();
        // Condição
        if (reserva.Status == "Cancelada") return BadRequest("Reserva cancelada não pode ser alterada.");

        // CORREÇÃO: apesar de a View enviar outros campos, funcionário só atualiza Status.
        if (status != "Pendente" &&
            status != "Confirmada" &&
            status != "Cancelada")
        {
            // Registra a mensagem de erro para a tela
            ModelState.AddModelError("Status", "Informe Pendente, Confirmada ou Cancelada.");
            // Mostra a view com o modelo
            return View("~/Views/Reserva/Edit.cshtml", reserva);
        }

        // Tenta executar; erros são tratados abaixo
        try
        {
            // Chama o repositório: UpdateStatus
            repository.UpdateStatus(reservaId, status);
        }
        // Erro vindo do banco de dados
        catch (SqlException)
        {
            // Registra a mensagem de erro para a tela
            ModelState.AddModelError("", "Não foi possível salvar o status. Verifique a conexão com o banco.");
            // Mostra a view com o modelo
            return View("~/Views/Reserva/Edit.cshtml", reserva);
        }
        // Redireciona para outra tela
        return RedirectToAction("Reservas");
    }

    // Ação atendida por requisições GET (abrir a tela)
    [HttpGet]
    // Ação Details: mostra os detalhes do registro
    public ActionResult Details(int id)
    {
        // Busca o registro no banco
        var reserva = repository.Read(id);
        // Se não encontrou o registro...
        if (reserva == null) return NotFound();
        // Mostra a view com o modelo
        return View("~/Views/Reserva/Details.cshtml", reserva);
    }

    // Ação atendida por requisições GET (abrir a tela)
    [HttpGet]
    // Ação Delete: confirmação de exclusão
    public ActionResult Delete(int id)
    {
        // CORREÇÃO: funcionário não possui ação de excluir reserva.
        return StatusCode(403);
    }

    // Ação atendida por requisições POST (envio do formulário)
    [HttpPost]
    // Ação Delete: confirmação de exclusão
    public ActionResult Delete(Reserva model)
    {
        // CORREÇÃO: impede exclusão no fluxo específico do restaurante.
        return StatusCode(403);
    }

    // Ação atendida por requisições GET (abrir a tela)
    [HttpGet]
    // Ação Create (GET)
    public ActionResult Create()
    {
        // CORREÇÃO: criar reserva não é uma operação do funcionário.
        return StatusCode(403);
    }
}
