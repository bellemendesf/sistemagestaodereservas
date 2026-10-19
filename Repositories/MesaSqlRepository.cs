using GestaoReservas.Models;
using Microsoft.Data.SqlClient;

public class MesaSqlRepository : DataConnection, IMesaRepository
{
    public MesaSqlRepository(IConfiguration configuration) : base(configuration) { }

    public void Create(Mesa mesa)
    {
        // UNIÃO: nr identifica a mesa; a View de A não informa AmbienteId.
        using (var cmd = new SqlCommand())
        {
            cmd.Connection = conn;
            cmd.CommandText = @"INSERT INTO mesas (nr, qtd_lugares, status, codigo_ambientes) VALUES (@Numero, @QtdLugares, @Status, NULLIF(@AmbienteId, 0));";
            cmd.Parameters.AddWithValue("@Numero", mesa.Numero);
            cmd.Parameters.AddWithValue("@QtdLugares", mesa.QtdLugares);
            cmd.Parameters.AddWithValue("@Status", mesa.Status);
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
    }

    public List<Mesa> Read()
    {
        var mesas = new List<Mesa>();

        using (var cmd = new SqlCommand())
        {
            cmd.Connection = conn;
            cmd.CommandText = @"SELECT nr, qtd_lugares, status, COALESCE(codigo_ambientes, 0) FROM mesas";

            conn.Open();
            try
            {
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        mesas.Add(new Mesa
                        {
                            Id = reader.GetInt32(0),
                            Numero = reader.GetInt32(0),
                            QtdLugares = reader.GetInt32(1),
                            Status = reader.GetString(2),
                            AmbienteId = reader.GetInt32(3)
                        });
                    }
                }
            }
            finally
            {
                conn.Close();
            }
        }

        return mesas;
    }

    public Mesa Read(int id)
    {
        Mesa mesa = null!;

        using (var cmd = new SqlCommand())
        {
            cmd.Connection = conn;
            cmd.CommandText = @"SELECT nr, qtd_lugares, status, COALESCE(codigo_ambientes, 0) FROM mesas WHERE nr = @Id";
            cmd.Parameters.AddWithValue("@Id", id);

            conn.Open();
            try
            {
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        mesa = new Mesa
                        {
                            Id = reader.GetInt32(0),
                            Numero = reader.GetInt32(0),
                            QtdLugares = reader.GetInt32(1),
                            Status = reader.GetString(2),
                            AmbienteId = reader.GetInt32(3)
                        };
                    }
                }
            }
            finally
            {
                conn.Close();
            }
        }

        return mesa;
    }

    public void Update(Mesa mesa)
    {
        using (var cmd = new SqlCommand())
        {
            cmd.Connection = conn;
            cmd.CommandText = @"UPDATE mesas SET nr = @Numero, qtd_lugares = @QtdLugares, status = @Status WHERE nr = @Id;";
            cmd.Parameters.AddWithValue("@Numero", mesa.Numero);
            cmd.Parameters.AddWithValue("@QtdLugares", mesa.QtdLugares);
            cmd.Parameters.AddWithValue("@Status", mesa.Status);
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
    }

    public void Delete(int id)
    {
        using (var cmd = new SqlCommand())
        {
            cmd.Connection = conn;
            cmd.CommandText = @"DELETE FROM mesas WHERE nr = @Id;";
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
