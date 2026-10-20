using GestaoReservas.Models;
using Microsoft.Data.SqlClient;

public class FuncionarioSqlRepository : DataConnection, IFuncionarioRepository
{
    public FuncionarioSqlRepository(IConfiguration configuration) : base(configuration) { }

    public void Create(Funcionario funcionario)
    {
        // UNIÃO: persiste os campos de A nas tabelas relacionadas de B.
        using (var cmd = new SqlCommand())
        {
            cmd.Connection = conn;
            cmd.CommandText = @"SET XACT_ABORT ON; BEGIN TRANSACTION;
                INSERT INTO pessoas (nome, cpf, senha) VALUES (@Nome, @Cpf, @Senha);
                INSERT INTO funcionarios (pessoa_id, email_institucional) VALUES (CAST(SCOPE_IDENTITY() AS INT), @EmailInstitucional);
                COMMIT TRANSACTION;";
            cmd.Parameters.AddWithValue("@Nome", funcionario.Nome);
            cmd.Parameters.AddWithValue("@Cpf", funcionario.Cpf);
            cmd.Parameters.AddWithValue("@Senha", funcionario.Senha ?? "");
            cmd.Parameters.AddWithValue("@Email_Institucional", funcionario.EmailInstitucional);

            conn.Open();
            try
            {
                cmd.ExecuteNonQuery();
            }
            finally
            {
                conn.Close();
            }
        }
    }

    public List<Funcionario> Read()
    {
        var funcionarios = new List<Funcionario>();

        using (var cmd = new SqlCommand())
        {
            cmd.Connection = conn;
            cmd.CommandText = @"SELECT p.id, p.nome, p.cpf, p.senha, f.email_institucional FROM pessoas p INNER JOIN funcionarios f ON p.id = f.pessoa_id";

            conn.Open();
            try
            {
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        funcionarios.Add(new Funcionario
                        {
                            Id = reader.GetInt32(0),
                            Nome = reader.GetString(1),
                            Cpf = reader.GetString(2),
                            Senha = reader.GetString(3),
                            EmailInstitucional = reader.GetString(4)
                        });
                    }
                }
            }
            finally
            {
                conn.Close();
            }
        }

        return funcionarios;
    }

    public Funcionario? Read(int id)
    {
        Funcionario? funcionario = null;

        using (var cmd = new SqlCommand())
        {
            cmd.Connection = conn;
            cmd.CommandText = @"SELECT p.id, p.nome, p.cpf, p.senha, f.email_institucional FROM pessoas p INNER JOIN funcionarios f ON p.id = f.pessoa_id WHERE p.id = @Id";
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
                            Senha = reader.GetString(3),
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

    public void Update(Funcionario funcionario)
    {
        using (var cmd = new SqlCommand())
        {
            cmd.Connection = conn;
            cmd.CommandText = @"SET XACT_ABORT ON; BEGIN TRANSACTION;
                UPDATE pessoas SET nome = @Nome, cpf = @Cpf, senha = COALESCE(NULLIF(@Senha, ''), senha) WHERE id = @Id;
                UPDATE funcionarios SET email_institucional = @Email_Institucional WHERE pessoa_id = @Id;
                COMMIT TRANSACTION;";
            cmd.Parameters.AddWithValue("@Nome", funcionario.Nome);
            cmd.Parameters.AddWithValue("@Cpf", funcionario.Cpf);
            cmd.Parameters.AddWithValue("@Senha", funcionario.Senha ?? "");
            cmd.Parameters.AddWithValue("@Email_Institucional", funcionario.EmailInstitucional);
            cmd.Parameters.AddWithValue("@Id", funcionario.Id);

            conn.Open();
            try
            {
                cmd.ExecuteNonQuery();
            }
            finally
            {
                conn.Close();
            }
        }
    }

    public void Delete(int id)
    {
        using (var cmd = new SqlCommand())
        {
            cmd.Connection = conn;
            cmd.CommandText = @"SET XACT_ABORT ON; BEGIN TRANSACTION;
                DELETE FROM funcionarios WHERE pessoa_id = @Id;
                DELETE FROM pessoas WHERE id = @Id AND NOT EXISTS (SELECT 1 FROM clientes WHERE pessoa_id = @Id) AND NOT EXISTS (SELECT 1 FROM funcionarios WHERE pessoa_id = @Id);
                COMMIT TRANSACTION;";
            cmd.Parameters.AddWithValue("@Id", id);

            conn.Open();
            try
            {
                cmd.ExecuteNonQuery();
            }
            finally
            {
                conn.Close();
            }
        }
    }
}
