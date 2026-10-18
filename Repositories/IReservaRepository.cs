using GestaoReservas.Models;

public interface IReservaRepository
{
    List<Reserva> Read();
    Reserva Read(int id);
    void Create(Reserva reserva);
    void Update(Reserva reserva);
    void Delete(int id);
}
