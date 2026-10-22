using M450_GS.App.TestDoubles;
using Moq;

namespace M450_GS.App.Tests.TestDoubles;

public class ReservationBatchTests
{
    [Theory]
    [InlineData("", "KONZERT", 1)]
    [InlineData("anna@test.ch", "", 1)]
    [InlineData("anna@test.ch", "KONZERT", 0)]
    public void ReserveBatch_InvalidEntry_ThrowsAndCallsNothing(string email, string eventCode, int seatCount)
    {
        var availability = new SeatAvailabilityStub("KONZERT", 10);
        var repository = new ReservationRepositoryStub(true);
        var sender = new ConfirmationSenderSpy();
        var service = new ReservationService(availability, repository, sender);
        var requests = new List<Reservation> { new Reservation(email, eventCode, seatCount) };

        Assert.ThrowsAny<ArgumentException>(() => service.ReserveBatch(requests));

        Assert.Empty(availability.Calls);
        Assert.Equal(0, repository.Calls);
        Assert.Empty(sender.Sent);
    }

    [Fact]
    public void ReserveBatch_EmptyList_Throws()
    {
        var service = new ReservationService(new SeatAvailabilityStub("KONZERT", 10), new ReservationRepositoryStub(true), new ConfirmationSenderSpy());

        Assert.Throws<ArgumentException>(() => service.ReserveBatch(new List<Reservation>()));
    }

    [Fact]
    public void ReserveBatch_SameEventTogetherTooMany_ReturnsFalseAndSavesNothing()
    {
        var repository = new Mock<IReservationRepository>();
        var sender = new ConfirmationSenderSpy();
        var service = new ReservationService(new SeatAvailabilityStub("KONZERT", 3), repository.Object, sender);
        var requests = new List<Reservation>
        {
            new Reservation("anna@test.ch", "KONZERT", 2),
            new Reservation("ben@test.ch", "KONZERT", 2)
        };

        bool result = service.ReserveBatch(requests);

        Assert.False(result);
        repository.Verify(r => r.TrySaveBatch(It.IsAny<IReadOnlyList<Reservation>>()), Times.Never());
        Assert.Empty(sender.Sent);
    }

    [Fact]
    public void ReserveBatch_EnoughSeats_SavesAllOnceAndSendsInOrder()
    {
        var availability = new SeatAvailabilityStub("KONZERT", 5);
        availability.Add("THEATER", 5);
        var repository = new Mock<IReservationRepository>();
        repository.Setup(r => r.TrySaveBatch(It.IsAny<IReadOnlyList<Reservation>>())).Returns(true);
        var sender = new ConfirmationSenderSpy();
        var service = new ReservationService(availability, repository.Object, sender);
        var requests = new List<Reservation>
        {
            new Reservation("anna@test.ch", "KONZERT", 2),
            new Reservation("ben@test.ch", "THEATER", 1),
            new Reservation("carla@test.ch", "KONZERT", 3)
        };

        bool result = service.ReserveBatch(requests);

        Assert.True(result);
        Assert.Equal(new List<string> { "KONZERT", "THEATER" }, availability.Calls);
        repository.Verify(r => r.TrySaveBatch(It.Is<IReadOnlyList<Reservation>>(x => x.SequenceEqual(requests))), Times.Once());
        Assert.Equal(requests, sender.Sent);
    }

    [Fact]
    public void ReserveBatch_SaveFails_ReturnsFalseAndSendsNothing()
    {
        var sender = new ConfirmationSenderSpy();
        var service = new ReservationService(new SeatAvailabilityStub("KONZERT", 5), new ReservationRepositoryStub(false), sender);
        var requests = new List<Reservation> { new Reservation("anna@test.ch", "KONZERT", 2) };

        bool result = service.ReserveBatch(requests);

        Assert.False(result);
        Assert.Empty(sender.Sent);
    }
}
