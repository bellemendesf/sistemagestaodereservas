// Importa os modelos do projeto
using GestaoReservas.Models;

// Interface IClienteRepository: define as operações que o repositório deve ter
public interface IClienteRepository
{
    // Retorna todos os registros
    List<Cliente> Read();
    // Retorna um registro pelo id
    Cliente? Read(int id);
    // Cadastra um registro
    void Create(Cliente cliente);
    // Altera um registro
    void Update(Cliente cliente);
    // Exclui um registro
    void Delete(int id);
}
