using GestaoReservas.Models;

public interface IAuthRepository
{
    Funcionario? Login(LoginViewModel model);
    Cliente? LoginCliente(LoginViewModel model);
}

