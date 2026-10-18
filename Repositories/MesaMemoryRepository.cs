using GestaoReservas.Models;

// CORREÇÃO: reativa o Repository em memória, conforme TarefaMemoryRepository do professor.
public class MesaMemoryRepository : IMesaRepository
{
    List<Mesa> lista = new List<Mesa>();
    // CORREÇÃO: IDs distintos são necessários para as rotas Edit, Details e Delete das Views.
    int proximoId = 1;

    public void Create(Mesa mesa)
    {
        // CORREÇÃO: atribui o identificador utilizado pelo CRUD antes de adicionar à lista.
        mesa.Id = proximoId++;
        lista.Add(mesa);
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

    public List<Mesa> Read()
    {
        return lista;
    }

    public Mesa Read(int id)
    {
        Mesa resultado = null!;
        foreach (var item in lista)
        {
            if (item.Id == id)
            {
                resultado = item;
            }
        }
        return resultado;
    }

    public void Update(Mesa mesa)
    {
        foreach (var item in lista)
        {
            if (item.Id == mesa.Id)
            {
                // CORREÇÃO: atualiza número, quantidade de lugares e status enviados pelo formulário.
                item.Numero = mesa.Numero;
                item.QtdLugares = mesa.QtdLugares;
                item.Status = mesa.Status;
            }
        }
    }
}
