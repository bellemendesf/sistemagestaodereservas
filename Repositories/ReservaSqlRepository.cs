// Importa os modelos do projeto
using GestaoReservas.Models;
// Importa as classes de acesso ao SQL Server
using Microsoft.Data.SqlClient;

// Repositório que grava e lê Reserva no SQL Server
// N:N: as mesas da reserva ficam na tabela Reservas_Mesas (a tabela reservas não guarda mesa).
public class ReservaSqlRepository : DataConnection, IReservaRepository
{
    // Construtor: repassa a configuração para a classe base
    public ReservaSqlRepository(IConfiguration configuration) : base(configuration) { }

    // Consulta base: VIEW v_reservas (número, quantidade, data e status em texto) + v_clientes (dados do cliente).
    // A view não traz o id do cliente, por isso liga com reservas só para pegar cliente_id.
    private const string SelectReservas = @"SELECT v.Codigo_Reserva, v.Quantidade_Pessoas, v.Data_Reserva,
                COALESCE(v.Status, 'Pendente'), -- status nulo vira Pendente
                r.cliente_id, c.Nome, c.CPF, c.Email, c.Telefone
                FROM v_reservas v
                INNER JOIN reservas r ON r.numero = v.Codigo_Reserva
                INNER JOIN v_clientes c ON c.Codigo = r.cliente_id";

    // Mesas de cada reserva: VIEW v_reservas_mesas (ligação N:N) + v_mesas (número, lugares e ambiente).
    private const string SelectMesas = @"SELECT rm.Codigo_Reserva, rm.Codigo_Mesa, m.Numero, m.Ambiente, m.Quantidade_Lugares
                FROM v_reservas_mesas rm
                INNER JOIN v_mesas m ON m.Codigo = rm.Codigo_Mesa";

    // Insere uma nova reserva e grava cada mesa escolhida na tabela de ligação
    public void Create(Reserva reserva)
    {
        conn.Open();
        try
        {
            using var tx = conn.BeginTransaction();
            try
            {
                // Confere de novo, dentro da transação, se alguma mesa já tem reserva ativa no mesmo dia
                // (evita duas reservas gravadas ao mesmo tempo para a mesma mesa).
                foreach (var mesaId in reserva.MesaIds.Distinct())
                {
                    using var conferir = new SqlCommand(@"SELECT COUNT(*)
                        FROM Reservas_Mesas rm WITH (UPDLOCK, HOLDLOCK)
                        INNER JOIN reservas r ON r.numero = rm.numero_reservas
                        WHERE rm.nr_mesas = @MesaId
                          AND ISNULL(r.status, 0) <> 2
                          AND CAST(r.data_hora AS date) = CAST(@DataHora AS date)", conn, tx);
                    conferir.Parameters.AddWithValue("@MesaId", mesaId);
                    conferir.Parameters.AddWithValue("@DataHora", reserva.DataHora);
                    if ((int)conferir.ExecuteScalar()! > 0)
                        throw new InvalidOperationException("A mesa escolhida não está livre nesta data.");
                }

                // Grava a reserva (status 0 = Pendente) e pega o número gerado
                using (var cmd = new SqlCommand(@"INSERT INTO reservas (qtd_pessoas, data_hora, status, cliente_id)
                    VALUES (@QtdPessoas, @DataHora, 0, @ClienteId);
                    SELECT CAST(SCOPE_IDENTITY() AS INT);", conn, tx))
                {
                    cmd.Parameters.AddWithValue("@QtdPessoas", reserva.QtdPessoas);
                    cmd.Parameters.AddWithValue("@DataHora", reserva.DataHora);
                    cmd.Parameters.AddWithValue("@ClienteId", reserva.ClienteId);
                    reserva.Id = (int)cmd.ExecuteScalar()!;
                }

                // Grava uma linha na tabela N:N para cada mesa escolhida
                foreach (var mesaId in reserva.MesaIds.Distinct())
                {
                    using var cmdMesa = new SqlCommand(
                        "INSERT INTO Reservas_Mesas (numero_reservas, nr_mesas) VALUES (@Reserva, @Mesa)", conn, tx);
                    cmdMesa.Parameters.AddWithValue("@Reserva", reserva.Id);
                    cmdMesa.Parameters.AddWithValue("@Mesa", mesaId);
                    cmdMesa.ExecuteNonQuery();
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

    // Lê todas as reservas (com as mesas de cada uma)
    public List<Reserva> Read()
    {
        var reservas = new List<Reserva>();
        conn.Open();
        try
        {
            using (var cmd = new SqlCommand(SelectReservas, conn))
            using (var reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                    reservas.Add(LerReserva(reader));
            }
            CarregarMesas(reservas, null);
        }
        finally
        {
            conn.Close();
        }
        return reservas;
    }

    // Lê uma reserva pelo número (null se não existir)
    public Reserva? Read(int id)
    {
        Reserva? reserva = null;
        conn.Open();
        try
        {
            using (var cmd = new SqlCommand(SelectReservas + " WHERE v.Codigo_Reserva = @Id", conn))
            {
                cmd.Parameters.AddWithValue("@Id", id);
                using var reader = cmd.ExecuteReader();
                if (reader.Read())
                    reserva = LerReserva(reader);
            }
            if (reserva != null)
                CarregarMesas(new List<Reserva> { reserva }, id);
        }
        finally
        {
            conn.Close();
        }
        return reserva;
    }

    // Monta o objeto Reserva a partir de uma linha da consulta base
    private static Reserva LerReserva(SqlDataReader reader) => new Reserva
    {
        Id = reader.GetInt32(0),
        QtdPessoas = reader.GetInt32(1),
        DataHora = reader.GetDateTime(2),
        Status = reader.GetString(3),
        ClienteId = reader.GetInt32(4),
        Cliente = new Cliente
        {
            Id = reader.GetInt32(4),
            Nome = reader.GetString(5),
            Cpf = reader.GetString(6),
            EmailPessoal = reader.GetString(7),
            Telefone = reader.GetString(8)
        }
    };

    // Preenche MesaIds e MesasDescricao das reservas (conexão já aberta)
    private void CarregarMesas(List<Reserva> reservas, int? reservaId)
    {
        var porNumero = reservas.ToDictionary(r => r.Id);
        using var cmd = new SqlCommand(
            SelectMesas + (reservaId == null ? "" : " WHERE rm.Codigo_Reserva = @Id") + " ORDER BY m.Ambiente, m.Numero", conn);
        if (reservaId != null) cmd.Parameters.AddWithValue("@Id", reservaId.Value);
        using var reader = cmd.ExecuteReader();
        while (reader.Read())
        {
            if (!porNumero.TryGetValue(reader.GetInt32(0), out var r)) continue;
            r.MesaIds.Add(reader.GetInt32(1));
            r.MesasDescricao.Add($"Mesa {reader.GetInt32(2)} · {reader.GetString(3)} · {reader.GetInt32(4)} lugares");
        }
    }

    // Atualiza a reserva: o cliente só altera o dia e a quantidade de pessoas (as mesas continuam as mesmas)
    public void Update(Reserva reserva)
    {
        using (var cmd = new SqlCommand())
        {
            cmd.Connection = conn;
            // Reserva cancelada (status 2) não é alterada
            cmd.CommandText = @"UPDATE reservas SET qtd_pessoas = @QtdPessoas, data_hora = @DataHora
                WHERE numero = @Id AND ISNULL(status, 0) <> 2;";
            cmd.Parameters.AddWithValue("@Id", reserva.Id);
            cmd.Parameters.AddWithValue("@QtdPessoas", reserva.QtdPessoas);
            cmd.Parameters.AddWithValue("@DataHora", reserva.DataHora);

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

    // Altera só o status (usado pelo funcionário)
    public void UpdateStatus(int id, string status)
    {
        if (status != "Pendente" && status != "Confirmada" && status != "Cancelada")
        {
            return;
        }

        using (var cmd = new SqlCommand())
        {
            cmd.Connection = conn;
            cmd.CommandText = @"UPDATE reservas SET status = CASE @Status WHEN 'Pendente' THEN 0 WHEN 'Confirmada' THEN 1 WHEN 'Cancelada' THEN 2 END WHERE numero = @Id AND ISNULL(status, 0) <> 2;";
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

    // Cancela a reserva (status 2); o registro e as mesas ligadas ficam guardados no histórico
    public void Delete(int id)
    {
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
