using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using Moq;
using NinetySixSimulator.Services;
using NinetySixSimulator.Services.Models;
using NinetySixSimulator.Tests.Utils;
using Xunit;

namespace NinetySixSimulator.Tests
{
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

            gamePlayMock.Setup(mock => mock.PlayerHasLost(player1DummyState)).Returns(winner == 2);
            gamePlayMock.Setup(mock => mock.PlayerHasLost(player2DummyState)).Returns(winner == 1);

            individualGameStateTrackerMock.Setup(mock => mock.InitializeGameState()).Returns(fakeGameState.Object);

            var objectUnderTest = new CoordinateGameplay(loggerDouble, gamePlayMock.Object, pointsCalculatorMock.Object, individualGameStateTrackerMock.Object, timerMock.Object);

            var dummySimulationDuration = TimeSpan.FromMinutes(4);
            var dummyGameParams = new GameParameters
            {
                FirstPlayerName = "Isaac",
                SecondPlayerName = "Daddy",
                TotalLengthOfSimulation = dummySimulationDuration
            };

            objectUnderTest.Play(dummyGameParams);

            Assert.True(loggerDouble.DebugEntries.Count() == 3);
            Assert.True(loggerDouble.InformationEntries.Count() == 1);

            Assert.True(loggerDouble.HasBeenLogged(LogLevel.Debug, $"{ dummyGameParams.FirstPlayerName} has { (winner == 1 ? 1 : 0)} wins, " +
                $"{dummyGameParams.SecondPlayerName} has {(winner == 2 ? 1 : 0)} wins, 0 ties"));
            Assert.True(loggerDouble.HasBeenLogged(LogLevel.Debug, $"Player {winner} has won!"));

            Assert.True(loggerDouble.HasBeenLogged(LogLevel.Information, "Simulation took 3 seconds..."));

            //make sure we only played war once
            gamePlayMock.Verify(mock => mock.War(It.IsAny<ITrackIndividualGameState>(), It.IsAny<int>()), Times.Once());

            //make sure there was no need to calculate points
            pointsCalculatorMock.Verify(mock => mock.GetPoints(It.IsAny<List<Card>>()), Times.Never());
        }
    }
}
