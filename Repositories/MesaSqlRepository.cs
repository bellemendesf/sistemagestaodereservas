using GestaoReservas.Models;
using Microsoft.Data.SqlClient;

public class MesaSqlRepository : DataConnection, IMesaRepository
{
    public void Create(Mesa Mesa)
    {
        throw new NotImplementedException();
    }

    public void Delete(int id)
    {
        throw new NotImplementedException();
    }

    public List<Mesa> Read()
    {
        SqlCommand cmd = new SqlCommand();
        cmd.CommandText = "SELECT * FROM Mesas";
        cmd.Connection = conn;

        List<Mesa> Mesas = new List<Mesa>();

        SqlDataReader reader = cmd.ExecuteReader();

        while(reader.Read())
        {
            Mesa m = new Mesa();
            m.IdMesa = reader.GetInt32(0);
            m.Texto = reader.GetString(1);
            m.Concluida = reader.GetBoolean(2);

            Mesas.Add(m);
        }

        return Mesas;
    }

    public Mesa Read(int id)
    {
        throw new NotImplementedException();
    }

    public void Update(Mesa Mesa)
    {
        throw new NotImplementedException();
    }
}