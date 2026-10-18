using GestaoReservas.Models;

// CORREÇÃO: reativa o Repository em memória, conforme TarefaMemoryRepository do professor.
public class FuncionarioMemoryRepository : IFuncionarioRepository
{
    List<Funcionario> lista = new List<Funcionario>();
    // CORREÇÃO: IDs distintos são necessários para as rotas Edit, Details e Delete das Views.
    int proximoId = 1;

    public void Create(Funcionario funcionario)
    {
        // CORREÇÃO: atribui o identificador utilizado pelo CRUD antes de adicionar à lista.
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
                // CORREÇÃO: atualiza os dados de pessoa e o e-mail institucional editados.
                item.Nome = funcionario.Nome;
                item.Cpf = funcionario.Cpf;
                item.EmailPessoal = funcionario.EmailPessoal;
                item.EmailInstitucional = funcionario.EmailInstitucional;
                // CORREÇÃO: não zera Codigo e conserva a senha se não foi informada nova senha.
                if (funcionario.Senha != null && funcionario.Senha != "")
                {
                    item.Senha = funcionario.Senha;
                }
            }
        }
    }
}
