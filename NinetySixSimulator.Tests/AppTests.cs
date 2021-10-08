using System;
using Microsoft.Extensions.Configuration;
using Moq;
using NinetySixSimulator.Services;
using NinetySixSimulator.Services.Models;
using NinetySixSimulator.Tests.Utils;
using Xunit;

namespace NinetySixSimulator.Tests
{
    public class AppTests
    {
        [Fact]
        public void RunTestMissingConfig()
        {
            var loggerDouble = new LoggerDouble<App>();
            var mockConfig = new Mock<IConfiguration>();
            var mockGameplayCoordinator = new Mock<ICoordinateGameplay>();
            var statsGathererMock = new Mock<IFinalizeStats>();
            var dummySimulationStats = new Mock<ISimulationStats>();

            var configurationSection = new Mock<IConfigurationSection>();

            mockConfig.Setup(a => a.GetSection(It.IsAny<string>())).Returns(configurationSection.Object);
            mockGameplayCoordinator.Setup(mock => mock.Play(It.IsAny<GameParameters>())).Returns(dummySimulationStats.Object);

            var objectUnderTest = new App(mockConfig.Object, loggerDouble, mockGameplayCoordinator.Object, statsGathererMock.Object);

            objectUnderTest.Run();

            mockGameplayCoordinator.Verify(mock => mock.Play(
                It.Is<GameParameters>(
                    item => item.FirstPlayerName == "Player 1"
                    && item.SecondPlayerName == "Player 2" &&
                    item.TotalLengthOfSimulation == TimeSpan.FromMinutes(60 * 24)
                   )), Times.Once());
            statsGathererMock.Verify(mock => mock.GetFinalStats(It.IsAny<GameParameters>(), dummySimulationStats.Object), Times.Once());
        }

        [Fact]
        public void RunTestWithConfig()
        {
            var loggerDouble = new LoggerDouble<App>();
            var mockConfig = new Mock<IConfiguration>();
            var mockGameplayCoordinator = new Mock<ICoordinateGameplay>();
            var statsGathererMock = new Mock<IFinalizeStats>();
            var dummySimulationStats = new Mock<ISimulationStats>();

            var configurationSection = new Mock<IConfigurationSection>();
            configurationSection.SetupSequence(a => a.Value).Returns("Isaac")
                .Returns("Daddy")
                .Returns("1");

            mockConfig.Setup(a => a.GetSection(It.IsAny<string>())).Returns(configurationSection.Object);
            mockGameplayCoordinator.Setup(mock => mock.Play(It.IsAny<GameParameters>())).Returns(dummySimulationStats.Object);

            var objectUnderTest = new App(mockConfig.Object, loggerDouble, mockGameplayCoordinator.Object, statsGathererMock.Object);

            objectUnderTest.Run();

            mockGameplayCoordinator.Verify(mock => mock.Play(
                It.Is<GameParameters>(
                    item => item.FirstPlayerName == "Isaac"
                    && item.SecondPlayerName == "Daddy" &&
                    item.TotalLengthOfSimulation == TimeSpan.FromMinutes(1)
                   )), Times.Once());
            statsGathererMock.Verify(mock => mock.GetFinalStats(It.IsAny<GameParameters>(), dummySimulationStats.Object), Times.Once());
        }
    }
}
