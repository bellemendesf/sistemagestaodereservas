using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Data.SqlClient;
using GestaoReservas.Models;

public class ClienteController : Controller
{
    // Dependência IClienteRepository recebida pelo construtor
    private readonly IClienteRepository repository;

    // Construtor: recebe as dependências por injeção
    public ClienteController(IClienteRepository repository)
    {
        // Guarda a dependência repository recebida
        this.repository = repository;
    }

    [Authorize(AuthenticationSchemes = "FuncionarioScheme")]
    public ActionResult Index() => View("index", repository.Read());

    [AllowAnonymous]
    [HttpGet]
    public ActionResult Create() => View(new Cliente());

    [AllowAnonymous]
    [HttpPost]
    public ActionResult Create(Cliente model)
    {
        // Verifica se o campo foi preenchido
        if (string.IsNullOrWhiteSpace(model.EmailPessoal))
            // Registra a mensagem de erro para a tela
            ModelState.AddModelError(nameof(model.EmailPessoal), "Informe o e-mail pessoal.");
        // Verifica se o campo foi preenchido
        if (string.IsNullOrWhiteSpace(model.Senha))
            // Registra a mensagem de erro para a tela
            ModelState.AddModelError(nameof(model.Senha), "Informe a senha.");
        // Se houver erro de validação, volta para o formulário
        if (!ModelState.IsValid) return View(model);
        // Tenta executar; erros são tratados abaixo
        try
        {
            // Chama o repositório: Create
            repository.Create(model);
            // ALTERAÇÃO: o cliente não vê mais a listagem de clientes; segue direto para a Nova reserva.
            return RedirectToAction("LoginCliente", "Auth");
        }
        // Erro vindo do banco de dados
        catch (InvalidOperationException ex)
        {
            ModelState.AddModelError("", ex.Message);
            return View(model);
        }
        catch (SqlException)
        {
            // Registra a mensagem de erro para a tela
            ModelState.AddModelError("", "Não foi possível cadastrar. Verifique os dados, duplicidades e a conexão com o banco.");
            // Mostra a view com o modelo
            return View(model);
        }
    }

    [Authorize(AuthenticationSchemes = "FuncionarioScheme")]
    [HttpGet]
    public ActionResult Edit([FromRoute] int id)
    {
        // Busca o registro no banco
        var model = repository.Read(id);
        // Se não encontrou o registro...
        if (model == null) return NotFound();
        // Mostra a view com o modelo
        return View(model);
    }

    [Authorize(AuthenticationSchemes = "FuncionarioScheme")]
    [HttpPost]
    public ActionResult Edit([FromRoute] int id, Cliente model)
    {
        // A rota identifica o registro; não permita trocar a identidade pelo formulário.
        model.Id = id;
        // Ignora a validação deste campo
        ModelState.Remove(nameof(model.Id));
        // Condição
        if (repository.Read(id) == null) return NotFound();
        // Verifica se o campo foi preenchido
        if (string.IsNullOrWhiteSpace(model.EmailPessoal))
            // Registra a mensagem de erro para a tela
            ModelState.AddModelError(nameof(model.EmailPessoal), "Informe o e-mail pessoal.");
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
            ModelState.AddModelError("", "Não foi possível salvar. Verifique os dados, duplicidades e vínculos existentes.");
            // Mostra a view com o modelo
            return View(model);
        }
    }

    [Authorize(AuthenticationSchemes = "FuncionarioScheme")]
    [HttpGet]
    public ActionResult Details(int id)
    {
        // Busca o registro no banco
        var model = repository.Read(id);
        // Se não encontrou o registro...
        if (model == null) return NotFound();
        // Mostra a view com o modelo
        return View(model);
    }

    [Authorize(AuthenticationSchemes = "FuncionarioScheme")]
    [HttpGet]
    public ActionResult Delete(int id)
    {
        // Busca o registro no banco
        var model = repository.Read(id);
        // Se não encontrou o registro...
        if (model == null) return NotFound();
        // Mostra a view com o modelo
        return View(model);
    }

    [Authorize(AuthenticationSchemes = "FuncionarioScheme")]
    [HttpPost, ActionName("Delete")]
    public ActionResult DeleteConfirmed([FromRoute] int id)
    {
        // Busca o registro no banco
        var model = repository.Read(id);
        // Se não encontrou o registro...
        if (model == null) return NotFound();
        // Tenta executar; erros são tratados abaixo
        try
        {
            // Chama o repositório: Delete
            repository.Delete(id);
            // Redireciona para outra tela
            return RedirectToAction(nameof(Index));
        }
        // Erro vindo do banco de dados
        catch (SqlException)
        {
            // Registra a mensagem de erro para a tela
            ModelState.AddModelError("", "Não foi possível excluir. O cadastro pode estar vinculado a outros registros; verifique também a conexão.");
            // Mostra a view com o modelo
            return View("Delete", model);
        }
    }
}
