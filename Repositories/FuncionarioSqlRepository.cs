// using GestaoReservas.Models;
// using Microsoft.Data.SqlClient;

// public class FuncionarioSqlRepository : DataConnection, IFuncionarioRepository
// {
//     public void Create(Funcionario Funcionario)
//     {
//         throw new NotImplementedException();
//     }

//     public void Delete(int id)
//     {
//         throw new NotImplementedException();
//     }

//     public List<Funcionario> Read()
//     {
//         SqlCommand cmd = new SqlCommand();
//         cmd.CommandText = "SELECT * FROM Funcionarios";
//         cmd.Connection = conn;

//         List<Funcionario> Funcionarios = new List<Funcionario>();

//         SqlDataReader reader = cmd.ExecuteReader();

//         while(reader.Read())
//         {
//             Funcionario f = new Funcionario();
//             f.Id = reader.GetInt32(0);
//             f.Nome = reader.GetString(1);
//             f.Concluida = reader.GetBoolean(2);

//             Funcionarios.Add(f);
//         }

//         return Funcionarios;
//     }

//     public Funcionario Read(int id)
//     {
//         throw new NotImplementedException();
//     }

//     public void Update(Funcionario Funcionario)
//     {
//         throw new NotImplementedException();
//     }
// }