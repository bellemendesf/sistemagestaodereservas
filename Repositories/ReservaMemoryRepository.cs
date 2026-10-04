public class ReservaMemoryRepository : IReservaRepository
{
    List<Reserva> lista = new List<Reserva>();

    public void Create(Reserva Reserva)
    {
        lista.Add(Reserva);
    }

    public void Delete(int id)
    {
        foreach(var Reserva in lista)
        {
            if(Reserva.IdReserva == id)
            {
                lista.Remove(Reserva);
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
        Reserva r = null;
        foreach(var Reserva in lista)
        {
            if(Reserva.IdReserva == id)
            {
                r = Reserva;
            }
        }
        return r;
    }

    public void Update(Reserva r)
    {
        foreach(var Reserva in lista)
        {
            if(Reserva.IdReserva == r.IdReserva)
            {
                Reserva.Texto = r.Texto;
            }
        }
    }
}