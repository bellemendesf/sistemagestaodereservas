using GestaoReservas.Models;

public interface IFuncionarioRepository
{
    List<Funcionario> Read();
    Funcionario Read(int id);
    void Create(Funcionario Funcionario);
    void Update(Funcionario Funcionario);
    void Delete(int id);
}