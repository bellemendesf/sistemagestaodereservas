using GestaoReservas.Models;

public interface IAmbienteRepository
{
    List<Ambiente> Read();
    Ambiente? Read(int id);
    void Create(Ambiente ambiente);
    void Update(Ambiente ambiente);
    void Delete(int id);
}
