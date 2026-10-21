using GestaoReservas.Models;
using Microsoft.Data.SqlClient;

public class MesaSqlRepository : DataConnection, IMesaRepository
{
    public MesaSqlRepository(IConfiguration configuration) : base(configuration) { }

   public void Create(Mesa mesa)
{
    using var cmd = new SqlCommand();
    cmd.Connection = conn;

    cmd.CommandText = @"
        INSERT INTO mesas
            (nr, numero, qtd_lugares, status, codigo_ambientes)
        VALUES
            (@Id, @Numero, @QtdLugares, @Status, @AmbienteId);";

    cmd.Parameters.AddWithValue("@Id", mesa.Numero);
    cmd.Parameters.AddWithValue("@Numero", mesa.Numero);
    cmd.Parameters.AddWithValue("@QtdLugares", mesa.QtdLugares);
    cmd.Parameters.AddWithValue("@Status",
        mesa.Status == "Livre" ? "Disponivel" : mesa.Status);
    cmd.Parameters.AddWithValue("@AmbienteId", mesa.AmbienteId);

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

public List<Mesa> Read()
{
    var mesas = new List<Mesa>();

    using var cmd = new SqlCommand();
    cmd.Connection = conn;

    cmd.CommandText = @"
        SELECT
            m.nr,
            m.numero,
            m.qtd_lugares,
            CASE
                WHEN m.status = 'Disponivel' THEN 'Livre'
                ELSE m.status
            END AS status,
            COALESCE(m.codigo_ambientes, 0),
            COALESCE(a.nome, 'Sem ambiente')
        FROM mesas m
        LEFT JOIN ambientes a
            ON m.codigo_ambientes = a.codigo
        ORDER BY m.numero;";

    conn.Open();

    try
    {
        using SqlDataReader reader = cmd.ExecuteReader();

        while (reader.Read())
        {
            mesas.Add(new Mesa
            {
                Id = reader.GetInt32(0),
                Numero = reader.GetInt32(1),
                QtdLugares = reader.GetInt32(2),
                Status = reader.GetString(3),
                AmbienteId = reader.GetInt32(4),
                AmbienteNome = reader.GetString(5)
            });
        }
    }
    finally
    {
        conn.Close();
    }

    return mesas;
}

   public Mesa? Read(int id)
{
    Mesa? mesa = null;

    using var cmd = new SqlCommand();
    cmd.Connection = conn;

    cmd.CommandText = @"
        SELECT
            nr,
            numero,
            qtd_lugares,
            CASE
                WHEN status = 'Disponivel' THEN 'Livre'
                ELSE status
            END,
            COALESCE(codigo_ambientes, 0)
        FROM mesas
        WHERE nr = @Id;";

    cmd.Parameters.AddWithValue("@Id", id);

    conn.Open();

    try
    {
        using SqlDataReader reader = cmd.ExecuteReader();

        if (reader.Read())
        {
            mesa = new Mesa
            {
                Id = reader.GetInt32(0),
                Numero = reader.GetInt32(1),
                QtdLugares = reader.GetInt32(2),
                Status = reader.GetString(3),
                AmbienteId = reader.GetInt32(4)
            };
        }
    }
    finally
    {
        conn.Close();
    }

    return mesa;
}

  public void Update(Mesa mesa)
{
    using var cmd = new SqlCommand();
    cmd.Connection = conn;

    cmd.CommandText = @"
        UPDATE mesas
        SET numero = @Numero,
            qtd_lugares = @QtdLugares,
            status = @Status,
            codigo_ambientes = @AmbienteId
        WHERE nr = @Id;";

    cmd.Parameters.AddWithValue("@Numero", mesa.Numero);
    cmd.Parameters.AddWithValue("@QtdLugares", mesa.QtdLugares);
    cmd.Parameters.AddWithValue("@Status",
        mesa.Status == "Livre" ? "Disponivel" : mesa.Status);
    cmd.Parameters.AddWithValue("@AmbienteId", mesa.AmbienteId);
    cmd.Parameters.AddWithValue("@Id", mesa.Id);

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
   public void Delete(int id)
{
    using var transactionConnection = new SqlConnection(
        conn.ConnectionString);

    transactionConnection.Open();

    using var transaction = transactionConnection.BeginTransaction();

    try
    {
        // Verifica se existe alguma reserva NÃO cancelada
        // vinculada à mesa.
        using (var verificar = new SqlCommand(@"
            SELECT COUNT(*)
            FROM reservas
            WHERE mesas_nr = @Id
              AND status <> 2;", transactionConnection, transaction))
        {
            verificar.Parameters.AddWithValue("@Id", id);

            int reservasAtivas = (int)verificar.ExecuteScalar()!;

            if (reservasAtivas > 0)
            {
                throw new InvalidOperationException(
                    "Não é possível excluir esta mesa porque existem reservas não canceladas vinculadas a ela.");
            }
        }

        // Remove os vínculos da tabela associativa
        // somente para reservas canceladas.
        using (var excluirVinculos = new SqlCommand(@"
            DELETE rm
            FROM Reservas_Mesas rm
            INNER JOIN reservas r
                ON r.numero = rm.numero_reservas
            WHERE rm.nr_mesas = @Id
              AND r.status = 2;", transactionConnection, transaction))
        {
            excluirVinculos.Parameters.AddWithValue("@Id", id);
            excluirVinculos.ExecuteNonQuery();
        }

        // Remove as reservas canceladas da relação direta
        // com a mesa, mas preserva os registros das reservas.
        using (var desvincularReservas = new SqlCommand(@"
            UPDATE reservas
            SET mesas_nr = NULL
            WHERE mesas_nr = @Id
              AND status = 2;", transactionConnection, transaction))
        {
            desvincularReservas.Parameters.AddWithValue("@Id", id);
            desvincularReservas.ExecuteNonQuery();
        }

        // Exclui a mesa somente depois de tratar os vínculos.
        using (var excluirMesa = new SqlCommand(
            "DELETE FROM mesas WHERE nr = @Id;",
            transactionConnection, transaction))
        {
            excluirMesa.Parameters.AddWithValue("@Id", id);
            excluirMesa.ExecuteNonQuery();
        }

        transaction.Commit();
    }
    catch
    {
        transaction.Rollback();
        throw;
    }
} }