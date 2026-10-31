// Importa os modelos do projeto
using GestaoReservas.Models;

// Interface IMesaRepository: define as operações que o repositório deve ter
public interface IMesaRepository
{
    // Retorna todos os registros
    List<Mesa> Read();
    // Retorna um registro pelo id
    Mesa? Read(int id);
    // Cadastra um registro
    void Create(Mesa mesa);
    // Altera um registro
    void Update(Mesa mesa);
    // Exclui um registro
    void Delete(int id);
}
