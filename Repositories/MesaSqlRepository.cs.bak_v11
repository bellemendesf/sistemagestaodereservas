// Importa os modelos do projeto
using GestaoReservas.Models;
// Importa as classes de acesso ao SQL Server
using Microsoft.Data.SqlClient;

// Repositório que grava e lê Mesa no SQL Server
public class MesaSqlRepository : DataConnection, IMesaRepository
{
    // Construtor: repassa a configuração para a classe base
    public MesaSqlRepository(IConfiguration configuration) : base(configuration) { }

   // Insere um novo registro no banco
   public void Create(Mesa mesa)
{
    // Cria o comando SQL (descartado ao final)
    using var cmd = new SqlCommand();
    // Associa o comando à conexão
    cmd.Connection = conn;

    // Texto SQL que será executado
    cmd.CommandText = @"
        INSERT INTO mesas
            (nr, numero, qtd_lugares, status, codigo_ambientes)
        VALUES
            (@Id, @Numero, @QtdLugares, @Status, @AmbienteId);";

    // Preenche o parâmetro @Id (evita SQL injection)
    cmd.Parameters.AddWithValue("@Id", mesa.Numero);
    // Preenche o parâmetro @Numero (evita SQL injection)
    cmd.Parameters.AddWithValue("@Numero", mesa.Numero);
    // Preenche o parâmetro @QtdLugares (evita SQL injection)
    cmd.Parameters.AddWithValue("@QtdLugares", mesa.QtdLugares);
    // Preenche o parâmetro @Status (evita SQL injection)
    cmd.Parameters.AddWithValue("@Status",
        mesa.Status == "Livre" ? "Disponivel" : mesa.Status);
    // Preenche o parâmetro @AmbienteId (evita SQL injection)
    cmd.Parameters.AddWithValue("@AmbienteId", mesa.AmbienteId);

    // Abre a conexão com o banco
    conn.Open();
    // Tenta executar; erros são tratados abaixo
    try
    {
        // Executa o comando (INSERT/UPDATE/DELETE)
        cmd.ExecuteNonQuery();
    }
    // Sempre executa, com ou sem erro
    finally
    {
        // Fecha a conexão com o banco
        conn.Close();
    }
}

// Lê todos os registros do banco
public List<Mesa> Read()
{
    // Lista que receberá os Mesa
    var mesas = new List<Mesa>();

    // Cria o comando SQL (descartado ao final)
    using var cmd = new SqlCommand();
    // Associa o comando à conexão
    cmd.Connection = conn;

    // Texto SQL que será executado
    cmd.CommandText = @"
        SELECT
            m.nr,
            m.numero,
            m.qtd_lugares,
            CASE
                WHEN m.status = 'Disponivel' THEN 'Livre'
                ELSE m.status
            END AS status,
            COALESCE(m.codigo_ambientes, 0),
            COALESCE(a.nome, 'Sem ambiente')
        FROM mesas m
        LEFT JOIN ambientes a
            ON m.codigo_ambientes = a.codigo
        ORDER BY m.numero;";

    // Abre a conexão com o banco
    conn.Open();

    // Tenta executar; erros são tratados abaixo
    try
    {
        // Executa a consulta e abre o leitor de resultados
        using SqlDataReader reader = cmd.ExecuteReader();

        // Percorre cada linha retornada
        while (reader.Read())
        {
            // Adiciona um Mesa à lista
            mesas.Add(new Mesa
            {
                // Lê a coluna 0 para Id
                Id = reader.GetInt32(0),
                // Lê a coluna 1 para Numero
                Numero = reader.GetInt32(1),
                // Lê a coluna 2 para QtdLugares
                QtdLugares = reader.GetInt32(2),
                // Lê a coluna 3 para Status
                Status = reader.GetString(3),
                // Lê a coluna 4 para AmbienteId
                AmbienteId = reader.GetInt32(4),
                // Lê a coluna 5 para AmbienteNome
                AmbienteNome = reader.GetString(5)
            });
        }
    }
    // Sempre executa, com ou sem erro
    finally
    {
        // Fecha a conexão com o banco
        conn.Close();
    }

    // Devolve o resultado
    return mesas;
}

   // Lê um registro pelo id (null se não existir)
   public Mesa? Read(int id)
{
    // mesa começa nulo (não encontrado)
    Mesa? mesa = null;

    // Cria o comando SQL (descartado ao final)
    using var cmd = new SqlCommand();
    // Associa o comando à conexão
    cmd.Connection = conn;

    // Texto SQL que será executado
    cmd.CommandText = @"
        SELECT
            nr,
            numero,
            qtd_lugares,
            CASE
                WHEN status = 'Disponivel' THEN 'Livre'
                ELSE status
            END,
            COALESCE(codigo_ambientes, 0)
        FROM mesas
        WHERE nr = @Id;";

    // Preenche o parâmetro @Id (evita SQL injection)
    cmd.Parameters.AddWithValue("@Id", id);

    // Abre a conexão com o banco
    conn.Open();

    // Tenta executar; erros são tratados abaixo
    try
    {
        // Executa a consulta e abre o leitor de resultados
        using SqlDataReader reader = cmd.ExecuteReader();

        // Se encontrou a linha, monta o objeto
        if (reader.Read())
        {
            // Monta o objeto Mesa com os dados da linha
            mesa = new Mesa
            {
                // Lê a coluna 0 para Id
                Id = reader.GetInt32(0),
                // Lê a coluna 1 para Numero
                Numero = reader.GetInt32(1),
                // Lê a coluna 2 para QtdLugares
                QtdLugares = reader.GetInt32(2),
                // Lê a coluna 3 para Status
                Status = reader.GetString(3),
                // Lê a coluna 4 para AmbienteId
                AmbienteId = reader.GetInt32(4)
            };
        }
    }
    // Sempre executa, com ou sem erro
    finally
    {
        // Fecha a conexão com o banco
        conn.Close();
    }

    // Devolve o resultado
    return mesa;
}

  // Atualiza um registro existente no banco
  public void Update(Mesa mesa)
{
    // Cria o comando SQL (descartado ao final)
    using var cmd = new SqlCommand();
    // Associa o comando à conexão
    cmd.Connection = conn;

    // Texto SQL que será executado
    cmd.CommandText = @"
        UPDATE mesas
        SET numero = @Numero,
            qtd_lugares = @QtdLugares,
            status = @Status,
            codigo_ambientes = @AmbienteId
        WHERE nr = @Id;";

    // Preenche o parâmetro @Numero (evita SQL injection)
    cmd.Parameters.AddWithValue("@Numero", mesa.Numero);
    // Preenche o parâmetro @QtdLugares (evita SQL injection)
    cmd.Parameters.AddWithValue("@QtdLugares", mesa.QtdLugares);
    // Preenche o parâmetro @Status (evita SQL injection)
    cmd.Parameters.AddWithValue("@Status",
        mesa.Status == "Livre" ? "Disponivel" : mesa.Status);
    // Preenche o parâmetro @AmbienteId (evita SQL injection)
    cmd.Parameters.AddWithValue("@AmbienteId", mesa.AmbienteId);
    // Preenche o parâmetro @Id (evita SQL injection)
    cmd.Parameters.AddWithValue("@Id", mesa.Id);

    // Abre a conexão com o banco
    conn.Open();

    // Tenta executar; erros são tratados abaixo
    try
    {
        // Executa o comando (INSERT/UPDATE/DELETE)
        cmd.ExecuteNonQuery();
    }
    // Sempre executa, com ou sem erro
    finally
    {
        // Fecha a conexão com o banco
        conn.Close();
    }
}
   // Remove (ou cancela) um registro no banco
   public void Delete(int id)
{
    // Abre uma conexão própria para a transação
    using var transactionConnection = new SqlConnection(
        conn.ConnectionString);

    // Abre a conexão da transação
    transactionConnection.Open();

    // Inicia a transação
    using var transaction = transactionConnection.BeginTransaction();

    // Tenta executar; erros são tratados abaixo
    try
    {
        // Verifica se existe alguma reserva NÃO cancelada
        // vinculada à mesa.
        using (var verificar = new SqlCommand(@"
            SELECT COUNT(*)
            FROM reservas
            WHERE mesas_nr = @Id
              AND status <> 2;", transactionConnection, transaction))
        {
            // Preenche o parâmetro @Id
            verificar.Parameters.AddWithValue("@Id", id);

            // Variável reservasAtivas
            int reservasAtivas = (int)verificar.ExecuteScalar()!;

            // Condição
            if (reservasAtivas > 0)
            {
                // Lança um erro com a mensagem
                throw new InvalidOperationException(
                    "Não é possível excluir esta mesa porque existem reservas não canceladas vinculadas a ela.");
            }
        }

        // Remove os vínculos da tabela associativa
        // somente para reservas canceladas.
        using (var excluirVinculos = new SqlCommand(@"
            DELETE rm
            FROM Reservas_Mesas rm
            INNER JOIN reservas r
                ON r.numero = rm.numero_reservas
            WHERE rm.nr_mesas = @Id
              AND r.status = 2;", transactionConnection, transaction))
        {
            // Preenche o parâmetro @Id
            excluirVinculos.Parameters.AddWithValue("@Id", id);
            // Executa o comando excluirVinculos
            excluirVinculos.ExecuteNonQuery();
        }

        // Remove as reservas canceladas da relação direta
        // com a mesa, mas preserva os registros das reservas.
        using (var desvincularReservas = new SqlCommand(@"
            UPDATE reservas
            SET mesas_nr = NULL
            WHERE mesas_nr = @Id
              AND status = 2;", transactionConnection, transaction))
        {
            // Preenche o parâmetro @Id
            desvincularReservas.Parameters.AddWithValue("@Id", id);
            // Executa o comando desvincularReservas
            desvincularReservas.ExecuteNonQuery();
        }

        // Exclui a mesa somente depois de tratar os vínculos.
        using (var excluirMesa = new SqlCommand(
            "DELETE FROM mesas WHERE nr = @Id;",
            transactionConnection, transaction))
        {
            // Preenche o parâmetro @Id
            excluirMesa.Parameters.AddWithValue("@Id", id);
            // Executa o comando excluirMesa
            excluirMesa.ExecuteNonQuery();
        }

        // Confirma a transação
        transaction.Commit();
    }
    // Qualquer erro
    catch
    {
        // Desfaz a transação em caso de erro
        transaction.Rollback();
        // Repassa o erro
        throw;
    }
} }