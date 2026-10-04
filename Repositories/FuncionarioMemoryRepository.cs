using GestaoReservas.Models;

public class FuncionarioMemoryRepository : IFuncionarioRepository
{
    List<Funcionario> lista = new List<Funcionario>();

    public void Create(Funcionario Funcionario)
    {
        lista.Add(Funcionario);
    }

    public void Create(Funcionario Funcionario)
    {
        throw new NotImplementedException();
    }

    public void Delete(int id)
    {
        foreach(var Funcionario in lista)
        {
            if(Funcionario.Id == id)
            {
                lista.Remove(Funcionario);
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
        Funcionario f = null;
        foreach(var Funcionario in lista)
        {
            if(Funcionario.Id == id)
            {
                f = Funcionario;
            }
        }
        return f;
    }

    public void Update(Funcionario f)
    {
        foreach(var Funcionario in lista)
        {
            if(Funcionario.Id == f.Id)
            {
                Funcionario.Email_inst = f.Email_inst;
            }
        }
    }

    public void Update(Funcionario Funcionario)
    {
        throw new NotImplementedException();
    }

    List<Funcionario> IFuncionarioRepository.Read()
    {
        throw new NotImplementedException();
    }

    Funcionario IFuncionarioRepository.Read(int id)
    {
        throw new NotImplementedException();
    }
}