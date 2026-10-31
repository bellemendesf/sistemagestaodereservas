// Importa os modelos do projeto
using GestaoReservas.Models;
// Importa as classes de acesso ao SQL Server
using Microsoft.Data.SqlClient;

// Repositório que grava e lê Pessoa no SQL Server
public class PessoaSqlRepository : DataConnection, IPessoaRepository
{
    // Construtor: repassa a configuração para a classe base
    public PessoaSqlRepository(IConfiguration configuration) : base(configuration) { }

    // Insere um novo registro no banco
    public void Create(Pessoa pessoa)
    {
        // UNIÃO: persiste os campos de A nas tabelas relacionadas de B.
        using (var cmd = new SqlCommand())
        {
            // Associa o comando à conexão
            cmd.Connection = conn;
            // Texto SQL que será executado
            cmd.CommandText = @"INSERT INTO pessoas (nome, cpf, senha) VALUES (@Nome, @Cpf, @Senha);";
            // Preenche o parâmetro @Nome (evita SQL injection)
            cmd.Parameters.AddWithValue("@Nome", pessoa.Nome);
            // Preenche o parâmetro @Cpf (evita SQL injection)
            cmd.Parameters.AddWithValue("@Cpf", pessoa.Cpf);
            // Preenche o parâmetro @Senha (evita SQL injection)
            cmd.Parameters.AddWithValue("@Senha", pessoa.Senha ?? "");

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
    public List<Pessoa> Read()
    {
        // Lista que receberá os Pessoa
        var pessoas = new List<Pessoa>();

        // Cria o comando SQL (descartado ao final)
        using (var cmd = new SqlCommand())
        {
            // Associa o comando à conexão
            cmd.Connection = conn;
            // Texto SQL que será executado
            cmd.CommandText = @"SELECT p.id, p.nome, p.cpf, p.senha FROM pessoas p";

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
                        // Adiciona um Pessoa à lista
                        pessoas.Add(new Pessoa
                        {
                            // Lê a coluna 0 para Id
                            Id = reader.GetInt32(0),
                            // Lê a coluna 1 para Nome
                            Nome = reader.GetString(1),
                            // Lê a coluna 2 para Cpf
                            Cpf = reader.GetString(2),
                            // Lê a coluna 3 para Senha
                            Senha = reader.GetString(3)
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
        return pessoas;
    }

    // Lê um registro pelo id (null se não existir)
    public Pessoa? Read(int id)
    {
        // pessoa começa nulo (não encontrado)
        Pessoa? pessoa = null;

        // Cria o comando SQL (descartado ao final)
        using (var cmd = new SqlCommand())
        {
            // Associa o comando à conexão
            cmd.Connection = conn;
            // Texto SQL que será executado
            cmd.CommandText = @"SELECT p.id, p.nome, p.cpf, p.senha FROM pessoas p WHERE p.id = @Id";
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
                        // Monta o objeto Pessoa com os dados da linha
                        pessoa = new Pessoa
                        {
                            // Lê a coluna 0 para Id
                            Id = reader.GetInt32(0),
                            // Lê a coluna 1 para Nome
                            Nome = reader.GetString(1),
                            // Lê a coluna 2 para Cpf
                            Cpf = reader.GetString(2),
                            // Lê a coluna 3 para Senha
                            Senha = reader.GetString(3)
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
        return pessoa;
    }

    // Atualiza um registro existente no banco
    public void Update(Pessoa pessoa)
    {
        // Cria o comando SQL (descartado ao final)
        using (var cmd = new SqlCommand())
        {
            // Associa o comando à conexão
            cmd.Connection = conn;
            // Texto SQL que será executado
            cmd.CommandText = @"UPDATE pessoas SET nome = @Nome, cpf = @Cpf, senha = COALESCE(NULLIF(@Senha, ''), senha) WHERE id = @Id;";
            // Preenche o parâmetro @Nome (evita SQL injection)
            cmd.Parameters.AddWithValue("@Nome", pessoa.Nome);
            // Preenche o parâmetro @Cpf (evita SQL injection)
            cmd.Parameters.AddWithValue("@Cpf", pessoa.Cpf);
            // Preenche o parâmetro @Senha (evita SQL injection)
            cmd.Parameters.AddWithValue("@Senha", pessoa.Senha ?? "");
            // Preenche o parâmetro @Id (evita SQL injection)
            cmd.Parameters.AddWithValue("@Id", pessoa.Id);

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
            cmd.CommandText = @"DELETE FROM pessoas WHERE id = @Id;";
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
