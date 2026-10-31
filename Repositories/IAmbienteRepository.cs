// Importa os modelos do projeto
using GestaoReservas.Models;

// Interface IAmbienteRepository: define as operações que o repositório deve ter
public interface IAmbienteRepository
{
    // Retorna todos os registros
    List<Ambiente> Read();
    // Retorna um registro pelo id
    Ambiente? Read(int id);
    // Cadastra um registro
    void Create(Ambiente ambiente);
    // Altera um registro
    void Update(Ambiente ambiente);
    // Exclui um registro
    void Delete(int id);
}
