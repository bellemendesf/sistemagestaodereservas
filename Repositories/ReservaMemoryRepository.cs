using GestaoReservas.Models;

// CORREÇÃO: reativa o Repository em memória, conforme TarefaMemoryRepository do professor.
public class ReservaMemoryRepository : IReservaRepository
{
    List<Reserva> lista = new List<Reserva>();
    int proximoId = 1;

    public void Create(Reserva reserva)
    {
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
                item.MesaId = reserva.MesaId;
                item.FuncionarioID = reserva.FuncionarioID;
                item.DataHora = reserva.DataHora;
                item.QtdPessoas = reserva.QtdPessoas;
                item.Status = reserva.Status;
            }
        }
    }
}
