using GestaoReservas.Models;

namespace GestaoReservas.Tests;

// Repositórios em memória usados apenas pelos testes.
public class AmbienteMemoryRepository : IAmbienteRepository
{
    private readonly List<Ambiente> itens = new();
    private int proximo = 1;
    public List<Ambiente> Read() => itens.ToList();
    public Ambiente? Read(int id) => itens.FirstOrDefault(x => x.Id == id);
    public void Create(Ambiente a) { a.Id = proximo++; itens.Add(a); }
    public void Update(Ambiente a) { Delete(a.Id); itens.Add(a); }
    public void Delete(int id) => itens.RemoveAll(x => x.Id == id);
}

public class MesaMemoryRepository : IMesaRepository
{
    private readonly List<Mesa> itens = new();
    private int proximo = 1;
    public List<Mesa> Read() => itens.ToList();
    public Mesa? Read(int id) => itens.FirstOrDefault(x => x.Id == id);
    public void Create(Mesa m) { m.Id = proximo++; itens.Add(m); }
    public void Update(Mesa m) { Delete(m.Id); itens.Add(m); }
    public void Delete(int id) => itens.RemoveAll(x => x.Id == id);
}

public class ClienteMemoryRepository : IClienteRepository
{
    private readonly List<Cliente> itens = new();
    private int proximo = 1;
    public List<Cliente> Read() => itens.ToList();
    public Cliente? Read(int id) => itens.FirstOrDefault(x => x.Id == id);
    public void Create(Cliente c) { c.Id = proximo++; itens.Add(c); }
    public void Update(Cliente c)
    {
        var atual = Read(c.Id);
        if (atual != null && string.IsNullOrEmpty(c.Senha)) c.Senha = atual.Senha;
        Delete(c.Id); itens.Add(c);
    }
    public void Delete(int id) => itens.RemoveAll(x => x.Id == id);
}

public class FuncionarioMemoryRepository : IFuncionarioRepository
{
    private readonly List<Funcionario> itens = new();
    private int proximo = 1;
    public List<Funcionario> Read() => itens.ToList();
    public Funcionario? Read(int id) => itens.FirstOrDefault(x => x.Id == id);
    public void Create(Funcionario f) { f.Id = proximo++; itens.Add(f); }
    public void Update(Funcionario f)
    {
        var atual = Read(f.Id);
        if (atual != null && string.IsNullOrEmpty(f.Senha)) f.Senha = atual.Senha;
        Delete(f.Id); itens.Add(f);
    }
    public void Delete(int id) => itens.RemoveAll(x => x.Id == id);
}

public class ReservaMemoryRepository : IReservaRepository
{
    private readonly List<Reserva> itens = new();
    private int proximo = 1;
    public List<Reserva> Read() => itens.ToList();
    public Reserva? Read(int id) => itens.FirstOrDefault(x => x.Id == id);
    public void Create(Reserva r) { r.Id = proximo++; r.Status = "Pendente"; itens.Add(r); }
    public void Update(Reserva r)
    {
        var atual = Read(r.Id);
        if (atual == null || atual.Status == "Cancelada") return;
        atual.MesaId = r.MesaId; atual.QtdPessoas = r.QtdPessoas; atual.DataHora = r.DataHora;
    }
    public void UpdateStatus(int id, string status)
    {
        var atual = Read(id);
        if (atual != null && atual.Status != "Cancelada") atual.Status = status;
    }
    public void Delete(int id) { var atual = Read(id); if (atual != null) atual.Status = "Cancelada"; }
}
