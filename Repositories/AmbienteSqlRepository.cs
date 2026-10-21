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
            cmd.CommandText = @"SET XACT_ABORT ON; BEGIN TRANSACTION;
                DECLARE @Codigo int = (SELECT ISNULL(MAX(codigo), 0) + 1 FROM ambientes WITH (UPDLOCK, HOLDLOCK));
                INSERT INTO ambientes (codigo, nome, status, capacidade) VALUES (@Codigo, @Nome, @Status, @Capacidade);
                COMMIT TRANSACTION;";
            cmd.Parameters.AddWithValue("@Nome", ambiente.Nome);
            cmd.Parameters.AddWithValue("@Status", ambiente.Status ?? "");
            cmd.Parameters.AddWithValue("@Capacidade", ambiente.Capacidade);

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

    public List<Ambiente> Read()
    {
        var ambientes = new List<Ambiente>();

        using (var cmd = new SqlCommand())
        {
            cmd.Connection = conn;
            cmd.CommandText = @"SELECT codigo, nome, COALESCE(status, ''), capacidade FROM ambientes";

            conn.Open();
            try
            {
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
            }
            finally
            {
                conn.Close();
            }
        }

        return ambientes;
    }

    public Ambiente? Read(int id)
    {
        Ambiente? ambiente = null;

        using (var cmd = new SqlCommand())
        {
            cmd.Connection = conn;
            cmd.CommandText = @"SELECT codigo, nome, COALESCE(status, ''), capacidade FROM ambientes WHERE codigo = @Id";
            cmd.Parameters.AddWithValue("@Id", id);

            conn.Open();
            try
            {
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
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
            }
            finally
            {
                conn.Close();
            }
        }

        return ambiente;
    }

    public void Update(Ambiente ambiente)
    {
        using (var cmd = new SqlCommand())
        {
            cmd.Connection = conn;
            cmd.CommandText = @"UPDATE ambientes SET nome = @Nome, status = @Status, capacidade = @Capacidade WHERE codigo = @Id;";
            cmd.Parameters.AddWithValue("@Nome", ambiente.Nome);
            cmd.Parameters.AddWithValue("@Status", ambiente.Status ?? "");
            cmd.Parameters.AddWithValue("@Capacidade", ambiente.Capacidade);
            cmd.Parameters.AddWithValue("@Id", ambiente.Id);

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
            cmd.CommandText = @"DELETE FROM ambientes WHERE codigo = @Id;";
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
