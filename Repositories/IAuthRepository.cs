using GestaoReservas.Models;

public interface IAuthRepository
{
    Funcionario? Login(string cpf, string senha);
}

