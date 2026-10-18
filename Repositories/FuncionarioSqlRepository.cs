using GestaoReservas.Models;
using Microsoft.Data.SqlClient;

public class FuncionarioSqlRepository : DataConnection, IFuncionarioRepository
{
    public FuncionarioSqlRepository(IConfiguration configuration) : base(configuration) { }

    public void Create(Funcionario funcionario)
    {
        using (var cmd = new SqlCommand())
        {
            cmd.Connection = conn;
            cmd.CommandText = @"
                INSERT INTO Pessoas (Nome, cpf, Email_pessoal, Senha) 
                VALUES (@Nome, @cpf, @Email_pessoal, @Senha);
                SELECT CAST(SCOPE_IDENTITY() AS INT);";
            cmd.Parameters.AddWithValue("@Nome", funcionario.Nome);
            cmd.Parameters.AddWithValue("@cpf", funcionario.Cpf);
            cmd.Parameters.AddWithValue("@Email", funcionario.EmailPessoal);
            cmd.Parameters.AddWithValue("@Senha", funcionario.Senha);

            conn.Open();
            int novoId = (int)cmd.ExecuteScalar();   // pega o Id gerado na tabela Pessoa

            using (var cmdFuncionario = new SqlCommand())
            {
                cmdFuncionario.Connection = conn;
                cmdFuncionario.CommandText = @"
                    INSERT INTO Funcionarios (email_institucuonal) 
                    VALUES (@email_institucional)";
                cmdFuncionario.Parameters.AddWithValue("@Id", novoId);   // usa o MESMO Id da Pessoa
                cmdFuncionario.Parameters.AddWithValue("@email_institucional", funcionario.EmailInstitucional);

                cmdFuncionario.ExecuteNonQuery();
            }

            conn.Close();
        }
    }

    public List<Funcionario> Read()
    {
        var funcionarios = new List<Funcionario>();

        using (var cmd = new SqlCommand())
        {
            cmd.Connection = conn;
            cmd.CommandText = @"
                SELECT p.Id, p.Nome, p.Email, p.Senha, p.Cpf, f.email_institucinal
                FROM Pessoas p, Funcionarios f WHERE p.Id = f.Id";

            conn.Open();
            using (SqlDataReader reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    funcionarios.Add(new Funcionario
                    {
                        Id = reader.GetInt32(0),
                        Nome = reader.GetString(1),
                        Cpf = reader.GetString(2),
                        EmailPessoal = reader.GetString(3),
                        Senha = reader.GetString(4),
                        EmailInstitucional = reader.GetString(5)
            
                    });
                }
            }
            conn.Close();
        }

        return funcionarios;
    }

    public Funcionario Read(int id)
    {
        Funcionario funcionario = null;

        using (var cmd = new SqlCommand())
        {
            cmd.Connection = conn;
            cmd.CommandText = @"
                SELECT p.Id, p.Nome, p.Email, p.Senha, p.Cpf, f.email_institucinal
                FROM Pessoas p, Funcionarios f WHERE p.Id = f.Id AND p.id = @id";
            cmd.Parameters.AddWithValue("@Id", id);

            conn.Open();
            using (SqlDataReader reader = cmd.ExecuteReader())
            {
                if (reader.Read())   // só UM registro esperado
                {
                    funcionario = new Funcionario
                    {
                        Id = reader.GetInt32(0),
                        Nome = reader.GetString(1),
                        Cpf = reader.GetString(2),
                        EmailPessoal = reader.GetString(3),
                        Senha = reader.GetString(4),
                        EmailInstitucional = reader.GetString(5)
                    };
                }
            }
            conn.Close();
        }

        return funcionario;
    }

    public void Update(Funcionario funcionario)
    {
        using (var cmdPessoa = new SqlCommand())
        {
            cmdPessoa.Connection = conn;
            cmdPessoa.CommandText = @"
                UPDATE Pessoas 
                SET Nome = @Nome, Cpf = @Cpf, Email = @Email, Senha = @Senha 
                WHERE Id = @Id";
            cmdPessoa.Parameters.AddWithValue("@Nome", funcionario.Nome);
            cmdPessoa.Parameters.AddWithValue("@Cpf", funcionario.Cpf);
            cmdPessoa.Parameters.AddWithValue("@Email", funcionario.EmailPessoal);
            cmdPessoa.Parameters.AddWithValue("@Senha", funcionario.Senha);
            cmdPessoa.Parameters.AddWithValue("@Id", funcionario.Id);

            conn.Open();
            cmdPessoa.ExecuteNonQuery();

            using (var cmdFuncionario = new SqlCommand())
            {
                cmdFuncionario.Connection = conn;
                cmdFuncionario.CommandText = @"
                    UPDATE Funcionarios
                    SET Email_institucional = @email_institucional 
                    WHERE Id = @Id";
                cmdFuncionario.Parameters.AddWithValue("@email_institucional", funcionario.EmailInstitucional);
                cmdFuncionario.Parameters.AddWithValue("@Id", funcionario.Id);
                cmdFuncionario.ExecuteNonQuery();
            }

            conn.Close();
        }
    }

    public void Delete(int id)
    {
        using (var cmdFuncionario = new SqlCommand())
        {
            cmdFuncionario.Connection = conn;
            cmdFuncionario.CommandText = "DELETE FROM Funcionarios WHERE Id = @Id";
            cmdFuncionario.Parameters.AddWithValue("@Id", id);

            conn.Open();
            cmdFuncionario.ExecuteNonQuery();   // apaga primeiro da tabela "filha" 

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