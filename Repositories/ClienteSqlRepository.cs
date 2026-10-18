using GestaoReservas.Models;
using Microsoft.Data.SqlClient;

public class ClienteSqlRepository : DataConnection, IClienteRepository
{
    public ClienteSqlRepository(IConfiguration configuration) : base(configuration) { }

    public void Create(Cliente cliente)
    {
        using (var cmd = new SqlCommand())
        {
            cmd.Connection = conn;
            cmd.CommandText = @"
                INSERT INTO Pessoas (Nome, Email, Senha) 
                VALUES (@Nome, @Email, @Senha);
                SELECT CAST(SCOPE_IDENTITY() AS INT);";
            cmd.Parameters.AddWithValue("@Nome", cliente.Nome);
            cmd.Parameters.AddWithValue("@Email_pessooal", cliente.EmailPessoal);
            cmd.Parameters.AddWithValue("@Senha", cliente.Senha);

            conn.Open();
            int novoId = (int)cmd.ExecuteScalar();   // pega o Id gerado na tabela Pessoa

            using (var cmdCliente = new SqlCommand())
            {
                cmdCliente.Connection = conn;
                cmdCliente.CommandText = @"
                    INSERT INTO Clientes (Telefone) 
                    VALUES (@Telefone)";
                cmdCliente.Parameters.AddWithValue("@Id", novoId);   // usa o MESMO Id da Pessoa
                cmdCliente.Parameters.AddWithValue("@Telefone", cliente.Telefone);
                cmdCliente.ExecuteNonQuery();
            }

            conn.Close();
        }
    }

    public List<Cliente> Read()
    {
        var clientes = new List<Cliente>();

        using (var cmd = new SqlCommand())
        {
            cmd.Connection = conn;
            cmd.CommandText = @"
                SELECT p.Id, p.Nome, p.Email_pessoal, p.Senha, c.Cpf, c.Telefone 
                FROM Pessoas p, Clientes c WHERE p.Id = c.Id";

            conn.Open();
            using (SqlDataReader reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    clientes.Add(new Cliente
                    {
                        Id = reader.GetInt32(0),
                        Nome = reader.GetString(1),
                        EmailPessoal = reader.GetString(2),
                        Senha = reader.GetString(3),
                        Cpf = reader.GetString(4),
                        Telefone = reader.GetString(5)
                    });
                }
            }
            conn.Close();
        }

        return clientes;
    }

    public Cliente Read(int id)
    {
        Cliente cliente = null;

        using (var cmd = new SqlCommand())
        {
            cmd.Connection = conn;
            cmd.CommandText = @"
                SELECT p.Id, p.Nome, p.Email_pessoal, p.Senha, c.Cpf, c.Telefone 
                FROM Pessoas p, Clientes c WHERE p.Id = c.Id
                WHERE p.Id = @Id";
            cmd.Parameters.AddWithValue("@Id", id);

            conn.Open();
            using (SqlDataReader reader = cmd.ExecuteReader())
            {
                if (reader.Read())   // só UM registro esperado
                {
                    cliente = new Cliente
                    {
                        Id = reader.GetInt32(0),
                        Nome = reader.GetString(1),
                        EmailPessoal = reader.GetString(2),
                        Senha = reader.GetString(3),
                        Cpf = reader.GetString(4),
                        Telefone = reader.GetString(5)
                    };
                }
            }
            conn.Close();
        }

        return cliente;
    }

    public void Update(Cliente cliente)
    {
        using (var cmdPessoa = new SqlCommand())
        {
            cmdPessoa.Connection = conn;
            cmdPessoa.CommandText = @"
                UPDATE Pessoas 
                SET Nome = @Nome, Email_pessoal = @Email_pessoal, Senha = @Senha 
                WHERE Id = @Id";
            cmdPessoa.Parameters.AddWithValue("@Nome", cliente.Nome);
            cmdPessoa.Parameters.AddWithValue("@Email_pessoal", cliente.EmailPessoal);
            cmdPessoa.Parameters.AddWithValue("@Senha", cliente.Senha);
            cmdPessoa.Parameters.AddWithValue("@Id", cliente.Id);

            conn.Open();
            cmdPessoa.ExecuteNonQuery();

            using (var cmdCliente = new SqlCommand())
            {
                cmdCliente.Connection = conn;
                cmdCliente.CommandText = @"
                    UPDATE Clientes 
                    SET Telefone = @Telefone 
                    WHERE Id = @Id";
                cmdCliente.Parameters.AddWithValue("@Telefone", cliente.Telefone);
                cmdCliente.Parameters.AddWithValue("@Id", cliente.Id);
                cmdCliente.ExecuteNonQuery();
            }

            conn.Close();
        }
    }

    public void Delete(int id)
    {
        using (var cmdCliente = new SqlCommand())
        {
            cmdCliente.Connection = conn;
            cmdCliente.CommandText = "DELETE FROM Clientes WHERE Id = @Id";
            cmdCliente.Parameters.AddWithValue("@Id", id);

            conn.Open();
            cmdCliente.ExecuteNonQuery();   // apaga primeiro da tabela "filha" (Cliente)

            using (var cmdPessoa = new SqlCommand())
            {
                cmdPessoa.Connection = conn;
                cmdPessoa.CommandText = "DELETE FROM Pessoas WHERE Id = @Id";
                cmdPessoa.Parameters.AddWithValue("@Id", id);
                cmdPessoa.ExecuteNonQuery();   // só depois apaga da tabela "pai" (Pessoa)
            }

            conn.Close();
        }
    }
}