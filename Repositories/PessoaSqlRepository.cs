using GestaoReservas.Models;
using Microsoft.Data.SqlClient;

public class PessoaSqlRepository : DataConnection, IPessoaRepository
{
    public PessoaSqlRepository(IConfiguration configuration) : base(configuration) { }

    public void Create(Pessoa pessoa)
    {
        // UNIÃO: persiste os campos de A nas tabelas relacionadas de B.
        using (var cmd = new SqlCommand())
        {
            cmd.Connection = conn;
            cmd.CommandText = @"INSERT INTO pessoas (nome, cpf, email_pessoal, senha) VALUES (@Nome, @Cpf, @Email_pessoal, @Senha);";
            cmd.Parameters.AddWithValue("@Nome", pessoa.Nome);
            cmd.Parameters.AddWithValue("@Cpf", pessoa.Cpf);
            cmd.Parameters.AddWithValue("@Email_pessoal", pessoa.EmailPessoal);
            cmd.Parameters.AddWithValue("@Senha", pessoa.Senha ?? "");

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

    public List<Pessoa> Read()
    {
        var pessoas = new List<Pessoa>();

        using (var cmd = new SqlCommand())
        {
            cmd.Connection = conn;
            cmd.CommandText = @"SELECT p.id, p.nome, p.cpf, p.email_pessoal, p.senha FROM pessoas p";

            conn.Open();
            try
            {
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
            }
            finally
            {
                conn.Close();
            }
        }

        return pessoas;
    }

    public Pessoa? Read(int id)
    {
        Pessoa? pessoa = null;

        using (var cmd = new SqlCommand())
        {
            cmd.Connection = conn;
            cmd.CommandText = @"SELECT p.id, p.nome, p.cpf, p.email_pessoal, p.senha FROM pessoas p WHERE p.id = @Id";
            cmd.Parameters.AddWithValue("@Id", id);

            conn.Open();
            try
            {
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
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
            }
            finally
            {
                conn.Close();
            }
        }

        return pessoa;
    }

    public void Update(Pessoa pessoa)
    {
        using (var cmd = new SqlCommand())
        {
            cmd.Connection = conn;
            cmd.CommandText = @"UPDATE pessoas SET nome = @Nome, cpf = @Cpf, email_pessoal = @Email_pessoal, senha = COALESCE(NULLIF(@Senha, ''), senha) WHERE id = @Id;";
            cmd.Parameters.AddWithValue("@Nome", pessoa.Nome);
            cmd.Parameters.AddWithValue("@Cpf", pessoa.Cpf);
            cmd.Parameters.AddWithValue("@Email_pessoal", pessoa.EmailPessoal);
            cmd.Parameters.AddWithValue("@Senha", pessoa.Senha ?? "");
            cmd.Parameters.AddWithValue("@Id", pessoa.Id);

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
            cmd.CommandText = @"DELETE FROM pessoas WHERE id = @Id;";
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
