using GestaoReservas.Models;
using Microsoft.Data.SqlClient;

public class AuthSqlRepository : DataConnection, IAuthRepository
{
    public AuthSqlRepository(IConfiguration configuration) : base(configuration) { }

    public Funcionario? Login(LoginViewModel model) =>
        AutenticarFuncionario(model.Cpf, model.Senha);

    public Cliente? LoginCliente(LoginViewModel model) =>
        AutenticarCliente(model.Cpf, model.Senha);

    private Funcionario? AutenticarFuncionario(string cpf, string senha)
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

            if (!SenhaConfere(reader.GetString(3), senha))
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

    private Cliente? AutenticarCliente(string cpf, string senha)
    {
        using var cmd = new SqlCommand(
            @"SELECT p.id, p.nome, p.cpf, p.senha, c.email_pessoal, c.telefone
              FROM pessoas p
              INNER JOIN clientes c ON p.id = c.pessoa_id
              WHERE p.cpf = @Cpf", conn);
        cmd.Parameters.AddWithValue("@Cpf", cpf);

        conn.Open();
        try
        {
            using var reader = cmd.ExecuteReader();
            if (!reader.Read())
                return null;

            if (!SenhaConfere(reader.GetString(3), senha))
                return null;

            return new Cliente
            {
                Id = reader.GetInt32(0),
                Nome = reader.GetString(1),
                Cpf = reader.GetString(2),
                EmailPessoal = reader.GetString(4),
                Telefone = reader.GetString(5)
            };
        }
        finally
        {
            conn.Close();
        }
    }

    private static bool SenhaConfere(string armazenada, string informada) =>
        armazenada.StartsWith("AQAAAA", StringComparison.Ordinal)
            ? PasswordHasherUtil.VerifyPassword(armazenada, informada)
            : armazenada == informada;
}
