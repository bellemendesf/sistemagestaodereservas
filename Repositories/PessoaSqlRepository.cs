using GestaoReservas.Models;
using Microsoft.Data.SqlClient;

public class PessoaSqlRepository : DataConnection, IPessoaRepository
{
    public PessoaSqlRepository(IConfiguration configuration) : base(configuration) { }

    public void Create(Pessoa pessoa)
    {
        using (var cmd = new SqlCommand())
        {
            cmd.Connection = conn;
            cmd.CommandText = @"
                INSERT INTO Pessoas (Nome, cpf, Email_pessoal, Senha) 
                VALUES (@Nome, @cpf, @Email_pessoal, @Senha);
                SELECT CAST(SCOPE_IDENTITY() AS INT);";
            cmd.Parameters.AddWithValue("@Nome", pessoa.Nome);
            cmd.Parameters.AddWithValue("@cpf", pessoa.Cpf);
            cmd.Parameters.AddWithValue("@Email_pessoal", pessoa.EmailPessoal);
            cmd.Parameters.AddWithValue("@Senha", pessoa.Senha);

            conn.Open();
            cmd.ExecuteNonQuery();
            conn.Close();
        }
    }

    public List<Pessoa> Read()
    {
        var pessoas = new List<Pessoa>();

        using (var cmd = new SqlCommand())
        {
            cmd.Connection = conn;
            cmd.CommandText = @"
                SELECT Id, Nome, Email, Senha, Cpf
                FROM Pessoa 
                ";

            conn.Open();
            using (SqlDataReader reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    pessoas.Add(new Pessoa
                    {
                        Id = reader.GetInt32(0),
                        Nome = reader.GetString(1),
                        Cpf = reader.GetString(2),
                        EmailPessoal = reader.GetString(3),
                        Senha = reader.GetString(4)
                       
                    });
                }
            }
            conn.Close();
        }

        return pessoas;
    }

    public Pessoa Read(int id)
    {
        Pessoa pessoa = null;

        using (var cmd = new SqlCommand())
        {
            cmd.Connection = conn;
            cmd.CommandText = @"
                SELECT Id, Nome, Email, Senha, Cpf
                FROM Pessoa 
                WHERE p.Id = @Id";
            cmd.Parameters.AddWithValue("@Id", id);

            conn.Open();
            using (SqlDataReader reader = cmd.ExecuteReader())
            {
                if (reader.Read())   // só UM registro esperado
                {
                    pessoa = new Pessoa
                    {
                        Id = reader.GetInt32(0),
                        Nome = reader.GetString(1),
                        Cpf = reader.GetString(2),
                        EmailPessoal = reader.GetString(3),
                        Senha = reader.GetString(4)
                        
                    };
                }
            }
            conn.Close();
        }

        return pessoa;
    }

    public void Update(Pessoa pessoa)
    {
        using (var cmdPessoa = new SqlCommand())
        {
            cmdPessoa.Connection = conn;
            cmdPessoa.CommandText = @"
                UPDATE Pessoas 
                SET Nome = @Nome, Cpf = @cpf, Email_pessoal = @Email_pessoal, Senha = @Senha 
                WHERE Id = @Id";
            cmdPessoa.Parameters.AddWithValue("@Nome", pessoa.Nome);
            cmdPessoa.Parameters.AddWithValue("@cpf", pessoa.Cpf);
            cmdPessoa.Parameters.AddWithValue("@Email", pessoa.EmailPessoal);
            cmdPessoa.Parameters.AddWithValue("@Senha", pessoa.Senha);
            cmdPessoa.Parameters.AddWithValue("@Id", pessoa.Id);

            conn.Open();
            cmdPessoa.ExecuteNonQuery();
            conn.Close();
        }
    }

    public void Delete(int id)
    {
        using (var cmdPessoa = new SqlCommand())
        {
            cmdPessoa.Connection = conn;
            cmdPessoa.CommandText = "DELETE FROM Pessoas WHERE Id = @Id";
            cmdPessoa.Parameters.AddWithValue("@Id", id);

            conn.Open();
            cmdPessoa.ExecuteNonQuery();  

            conn.Close();
        }
    }
}