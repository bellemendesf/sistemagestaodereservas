using Microsoft.Data.SqlClient;

public class DataConnection
{
    protected SqlConnection conn;

    public DataConnection(IConfiguration configuration)
    {
        string connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Connection string 'DefaultConnection' is not configured.");
        conn = new SqlConnection(connectionString);
    }
}