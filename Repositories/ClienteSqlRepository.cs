// Importa os modelos do projeto
using GestaoReservas.Models;
// Importa as classes de acesso ao SQL Server
using Microsoft.Data.SqlClient;

// Repositório que grava e lê Cliente no SQL Server
public class ClienteSqlRepository : DataConnection, IClienteRepository
{
    // Construtor: repassa a configuração para a classe base
    public ClienteSqlRepository(IConfiguration configuration) : base(configuration) { }

    // Insere cliente: reutiliza pessoa existente (ex.: funcionário com mesmo CPF)
    public void Create(Cliente cliente)
    {
        conn.Open();
        try
        {
            int? pessoaId = ObterPessoaIdPorCpf(cliente.Cpf);

            using var tx = conn.BeginTransaction();
            try
            {
                if (pessoaId == null)
                {
                    // CORREÇÃO: a senha não fica mais em pessoas (vai para clientes.senha_hash)
                    using (var cmdPessoa = new SqlCommand(
                               @"INSERT INTO pessoas (nome, cpf) VALUES (@Nome, @Cpf);
                                 SELECT CAST(SCOPE_IDENTITY() AS INT);", conn, tx))
                    {
                        cmdPessoa.Parameters.AddWithValue("@Nome", cliente.Nome);
                        cmdPessoa.Parameters.AddWithValue("@Cpf", cliente.Cpf);
                        cmdPessoa.Parameters.AddWithValue("@Senha", ""); // script novo: pessoas não tem coluna senha (parâmetro sem uso)
                        pessoaId = (int)cmdPessoa.ExecuteScalar()!;
                    }
                }
                else
                {
                    if (JaEhCliente(pessoaId.Value, tx))
                        throw new InvalidOperationException("Este CPF já está cadastrado como cliente.");

                    using (var cmdNome = new SqlCommand(
                               "UPDATE pessoas SET nome = @Nome WHERE id = @Id", conn, tx))
                    {
                        cmdNome.Parameters.AddWithValue("@Nome", cliente.Nome);
                        cmdNome.Parameters.AddWithValue("@Id", pessoaId.Value);
                        cmdNome.ExecuteNonQuery();
                    }
                }

                using (var cmdCliente = new SqlCommand(
                           @"INSERT INTO clientes (pessoa_id, email_pessoal, telefone, senha_hash)
                             VALUES (@PessoaId, @Email_pessoal, @Telefone, @SenhaHash)", conn, tx))
                {
                    // CORREÇÃO: grava o hash da senha do cliente na própria tabela clientes (CPF novo ou já existente)
                    cmdCliente.Parameters.AddWithValue("@SenhaHash", PasswordHasherUtil.HashPassword(cliente.Senha ?? ""));
                    cmdCliente.Parameters.AddWithValue("@PessoaId", pessoaId.Value);
                    cmdCliente.Parameters.AddWithValue("@Email_pessoal", cliente.EmailPessoal);
                    cmdCliente.Parameters.AddWithValue("@Telefone", cliente.Telefone);
                    cmdCliente.ExecuteNonQuery();
                }

                tx.Commit();
            }
            catch
            {
                tx.Rollback();
                throw;
            }
        }
        finally
        {
            conn.Close();
        }
    }

    // Busca id da pessoa pelo CPF (null se não existir)
    private int? ObterPessoaIdPorCpf(string cpf)
    {
        using var cmd = new SqlCommand("SELECT id FROM pessoas WHERE cpf = @Cpf", conn);
        cmd.Parameters.AddWithValue("@Cpf", cpf);
        var resultado = cmd.ExecuteScalar();
        return resultado == null || resultado == DBNull.Value ? null : Convert.ToInt32(resultado);
    }

    // Verifica se a pessoa já possui cadastro de cliente
    private static bool JaEhCliente(int pessoaId, SqlTransaction tx)
    {
        using var cmd = new SqlCommand("SELECT 1 FROM clientes WHERE pessoa_id = @Id", tx.Connection, tx);
        cmd.Parameters.AddWithValue("@Id", pessoaId);
        return cmd.ExecuteScalar() != null;
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
            cmd.CommandText = @"SELECT Codigo, Nome, CPF, Email, '', Telefone FROM v_clientes"; // VIEW v_clientes do script (o hash da senha não sai do banco)

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
            cmd.CommandText = @"SELECT Codigo, Nome, CPF, Email, '', Telefone FROM v_clientes WHERE Codigo = @Id"; // VIEW v_clientes do script
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
                UPDATE pessoas SET nome = @Nome, cpf = @Cpf WHERE id = @Id;
                UPDATE clientes SET telefone = @Telefone, email_pessoal = @Email_pessoal, senha_hash = COALESCE(NULLIF(@Senha, ''), senha_hash) WHERE pessoa_id = @Id;
                COMMIT TRANSACTION;";
            // Preenche o parâmetro @Nome (evita SQL injection)
            cmd.Parameters.AddWithValue("@Nome", cliente.Nome);
            // Preenche o parâmetro @Cpf (evita SQL injection)
            cmd.Parameters.AddWithValue("@Cpf", cliente.Cpf);
            // Preenche o parâmetro @Email_pessoal (evita SQL injection)
            cmd.Parameters.AddWithValue("@Email_pessoal", cliente.EmailPessoal);
            // Preenche o parâmetro @Senha (evita SQL injection)
            // CORREÇÃO: nova senha vira hash (antes ia em texto puro); vazia mantém a atual
            cmd.Parameters.AddWithValue("@Senha", string.IsNullOrWhiteSpace(cliente.Senha) ? "" : PasswordHasherUtil.HashPassword(cliente.Senha));
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
