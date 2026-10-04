using GestaoReservas.Models;
using Microsoft.Data.SqlClient;

public class PessoaSqlRepository : DataConnection, IPessoaRepository
{
    public void Create(Pessoa Pessoa)
    {
        throw new NotImplementedException();
    }

    public void Delete(int id)
    {
        throw new NotImplementedException();
    }

    public List<Pessoa> Read()
    {
        SqlCommand cmd = new SqlCommand();
        cmd.CommandText = "SELECT * FROM Pessoas";
        cmd.Connection = conn;

        List<Pessoa> Pessoas = new List<Pessoa>();

        SqlDataReader reader = cmd.ExecuteReader();

        while(reader.Read())
        {
            Pessoa p = new Pessoa();
            p.Id = reader.GetInt32(0);
            p.Nome = reader.GetString(1);
            p.Cpf = reader.GetString(2);

            Pessoas.Add(p);
        }

        return Pessoas;
    }

    public Pessoa Read(int id)
    {
        throw new NotImplementedException();
    }

    public void Update(Pessoa Pessoa)
    {
        throw new NotImplementedException();
    }
}