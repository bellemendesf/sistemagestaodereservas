// using GestaoReservas.Models;

// public class PessoaMemoryRepository : IPessoaRepository
// {
//     List<Pessoa> lista = new List<Pessoa>();

//     public void Create(Pessoa Pessoa)
//     {
//         lista.Add(Pessoa);
//     }

//     public void Delete(int id)
//     {
//         foreach(var Pessoa in lista)
//         {
//             if(Pessoa.Id == id)
//             {
//                 lista.Remove(Pessoa);
//                 break;
//             }
//         }
//     }

//     public List<Pessoa> Read()
//     {
//         return lista;
//     }

//     public Pessoa Read(int id)
//     {
//         Pessoa p = null;
//         foreach(var Pessoa in lista)
//         {
//             if(Pessoa.Id == id)
//             {
//                 p = Pessoa;
//             }
//         }
//         return p;
//     }

//     public void Update(Pessoa p)
//     {
//         foreach(var Pessoa in lista)
//         {
//             if(Pessoa.Id == p.Id)
//             {
//                 Pessoa.Texto = p.Texto;
//             }
//         }
//     }
// }