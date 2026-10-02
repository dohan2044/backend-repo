using FluentAssertions;
using Journey_of_faith.Application.usecases.churchs.commands;
using Journey_of_faith.Domain.interfaces;
using Moq;

namespace UnitTesting.HandlerTests.churchs;

public class CreateMassAndLiturgyHandlerTest
{
    [Fact]
    public async Task Handle_ShouldMapMassScheduleAndLiturgy_ThenReturnRepositoryResult()
    {
        var repository = new Mock<IChurchRepository>();
        MassAndLiturgyInsert? capturedItem = null;
        repository
            .Setup(x => x.CreateMassAndLiturgyAsync(It.IsAny<IReadOnlyCollection<MassAndLiturgyInsert>>()))
            .Callback<IReadOnlyCollection<MassAndLiturgyInsert>>(items => capturedItem = items.Single())
            .ReturnsAsync(true);

        var handler = new CreateMassAndLiturgyHandler(repository.Object);
        var massDate = new DateTime(2026, 9, 13);
        var command = new CreateMassAndLiturgyCommand
        {
            Items =
            [
                new CreateMassAndLiturgyItem
                {
                    CreateMassSchedule = new CreateMassScheduleCommand
                    {
                        ChurchId = 12,
                        Name = "Chua nhat XXIV thuong nien",
                        Date = massDate,
                        Time = "18:00"
                    },
                    CreateLiturgy = new CreateLiturgyCommand
                    {
                        Reading = "Bai doc mot",
                        ResponsorialPsalm = "Dap ca",
                        Gospel = "Tin mung",
                        EndWord = "Loi nguyen ket"
                    }
                }
            ]
        };

        var result = await handler.Handle(command, CancellationToken.None);

        result.Should().BeTrue();
        capturedItem.Should().NotBeNull();
        capturedItem!.MassSchedules.ChurchId.Should().Be(12);
        capturedItem.MassSchedules.Name.Should().Be("Chua nhat XXIV thuong nien");
        capturedItem.MassSchedules.Date.Should().Be(massDate);
        capturedItem.MassSchedules.Time.Should().Be("18:00");
        capturedItem.Liturgies.ReadingOne.Should().Be("Bai doc mot");
        capturedItem.Liturgies.ResponsorialPsalm.Should().Be("Dap ca");
        capturedItem.Liturgies.GoodNew.Should().Be("Tin mung");
        capturedItem.Liturgies.EndWord.Should().Be("Loi nguyen ket");
    }

    [Fact]
    public async Task Handle_ShouldNotCallRepository_WhenCancellationIsRequested()
    {
        var repository = new Mock<IChurchRepository>();
        var handler = new CreateMassAndLiturgyHandler(repository.Object);
        using var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.Cancel();

        var act = () => handler.Handle(new CreateMassAndLiturgyCommand(), cancellationTokenSource.Token);

        await act.Should().ThrowAsync<OperationCanceledException>();
        repository.Verify(
            x => x.CreateMassAndLiturgyAsync(It.IsAny<IReadOnlyCollection<MassAndLiturgyInsert>>()),
            Times.Never);
    }
}
