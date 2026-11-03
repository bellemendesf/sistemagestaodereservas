// Importa os utilitários de hash de senha do ASP.NET Core
using Microsoft.AspNetCore.Identity;
// Importa os modelos do projeto
using GestaoReservas.Models;

// Classe utilitária para gerar hashes de senha
// Esta classe pode ser usada para migrar senhas existentes ou gerar novos hashes
public static class PasswordHasherUtil
{
    // Instância estática do PasswordHasher
    private static readonly PasswordHasher<Funcionario> _hasher = new PasswordHasher<Funcionario>();

    // Método que gera um hash de senha
    // Recebe a senha em texto puro e retorna o hash
    public static string HashPassword(string plainPassword)
    {
        // Cria um objeto Funcionario temporário apenas para usar o PasswordHasher
        var funcionarioTemp = new Funcionario();
        
        // Gera o hash da senha usando o PasswordHasher
        var hashedPassword = _hasher.HashPassword(funcionarioTemp, plainPassword);
        
        // Retorna o hash da senha
        return hashedPassword;
    }

    // Método que verifica se uma senha corresponde a um hash
    // Recebe o hash armazenado e a senha em texto puro para comparar
    public static bool VerifyPassword(string hash, string plainPassword)
    {
        // Cria um objeto Funcionario temporário apenas para usar o PasswordHasher
        var funcionarioTemp = new Funcionario { Senha = hash };
        
        // Verifica se a senha corresponde ao hash
        var result = _hasher.VerifyHashedPassword(funcionarioTemp, hash, plainPassword);
        
        // Retorna true se a senha for válida (Success ou SuccessRehashNeeded)
        return result != PasswordVerificationResult.Failed;
    }
}
