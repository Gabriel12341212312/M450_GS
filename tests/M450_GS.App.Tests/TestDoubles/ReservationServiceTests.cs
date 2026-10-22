using M450_GS.App.TestDoubles;
using Moq;

namespace M450_GS.App.Tests.TestDoubles;

public class ReservationServiceTests
{
    [Theory]
    [InlineData(3, true)]
    [InlineData(1, false)]
    [InlineData(2, true)]
    public void Reserve_TwoSeatsRequested_ReturnsExpectedResult(int availableSeats, bool expected)
    {
        var availability = new SeatAvailabilityStub("KONZERT", availableSeats);
        var service = new ReservationService(availability, new ReservationRepositoryStub(true), new ConfirmationSenderSpy());

        bool result = service.Reserve("anna@test.ch", "KONZERT", 2);

        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Reserve_InvalidSeatCount_ThrowsAndCallsNothing(int seatCount)
    {
        var availability = new SeatAvailabilityStub("KONZERT", 10);
        var repository = new ReservationRepositoryStub(true);
        var sender = new ConfirmationSenderSpy();
        var service = new ReservationService(availability, repository, sender);

        Assert.Throws<ArgumentOutOfRangeException>(() => service.Reserve("anna@test.ch", "KONZERT", seatCount));

        Assert.Empty(availability.Calls);
        Assert.Equal(0, repository.Calls);
        Assert.Empty(sender.Sent);
    }

    [Fact]
    public void Reserve_EmptyEventCode_ThrowsAndCallsNothing()
    {
        var availability = new SeatAvailabilityStub("KONZERT", 10);
        var repository = new ReservationRepositoryStub(true);
        var sender = new ConfirmationSenderSpy();
        var service = new ReservationService(availability, repository, sender);

        Assert.Throws<ArgumentException>(() => service.Reserve("anna@test.ch", "", 2));

        Assert.Empty(availability.Calls);
        Assert.Equal(0, repository.Calls);
        Assert.Empty(sender.Sent);
    }

    [Fact]
    public void Reserve_SaveSucceeds_SendsOneConfirmation()
    {
        var sender = new ConfirmationSenderSpy();
        var service = new ReservationService(new SeatAvailabilityStub("KONZERT", 3), new ReservationRepositoryStub(true), sender);

        service.Reserve("anna@test.ch", "KONZERT", 2);

        Assert.Single(sender.Sent);
        Assert.Equal(new Reservation("anna@test.ch", "KONZERT", 2), sender.Sent[0]);
    }

    [Fact]
    public void Reserve_NotEnoughSeats_SendsNoConfirmation()
    {
        var sender = new ConfirmationSenderSpy();
        var service = new ReservationService(new SeatAvailabilityStub("KONZERT", 1), new ReservationRepositoryStub(true), sender);

        service.Reserve("anna@test.ch", "KONZERT", 2);

        Assert.Empty(sender.Sent);
    }

    [Fact]
    public void Reserve_SaveFails_SendsNoConfirmation()
    {
        var sender = new ConfirmationSenderSpy();
        var service = new ReservationService(new SeatAvailabilityStub("KONZERT", 3), new ReservationRepositoryStub(false), sender);

        service.Reserve("anna@test.ch", "KONZERT", 2);

        Assert.Empty(sender.Sent);
    }

    [Fact]
    public void Reserve_EmailOnlySpaces_ThrowsAndSavesNothing()
    {
        var availability = new SeatAvailabilityStub("KONZERT", 3);
        var repository = new Mock<IReservationRepository>();
        var sender = new ConfirmationSenderSpy();
        var service = new ReservationService(availability, repository.Object, sender);

        Assert.Throws<ArgumentException>(() => service.Reserve("   ", "KONZERT", 2));

        Assert.Empty(availability.Calls);
        repository.Verify(r => r.TrySave(It.IsAny<Reservation>()), Times.Never());
        Assert.Empty(sender.Sent);
    }
}
