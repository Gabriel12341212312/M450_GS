using M450_GS.App.TestDoubles;
using Moq;

namespace M450_GS.App.Tests.TestDoubles;

public class ReservationServiceMoqTests
{
    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void Reserve_EnoughSeats_SavesCorrectReservationOnce(bool saveResult)
    {
        var repository = new Mock<IReservationRepository>();
        repository.Setup(r => r.TrySave(It.IsAny<Reservation>())).Returns(saveResult);
        var sender = new ConfirmationSenderSpy();
        var service = new ReservationService(new SeatAvailabilityStub("KONZERT", 3), repository.Object, sender);

        bool result = service.Reserve("anna@test.ch", "KONZERT", 2);

        Assert.Equal(saveResult, result);
        repository.Verify(r => r.TrySave(It.Is<Reservation>(x =>
            x.CustomerEmail == "anna@test.ch" && x.EventCode == "KONZERT" && x.SeatCount == 2)), Times.Once());
        Assert.Equal(saveResult, sender.Sent.Count == 1);
    }

    [Fact]
    public void Reserve_NotEnoughSeats_NeverSaves()
    {
        var repository = new Mock<IReservationRepository>();
        repository.Setup(r => r.TrySave(It.IsAny<Reservation>())).Returns(true);
        var sender = new ConfirmationSenderSpy();
        var service = new ReservationService(new SeatAvailabilityStub("KONZERT", 1), repository.Object, sender);

        bool result = service.Reserve("anna@test.ch", "KONZERT", 2);

        Assert.False(result);
        repository.Verify(r => r.TrySave(It.IsAny<Reservation>()), Times.Never());
        Assert.Empty(sender.Sent);
    }
}
