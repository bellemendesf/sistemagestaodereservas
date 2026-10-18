using GestaoReservas.Models;

public class ReservaMemoryRepository : IReservaRepository
{
    List<Reserva> lista = new List<Reserva>();
    int proximoId = 1;

    public void Create(Reserva reserva)
    {
        reserva.Id = proximoId++;
        // CORREÇÃO: cliente não define o status inicial, mesmo que o formulário o envie.
        reserva.Status = "Pendente";
        lista.Add(reserva);
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
                break;
            }
        }
        return resultado;
    }

    public void Update(Reserva reserva)
    {
        foreach (var item in lista)
        {
            if (item.Id == reserva.Id && item.Status != "Cancelada")
            {
                // CORREÇÃO: cliente altera apenas dados da reserva; não altera status,
                // funcionário responsável nem o Cliente associado.
                item.MesaId = reserva.MesaId;
                item.DataHora = reserva.DataHora;
                item.QtdPessoas = reserva.QtdPessoas;
                break;
            }
        }
    }

    public void UpdateStatus(int id, string status)
    {
        // CORREÇÃO: aceita apenas os estados conhecidos e não reativa reservas canceladas.
        if (status != "Pendente" && status != "Confirmada" && status != "Cancelada")
        {
            return;
        }

        foreach (var item in lista)
        {
            if (item.Id == id && item.Status != "Cancelada")
            {
                item.Status = status;
                break;
            }
        }
    }

    public void Delete(int id)
    {
        foreach (var item in lista)
        {
            if (item.Id == id)
            {
                // CORREÇÃO: exclusão lógica; mantém o histórico e apenas cancela a reserva.
                item.Status = "Cancelada";
                break;
            }
        }
    }
}
