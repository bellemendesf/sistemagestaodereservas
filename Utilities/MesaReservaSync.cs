using GestaoReservas.Models;

public static class MesaReservaSync
{
    public static void AplicarStatusReserva(
        IReservaRepository reservas,
        IMesaRepository mesas,
        Reserva reserva,
        string statusReserva)
    {
        if (reserva.MesaId is not int mesaId) return;
        var mesa = mesas.Read(mesaId);
        if (mesa == null) return;

        if (statusReserva == "Confirmada")
        {
            mesa.Status = "Reservada";
            mesas.Update(mesa);
            return;
        }

        if (statusReserva is not ("Pendente" or "Cancelada")) return;

        var outraConfirmada = reservas.Read().Any(r =>
            r.Id != reserva.Id &&
            r.Status == "Confirmada" &&
            r.MesaId == mesaId);

        if (outraConfirmada || mesa.Status != "Reservada") return;

        mesa.Status = "Livre";
        mesas.Update(mesa);
    }
}
