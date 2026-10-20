using GestaoReservas.Models;
using Microsoft.Data.SqlClient;

public class ClienteSqlRepository : DataConnection, IClienteRepository
{
    public ClienteSqlRepository(IConfiguration configuration) : base(configuration) { }

    public void Create(Cliente cliente)
    {
        // UNIÃO: persiste os campos de A nas tabelas relacionadas de B.
        using (var cmd = new SqlCommand())
        {
            cmd.Connection = conn;
            cmd.CommandText = @"SET XACT_ABORT ON; BEGIN TRANSACTION;
                INSERT INTO pessoas (nome, cpf, senha) VALUES (@Nome, @Cpf,  @Senha);
                INSERT INTO clientes (pessoa_id, email_pessoal, telefone) VALUES (CAST(SCOPE_IDENTITY() AS INT), @Email_pessoal, @Telefone);
                COMMIT TRANSACTION;";
            cmd.Parameters.AddWithValue("@Nome", cliente.Nome);
            cmd.Parameters.AddWithValue("@Cpf", cliente.Cpf);
            cmd.Parameters.AddWithValue("@Email_pessoal", cliente.EmailPessoal);
            cmd.Parameters.AddWithValue("@Senha", cliente.Senha ?? "");
            cmd.Parameters.AddWithValue("@Telefone", cliente.Telefone);

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

    public List<Cliente> Read()
    {
        var clientes = new List<Cliente>();

        using (var cmd = new SqlCommand())
        {
            cmd.Connection = conn;
            cmd.CommandText = @"SELECT p.id, p.nome, p.cpf, p.email_pessoal, p.senha, t.telefone FROM pessoas p INNER JOIN clientes t ON p.id = t.pessoa_id";

            conn.Open();
            try
            {
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        clientes.Add(new Cliente
                        {
                            Id = reader.GetInt32(0),
                            Nome = reader.GetString(1),
                            Cpf = reader.GetString(2),
                            EmailPessoal = reader.GetString(3),
                            Senha = reader.GetString(4),
                            Telefone = reader.GetString(5)
                        });
                    }
                }
            }
            finally
            {
                conn.Close();
            }
        }

        return clientes;
    }

    public Cliente? Read(int id)
    {
        Cliente? cliente = null;

        using (var cmd = new SqlCommand())
        {
            cmd.Connection = conn;
            cmd.CommandText = @"SELECT p.id, p.nome, p.cpf, c.email_pessoal, p.senha, t.telefone FROM pessoas p INNER JOIN clientes t ON p.id = t.pessoa_id WHERE p.id = @Id";
            cmd.Parameters.AddWithValue("@Id", id);

            conn.Open();
            try
            {
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        cliente = new Cliente
                        {
                            Id = reader.GetInt32(0),
                            Nome = reader.GetString(1),
                            Cpf = reader.GetString(2),
                            EmailPessoal = reader.GetString(3),
                            Senha = reader.GetString(4),
                            Telefone = reader.GetString(5)
                        };
                    }
                }
            }
            finally
            {
                conn.Close();
            }
        }

        return cliente;
    }

    public void Update(Cliente cliente)
    {
        using (var cmd = new SqlCommand())
        {
            cmd.Connection = conn;
            cmd.CommandText = @"SET XACT_ABORT ON; BEGIN TRANSACTION;
                UPDATE pessoas SET nome = @Nome, cpf = @Cpf, senha = COALESCE(NULLIF(@Senha, ''), senha) WHERE id = @Id;
                UPDATE clientes SET telefone = @Telefone, email_pessoal = @Email_pessoal, WHERE pessoa_id = @Id;
                COMMIT TRANSACTION;";
            cmd.Parameters.AddWithValue("@Nome", cliente.Nome);
            cmd.Parameters.AddWithValue("@Cpf", cliente.Cpf);
            cmd.Parameters.AddWithValue("@Email_pessoal", cliente.EmailPessoal);
            cmd.Parameters.AddWithValue("@Senha", cliente.Senha ?? "");
            cmd.Parameters.AddWithValue("@Telefone", cliente.Telefone);
            cmd.Parameters.AddWithValue("@Id", cliente.Id);

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
                DELETE FROM clientes WHERE pessoa_id = @Id;
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
