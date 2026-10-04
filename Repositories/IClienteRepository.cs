using GestaoReservas.Models;

public interface IClienteRepository
{
    List<Cliente> Read();
    Cliente Read(int id);
    void Create(Cliente cliente);
    void Update(Cliente cliente);
    void Delete(int id);
}