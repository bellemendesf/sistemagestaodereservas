using GestaoReservas.Models;
using Microsoft.Data.SqlClient;

public class ReservaSqlRepository : DataConnection, IReservaRepository
{
    public ReservaSqlRepository(IConfiguration configuration) : base(configuration) { }

    public void Create(Reserva reserva)
    {
        using (var cmd = new SqlCommand())
        {
            cmd.Connection = conn;
            cmd.CommandText = @"
                INSERT INTO Reservas (Qtd_pessoas, data_hora, status, mesas_nr, cliente_id, funcionario_id) 
                VALUES (@Qtd_pessoas, @Data_hora, @Status, @Cliente_id, @Funcionario_id)";
            cmd.Parameters.AddWithValue("@Qtd_pessoas", reserva.QtdPessoas);
            cmd.Parameters.AddWithValue("@Data_hora", reserva.DataHora);
            cmd.Parameters.AddWithValue("@Status", reserva.Status);
            cmd.Parameters.AddWithValue("@Mesas_nr", reserva.MesaId);
            cmd.Parameters.AddWithValue("@Cliente_id", reserva.ClienteId
            );
            cmd.Parameters.AddWithValue("@Funcionario_id", reserva.FuncionarioID);

            conn.Open();
            cmd.ExecuteNonQuery();
            conn.Close();
        }
    }

    public List<Reserva> Read()
    {
        var reservas = new List<Reserva>();

        using (var cmd = new SqlCommand())
        {
            cmd.Connection = conn;
            cmd.CommandText = "SELECT numero, cliente_id, mesa_nr, funcionario_id, Data_Hora, Qtd_Pessoas, Status FROM Reservas";

            conn.Open();
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
                        FuncionarioID = reader.GetInt32(6)
             
                    });
                }
            }
            conn.Close();
        }

        return reservas;
    }

    public Reserva Read(int id)
    {
        Reserva reserva = null;

        using (var cmd = new SqlCommand())
        {
            cmd.Connection = conn;
            cmd.CommandText = "SELECT numerp, Cliente_Id, Mesa_nr, Funcionario_id, Data_Hora, Qtd_Pessoas, Status FROM Reservas WHERE numero = @numero";
            cmd.Parameters.AddWithValue("@numero", id);

            conn.Open();
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
                        FuncionarioID = reader.GetInt32(6)
                    };
                }
            }
            conn.Close();
        }

        return reserva;
    }

    public void Update(Reserva reserva)
    {
        using (var cmd = new SqlCommand())
        {
            cmd.Connection = conn;
            cmd.CommandText = @"
                UPDATE Reserva SET Cliente_Id = @Cliente_Id, Mesa_nr = @Mesa_nr, Funcionario_id = @Funcionario_Id, 
                Data_Hora = @Data_Hora, Qtd_Pessoas = @Qtd_Pessoas, Status = @Status WHERE Id = @Id";
            cmd.Parameters.AddWithValue("@Cliente_Id", reserva.ClienteId);
            cmd.Parameters.AddWithValue("@Mesa_nr", reserva.MesaId);
            cmd.Parameters.AddWithValue("@Funcionario_Id", reserva.FuncionarioID);
            cmd.Parameters.AddWithValue("@Data_Hora", reserva.DataHora);
            cmd.Parameters.AddWithValue("@Qtd_Pessoas", reserva.QtdPessoas);
            cmd.Parameters.AddWithValue("@Status", reserva.Status);
            cmd.Parameters.AddWithValue("@numero", reserva.Id);

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
            cmd.CommandText = "DELETE FROM Reserva WHERE numero = @numero";
            cmd.Parameters.AddWithValue("@numero", id);

            conn.Open();
            cmd.ExecuteNonQuery();
            conn.Close();
        }
    }
}