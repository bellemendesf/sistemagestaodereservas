using GestaoReservas.Models;
using Microsoft.Data.SqlClient;

public class AuthSqlRepository : DataConnection, IAuthRepository
{
    public AuthSqlRepository(IConfiguration configuration) : base(configuration) { }

    public Funcionario? Login(string cpf, string senha)
    {
        using var cmd = new SqlCommand(
            @"SELECT p.id, p.nome, p.cpf, p.senha, f.email_institucional
              FROM pessoas p
              INNER JOIN funcionarios f ON p.id = f.pessoa_id
              WHERE p.cpf = @Cpf", conn);
        cmd.Parameters.AddWithValue("@Cpf", cpf);

        conn.Open();
        try
        {
            using var reader = cmd.ExecuteReader();
            if (!reader.Read())
                return null;

            var hash = reader.GetString(3);
            var ok = hash.StartsWith("AQAAAA", StringComparison.Ordinal)
                ? PasswordHasherUtil.VerifyPassword(hash, senha)
                : hash == senha;

            if (!ok)
                return null;

            return new Funcionario
            {
                Id = reader.GetInt32(0),
                Nome = reader.GetString(1),
                Cpf = reader.GetString(2),
                EmailInstitucional = reader.GetString(4)
            };
        }
        finally
        {
            conn.Close();
        }
    }
}
