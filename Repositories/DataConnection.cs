using Microsoft.Data.SqlClient;

public abstract class DataConnection : IDisposable
{
    protected readonly SqlConnection conn;

    protected DataConnection(IConfiguration configuration)
    {
        string connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Configure ConnectionStrings:DefaultConnection em appsettings.json.");
        conn = new SqlConnection(connectionString);
    }

    public void Dispose()
    {
        conn.Dispose();
        GC.SuppressFinalize(this);
    }
}
