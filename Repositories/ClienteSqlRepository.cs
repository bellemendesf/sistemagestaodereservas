// Importa os modelos do projeto
using GestaoReservas.Models;
// Importa as classes de acesso ao SQL Server
using Microsoft.Data.SqlClient;

// Repositório que grava e lê Cliente no SQL Server
public class ClienteSqlRepository : DataConnection, IClienteRepository
{
    // Construtor: repassa a configuração para a classe base
    public ClienteSqlRepository(IConfiguration configuration) : base(configuration) { }

    // Insere um novo registro no banco
    public void Create(Cliente cliente)
    {
        // UNIÃO: persiste os campos de A nas tabelas relacionadas de B.
        using (var cmd = new SqlCommand())
        {
            // Associa o comando à conexão
            cmd.Connection = conn;
            // Texto SQL que será executado
            cmd.CommandText = @"SET XACT_ABORT ON; BEGIN TRANSACTION;
                INSERT INTO pessoas (nome, cpf, senha) VALUES (@Nome, @Cpf,  @Senha);
                INSERT INTO clientes (pessoa_id, email_pessoal, telefone) VALUES (CAST(SCOPE_IDENTITY() AS INT), @Email_pessoal, @Telefone);
                COMMIT TRANSACTION;";
            // Preenche o parâmetro @Nome (evita SQL injection)
            cmd.Parameters.AddWithValue("@Nome", cliente.Nome);
            // Preenche o parâmetro @Cpf (evita SQL injection)
            cmd.Parameters.AddWithValue("@Cpf", cliente.Cpf);
            // Preenche o parâmetro @Email_pessoal (evita SQL injection)
            cmd.Parameters.AddWithValue("@Email_pessoal", cliente.EmailPessoal);
            // Preenche o parâmetro @Senha (evita SQL injection)
            cmd.Parameters.AddWithValue("@Senha", cliente.Senha ?? "");
            // Preenche o parâmetro @Telefone (evita SQL injection)
            cmd.Parameters.AddWithValue("@Telefone", cliente.Telefone);

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
    public List<Cliente> Read()
    {
        // Lista que receberá os Cliente
        var clientes = new List<Cliente>();

        // Cria o comando SQL (descartado ao final)
        using (var cmd = new SqlCommand())
        {
            // Associa o comando à conexão
            cmd.Connection = conn;
            // Texto SQL que será executado
            cmd.CommandText = @"SELECT p.id, p.nome, p.cpf, c.email_pessoal, p.senha, c.telefone FROM pessoas p INNER JOIN clientes c ON p.id = c.pessoa_id";

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
                        // Adiciona um Cliente à lista
                        clientes.Add(new Cliente
                        {
                            // Lê a coluna 0 para Id
                            Id = reader.GetInt32(0),
                            // Lê a coluna 1 para Nome
                            Nome = reader.GetString(1),
                            // Lê a coluna 2 para Cpf
                            Cpf = reader.GetString(2),
                            // Lê a coluna 3 para EmailPessoal
                            EmailPessoal = reader.GetString(3),
                            // Lê a coluna 4 para Senha
                            Senha = reader.GetString(4),
                            // Lê a coluna 5 para Telefone
                            Telefone = reader.GetString(5)
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
        return clientes;
    }

    // Lê um registro pelo id (null se não existir)
    public Cliente? Read(int id)
    {
        // cliente começa nulo (não encontrado)
        Cliente? cliente = null;

        // Cria o comando SQL (descartado ao final)
        using (var cmd = new SqlCommand())
        {
            // Associa o comando à conexão
            cmd.Connection = conn;
            // Texto SQL que será executado
            cmd.CommandText = @"SELECT p.id, p.nome, p.cpf, c.email_pessoal, p.senha, c.telefone FROM pessoas p INNER JOIN clientes c ON p.id = c.pessoa_id WHERE p.id = @Id";
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
                        // Monta o objeto Cliente com os dados da linha
                        cliente = new Cliente
                        {
                            // Lê a coluna 0 para Id
                            Id = reader.GetInt32(0),
                            // Lê a coluna 1 para Nome
                            Nome = reader.GetString(1),
                            // Lê a coluna 2 para Cpf
                            Cpf = reader.GetString(2),
                            // Lê a coluna 3 para EmailPessoal
                            EmailPessoal = reader.GetString(3),
                            // Lê a coluna 4 para Senha
                            Senha = reader.GetString(4),
                            // Lê a coluna 5 para Telefone
                            Telefone = reader.GetString(5)
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
        return cliente;
    }

    // Atualiza um registro existente no banco
    public void Update(Cliente cliente)
    {
        // Cria o comando SQL (descartado ao final)
        using (var cmd = new SqlCommand())
        {
            // Associa o comando à conexão
            cmd.Connection = conn;
            // Texto SQL que será executado
            cmd.CommandText = @"SET XACT_ABORT ON; BEGIN TRANSACTION;
                UPDATE pessoas SET nome = @Nome, cpf = @Cpf, senha = COALESCE(NULLIF(@Senha, ''), senha) WHERE id = @Id;
                UPDATE clientes SET telefone = @Telefone, email_pessoal = @Email_pessoal WHERE pessoa_id = @Id;
                COMMIT TRANSACTION;";
            // Preenche o parâmetro @Nome (evita SQL injection)
            cmd.Parameters.AddWithValue("@Nome", cliente.Nome);
            // Preenche o parâmetro @Cpf (evita SQL injection)
            cmd.Parameters.AddWithValue("@Cpf", cliente.Cpf);
            // Preenche o parâmetro @Email_pessoal (evita SQL injection)
            cmd.Parameters.AddWithValue("@Email_pessoal", cliente.EmailPessoal);
            // Preenche o parâmetro @Senha (evita SQL injection)
            cmd.Parameters.AddWithValue("@Senha", cliente.Senha ?? "");
            // Preenche o parâmetro @Telefone (evita SQL injection)
            cmd.Parameters.AddWithValue("@Telefone", cliente.Telefone);
            // Preenche o parâmetro @Id (evita SQL injection)
            cmd.Parameters.AddWithValue("@Id", cliente.Id);

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
                DELETE FROM clientes WHERE pessoa_id = @Id;
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
