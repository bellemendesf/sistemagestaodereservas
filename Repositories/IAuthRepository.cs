// Importa os modelos do projeto
using GestaoReservas.Models;

// Contrato do repositório usado no login de funcionários
public interface IAuthRepository
{
    // Autentica por CPF/senha; retorna null se falhar ou não for funcionário
    Funcionario? AuthenticateFuncionario(string cpf, string senha);

    // Busca funcionário pelo id após login (sem expor senha)
    Funcionario? GetFuncionarioById(int id);
}

