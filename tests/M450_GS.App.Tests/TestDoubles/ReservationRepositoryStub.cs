using M450_GS.App.TestDoubles;

namespace M450_GS.App.Tests.TestDoubles;

public class ReservationRepositoryStub : IReservationRepository
{
    private readonly bool _result;

    public int Calls { get; private set; }

    public ReservationRepositoryStub(bool result)
    {
        _result = result;
    }

    public bool TrySave(Reservation reservation)
    {
        Calls++;
        return _result;
    }

    public bool TrySaveBatch(IReadOnlyList<Reservation> reservations)
    {
        Calls++;
        return _result;
    }
}
