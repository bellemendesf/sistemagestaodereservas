using GestaoReservas.Models;

public class MesaMemoryRepository : IMesaRepository
{
    List<Mesa> lista = new List<Mesa>();

    public void Create(Mesa Mesa)
    {
        lista.Add(Mesa);
    }

    public void Delete(int id)
    {
        foreach(var Mesa in lista)
        {
            if(Mesa.IdMesa == id)
            {
                lista.Remove(Mesa);
                break;
            }
        }
    }

    public List<Mesa> Read()
    {
        return lista;
    }

    public Mesa Read(int id)
    {
        Mesa m = null;
        foreach(var Mesa in lista)
        {
            if(Mesa.IdMesa == id)
            {
                m = Mesa;
            }
        }
        return m;
    }

    public void Update(Mesa m)
    {
        foreach(var Mesa in lista)
        {
            if(Mesa.IdMesa == m.IdMesa)
            {
                Mesa.Texto = m.Texto;
            }
        }
    }
}