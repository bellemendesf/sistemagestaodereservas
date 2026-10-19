using GestaoReservas.Models;

// CORREÇÃO: reativa o Repository em memória, conforme TarefaMemoryRepository do professor.
public class AmbienteMemoryRepository : IAmbienteRepository
{
    List<Ambiente> lista = new List<Ambiente>();
    // CORREÇÃO: IDs distintos são necessários para as rotas Edit, Details e Delete das Views.
    int proximoId = 1;

    public void Create(Ambiente ambiente)
    {
        // CORREÇÃO: atribui o identificador utilizado pelo CRUD antes de adicionar à lista.
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

    public Ambiente? Read(int id)
    {
        Ambiente? resultado = null;
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
                // CORREÇÃO: atualiza nome, capacidade e status que a View de edição envia.
                item.Nome = ambiente.Nome;
                item.Status = ambiente.Status;
                item.Capacidade = ambiente.Capacidade;
            }
        }
    }
}
