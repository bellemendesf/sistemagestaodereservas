// Importa os modelos do projeto
using GestaoReservas.Models;
// Importa as classes de acesso ao SQL Server
using Microsoft.Data.SqlClient;

// Repositório que grava e lê Ambiente no SQL Server
public class AmbienteSqlRepository : DataConnection, IAmbienteRepository
{
    // Construtor: repassa a configuração para a classe base
    public AmbienteSqlRepository(IConfiguration configuration) : base(configuration) { }

    // Insere um novo registro no banco
    public void Create(Ambiente ambiente)
    {
        // Cria o comando SQL (descartado ao final)
        using (var cmd = new SqlCommand())
        {
            // Associa o comando à conexão
            cmd.Connection = conn;
            // Texto SQL que será executado
            cmd.CommandText = @"SET XACT_ABORT ON; BEGIN TRANSACTION;
                DECLARE @Codigo int = (SELECT ISNULL(MAX(codigo), 0) + 1 FROM ambientes WITH (UPDLOCK, HOLDLOCK));
                INSERT INTO ambientes (codigo, nome, status, capacidade) VALUES (@Codigo, @Nome, @Status, @Capacidade);
                COMMIT TRANSACTION;";
            // Preenche o parâmetro @Nome (evita SQL injection)
            cmd.Parameters.AddWithValue("@Nome", ambiente.Nome);
            // Preenche o parâmetro @Status (evita SQL injection)
            cmd.Parameters.AddWithValue("@Status", ambiente.Status ?? "");
            // Preenche o parâmetro @Capacidade (evita SQL injection)
            cmd.Parameters.AddWithValue("@Capacidade", ambiente.Capacidade);

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
    public List<Ambiente> Read()
    {
        // Lista que receberá os Ambiente
        var ambientes = new List<Ambiente>();

        // Cria o comando SQL (descartado ao final)
        using (var cmd = new SqlCommand())
        {
            // Associa o comando à conexão
            cmd.Connection = conn;
            // Texto SQL que será executado
            cmd.CommandText = @"SELECT codigo, nome, COALESCE(status, ''), capacidade FROM ambientes";

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
                        // Adiciona um Ambiente à lista
                        ambientes.Add(new Ambiente
                        {
                            // Lê a coluna 0 para Id
                            Id = reader.GetInt32(0),
                            // Lê a coluna 1 para Nome
                            Nome = reader.GetString(1),
                            // Lê a coluna 2 para Status
                            Status = reader.GetString(2),
                            // Lê a coluna 3 para Capacidade
                            Capacidade = reader.GetInt32(3)
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
        return ambientes;
    }

    // Lê um registro pelo id (null se não existir)
    public Ambiente? Read(int id)
    {
        // ambiente começa nulo (não encontrado)
        Ambiente? ambiente = null;

        // Cria o comando SQL (descartado ao final)
        using (var cmd = new SqlCommand())
        {
            // Associa o comando à conexão
            cmd.Connection = conn;
            // Texto SQL que será executado
            cmd.CommandText = @"SELECT codigo, nome, COALESCE(status, ''), capacidade FROM ambientes WHERE codigo = @Id";
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
                        // Monta o objeto Ambiente com os dados da linha
                        ambiente = new Ambiente
                        {
                            // Lê a coluna 0 para Id
                            Id = reader.GetInt32(0),
                            // Lê a coluna 1 para Nome
                            Nome = reader.GetString(1),
                            // Lê a coluna 2 para Status
                            Status = reader.GetString(2),
                            // Lê a coluna 3 para Capacidade
                            Capacidade = reader.GetInt32(3)
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
        return ambiente;
    }

    // Atualiza um registro existente no banco
    public void Update(Ambiente ambiente)
    {
        // Cria o comando SQL (descartado ao final)
        using (var cmd = new SqlCommand())
        {
            // Associa o comando à conexão
            cmd.Connection = conn;
            // Texto SQL que será executado
            cmd.CommandText = @"UPDATE ambientes SET nome = @Nome, status = @Status, capacidade = @Capacidade WHERE codigo = @Id;";
            // Preenche o parâmetro @Nome (evita SQL injection)
            cmd.Parameters.AddWithValue("@Nome", ambiente.Nome);
            // Preenche o parâmetro @Status (evita SQL injection)
            cmd.Parameters.AddWithValue("@Status", ambiente.Status ?? "");
            // Preenche o parâmetro @Capacidade (evita SQL injection)
            cmd.Parameters.AddWithValue("@Capacidade", ambiente.Capacidade);
            // Preenche o parâmetro @Id (evita SQL injection)
            cmd.Parameters.AddWithValue("@Id", ambiente.Id);

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
            cmd.CommandText = @"DELETE FROM ambientes WHERE codigo = @Id;";
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
