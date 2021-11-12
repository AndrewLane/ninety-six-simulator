using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using Moq;
using NinetySixSimulator.Services;
using NinetySixSimulator.Services.Models;
using NinetySixSimulator.Tests.Utils;
using Xunit;

namespace NinetySixSimulator.Tests;

public class CoordinateGamePlayTests
{
    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    public void PlayTestWhenOnePlayerLosesImmediatelyAndThenWeTimeOut(int winner)
    {
        var loggerDouble = new LoggerDouble<CoordinateGameplay>();
        var gamePlayMock = new Mock<IPlayWar>();
        var pointsCalculatorMock = new Mock<IPointsCalculator>();
        var individualGameStateTrackerMock = new Mock<ITrackIndividualGameState>();
        var timerMock = new Mock<IDateTime>();
        var statsCompilerMock = new Mock<ICompileStats>();
        var singleGameStatsMock = new Mock<ISingleGameStats>();

        var dummyStart = new DateTime(year: 2011, month: 8, day: 23, hour: 16, minute: 0, second: 0);
        var dummySimulationEnd = new DateTime(year: 2011, month: 8, day: 23, hour: 16, minute: 0, second: 3); // 3 seconds later
        timerMock.SetupSequence(mock => mock.Now)
            .Returns(dummyStart)
            .Returns(dummySimulationEnd);

        var fakeGameState = new Mock<ITrackIndividualGameState>();

        var player1DummyState = new PlayerGameState { };
        var player2DummyState = new PlayerGameState { };
        fakeGameState.Setup(mock => mock.FirstPlayerState).Returns(player1DummyState);
        fakeGameState.Setup(mock => mock.SecondPlayerState).Returns(player2DummyState);
        var dummyDuration = TimeSpan.FromMinutes(5);
        fakeGameState.Setup(mock => mock.TimeElapsed).Returns(dummyDuration);
        fakeGameState.Setup(mock => mock.Stats).Returns(singleGameStatsMock.Object);

        gamePlayMock.Setup(mock => mock.PlayerHasLost(player1DummyState)).Returns(winner == 2);
        gamePlayMock.Setup(mock => mock.PlayerHasLost(player2DummyState)).Returns(winner == 1);

        individualGameStateTrackerMock.Setup(mock => mock.InitializeGameState()).Returns(fakeGameState.Object);

        var objectUnderTest = new CoordinateGameplay(loggerDouble, gamePlayMock.Object, pointsCalculatorMock.Object,
            individualGameStateTrackerMock.Object, timerMock.Object, statsCompilerMock.Object);

        var dummySimulationDuration = TimeSpan.FromMinutes(4);
        var dummyGameParams = new GameParameters
        {
            FirstPlayerName = "Isaac",
            SecondPlayerName = "Daddy",
            TotalLengthOfSimulation = dummySimulationDuration
        };

        var result = objectUnderTest.Play(dummyGameParams);
        Assert.True(result.TotalSimulationTime == (dummySimulationEnd - dummyStart));

        Assert.True(loggerDouble.DebugEntries.Count() == 3);
        Assert.True(loggerDouble.InformationEntries.Count() == 1);

        Assert.True(loggerDouble.HasBeenLogged(LogLevel.Debug, $"{ dummyGameParams.FirstPlayerName} has { (winner == 1 ? 1 : 0)} wins, " +
            $"{dummyGameParams.SecondPlayerName} has {(winner == 2 ? 1 : 0)} wins, 0 ties"));
        Assert.True(loggerDouble.HasBeenLogged(LogLevel.Debug, $"Player {winner} has won this game!"));

        Assert.True(loggerDouble.HasBeenLogged(LogLevel.Information, "Simulation took 3 seconds..."));

        //make sure we only played war once
        gamePlayMock.Verify(mock => mock.War(It.IsAny<ITrackIndividualGameState>(), It.IsAny<int>()), Times.Once());

        //make sure we never had to transition
        gamePlayMock.Verify(mock => mock.Transition(It.IsAny<ITrackIndividualGameState>()), Times.Never());

        //make sure there was no need to calculate points
        pointsCalculatorMock.Verify(mock => mock.GetPoints(It.IsAny<List<Card>>()), Times.Never());

        singleGameStatsMock.VerifySet(mock => mock.Winner = winner, Times.Once());
        singleGameStatsMock.VerifySet(mock => mock.SimulationTime = It.IsAny<TimeSpan>(), Times.Once());
        statsCompilerMock.Verify(mock => mock.UpdateStats(It.IsAny<ISimulationStats>(), It.IsAny<ISingleGameStats>()), Times.Once());
    }

    [Fact]
    public void PlayTestWhenSimulationEndsAfterTwoTieGamesInARow()
    {
        var loggerDouble = new LoggerDouble<CoordinateGameplay>();
        var gamePlayMock = new Mock<IPlayWar>();
        var pointsCalculatorMock = new Mock<IPointsCalculator>();
        var individualGameStateTrackerMock = new Mock<ITrackIndividualGameState>();
        var timerMock = new Mock<IDateTime>();
        var statsCompilerMock = new Mock<ICompileStats>();
        var singleGameStatsMock = new Mock<ISingleGameStats>();

        var dummyStart = new DateTime(year: 2011, month: 8, day: 23, hour: 16, minute: 0, second: 0);
        var dummySimulationEnd = new DateTime(year: 2011, month: 8, day: 23, hour: 16, minute: 0, second: 3); // 3 seconds later
        timerMock.SetupSequence(mock => mock.Now)
            .Returns(dummyStart)
            .Returns(dummySimulationEnd);

        // use a real game state to easily initialize a full deck
        var realGameState = new TrackIndividualGameState().InitializeGameState();
        var fakeGameState = new Mock<ITrackIndividualGameState>();

        var player1DummyState = realGameState.FirstPlayerState;
        var player2DummyState = realGameState.SecondPlayerState;
        fakeGameState.Setup(mock => mock.FirstPlayerState).Returns(player1DummyState);
        fakeGameState.Setup(mock => mock.SecondPlayerState).Returns(player2DummyState);
        fakeGameState.SetupSequence(mock => mock.TimeElapsed)
            .Returns(TimeSpan.FromMinutes(4)) // first game 4 minutes
            .Returns(TimeSpan.FromMinutes(4)) // but TimeElapsed is invoked ...
            .Returns(TimeSpan.FromMinutes(4)) // three times
            .Returns(TimeSpan.FromMinutes(7)) // second game 7 minutes
            .Returns(TimeSpan.FromMinutes(7)) // but TimeElapsed gets invoked ...
            .Returns(TimeSpan.FromMinutes(7)); // trhee times
        fakeGameState.Setup(mock => mock.Stats).Returns(singleGameStatsMock.Object);

        gamePlayMock.Setup(mock => mock.PlayerHasLost(It.IsAny<PlayerGameState>())).Returns(false);

        fakeGameState.SetupSequence(mock => mock.TimedOut)
            .Returns(false)
            .Returns(false)
            .Returns(true)
            .Returns(false)
            .Returns(true);

        individualGameStateTrackerMock.Setup(mock => mock.InitializeGameState()).Returns(fakeGameState.Object);

        pointsCalculatorMock.Setup(mock => mock.GetPoints(It.IsAny<List<Card>>())).Returns(48); // always return a tie

        var objectUnderTest = new CoordinateGameplay(loggerDouble, gamePlayMock.Object, pointsCalculatorMock.Object,
            individualGameStateTrackerMock.Object, timerMock.Object, statsCompilerMock.Object);

        var dummySimulationDuration = TimeSpan.FromMinutes(10);
        var dummyGameParams = new GameParameters
        {
            FirstPlayerName = "Isaac",
            SecondPlayerName = "Daddy",
            TotalLengthOfSimulation = dummySimulationDuration
        };

        var result = objectUnderTest.Play(dummyGameParams);
        Assert.True(result.TotalSimulationTime == (dummySimulationEnd - dummyStart));

        Assert.True(loggerDouble.DebugEntries.Count() == 8);
        Assert.True(loggerDouble.InformationEntries.Count() == 1);

        Assert.True(loggerDouble.HasBeenLogged(LogLevel.Debug, $"{ dummyGameParams.FirstPlayerName} has 0 wins, " +
$"{dummyGameParams.SecondPlayerName} has 0 wins, 1 ties"));
        Assert.True(loggerDouble.HasBeenLogged(LogLevel.Debug, $"{ dummyGameParams.FirstPlayerName} has 0 wins, " +
            $"{dummyGameParams.SecondPlayerName} has 0 wins, 2 ties"));
        Assert.True(loggerDouble.HasBeenLogged(LogLevel.Debug, "This game is a tie."));
        Assert.True(loggerDouble.HasBeenLogged(LogLevel.Debug, "Game has timed out after 240 seconds."));
        Assert.True(loggerDouble.HasBeenLogged(LogLevel.Debug, "Game has timed out after 420 seconds."));

        Assert.True(loggerDouble.HasBeenLogged(LogLevel.Information, "Simulation took 3 seconds..."));

        gamePlayMock.Verify(mock => mock.War(It.IsAny<ITrackIndividualGameState>(), It.IsAny<int>()), Times.Exactly(5));

        gamePlayMock.Verify(mock => mock.Transition(It.IsAny<ITrackIndividualGameState>()), Times.Exactly(5));

        pointsCalculatorMock.Verify(mock => mock.GetPoints(It.IsAny<List<Card>>()), Times.Exactly(4));

        singleGameStatsMock.VerifySet(mock => mock.Winner = 0, Times.Exactly(2));
        singleGameStatsMock.VerifySet(mock => mock.SimulationTime = It.IsAny<TimeSpan>(), Times.Exactly(2));
        statsCompilerMock.Verify(mock => mock.UpdateStats(It.IsAny<ISimulationStats>(), It.IsAny<ISingleGameStats>()), Times.Exactly(2));
    }

    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    public void PlayTestWhenWithTimeoutButAWinner(int winner)
    {
        var loggerDouble = new LoggerDouble<CoordinateGameplay>();
        var gamePlayMock = new Mock<IPlayWar>();
        var pointsCalculatorMock = new Mock<IPointsCalculator>();
        var individualGameStateTrackerMock = new Mock<ITrackIndividualGameState>();
        var timerMock = new Mock<IDateTime>();
        var statsCompilerMock = new Mock<ICompileStats>();
        var singleGameStatsMock = new Mock<ISingleGameStats>();

        // use a real game state to easily initialize a full deck
        var realGameState = new TrackIndividualGameState().InitializeGameState();
        var fakeGameState = new Mock<ITrackIndividualGameState>();

        var player1DummyState = realGameState.FirstPlayerState;
        var player2DummyState = realGameState.SecondPlayerState;
        fakeGameState.Setup(mock => mock.FirstPlayerState).Returns(player1DummyState);
        fakeGameState.Setup(mock => mock.SecondPlayerState).Returns(player2DummyState);
        var dummyDuration = TimeSpan.FromMinutes(10);
        fakeGameState.Setup(mock => mock.TimeElapsed).Returns(dummyDuration);

        gamePlayMock.Setup(mock => mock.PlayerHasLost(It.IsAny<PlayerGameState>())).Returns(false);

        fakeGameState.Setup(mock => mock.TimedOut).Returns(true);
        fakeGameState.Setup(mock => mock.Stats).Returns(singleGameStatsMock.Object);

        individualGameStateTrackerMock.Setup(mock => mock.InitializeGameState()).Returns(fakeGameState.Object);

        pointsCalculatorMock.SetupSequence(mock => mock.GetPoints(It.IsAny<List<Card>>()))
            .Returns(winner == 1 ? 49 : 47)
            .Returns(winner == 2 ? 49 : 47);

        var objectUnderTest = new CoordinateGameplay(loggerDouble, gamePlayMock.Object, pointsCalculatorMock.Object,
            individualGameStateTrackerMock.Object, timerMock.Object, statsCompilerMock.Object);

        var dummySimulationDuration = TimeSpan.FromMinutes(10);
        var dummyGameParams = new GameParameters
        {
            FirstPlayerName = "Isaac",
            SecondPlayerName = "Daddy",
            TotalLengthOfSimulation = dummySimulationDuration
        };

        objectUnderTest.Play(dummyGameParams);

        Assert.True(loggerDouble.DebugEntries.Count() == 4);
        Assert.True(loggerDouble.InformationEntries.Count() == 1);

        Assert.True(loggerDouble.HasBeenLogged(LogLevel.Debug, $"{ dummyGameParams.FirstPlayerName} has {(winner == 1 ? 1 : 0)} wins, " +
$"{dummyGameParams.SecondPlayerName} has {(winner == 2 ? 1 : 0)} wins, 0 ties"));
        Assert.True(loggerDouble.HasBeenLogged(LogLevel.Debug, $"Player {winner} has won this game!"));

        gamePlayMock.Verify(mock => mock.War(It.IsAny<ITrackIndividualGameState>(), It.IsAny<int>()), Times.Once());
        gamePlayMock.Verify(mock => mock.Transition(It.IsAny<ITrackIndividualGameState>()), Times.Once());
        pointsCalculatorMock.Verify(mock => mock.GetPoints(It.IsAny<List<Card>>()), Times.Exactly(2));
        singleGameStatsMock.VerifySet(mock => mock.Winner = winner, Times.Once());
        singleGameStatsMock.VerifySet(mock => mock.SimulationTime = It.IsAny<TimeSpan>(), Times.Once());
        statsCompilerMock.Verify(mock => mock.UpdateStats(It.IsAny<ISimulationStats>(), It.IsAny<ISingleGameStats>()), Times.Once());
    }

}
