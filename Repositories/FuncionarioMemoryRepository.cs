using GestaoReservas.Models;

// CORREÇÃO: reativa o Repository em memória, conforme TarefaMemoryRepository do professor.
public class FuncionarioMemoryRepository : IFuncionarioRepository
{
    List<Funcionario> lista = new List<Funcionario>();
    int proximoId = 1;

    public void Create(Funcionario funcionario)
    {
        funcionario.Id = proximoId++;
        lista.Add(funcionario);
    }

    public void Delete(int id)
    {
        foreach (var item in lista)
        {
            if (item.Id == id)
            {
                lista.Remove(item);
                break;
            }
        }
    }

    public List<Funcionario> Read()
    {
        return lista;
    }

    public Funcionario Read(int id)
    {
        Funcionario resultado = null!;
        foreach (var item in lista)
        {
            if (item.Id == id)
            {
                resultado = item;
            }
        }
        return resultado;
    }

    public void Update(Funcionario funcionario)
    {
        foreach (var item in lista)
        {
            if (item.Id == funcionario.Id)
            {
                item.Nome = funcionario.Nome;
                item.Cpf = funcionario.Cpf;
                item.EmailPessoal = funcionario.EmailPessoal;
                item.EmailInstitucional = funcionario.EmailInstitucional;
                if (funcionario.Senha != null && funcionario.Senha != "")
                {
                    item.Senha = funcionario.Senha;
                }
            }
        }
    }
}
