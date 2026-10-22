using M450_GS.App.TestDoubles;

namespace M450_GS.App.Tests.TestDoubles;

public class ConfirmationSenderSpy : IConfirmationSender
{
    public List<Reservation> Sent { get; } = new List<Reservation>();

    public void Send(Reservation reservation)
    {
        Sent.Add(reservation);
    }
}
