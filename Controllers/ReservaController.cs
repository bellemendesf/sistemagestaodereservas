using Microsoft.Data.SqlClient;
using Microsoft.AspNetCore.Mvc;
using GestaoReservas.Models;

public class ReservaController : Controller
{
    private IReservaRepository repository;

    public ReservaController(IReservaRepository repository)
    {
        this.repository = repository;
    }

    public ActionResult Index()
    {
        return View(repository.Read());
    }

    [HttpGet]
    public ActionResult Create()
    {
        Reserva model = new Reserva();
        model.Cliente = new Cliente();
        model.Cliente.Cpf = "";
        model.Cliente.EmailPessoal = "";
        model.Cliente.Senha = "";
        model.QtdPessoas = 2;
        model.Status = "Pendente";
        return View(model);
    }

    [HttpPost]
    public ActionResult Create(Reserva model)
    {
        // UNIÃO: confere os campos de A necessários às relações obrigatórias de B.
        if (model.Cliente == null || string.IsNullOrWhiteSpace(model.Cliente.Nome) ||
            string.IsNullOrWhiteSpace(model.Cliente.Cpf) || string.IsNullOrWhiteSpace(model.Cliente.Telefone) ||
            model.MesaId <= 0 || model.FuncionarioID <= 0 || model.QtdPessoas <= 0 ||
            model.DataHora == default)
        {
            return BadRequest("Informe nome, CPF, telefone, número de uma mesa cadastrada, ID de um funcionário cadastrado, data, hora e quantidade de pessoas.");
        }

        // CORREÇÃO: o status inicial é fixo, independentemente do valor enviado pela View.
        model.Status = "Pendente";
        try
        {
            repository.Create(model);
        }
        catch (SqlException)
        {
            return BadRequest("Não foi possível gravar. Verifique os dados obrigatórios, registros duplicados, vínculos existentes e a conexão com o banco.");
        }
        return RedirectToAction("Index");
    }

    [HttpGet]
    public ActionResult Edit(int id)
    {
        Reserva reserva = repository.Read(id);
        if (reserva == null) return NotFound();
        if (reserva.Status == "Cancelada") return BadRequest("Reserva cancelada não pode ser editada.");
        return View(reserva);
    }

    [HttpPost]
    public ActionResult Edit(int id, Reserva model)
    {
        // CORREÇÃO: lê o ID da rota ou do campo oculto enviado pela View Edit.
        model.Id = id > 0 ? id : model.Id;
        Reserva reserva = repository.Read(model.Id);
        if (reserva == null) return NotFound();
        if (reserva.Status == "Cancelada") return BadRequest("Reserva cancelada não pode ser editada.");

        // CORREÇÃO: o Repository preserva o Status e o FuncionarioID originais.
        try
        {
            repository.Update(model);
        }
        catch (SqlException)
        {
            return BadRequest("Não foi possível gravar. Verifique os dados obrigatórios, registros duplicados, vínculos existentes e a conexão com o banco.");
        }
        return RedirectToAction("Index");
    }

    [HttpGet]
    public ActionResult Details(int id)
    {
        Reserva reserva = repository.Read(id);
        if (reserva == null) return NotFound();
        return View(reserva);
    }

    [HttpGet]
    public ActionResult Delete(int id)
    {
        Reserva reserva = repository.Read(id);
        if (reserva == null) return NotFound();
        // CORREÇÃO: mantém a View original, mas o POST fará cancelamento lógico.
        return View(reserva);
    }

    [HttpPost]
    public ActionResult Delete(Reserva model)
    {
        if (repository.Read(model.Id) == null) return NotFound();
        // CORREÇÃO: o método Delete do Repository apenas muda o status para Cancelada.
        try
        {
            repository.Delete(model.Id);
        }
        catch (SqlException)
        {
            return BadRequest("Não foi possível gravar. Verifique os dados obrigatórios, registros duplicados, vínculos existentes e a conexão com o banco.");
        }
        return RedirectToAction("Index");
    }
}
