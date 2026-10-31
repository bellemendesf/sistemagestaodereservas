// Importa os modelos do projeto
using GestaoReservas.Models;

// Interface IReservaRepository: define as operações que o repositório deve ter
public interface IReservaRepository
{
    // Retorna todos os registros
    List<Reserva> Read();
    // Retorna um registro pelo id
    Reserva? Read(int id);
    // Cadastra um registro
    void Create(Reserva reserva);
    // Altera um registro
    void Update(Reserva reserva);
    // Exclui um registro
    void Delete(int id);

    // CORREÇÃO: separa a alteração de status (funcionário) da edição dos dados (cliente).
    void UpdateStatus(int id, string status);
}
