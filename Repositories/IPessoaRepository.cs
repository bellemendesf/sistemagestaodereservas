using GestaoReservas.Models;

public interface IPessoaRepository
{
    List<Pessoa> Read();
    Pessoa Read(int id);
    void Create(Pessoa pessoa);
    void Update(Pessoa pessoa);
    void Delete(int id);
}
