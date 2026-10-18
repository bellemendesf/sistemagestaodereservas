using GestaoReservas.Models;

public interface IMesaRepository
{
    List<Mesa> Read();
    Mesa Read(int id);
    void Create(Mesa mesa);
    void Update(Mesa mesa);
    void Delete(int id);
}
