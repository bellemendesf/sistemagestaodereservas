using GestaoReservas.Models;

public interface IReservaRepository
{
    List<Reserva> Read();
    Reserva Read(int id);
    void Create(Reserva Reserva);
    void Update(Reserva Reserva);
    void Delete(int id);
}