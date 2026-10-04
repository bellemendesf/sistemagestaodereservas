using GestaoReservas.Models;

public class ClienteMemoryRepository : IClienteRepository
{
    List<Cliente> lista = new List<Cliente>();

    public void Create(Cliente Cliente)
    {
        lista.Add(Cliente);
    }

    public void Delete(int id)
    {
        foreach(var Cliente in lista)
        {
            if(Cliente.Id == id)
            {
                lista.Remove(Cliente);
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
        Cliente c = null;
        foreach(var Cliente in lista)
        {
            if(Cliente.Id == id)
            {
                c = Cliente;
            }
        }
        return c;
    }

    public void Update(Cliente c)
    {
        foreach(var Cliente in lista)
        {
            if(Cliente.Id == c.Id)
            {
                Cliente.Telefone = c.Telefone;
            }
        }
    }
}