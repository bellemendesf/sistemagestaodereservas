using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;

// Controller MVC: recebe os formulários e mantém o CRUD em uma lista estática.
// Os models representam os campos do DER; esta versão não acessa o banco.
public class ClienteController : Controller
{
    // A lista estática mantém os cadastros entre requisições, sem banco de dados.
    private static List<Cliente> lista = new List<Cliente>();
    private static int proximoId = 1;

    // Protege a lista quando duas requisições chegam ao mesmo tempo.
    private static object controle = new object();
    private PasswordHasher<Pessoa> hasher = new PasswordHasher<Pessoa>();

    // Envia os cadastros em memória para a página de listagem.
    public ActionResult Index()
    {
        lock (controle)
        {
            return View(lista.ToList());
        }
    }

    // Abre o formulário de inclusão com os campos herdados de Pessoa.
    [HttpGet]
    public ActionResult Create()
    {
        return View(new Cliente());
    }

    // Valida os campos do DER e cadastra na lista, atribuindo ID e hash da senha.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult Create(Cliente model)
    {
        lock (controle)
        {
            if (string.IsNullOrWhiteSpace(model.Senha))
            {
                ModelState.AddModelError("Senha", "Informe a senha.");
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            model.Id = proximoId;
            proximoId++;
            model.Senha = hasher.HashPassword(model, model.Senha!);
            lista.Add(model);
            return RedirectToAction("Index");
        }
    }

    // Localiza o cadastro pela chave Id e abre a página de edição.
    [HttpGet]
    public ActionResult Edit(int id)
    {
        lock (controle)
        {
            Cliente? model = lista.FirstOrDefault(item => item.Id == id);
            if (model == null)
            {
                return NotFound();
            }

            return View(model);
        }
    }

    // Usa o ID da rota para atualizar o registro e valida os campos enviados.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult Edit(int id, Cliente model)
    {
        lock (controle)
        {
            Cliente? atual = lista.FirstOrDefault(item => item.Id == id);
            if (atual == null)
            {
                return NotFound();
            }

            model.Id = id;
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // Senha vazia conserva o hash anterior; os demais campos são substituídos.
            model.Senha = string.IsNullOrWhiteSpace(model.Senha)
                ? atual.Senha
                : hasher.HashPassword(model, model.Senha);
            lista[lista.IndexOf(atual)] = model;
            return RedirectToAction("Index");
        }
    }

    // Exibe os dados do DER em modo de leitura, sem mostrar senha ou hash.
    [HttpGet]
    public ActionResult Details(int id)
    {
        lock (controle)
        {
            Cliente? model = lista.FirstOrDefault(item => item.Id == id);
            if (model == null)
            {
                return NotFound();
            }

            return View(model);
        }
    }

    // Exibe a confirmação; acessar este endereço não exclui nenhum dado.
    [HttpGet]
    public ActionResult Delete(int id)
    {
        lock (controle)
        {
            Cliente? model = lista.FirstOrDefault(item => item.Id == id);
            if (model == null)
            {
                return NotFound();
            }

            return View(model);
        }
    }

    // Confirma a exclusão por POST; ActionName mantém o endereço /Cliente/Delete/id.
    [HttpPost]
    [ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public ActionResult DeleteConfirmed(int id)
    {
        lock (controle)
        {
            Cliente? atual = lista.FirstOrDefault(item => item.Id == id);
            if (atual == null)
            {
                return NotFound();
            }

            lista.Remove(atual);
            return RedirectToAction("Index");
        }
    }
}