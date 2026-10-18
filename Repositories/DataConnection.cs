using Microsoft.Data.SqlClient;

public class DataConnection
{
    protected SqlConnection conn;

    public DataConnection(IConfiguration configuration)
    {
        string connectionString = configuration.GetConnectionString("DefaultConnection");
        conn = new SqlConnection(connectionString);
    }
}