// Importa os modelos do projeto
using GestaoReservas.Models;
// Importa as classes de acesso ao SQL Server
using Microsoft.Data.SqlClient;
// Importa PasswordHasher para comparar senha informada com hash do banco
using Microsoft.AspNetCore.Identity;

// Repositório de autenticação: login de funcionário via ADO.NET
public class AuthSqlRepository : DataConnection, IAuthRepository
{
    // Hasher usado na verificação de senha no login
    private readonly PasswordHasher<Funcionario> _passwordHasher;

    // Construtor: abre conexão (DataConnection) e prepara o hasher
    public AuthSqlRepository(IConfiguration configuration) : base(configuration)
    {
        _passwordHasher = new PasswordHasher<Funcionario>();
    }

    // Valida CPF/senha e retorna Funcionario somente se existir na tabela funcionarios
    public Funcionario? AuthenticateFuncionario(string cpf, string senha)
    {
        Funcionario? funcionario = null;

        using (var cmd = new SqlCommand())
        {
            cmd.Connection = conn;
            // JOIN garante que a pessoa é funcionário (cliente com mesmo CPF não entra)
            cmd.CommandText = @"SELECT p.id, p.nome, p.cpf, p.senha, f.email_institucional 
                                FROM pessoas p 
                                INNER JOIN funcionarios f ON p.id = f.pessoa_id 
                                WHERE p.cpf = @Cpf";
            cmd.Parameters.AddWithValue("@Cpf", cpf);

            conn.Open();
            try
            {
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        var funcionarioTemp = new Funcionario
                        {
                            Id = reader.GetInt32(0),
                            Nome = reader.GetString(1),
                            Cpf = reader.GetString(2),
                            Senha = reader.GetString(3),
                            EmailInstitucional = reader.GetString(4)
                        };

                        var senhaArmazenada = funcionarioTemp.Senha;
                        var senhaValida = false;

                        // Hash ASP.NET Core (formato AQAAAA...)
                        if (senhaArmazenada.StartsWith("AQAAAA", StringComparison.Ordinal))
                        {
                            var result = _passwordHasher.VerifyHashedPassword(
                                funcionarioTemp, senhaArmazenada, senha);
                            senhaValida = result != PasswordVerificationResult.Failed;
                        }
                        else
                        {
                            // Compatibilidade temporária com bancos antigos em texto puro
                            senhaValida = senhaArmazenada == senha;
                        }

                        if (senhaValida)
                        {
                            funcionarioTemp.Senha = "";
                            funcionario = funcionarioTemp;
                        }
                    }
                }
            }
            finally
            {
                conn.Close();
            }
        }

        return funcionario;
    }

    // Busca funcionário autenticado pelo id (sem retornar senha)
    public Funcionario? GetFuncionarioById(int id)
    {
        Funcionario? funcionario = null;

        using (var cmd = new SqlCommand())
        {
            cmd.Connection = conn;
            cmd.CommandText = @"SELECT p.id, p.nome, p.cpf, p.senha, f.email_institucional 
                                FROM pessoas p 
                                INNER JOIN funcionarios f ON p.id = f.pessoa_id 
                                WHERE p.id = @Id";
            cmd.Parameters.AddWithValue("@Id", id);

            conn.Open();
            try
            {
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        funcionario = new Funcionario
                        {
                            Id = reader.GetInt32(0),
                            Nome = reader.GetString(1),
                            Cpf = reader.GetString(2),
                            Senha = "",
                            EmailInstitucional = reader.GetString(4)
                        };
                    }
                }
            }
            finally
            {
                conn.Close();
            }
        }

        return funcionario;
    }
}
