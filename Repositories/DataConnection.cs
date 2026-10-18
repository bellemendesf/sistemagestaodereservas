// using Microsoft.Data.SqlClient;

// public abstract class DataConnection
// {
//     protected SqlConnection conn;

//     public DataConnection()
//     {
//         string strConn = @"localhost\MSSQLSERVER04;
//         Integrated Security=True
//         Database=mesaReserva;
//         TrustServerCertificate=true";
//         conn = new SqlConnection(strConn);
//         conn.Open();
//     }

//     public void Dispose()
//     {
//         conn.Close();
//     }
// }