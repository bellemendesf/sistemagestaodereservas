using Microsoft.AspNetCore.Mvc;  // Importa as classes do ASP.NET MVC (Controller, ActionResult, etc.)

public class MesaController : Controller
{
     private List<Mesa> mesas = new List<Mesa>();
        // GET: Entidade
        // Método chamado quando o usuário acessa a URL /Entidade (sem parâmetros)
        public ActionResult Index()
        {
            Mesa e1 = new Mesa{Id= 1, Numero = 1,QtdLugares = 4, Status = "Livre"};
            Mesa e2 = new Mesa{Id= 2, Numero = 2,QtdLugares = 4, Status = "Reservada"};
            Mesa e3 = new Mesa{Id= 3, Numero = 3,QtdLugares = 4, Status = "Ocupada"};
            Mesa i1 = new Mesa{Id= 4, Numero = 4,QtdLugares = 8, Status = "Livre"};
            Mesa i2 = new Mesa{Id= 5, Numero = 5,QtdLugares = 3, Status = "Livre"};
            Mesa s1 = new Mesa{Id= 6, Numero = 8,QtdLugares = 2, Status = "Reservada"};
            Mesa s2 = new Mesa{Id= 7, Numero = 9,QtdLugares = 6, Status = "Reservada"};
            return View(mesas);                      
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
                return View(mesas);      // Se inválido, devolve o mesmo formulário preenchido, mostrando os erros

            //_repositorio.Inserir(entidade); // Se válido, manda o repositório salvar no banco de dados
            return RedirectToAction("Index");  // Redireciona o usuário de volta para a listagem (evita reenvio do form ao atualizar a página)
        }

        // GET: Entidade/Edit/5
        // Chamado quando o usuário clica em "Editar" em um registro específico
        [HttpGet]
        public ActionResult Edit(int id)
        {
            //var entidade = _repositorio.ObterPorId(id);  // Busca o registro atual no banco pelo id
            if (mesas == null) return NotFound(); // Se não existir, retorna 404
            return View(mesas);                       // Mostra o formulário já preenchido com os dados atuais
        }

        // POST: Entidade/Edit
        // Chamado quando o usuário altera os dados e clica em "Salvar"
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Mesa model)
        {
            if (!ModelState.IsValid)          // Mesma validação do Create
                return View(mesas);

            //_repositorio.Atualizar(mesas); // Manda o repositório atualizar o registro existente no banco
            return RedirectToAction("Index"); // Volta para a listagem
        }

        // GET: Entidade/Delete/5
        // Chamado quando o usuário clica em "Excluir" — mostra uma tela de confirmação
        [HttpGet]
        public ActionResult Delete(int id)
        {
            //var entidade = _repositorio.ObterPorId(id);
            if (mesas == null) return NotFound();
            return View(mesas);  // Mostra os dados do registro perguntando "tem certeza que quer excluir?"
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


                 
             

