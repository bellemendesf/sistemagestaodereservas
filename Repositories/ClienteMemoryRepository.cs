using GestaoReservas.Models;

// CORREÇÃO: reativa o Repository em memória, conforme TarefaMemoryRepository do professor.
public class ClienteMemoryRepository : IClienteRepository
{
    List<Cliente> lista = new List<Cliente>();
    // CORREÇÃO: IDs distintos são necessários para as rotas Edit, Details e Delete das Views.
    int proximoId = 1;

    public void Create(Cliente cliente)
    {
        // CORREÇÃO: atribui o identificador utilizado pelo CRUD antes de adicionar à lista.
        cliente.Id = proximoId++;
        lista.Add(cliente);
    }

    public void Delete(int id)
    {
        foreach (var item in lista)
        {
            if (item.Id == id)
            {
                lista.Remove(item);
                break;
            }
        }
    }

    public List<Cliente> Read()
    {
        return lista;
    }

    public Cliente Read(int id)
    {
        Cliente resultado = null!;
        foreach (var item in lista)
        {
            if (item.Id == id)
            {
                resultado = item;
            }
        }
        return resultado;
    }

    public void Update(Cliente cliente)
    {
        foreach (var item in lista)
        {
            if (item.Id == cliente.Id)
            {
                // CORREÇÃO: atualiza nome, CPF, e-mail e telefone que a View de edição envia.
                item.Nome = cliente.Nome;
                item.Cpf = cliente.Cpf;
                item.EmailPessoal = cliente.EmailPessoal;
                item.Telefone = cliente.Telefone;
                // CORREÇÃO: a View Edit permite senha em branco; conserva a senha atual nesse caso.
                if (cliente.Senha != null && cliente.Senha != "")
                {
                    item.Senha = cliente.Senha;
                }
            }
        }
    }
}
