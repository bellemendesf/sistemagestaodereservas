// Importa as classes do ASP.NET MVC (Controller, ActionResult...)
using Microsoft.AspNetCore.Mvc;
// Importa as classes de acesso ao SQL Server
using Microsoft.Data.SqlClient;
// Importa os modelos do projeto
using GestaoReservas.Models;

// Controller responsável pelas telas de Funcionario
public class FuncionarioController : Controller
{
    // Dependência IFuncionarioRepository recebida pelo construtor
    private readonly IFuncionarioRepository repository;

    // Construtor: recebe as dependências por injeção
    public FuncionarioController(IFuncionarioRepository repository)
    {
        // Guarda a dependência repository recebida
        this.repository = repository;
    }

    // Ação Index: mostra a listagem
    public ActionResult Index() => View("index", repository.Read());

    // Ação atendida por requisições GET (abrir a tela)
    [HttpGet]
    // Ação Create (GET): abre o formulário vazio
    public ActionResult Create() => View(new Funcionario());

    // Ação atendida por requisições POST (envio do formulário)
    [HttpPost]
    // Ação Create (POST): valida e grava o cadastro
    public ActionResult Create(Funcionario model)
    {
        // Verifica se o campo foi preenchido
        if (string.IsNullOrWhiteSpace(model.EmailInstitucional))
            // Registra a mensagem de erro para a tela
            ModelState.AddModelError(nameof(model.EmailInstitucional), "Informe o e-mail institucional.");
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
            // Redireciona para outra tela
            return RedirectToAction(nameof(Index));
        }
        // Erro vindo do banco de dados
        catch (SqlException)
        {
            // Registra a mensagem de erro para a tela
            ModelState.AddModelError("", "Não foi possível cadastrar. Verifique os dados, duplicidades e a conexão com o banco.");
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
        var model = repository.Read(id);
        // Se não encontrou o registro...
        if (model == null) return NotFound();
        // Mostra a view com o modelo
        return View(model);
    }

    // Ação atendida por requisições POST (envio do formulário)
    [HttpPost]
    // Ação Edit (POST): valida e salva as alterações
    public ActionResult Edit([FromRoute] int id, Funcionario model)
    {
        // A rota identifica o registro; não permita trocar a identidade pelo formulário.
        model.Id = id;
        // Ignora a validação deste campo
        ModelState.Remove(nameof(model.Id));
        // Condição
        if (repository.Read(id) == null) return NotFound();
        // Verifica se o campo foi preenchido
        if (string.IsNullOrWhiteSpace(model.EmailInstitucional))
            // Registra a mensagem de erro para a tela
            ModelState.AddModelError(nameof(model.EmailInstitucional), "Informe o e-mail institucional.");
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
