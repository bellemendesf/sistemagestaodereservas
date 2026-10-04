using GestaoReservas.Models;

public class AmbienteMemoryRepository : IAmbienteRepository
{
    List<Ambiente> lista = new List<Ambiente>();

    public void Create(Ambiente Ambiente)
    {
        lista.Add(Ambiente);
    }

    public void Delete(int id)
    {
        foreach(var Ambiente in lista)
        {
            if(Ambiente.Id == id)
            {
                lista.Remove(Ambiente);
                break;
            }
        }
    }

    public List<Ambiente> Read()
    {
        return lista;
    }

    public Ambiente Read(int id)
    {
        Ambiente a = null;
        foreach(var Ambiente in lista)
        {
            if(Ambiente.Id == id)
            {
                a = Ambiente;
            }
        }
        return a;
    }

    public void Update(Ambiente a)
    {
        foreach(var Ambiente in lista)
        {
            if(Ambiente.Id == a.Id)
            {
                Ambiente.Texto = a.Texto;
            }
        }
    }
}