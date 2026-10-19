using GestaoReservas.Models;
using Microsoft.Data.SqlClient;

public class ReservaSqlRepository : DataConnection, IReservaRepository
{
    public ReservaSqlRepository(IConfiguration configuration) : base(configuration) { }

    public void Create(Reserva reserva)
    {
        // UNIÃO: liga o Cliente recebido por A ao cadastro e à reserva SQL de B.
        using (var cmd = new SqlCommand())
        {
            cmd.Connection = conn;
            cmd.CommandText = @"SET XACT_ABORT ON; BEGIN TRANSACTION;
                DECLARE @PessoaId int;
                SELECT @PessoaId = id FROM pessoas WITH (UPDLOCK, HOLDLOCK) WHERE cpf = @Cpf;
                IF @PessoaId IS NULL
                BEGIN
                    INSERT INTO pessoas (nome, cpf, email_pessoal, senha) VALUES (@Nome, @Cpf, @Email, '');
                    SET @PessoaId = CAST(SCOPE_IDENTITY() AS INT);
                END;
                IF NOT EXISTS (SELECT 1 FROM clientes WHERE pessoa_id = @PessoaId)
                    INSERT INTO clientes (pessoa_id, telefone) VALUES (@PessoaId, @Telefone);
                INSERT INTO reservas (qtd_pessoas, horario, dia, status, cliente_id, funcionario_id)
                VALUES (@QtdPessoas, CAST(@DataHora AS time), CAST(@DataHora AS date), 0, @PessoaId, @FuncionarioID);
                DECLARE @ReservaId int = CAST(SCOPE_IDENTITY() AS INT);
                INSERT INTO Reservas_Mesas (numero_reservas, nr_mesas) VALUES (@ReservaId, @MesaId);
                COMMIT TRANSACTION;";
            cmd.Parameters.AddWithValue("@Nome", reserva.Cliente.Nome);
            cmd.Parameters.AddWithValue("@Cpf", reserva.Cliente.Cpf);
            cmd.Parameters.AddWithValue("@Email", reserva.Cliente.EmailPessoal ?? "");
            cmd.Parameters.AddWithValue("@Telefone", reserva.Cliente.Telefone);
            cmd.Parameters.AddWithValue("@QtdPessoas", reserva.QtdPessoas);
            cmd.Parameters.AddWithValue("@DataHora", reserva.DataHora);
            cmd.Parameters.AddWithValue("@FuncionarioID", reserva.FuncionarioID);
            cmd.Parameters.AddWithValue("@MesaId", reserva.MesaId);

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

    public List<Reserva> Read()
    {
        var reservas = new List<Reserva>();

        using (var cmd = new SqlCommand())
        {
            cmd.Connection = conn;
            cmd.CommandText = @"SELECT r.numero, r.qtd_pessoas,
                DATEADD(SECOND, DATEDIFF(SECOND, CAST('00:00:00' AS time), r.horario), CAST(r.dia AS datetime2)),
                CASE r.status WHEN 0 THEN 'Pendente' WHEN 1 THEN 'Confirmada' WHEN 2 THEN 'Cancelada' END,
                rm.nr_mesas, r.cliente_id, r.funcionario_id, p.nome, p.cpf, p.email_pessoal, c.telefone
                FROM reservas r
                INNER JOIN Reservas_Mesas rm ON rm.numero_reservas = r.numero
                INNER JOIN clientes c ON c.pessoa_id = r.cliente_id
                INNER JOIN pessoas p ON p.id = c.pessoa_id";

            conn.Open();
            try
            {
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        reservas.Add(new Reserva
                        {
                            Id = reader.GetInt32(0),
                            QtdPessoas = reader.GetInt32(1),
                            DataHora = reader.GetDateTime(2),
                            Status = reader.GetString(3),
                            MesaId = reader.GetInt32(4),
                            ClienteId = reader.GetInt32(5),
                            FuncionarioID = reader.GetInt32(6),
                            Cliente = new Cliente
                            {
                                Id = reader.GetInt32(5),
                                Nome = reader.GetString(7),
                                Cpf = reader.GetString(8),
                                EmailPessoal = reader.GetString(9),
                                Telefone = reader.GetString(10)
                            }
                        });
                    }
                }
            }
            finally
            {
                conn.Close();
            }
        }

        return reservas;
    }

    public Reserva? Read(int id)
    {
        Reserva? reserva = null;

        using (var cmd = new SqlCommand())
        {
            cmd.Connection = conn;
            cmd.CommandText = @"SELECT r.numero, r.qtd_pessoas,
                DATEADD(SECOND, DATEDIFF(SECOND, CAST('00:00:00' AS time), r.horario), CAST(r.dia AS datetime2)),
                CASE r.status WHEN 0 THEN 'Pendente' WHEN 1 THEN 'Confirmada' WHEN 2 THEN 'Cancelada' END,
                rm.nr_mesas, r.cliente_id, r.funcionario_id, p.nome, p.cpf, p.email_pessoal, c.telefone
                FROM reservas r
                INNER JOIN Reservas_Mesas rm ON rm.numero_reservas = r.numero
                INNER JOIN clientes c ON c.pessoa_id = r.cliente_id
                INNER JOIN pessoas p ON p.id = c.pessoa_id WHERE r.numero = @Id";
            cmd.Parameters.AddWithValue("@Id", id);

            conn.Open();
            try
            {
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        reserva = new Reserva
                        {
                            Id = reader.GetInt32(0),
                            QtdPessoas = reader.GetInt32(1),
                            DataHora = reader.GetDateTime(2),
                            Status = reader.GetString(3),
                            MesaId = reader.GetInt32(4),
                            ClienteId = reader.GetInt32(5),
                            FuncionarioID = reader.GetInt32(6),
                            Cliente = new Cliente
                            {
                                Id = reader.GetInt32(5),
                                Nome = reader.GetString(7),
                                Cpf = reader.GetString(8),
                                EmailPessoal = reader.GetString(9),
                                Telefone = reader.GetString(10)
                            }
                        };
                    }
                }
            }
            finally
            {
                conn.Close();
            }
        }

        return reserva;
    }

    public void Update(Reserva reserva)
    {
        // UNIÃO: preserva cliente, funcionário e status como no repositório de A.
        using (var cmd = new SqlCommand())
        {
            cmd.Connection = conn;
            cmd.CommandText = @"SET XACT_ABORT ON; BEGIN TRANSACTION;
                IF EXISTS (SELECT 1 FROM reservas WITH (UPDLOCK, HOLDLOCK) WHERE numero = @Id AND status <> 2)
                BEGIN
                    UPDATE reservas SET qtd_pessoas = @QtdPessoas, dia = CAST(@DataHora AS date), horario = CAST(@DataHora AS time) WHERE numero = @Id;
                    DELETE FROM Reservas_Mesas WHERE numero_reservas = @Id;
                    INSERT INTO Reservas_Mesas (numero_reservas, nr_mesas) VALUES (@Id, @MesaId);
                END;
                COMMIT TRANSACTION;";
            cmd.Parameters.AddWithValue("@Id", reserva.Id);
            cmd.Parameters.AddWithValue("@QtdPessoas", reserva.QtdPessoas);
            cmd.Parameters.AddWithValue("@DataHora", reserva.DataHora);
            cmd.Parameters.AddWithValue("@MesaId", reserva.MesaId);

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

    public void UpdateStatus(int id, string status)
    {
        // UNIÃO: aplica no SQL os estados e o bloqueio de canceladas da versão A.
        if (status != "Pendente" && status != "Confirmada" && status != "Cancelada")
        {
            return;
        }

        using (var cmd = new SqlCommand())
        {
            cmd.Connection = conn;
            cmd.CommandText = @"UPDATE reservas SET status = CASE @Status WHEN 'Pendente' THEN 0 WHEN 'Confirmada' THEN 1 WHEN 'Cancelada' THEN 2 END WHERE numero = @Id AND status <> 2;";
            cmd.Parameters.AddWithValue("@Status", status);
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

    public void Delete(int id)
    {
        // UNIÃO: cancela no banco sem apagar o histórico, conforme A.
        using (var cmd = new SqlCommand())
        {
            cmd.Connection = conn;
            cmd.CommandText = @"UPDATE reservas SET status = 2 WHERE numero = @Id;";
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
