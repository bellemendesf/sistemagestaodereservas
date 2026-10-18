using GestaoReservas.Models;

// CORREÇÃO: reativa o Repository em memória, conforme TarefaMemoryRepository do professor.
public class ClienteMemoryRepository : IClienteRepository
{
    List<Cliente> lista = new List<Cliente>();
    int proximoId = 1;

    public void Create(Cliente cliente)
    {
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
                item.Nome = cliente.Nome;
                item.Cpf = cliente.Cpf;
                item.EmailPessoal = cliente.EmailPessoal;
                item.Telefone = cliente.Telefone;
                if (cliente.Senha != null && cliente.Senha != "")
                {
                    item.Senha = cliente.Senha;
                }
            }
        }
    }
}
