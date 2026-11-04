using GestaoReservas.Models;

// Atualiza o status das mesas conforme o status da reserva.
// N:N: a reserva pode ter várias mesas, então a regra é aplicada em cada uma delas.
public static class MesaReservaSync
{
    public static void AplicarStatusReserva(
        IReservaRepository reservas,
        IMesaRepository mesas,
        Reserva reserva,
        string statusReserva)
    {
        foreach (var mesaId in reserva.MesaIds.Distinct())
        {
            var mesa = mesas.Read(mesaId);
            if (mesa == null) continue;

            if (statusReserva == "Confirmada")
            {
                mesa.Status = "Reservada";
                mesas.Update(mesa);
                continue;
            }

            if (statusReserva is not ("Pendente" or "Cancelada")) continue;

            // Só libera a mesa se nenhuma outra reserva confirmada usa essa mesa
            var outraConfirmada = reservas.Read().Any(r =>
                r.Id != reserva.Id &&
                r.Status == "Confirmada" &&
                r.MesaIds.Contains(mesaId));

            if (outraConfirmada || mesa.Status != "Reservada") continue;

            mesa.Status = "Livre";
            mesas.Update(mesa);
        }
    }
}
