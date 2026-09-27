using GestaoReservas.Models;
using Microsoft.AspNetCore.Mvc;    // Importa as classes do ASP.NET MVC (Controller, ActionResult, etc.)

public class AmbienteController : Controller
{
   
    // Um Controller é a camada do MVC que recebe a requisição do usuário (clique, formulário, URL),
    // decide o que fazer com ela (chamando o Model/Repositório) e devolve uma resposta (uma View ou um redirect).
    
    // Instância do repositório que faz o acesso ao banco (SELECT, INSERT, UPDATE, DELETE)
    // O controller NÃO acessa o banco diretamente — ele delega isso ao repositório
        
        private List<Ambiente> ambientes = new List<Ambiente>();
        // GET: Entidade
        // Método chamado quando o usuário acessa a URL /Entidade (sem parâmetros)
        public ActionResult Index()
        {
            Ambiente externo = new Ambiente{Id= 1, Nome = "Externo", Capacidade = 3};
            Ambiente interno = new Ambiente{Id= 2, Nome = "Interno", Capacidade = 4};
            Ambiente sacada = new Ambiente{Id= 3, Nome = "Sacada", Capacidade = 5};
            return View(ambientes);                      
        }

        // GET: Entidade/Create
        // Chamado quando o usuário clica em "Criar novo" — apenas mostra o formulário vazio
        [HttpGet]
        public ActionResult Create()
        {
            return View();  // Retorna a View "Create.cshtml" sem nenhum dado (formulário em branco)
        }

        // POST: Entidade/Create
        // Chamado quando o usuário PREENCHE o formulário e clica em "Salvar"
        [HttpPost]                          // Indica que este método só responde a requisições POST (envio de formulário)
        [ValidateAntiForgeryToken]          // Protege contra ataques CSRF (verifica um token de segurança oculto no form)
        public ActionResult Create(Ambiente model)  // O MVC pega os campos do formulário e monta o objeto "entidade" automaticamente
        {
            if (!ModelState.IsValid)        // Verifica se os dados enviados respeitam as validações do Model ( campos obrigatórios, etc.)
                return View(ambientes);      // Se inválido, devolve o mesmo formulário preenchido, mostrando os erros

            //_repositorio.Inserir(entidade); // Se válido, manda o repositório salvar no banco de dados
            return RedirectToAction("Index");  // Redireciona o usuário de volta para a listagem (evita reenvio do form ao atualizar a página)
        }

        // GET: Entidade/Edit/5
        // Chamado quando o usuário clica em "Editar" em um registro específico
        [HttpGet]
        public ActionResult Edit(int id)
        {
            //var entidade = _repositorio.ObterPorId(id);  // Busca o registro atual no banco pelo id
            if (ambientes == null) return NotFound(); // Se não existir, retorna 404
            return View(ambientes);                       // Mostra o formulário já preenchido com os dados atuais
        }

        // POST: Entidade/Edit
        // Chamado quando o usuário altera os dados e clica em "Salvar"
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Ambiente model)
        {
            if (!ModelState.IsValid)          // Mesma validação do Create
                return View(ambientes);

            //_repositorio.Atualizar(ambientes); // Manda o repositório atualizar o registro existente no banco
            return RedirectToAction("Index"); // Volta para a listagem
        }

        // GET: Entidade/Delete/5
        // Chamado quando o usuário clica em "Excluir" — mostra uma tela de confirmação
        [HttpGet]
        public ActionResult Delete(int id)
        {
            //var entidade = _repositorio.ObterPorId(id);
            if (ambientes == null) return NotFound();
            return View(ambientes);  // Mostra os dados do registro perguntando "tem certeza que quer excluir?"
        }

        // POST: Entidade/Delete/5
        // Chamado quando o usuário CONFIRMA a exclusão
        [HttpPost, ActionName("Delete")]   // "ActionName" permite ter dois métodos "Delete" (GET e POST) sem conflito de nome
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            //_repositorio.Excluir(id);          // Manda o repositório apagar o registro do banco
            return RedirectToAction("Index");  // Volta para a listagem
        }
    
}

                 
             

