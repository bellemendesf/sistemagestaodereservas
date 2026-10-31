// Importa os modelos do projeto
using GestaoReservas.Models;
// Importa as classes de acesso ao SQL Server
using Microsoft.Data.SqlClient;

// Repositório que grava e lê Funcionario no SQL Server
public class FuncionarioSqlRepository : DataConnection, IFuncionarioRepository
{
    // Construtor: repassa a configuração para a classe base
    public FuncionarioSqlRepository(IConfiguration configuration) : base(configuration) { }

    // Insere um novo registro no banco
    public void Create(Funcionario funcionario)
    {
        // UNIÃO: persiste os campos de A nas tabelas relacionadas de B.
        using (var cmd = new SqlCommand())
        {
            // Associa o comando à conexão
            cmd.Connection = conn;
            // Texto SQL que será executado
            cmd.CommandText = @"SET XACT_ABORT ON; BEGIN TRANSACTION;
                INSERT INTO pessoas (nome, cpf, senha) VALUES (@Nome, @Cpf, @Senha);
                INSERT INTO funcionarios (pessoa_id, email_institucional) VALUES (CAST(SCOPE_IDENTITY() AS INT), @EmailInstitucional);
                COMMIT TRANSACTION;";
            // Preenche o parâmetro @Nome (evita SQL injection)
            cmd.Parameters.AddWithValue("@Nome", funcionario.Nome);
            // Preenche o parâmetro @Cpf (evita SQL injection)
            cmd.Parameters.AddWithValue("@Cpf", funcionario.Cpf);
            // Preenche o parâmetro @Senha (evita SQL injection)
            cmd.Parameters.AddWithValue("@Senha", funcionario.Senha ?? "");
            // Preenche o parâmetro @EmailInstitucional (evita SQL injection)
            cmd.Parameters.AddWithValue("@EmailInstitucional", funcionario.EmailInstitucional);

            // Abre a conexão com o banco
            conn.Open();
            // Tenta executar; erros são tratados abaixo
            try
            {
                // Executa o comando (INSERT/UPDATE/DELETE)
                cmd.ExecuteNonQuery();
            }
            // Sempre executa, com ou sem erro
            finally
            {
                // Fecha a conexão com o banco
                conn.Close();
            }
        }
    }

    // Lê todos os registros do banco
    public List<Funcionario> Read()
    {
        // Lista que receberá os Funcionario
        var funcionarios = new List<Funcionario>();

        // Cria o comando SQL (descartado ao final)
        using (var cmd = new SqlCommand())
        {
            // Associa o comando à conexão
            cmd.Connection = conn;
            // Texto SQL que será executado
            cmd.CommandText = @"SELECT p.id, p.nome, p.cpf, p.senha, f.email_institucional FROM pessoas p INNER JOIN funcionarios f ON p.id = f.pessoa_id";

            // Abre a conexão com o banco
            conn.Open();
            // Tenta executar; erros são tratados abaixo
            try
            {
                // Executa a consulta e abre o leitor de resultados
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    // Percorre cada linha retornada
                    while (reader.Read())
                    {
                        // Adiciona um Funcionario à lista
                        funcionarios.Add(new Funcionario
                        {
                            // Lê a coluna 0 para Id
                            Id = reader.GetInt32(0),
                            // Lê a coluna 1 para Nome
                            Nome = reader.GetString(1),
                            // Lê a coluna 2 para Cpf
                            Cpf = reader.GetString(2),
                            // Lê a coluna 3 para Senha
                            Senha = reader.GetString(3),
                            // Lê a coluna 4 para EmailInstitucional
                            EmailInstitucional = reader.GetString(4)
                        });
                    }
                }
            }
            // Sempre executa, com ou sem erro
            finally
            {
                // Fecha a conexão com o banco
                conn.Close();
            }
        }

        // Devolve o resultado
        return funcionarios;
    }

    // Lê um registro pelo id (null se não existir)
    public Funcionario? Read(int id)
    {
        // funcionario começa nulo (não encontrado)
        Funcionario? funcionario = null;

        // Cria o comando SQL (descartado ao final)
        using (var cmd = new SqlCommand())
        {
            // Associa o comando à conexão
            cmd.Connection = conn;
            // Texto SQL que será executado
            cmd.CommandText = @"SELECT p.id, p.nome, p.cpf, p.senha, f.email_institucional FROM pessoas p INNER JOIN funcionarios f ON p.id = f.pessoa_id WHERE p.id = @Id";
            // Preenche o parâmetro @Id (evita SQL injection)
            cmd.Parameters.AddWithValue("@Id", id);

            // Abre a conexão com o banco
            conn.Open();
            // Tenta executar; erros são tratados abaixo
            try
            {
                // Executa a consulta e abre o leitor de resultados
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    // Se encontrou a linha, monta o objeto
                    if (reader.Read())
                    {
                        // Monta o objeto Funcionario com os dados da linha
                        funcionario = new Funcionario
                        {
                            // Lê a coluna 0 para Id
                            Id = reader.GetInt32(0),
                            // Lê a coluna 1 para Nome
                            Nome = reader.GetString(1),
                            // Lê a coluna 2 para Cpf
                            Cpf = reader.GetString(2),
                            // Lê a coluna 3 para Senha
                            Senha = reader.GetString(3),
                            // Lê a coluna 4 para EmailInstitucional
                            EmailInstitucional = reader.GetString(4)
                        };
                    }
                }
            }
            // Sempre executa, com ou sem erro
            finally
            {
                // Fecha a conexão com o banco
                conn.Close();
            }
        }

        // Devolve o resultado
        return funcionario;
    }

    // Atualiza um registro existente no banco
    public void Update(Funcionario funcionario)
    {
        // Cria o comando SQL (descartado ao final)
        using (var cmd = new SqlCommand())
        {
            // Associa o comando à conexão
            cmd.Connection = conn;
            // Texto SQL que será executado
            cmd.CommandText = @"SET XACT_ABORT ON; BEGIN TRANSACTION;
                UPDATE pessoas SET nome = @Nome, cpf = @Cpf, senha = COALESCE(NULLIF(@Senha, ''), senha) WHERE id = @Id;
                UPDATE funcionarios SET email_institucional = @Email_Institucional WHERE pessoa_id = @Id;
                COMMIT TRANSACTION;";
            // Preenche o parâmetro @Nome (evita SQL injection)
            cmd.Parameters.AddWithValue("@Nome", funcionario.Nome);
            // Preenche o parâmetro @Cpf (evita SQL injection)
            cmd.Parameters.AddWithValue("@Cpf", funcionario.Cpf);
            // Preenche o parâmetro @Senha (evita SQL injection)
            cmd.Parameters.AddWithValue("@Senha", funcionario.Senha ?? "");
            // Preenche o parâmetro @Email_Institucional (evita SQL injection)
            cmd.Parameters.AddWithValue("@Email_Institucional", funcionario.EmailInstitucional);
            // Preenche o parâmetro @Id (evita SQL injection)
            cmd.Parameters.AddWithValue("@Id", funcionario.Id);

            // Abre a conexão com o banco
            conn.Open();
            // Tenta executar; erros são tratados abaixo
            try
            {
                // Executa o comando (INSERT/UPDATE/DELETE)
                cmd.ExecuteNonQuery();
            }
            // Sempre executa, com ou sem erro
            finally
            {
                // Fecha a conexão com o banco
                conn.Close();
            }
        }
    }

    // Remove (ou cancela) um registro no banco
    public void Delete(int id)
    {
        // Cria o comando SQL (descartado ao final)
        using (var cmd = new SqlCommand())
        {
            // Associa o comando à conexão
            cmd.Connection = conn;
            // Texto SQL que será executado
            cmd.CommandText = @"SET XACT_ABORT ON; BEGIN TRANSACTION;
                DELETE FROM funcionarios WHERE pessoa_id = @Id;
                DELETE FROM pessoas WHERE id = @Id AND NOT EXISTS (SELECT 1 FROM clientes WHERE pessoa_id = @Id) AND NOT EXISTS (SELECT 1 FROM funcionarios WHERE pessoa_id = @Id);
                COMMIT TRANSACTION;";
            // Preenche o parâmetro @Id (evita SQL injection)
            cmd.Parameters.AddWithValue("@Id", id);

            // Abre a conexão com o banco
            conn.Open();
            // Tenta executar; erros são tratados abaixo
            try
            {
                // Executa o comando (INSERT/UPDATE/DELETE)
                cmd.ExecuteNonQuery();
            }
            // Sempre executa, com ou sem erro
            finally
            {
                // Fecha a conexão com o banco
                conn.Close();
            }
        }
    }
}
