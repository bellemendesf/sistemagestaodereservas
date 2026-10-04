using GestaoReservas.Models;
using Microsoft.Data.SqlClient;

public class ReservaSqlRepository : DataConnection, IReservaRepository
{
    public void Create(Reserva Reserva)
    {
        throw new NotImplementedException();
    }

    public void Delete(int id)
    {
        throw new NotImplementedException();
    }

    public List<Reserva> Read()
    {
        SqlCommand cmd = new SqlCommand();
        cmd.CommandText = "SELECT * FROM Reservas";
        cmd.Connection = conn;

        List<Reserva> Reservas = new List<Reserva>();

        SqlDataReader reader = cmd.ExecuteReader();

        while(reader.Read())
        {
            Reserva r = new Reserva();
            r.Id = reader.GetInt32(0);
            r.Cliente = reader.GetString(1);
            r.DataHora = reader.GetDateTime(2);
            r.QtdPessoas = reader.GetInt32(3);
            r.MesaId = reader.GetInt32(4);
            r.Status = reader.GetString(5);

            Reservas.Add(r);
        }

        return Reservas;
    }

    public Reserva Read(int id)
    {
        throw new NotImplementedException();
    }

    public void Update(Reserva Reserva)
    {
        throw new NotImplementedException();
    }
}