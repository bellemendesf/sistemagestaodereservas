using GestaoReservas.Models;
using Microsoft.Data.SqlClient;

public class AmbienteSqlRepository : DataConnection, IAmbienteRepository
{
    public void Create(Ambiente Ambiente)
    {
        throw new NotImplementedException();
    }

    public void Delete(int id)
    {
        throw new NotImplementedException();
    }

    public List<Ambiente> Read()
    {
        SqlCommand cmd = new SqlCommand();
        cmd.CommandText = "SELECT * FROM Ambientes";
        cmd.Connection = conn;

        List<Ambiente> Ambientes = new List<Ambiente>();

        SqlDataReader reader = cmd.ExecuteReader();

        while(reader.Read())
        {
            Ambiente a = new Ambiente();
            a.Id = reader.GetInt32(0);
            a.Nome = reader.GetString(1);
            a.Capacidade = reader.GetInt32(2);
            a.Status = reader.GetString(3);

            Ambientes.Add(a);
        }

        return Ambientes;
    }

    public Ambiente Read(int id)
    {
        throw new NotImplementedException();
    }

    public void Update(Ambiente Ambiente)
    {
        throw new NotImplementedException();
    }
}