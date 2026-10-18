using GestaoReservas.Models;

// CORREÇÃO: reativa o Repository em memória, conforme TarefaMemoryRepository do professor.
public class AmbienteMemoryRepository : IAmbienteRepository
{
    List<Ambiente> lista = new List<Ambiente>();
    int proximoId = 1;

    public void Create(Ambiente ambiente)
    {
        ambiente.Id = proximoId++;
        lista.Add(ambiente);
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

    public List<Ambiente> Read()
    {
        return lista;
    }

    public Ambiente Read(int id)
    {
        Ambiente resultado = null!;
        foreach (var item in lista)
        {
            if (item.Id == id)
            {
                resultado = item;
            }
        }
        return resultado;
    }

    public void Update(Ambiente ambiente)
    {
        foreach (var item in lista)
        {
            if (item.Id == ambiente.Id)
            {
                item.Nome = ambiente.Nome;
                item.Status = ambiente.Status;
                item.Capacidade = ambiente.Capacidade;
            }
        }
    }
}
