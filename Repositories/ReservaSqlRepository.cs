// Importa os modelos do projeto
using GestaoReservas.Models;
// Importa as classes de acesso ao SQL Server
using Microsoft.Data.SqlClient;

// Repositório que grava e lê Reserva no SQL Server
public class ReservaSqlRepository : DataConnection, IReservaRepository
{
    // Construtor: repassa a configuração para a classe base
    public ReservaSqlRepository(IConfiguration configuration) : base(configuration) { }

    // Insere um novo registro no banco
    public void Create(Reserva reserva)
    {
        // UNIÃO: liga o Cliente recebido por A ao cadastro e à reserva SQL de B.
        using (var cmd = new SqlCommand())
        {
            // Associa o comando à conexão
            cmd.Connection = conn;
            // Texto SQL que será executado
            cmd.CommandText = @"SET XACT_ABORT ON; BEGIN TRANSACTION;
                DECLARE @PessoaId int;
                SELECT @PessoaId = id FROM pessoas WITH (UPDLOCK, HOLDLOCK) WHERE cpf = @Cpf;
                IF @PessoaId IS NULL
                BEGIN
                    INSERT INTO pessoas (nome, cpf, senha) VALUES (@Nome, @Cpf, '');
                    SET @PessoaId = CAST(SCOPE_IDENTITY() AS INT);
                END;
                IF NOT EXISTS (SELECT 1 FROM clientes WHERE pessoa_id = @PessoaId)
                    INSERT INTO clientes (pessoa_id, email_pessoal, telefone) VALUES (@PessoaId, @Email_pessoal, @Telefone);
                INSERT INTO reservas (qtd_pessoas, data_hora, status, mesas_nr, cliente_id)
                VALUES (@QtdPessoas, @DataHora, 0, @MesaId, @PessoaId);
                DECLARE @ReservaId int = CAST(SCOPE_IDENTITY() AS INT);
                INSERT INTO Reservas_Mesas (numero_reservas, nr_mesas) VALUES (@ReservaId, @MesaId);
                COMMIT TRANSACTION;
                SELECT @ReservaId;";
            // Preenche o parâmetro @Nome (evita SQL injection)
            cmd.Parameters.AddWithValue("@Nome", reserva.Cliente.Nome);
            // Preenche o parâmetro @Cpf (evita SQL injection)
            cmd.Parameters.AddWithValue("@Cpf", reserva.Cliente.Cpf);
            // Preenche o parâmetro @Email_pessoal (evita SQL injection)
            cmd.Parameters.AddWithValue("@Email_pessoal", reserva.Cliente.EmailPessoal ?? "");
            // Preenche o parâmetro @Telefone (evita SQL injection)
            cmd.Parameters.AddWithValue("@Telefone", reserva.Cliente.Telefone);
            // Preenche o parâmetro @QtdPessoas (evita SQL injection)
            cmd.Parameters.AddWithValue("@QtdPessoas", reserva.QtdPessoas);
            // Preenche o parâmetro @DataHora (evita SQL injection)
            cmd.Parameters.AddWithValue("@DataHora", reserva.DataHora);
            // Preenche o parâmetro @MesaId (evita SQL injection)
            cmd.Parameters.AddWithValue("@MesaId", reserva.MesaId);

            // Abre a conexão com o banco
            conn.Open();
            // Tenta executar; erros são tratados abaixo
            try
            {
                // ALTERAÇÃO: guarda o número gerado para mostrar a reserva (com status) após o cadastro.
                reserva.Id = Convert.ToInt32(cmd.ExecuteScalar());
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
    public List<Reserva> Read()
    {
        // Lista que receberá os Reserva
        var reservas = new List<Reserva>();

        // Cria o comando SQL (descartado ao final)
        using (var cmd = new SqlCommand())
        {
            // Associa o comando à conexão
            cmd.Connection = conn;
            // Texto SQL que será executado
            cmd.CommandText = @"SELECT r.numero, r.qtd_pessoas,
                r.data_hora,
                CASE r.status WHEN 0 THEN 'Pendente' WHEN 1 THEN 'Confirmada' WHEN 2 THEN 'Cancelada' ELSE 'Pendente' END, -- CORREÇÃO: status nulo vira Pendente
                r.mesas_nr, r.cliente_id, p.nome, p.cpf, c.email_pessoal, c.telefone
                FROM reservas r
                INNER JOIN clientes c ON c.pessoa_id = r.cliente_id
                INNER JOIN pessoas p ON p.id = c.pessoa_id";

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
                        // Adiciona um Reserva à lista
                        reservas.Add(new Reserva
                        {
                            // Lê a coluna 0 para Id
                            Id = reader.GetInt32(0),
                            // Lê a coluna 1 para QtdPessoas
                            QtdPessoas = reader.GetInt32(1),
                            // Lê a coluna 2 para DataHora
                            DataHora = reader.GetDateTime(2),
                            // Lê a coluna 3 para Status
                            Status = reader.GetString(3),
                            // Lê MesaId tratando valor nulo
                            MesaId = reader.IsDBNull(4)
                            // Se for nulo, fica sem valor
                            ? null
                            // Senão, lê o valor
                            : reader.GetInt32(4),
                            // Lê a coluna 5 para ClienteId
                            ClienteId = reader.GetInt32(5),
                            // Monta o objeto Cliente com os dados da linha
                            Cliente = new Cliente
                            {
                                // Lê a coluna 5 para Id
                                Id = reader.GetInt32(5),
                                // Lê a coluna 6 para Nome
                                Nome = reader.GetString(6),
                                // Lê a coluna 7 para Cpf
                                Cpf = reader.GetString(7),
                                // Lê a coluna 8 para EmailPessoal
                                EmailPessoal = reader.GetString(8),
                                // Lê a coluna 9 para Telefone
                                Telefone = reader.GetString(9)
                            }
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
        return reservas;
    }

    // Lê um registro pelo id (null se não existir)
    public Reserva? Read(int id)
    {
        // reserva começa nulo (não encontrado)
        Reserva? reserva = null;

        // Cria o comando SQL (descartado ao final)
        using (var cmd = new SqlCommand())
        {
            // Associa o comando à conexão
            cmd.Connection = conn;
            // Texto SQL que será executado
            cmd.CommandText = @"SELECT r.numero, r.qtd_pessoas,
                r.data_hora,
                CASE r.status WHEN 0 THEN 'Pendente' WHEN 1 THEN 'Confirmada' WHEN 2 THEN 'Cancelada' ELSE 'Pendente' END, -- CORREÇÃO: status nulo vira Pendente
                r.mesas_nr, r.cliente_id, p.nome, p.cpf, c.email_pessoal, c.telefone
                FROM reservas r
                INNER JOIN clientes c ON c.pessoa_id = r.cliente_id
                INNER JOIN pessoas p ON p.id = c.pessoa_id WHERE r.numero = @Id";
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
                        // Monta o objeto Reserva com os dados da linha
                        reserva = new Reserva
                        {
                            // Lê a coluna 0 para Id
                            Id = reader.GetInt32(0),
                            // Lê a coluna 1 para QtdPessoas
                            QtdPessoas = reader.GetInt32(1),
                            // Lê a coluna 2 para DataHora
                            DataHora = reader.GetDateTime(2),
                            // Lê a coluna 3 para Status
                            Status = reader.GetString(3),
                            // Lê a coluna 4 para MesaId
                            // CORREÇÃO: reserva sem mesa (mesas_nr nulo) não quebra mais a tela.
                            MesaId = reader.IsDBNull(4) ? null : reader.GetInt32(4),
                            // Lê a coluna 5 para ClienteId
                            ClienteId = reader.GetInt32(5),
                            // Monta o objeto Cliente com os dados da linha
                            Cliente = new Cliente
                            {
                                // Lê a coluna 5 para Id
                                Id = reader.GetInt32(5),
                                // Lê a coluna 6 para Nome
                                Nome = reader.GetString(6),
                                // Lê a coluna 7 para Cpf
                                Cpf = reader.GetString(7),
                                // Lê a coluna 8 para EmailPessoal
                                EmailPessoal = reader.GetString(8),
                                // Lê a coluna 9 para Telefone
                                Telefone = reader.GetString(9)
                            }
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
        return reserva;
    }

    // Atualiza um registro existente no banco
    public void Update(Reserva reserva)
    {
        // UNIÃO: preserva cliente, funcionário e status como no repositório de A.
        using (var cmd = new SqlCommand())
        {
            // Associa o comando à conexão
            cmd.Connection = conn;
            // Texto SQL que será executado
            cmd.CommandText = @"SET XACT_ABORT ON; BEGIN TRANSACTION;
                IF EXISTS (SELECT 1 FROM reservas WITH (UPDLOCK, HOLDLOCK) WHERE numero = @Id AND status <> 2)
                BEGIN
                    UPDATE reservas SET qtd_pessoas = @QtdPessoas, data_hora = @DataHora, mesas_nr = @MesaId WHERE numero = @Id;
                    DELETE FROM Reservas_Mesas WHERE numero_reservas = @Id;
                    INSERT INTO Reservas_Mesas (numero_reservas, nr_mesas) VALUES (@Id, @MesaId);
                END;
                COMMIT TRANSACTION;";
            // Preenche o parâmetro @Id (evita SQL injection)
            cmd.Parameters.AddWithValue("@Id", reserva.Id);
            // Preenche o parâmetro @QtdPessoas (evita SQL injection)
            cmd.Parameters.AddWithValue("@QtdPessoas", reserva.QtdPessoas);
            // Preenche o parâmetro @DataHora (evita SQL injection)
            cmd.Parameters.AddWithValue("@DataHora", reserva.DataHora);
            // Preenche o parâmetro @MesaId (evita SQL injection)
            cmd.Parameters.AddWithValue("@MesaId", reserva.MesaId);

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

    // Atualiza apenas o status da reserva
    public void UpdateStatus(int id, string status)
    {
        // UNIÃO: aplica no SQL os estados e o bloqueio de canceladas da versão A.
        if (status != "Pendente" && status != "Confirmada" && status != "Cancelada")
        {
            return;
        }

        // Cria o comando SQL (descartado ao final)
        using (var cmd = new SqlCommand())
        {
            // Associa o comando à conexão
            cmd.Connection = conn;
            // Texto SQL que será executado
            cmd.CommandText = @"UPDATE reservas SET status = CASE @Status WHEN 'Pendente' THEN 0 WHEN 'Confirmada' THEN 1 WHEN 'Cancelada' THEN 2 END WHERE numero = @Id AND status <> 2;";
            // Preenche o parâmetro @Status (evita SQL injection)
            cmd.Parameters.AddWithValue("@Status", status);
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

    // Remove (ou cancela) um registro no banco
    public void Delete(int id)
    {
        // UNIÃO: cancela no banco sem apagar o histórico, conforme A.
        using (var cmd = new SqlCommand())
        {
            // Associa o comando à conexão
            cmd.Connection = conn;
            // Texto SQL que será executado
            cmd.CommandText = @"UPDATE reservas SET status = 2 WHERE numero = @Id;";
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
