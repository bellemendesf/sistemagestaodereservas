// Importa os modelos do projeto
using GestaoReservas.Models;

// Interface IPessoaRepository: define as operações que o repositório deve ter
public interface IPessoaRepository
{
    // Retorna todos os registros
    List<Pessoa> Read();
    // Retorna um registro pelo id
    Pessoa? Read(int id);
    // Cadastra um registro
    void Create(Pessoa pessoa);
    // Altera um registro
    void Update(Pessoa pessoa);
    // Exclui um registro
    void Delete(int id);
}
