using M450_GS.App.TestDoubles;

namespace M450_GS.App.Tests.TestDoubles;

public class SeatAvailabilityStub : ISeatAvailability
{
    private readonly Dictionary<string, int> _seats = new Dictionary<string, int>();

    public List<string> Calls { get; } = new List<string>();

    public SeatAvailabilityStub(string eventCode, int seats)
    {
        _seats[eventCode] = seats;
    }

    public void Add(string eventCode, int seats)
    {
        _seats[eventCode] = seats;
    }

    public int GetAvailableSeats(string eventCode)
    {
        Calls.Add(eventCode);
        return _seats.GetValueOrDefault(eventCode);
    }
}
