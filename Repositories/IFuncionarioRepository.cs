// Importa os modelos do projeto
using GestaoReservas.Models;

// Interface IFuncionarioRepository: define as operações que o repositório deve ter
public interface IFuncionarioRepository
{
    // Retorna todos os registros
    List<Funcionario> Read();
    // Retorna um registro pelo id
    Funcionario? Read(int id);
    // Cadastra um registro
    void Create(Funcionario funcionario);
    // Altera um registro
    void Update(Funcionario funcionario);
    // Exclui um registro
    void Delete(int id);
}
