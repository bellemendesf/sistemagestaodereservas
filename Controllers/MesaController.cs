
// Importa os modelos do projeto
using GestaoReservas.Models;
// Importa as classes do ASP.NET MVC (Controller, ActionResult...)
using Microsoft.AspNetCore.Mvc;
// Importa SelectList para montar listas de seleção
using Microsoft.AspNetCore.Mvc.Rendering;
// Importa as classes de acesso ao SQL Server
using Microsoft.Data.SqlClient;

// Controller responsável pelas telas de Mesa
public class MesaController : Controller
{
    // Dependência IMesaRepository recebida pelo construtor
    private readonly IMesaRepository repository;
    // Dependência IAmbienteRepository recebida pelo construtor
    private readonly IAmbienteRepository ambienteRepository;

    // Construtor: recebe as dependências por injeção
    public MesaController(
        IMesaRepository repository,
        IAmbienteRepository ambienteRepository)
    {
        // Guarda a dependência repository recebida
        this.repository = repository;
        // Guarda a dependência ambienteRepository recebida
        this.ambienteRepository = ambienteRepository;
    }

    // Método auxiliar CarregarAmbientes
    private void CarregarAmbientes(int? selecionado = null)
    {
        // Envia Ambientes para a view
        ViewBag.Ambientes = new SelectList(
            ambienteRepository.Read(),
            "Id",
            "Nome",
            selecionado
        );
    }

    // Ação Index: mostra a listagem
    public ActionResult Index()
    {
        // Mostra a view com o modelo
        return View("index", repository.Read());
    }

    // Ação atendida por requisições GET (abrir a tela)
    [HttpGet]
    // Ação Create (GET)
    public ActionResult Create()
    {
        // Carrega a lista de ambientes para o select
        CarregarAmbientes();
        // Mostra a view com o modelo
        return View(new Mesa());
    }

    // Ação atendida por requisições POST (envio do formulário)
    [HttpPost]
    // Exige o token antifalsificação do formulário
    [ValidateAntiForgeryToken]
    // Ação Create (POST): valida e grava o cadastro
    public ActionResult Create(Mesa model)
    {
        // Se houver erro de validação, volta para o formulário
        if (!ModelState.IsValid)
        {
            // Carrega a lista de ambientes para o select
            CarregarAmbientes(model.AmbienteId);
            // Mostra a view com o modelo
            return View(model);
        }

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
            ModelState.AddModelError(
                "",
                "Não foi possível cadastrar a mesa. Verifique os dados e o ambiente selecionado."
            );

            // Carrega a lista de ambientes para o select
            CarregarAmbientes(model.AmbienteId);
            // Mostra a view com o modelo
            return View(model);
        }
    }

    // Ação atendida por requisições GET (abrir a tela)
    [HttpGet]
    // Ação Edit (GET): abre o formulário de edição
    public ActionResult Edit(int id)
    {
        // Busca o registro no banco
        var model = repository.Read(id);

        // Se não encontrou o registro...
        if (model == null)
            // Registro não existe: responde 404
            return NotFound();

        // Carrega a lista de ambientes para o select
        CarregarAmbientes(model.AmbienteId);
        // Mostra a view com o modelo
        return View(model);
    }

    // Ação atendida por requisições POST (envio do formulário)
    [HttpPost]
// Exige o token antifalsificação do formulário
[ValidateAntiForgeryToken]
// Ação Edit (POST): valida e salva as alterações
public ActionResult Edit(int id, Mesa model)
{
    // O ID original identifica a mesa no banco.
    // Ele não deve ser alterado pelo formulário.
    var mesaExistente = repository.Read(id);

    // Se não encontrou o registro...
    if (mesaExistente == null)
        // Registro não existe: responde 404
        return NotFound();

    // Ajusta Id do modelo
    model.Id = id;
    // CORREÇÃO: mantém o número original (ele é igual à chave nr da mesa).
    model.Numero = mesaExistente.Numero;
    ModelState.Remove(nameof(model.Numero));
    // Ignora a validação deste campo
    ModelState.Remove(nameof(model.Id));

    // Se houver erro de validação, volta para o formulário
    if (!ModelState.IsValid)
    {
        // Carrega a lista de ambientes para o select
        CarregarAmbientes(model.AmbienteId);
        // Mostra a view com o modelo
        return View(model);
    }

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
        ModelState.AddModelError(
            "",
            "Não foi possível salvar. Verifique os dados, o ambiente e os vínculos com reservas."
        );

        // Carrega a lista de ambientes para o select
        CarregarAmbientes(model.AmbienteId);
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
        if (model == null)
            // Registro não existe: responde 404
            return NotFound();

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
        if (model == null)
            // Registro não existe: responde 404
            return NotFound();

        // Mostra a view com o modelo
        return View(model);
    }

    // Ação atendida por requisições POST (envio do formulário)
    [HttpPost]
    // Define o nome da rota desta ação
    [ActionName("Delete")]
    // Exige o token antifalsificação do formulário
    [ValidateAntiForgeryToken]
    // Ação que executa a exclusão confirmada
    public ActionResult DeleteConfirmed(int id)
    {
        // Busca o registro no banco
        var model = repository.Read(id);

        // Se não encontrou o registro...
        if (model == null)
            // Registro não existe: responde 404
            return NotFound();

        // Tenta executar; erros são tratados abaixo
        try
        {
            // Chama o repositório: Delete
            repository.Delete(id);
            // Redireciona para outra tela
            return RedirectToAction(nameof(Index));
        }
        // Erro de regra (ex.: mesa com reservas)
        catch (InvalidOperationException ex)
{
    // Registra a mensagem de erro para a tela
    ModelState.AddModelError("", ex.Message);
    // Mostra a view com o modelo
    return View("Delete", model);
}
// Erro vindo do banco de dados
catch (SqlException)
{
    // Registra a mensagem de erro para a tela
    ModelState.AddModelError(
        "",
        "Não foi possível excluir a mesa. Verifique os vínculos existentes."
    );

    // Mostra a view com o modelo
    return View("Delete", model);
}
    }
}
