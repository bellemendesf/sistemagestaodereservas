using GestaoReservas.Models;

// CORREÇÃO: reativa o Repository em memória, conforme TarefaMemoryRepository do professor.
public class ReservaMemoryRepository : IReservaRepository
{
    List<Reserva> lista = new List<Reserva>();
    // CORREÇÃO: IDs distintos são necessários para as rotas Edit, Details e Delete das Views.
    int proximoId = 1;

    public void Create(Reserva reserva)
    {
        // CORREÇÃO: atribui o identificador utilizado pelo CRUD antes de adicionar à lista.
        reserva.Id = proximoId++;
        lista.Add(reserva);
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

    public List<Reserva> Read()
    {
        return lista;
    }

    public Reserva Read(int id)
    {
        Reserva resultado = null!;
        foreach (var item in lista)
        {
            if (item.Id == id)
            {
                resultado = item;
            }
        }
        return resultado;
    }

    public void Update(Reserva reserva)
    {
        foreach (var item in lista)
        {
            if (item.Id == reserva.Id)
            {
                // CORREÇÃO: atualiza somente os campos enviados pela View Edit, preservando o objeto Cliente.
                item.MesaId = reserva.MesaId;
                item.FuncionarioID = reserva.FuncionarioID;
                item.DataHora = reserva.DataHora;
                item.QtdPessoas = reserva.QtdPessoas;
                item.Status = reserva.Status;
            }
        }
    }
}
