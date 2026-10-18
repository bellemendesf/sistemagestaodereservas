using GestaoReservas.Models;
using Microsoft.Data.SqlClient;

public class MesaSqlRepository : DataConnection, IMesaRepository
{
    public MesaSqlRepository(IConfiguration configuration) : base(configuration) { }

    public void Create(Mesa mesa)
    {
        using (var cmd = new SqlCommand())
        {
            cmd.Connection = conn;
            cmd.CommandText = @"
                INSERT INTO Mesa (Numero, QtdLugares, Status, AmbienteId) 
                VALUES (@Numero, @QtdLugares, @Status, @AmbienteId)";
            cmd.Parameters.AddWithValue("@Numero", mesa.Numero);
            cmd.Parameters.AddWithValue("@QtdLugares", mesa.QtdLugares);
            cmd.Parameters.AddWithValue("@Status", mesa.Status);
            cmd.Parameters.AddWithValue("@AmbienteId", mesa.AmbienteId);

            conn.Open();
            cmd.ExecuteNonQuery();
            conn.Close();
        }
    }

    public List<Mesa> Read()
    {
        var mesas = new List<Mesa>();

        using (var cmd = new SqlCommand())
        {
            cmd.Connection = conn;
            cmd.CommandText = "SELECT Id, Numero, QtdLugares, Status, AmbienteId FROM Mesa";

            conn.Open();
            using (SqlDataReader reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    mesas.Add(new Mesa
                    {
                        Id = reader.GetInt32(0),
                        Numero = reader.GetInt32(1),
                        QtdLugares = reader.GetInt32(2),
                        Status = reader.GetString(3),
                        AmbienteId = reader.GetInt32(4)
                    });
                }
            }
            conn.Close();
        }

        return mesas;
    }

    public Mesa Read(int id)
    {
        Mesa mesa = null;

        using (var cmd = new SqlCommand())
        {
            cmd.Connection = conn;
            cmd.CommandText = "SELECT Id, Numero, QtdLugares, Status, AmbienteId FROM Mesa WHERE Id = @Id";
            cmd.Parameters.AddWithValue("@Id", id);

            conn.Open();
            using (SqlDataReader reader = cmd.ExecuteReader())
            {
                if (reader.Read())   // só UM registro esperado
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
            conn.Close();
        }

        return mesa;
    }

    public void Update(Mesa mesa)
    {
        using (var cmd = new SqlCommand())
        {
            cmd.Connection = conn;
            cmd.CommandText = @"
                UPDATE Mesa 
                SET Numero = @Numero, QtdLugares = @QtdLugares, Status = @Status, AmbienteId = @AmbienteId 
                WHERE Id = @Id";
            cmd.Parameters.AddWithValue("@Numero", mesa.Numero);
            cmd.Parameters.AddWithValue("@QtdLugares", mesa.QtdLugares);
            cmd.Parameters.AddWithValue("@Status", mesa.Status);
            cmd.Parameters.AddWithValue("@AmbienteId", mesa.AmbienteId);
            cmd.Parameters.AddWithValue("@Id", mesa.Id);

            conn.Open();
            cmd.ExecuteNonQuery();
            conn.Close();
        }
    }

    public void Delete(int id)
    {
        using (var cmd = new SqlCommand())
        {
            cmd.Connection = conn;
            cmd.CommandText = "DELETE FROM Mesa WHERE Id = @Id";
            cmd.Parameters.AddWithValue("@Id", id);

            conn.Open();
            cmd.ExecuteNonQuery();
            conn.Close();
        }
    }
}