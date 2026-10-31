// Importa as classes de acesso ao SQL Server
using Microsoft.Data.SqlClient;

// Classe base DataConnection
public abstract class DataConnection : IDisposable
{
    // Conexão com o banco compartilhada pelos repositórios
    protected readonly SqlConnection conn;

    // Construtor: cria a conexão a partir do appsettings.json
    protected DataConnection(IConfiguration configuration)
    {
        // Variável connectionString
        string connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Configure ConnectionStrings:DefaultConnection em appsettings.json.");
        // Cria a conexão com o banco
        conn = new SqlConnection(connectionString);
    }

    // Libera a conexão quando o objeto é descartado
    public void Dispose()
    {
        // Descarta a conexão
        conn.Dispose();
        // Evita finalização duplicada
        GC.SuppressFinalize(this);
    }
}
