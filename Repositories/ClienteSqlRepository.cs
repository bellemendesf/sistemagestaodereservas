// using GestaoReservas.Models;
// using Microsoft.Data.SqlClient;

// public class ClienteSqlRepository : DataConnection, IClienteRepository
// {
//     public void Create(Cliente
//  Cliente
// )
//     {
//         throw new NotImplementedException();
//     }

//     public void Delete(int id)
//     {
//         throw new NotImplementedException();
//     }

//     public List<Cliente
// > Read()
//     {
//         SqlCommand cmd = new SqlCommand();
//         cmd.CommandText = "SELECT * FROM Clientes";
//         cmd.Connection = conn;

//         List<Cliente> Clientes = new List<Cliente>();

//         SqlDataReader reader = cmd.ExecuteReader();

//         while(reader.Read())
//         {
//             Cliente
//             c = new Cliente();
//             c.Telefone = reader.GetString(0);

//             Clientes.Add(c);
//         }

//         return Clientes;
//     }

//     public Cliente
//  Read(int id)
//     {
//         throw new NotImplementedException();
//     }

//     public void Update(Cliente Cliente)
//     {
//         throw new NotImplementedException();
//     }
// }