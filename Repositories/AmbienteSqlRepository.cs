using GestaoReservas.Models;
using Microsoft.Data.SqlClient;

public class AmbienteSqlRepository : DataConnection, IAmbienteRepository
{
    public AmbienteSqlRepository(IConfiguration configuration) : base(configuration) { }

    public void Create(Ambiente ambiente)
    {
        using (var cmd = new SqlCommand())
        {
            cmd.Connection = conn;
            cmd.CommandText = @"
                INSERT INTO Mesa (Codigo, Nome, Status, Capacidade) 
                VALUES (@Codigo, @Nome, @Status, @Capacidade)";
            cmd.Parameters.AddWithValue("@Codigo", ambiente.Id);
            cmd.Parameters.AddWithValue("@Nome", ambiente.Nome);
            cmd.Parameters.AddWithValue("@Status", ambiente.Status);
            cmd.Parameters.AddWithValue("@Capacidade", ambiente.Capacidade);

            conn.Open();
            cmd.ExecuteNonQuery();
            conn.Close();
        }
    }

    public List<Ambiente> Read()
    {
        var ambientes = new List<Ambiente>();

        using (var cmd = new SqlCommand())
        {
            cmd.Connection = conn;
            cmd.CommandText = "SELECT Codigo, Nome, Status, Capacidade FROM Ambientes";

            conn.Open();
            using (SqlDataReader reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    ambientes.Add(new Ambiente
                    {
                        Id = reader.GetInt32(0),
                        Nome = reader.GetString(1),
                        Status = reader.GetString(2),
                        Capacidade = reader.GetInt32(3)
                    });
                }
            }
            conn.Close();
        }

        return ambientes;
    }

    public Ambiente Read(int id)
    {
        Ambiente ambiente = null;

        using (var cmd = new SqlCommand())
        {
            cmd.Connection = conn;
            cmd.CommandText = "SELECT Codigo, Nome, Status, Capacidade FROM Ambiente WHERE codigo = @codigo";
            cmd.Parameters.AddWithValue("@codigo", id);

            conn.Open();
            using (SqlDataReader reader = cmd.ExecuteReader())
            {
                if (reader.Read())   // só UM registro esperado
                {
                    ambiente = new Ambiente
                    {
                        Id = reader.GetInt32(0),
                        Nome = reader.GetString(1),
                        Status = reader.GetString(2),
                        Capacidade = reader.GetInt32(3)
                    };
                }
            }
            conn.Close();
        }

        return ambiente;
    }

    public void Update(Ambiente ambiente)
    {
        using (var cmd = new SqlCommand())
        {
            cmd.Connection = conn;
            cmd.CommandText = @"
                UPDATE Ambiente 
                SET Codigo = @codigo, Nome = @Nome, Status = @Status, Capacidade = @Capacidade 
                WHERE Codigo = @Codigo";
            cmd.Parameters.AddWithValue("@Codigo", ambiente.Id);
            cmd.Parameters.AddWithValue("@Nome", ambiente.Nome);
            cmd.Parameters.AddWithValue("@Status", ambiente.Status);
            cmd.Parameters.AddWithValue("@Capacidade", ambiente.Capacidade);

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
            cmd.CommandText = "DELETE FROM ambientes WHERE codigo = @codigo";
            cmd.Parameters.AddWithValue("@codigo", id);

            conn.Open();
            cmd.ExecuteNonQuery();
            conn.Close();
        }
    }
}