using System;
using System.Collections.Generic;
using NinetySixSimulator.Services;
using NinetySixSimulator.Services.Models;
using Xunit;

namespace NinetySixSimulator.Tests
{
    public class FinalizeStatsTests
    {
        [Fact]
        public void FinalizeStatsWithNoWars()
        {
            var objectUnderTest = new FinalizeStats();
            var dummyGameParams = new GameParameters { FirstPlayerName = "Alice", SecondPlayerName = "Bob", TotalLengthOfSimulation = TimeSpan.FromMinutes(10) };
            var dummyStats = new SimulationStats
            {
                TotalSimulationTime = TimeSpan.FromMilliseconds(500),
                ShortestGame = TimeSpan.FromSeconds(90),
                Player1Wins = 50,
                Player2Wins = 50,
                WarsByDepth = new List<int>(),
            };
            var result = objectUnderTest.GetFinalStats(dummyGameParams, dummyStats);
            Assert.Equal(@"
Finished simulation of 00:10:00 of game play between Alice and Bob...

Simulation Time: 00:00:00.5000000
Shortest Game: 00:01:30
Total Games Played: 100
Ties: 0 (0.00000 %)
Alice Wins: 50 (50.00000 %)
Bob Wins: 50 (50.00000 %)
No wars
", result);
        }

        [Fact]
        public void FinalizeStatsManyWars()
        {
            var objectUnderTest = new FinalizeStats();
            var dummyGameParams = new GameParameters { FirstPlayerName = "Alice", SecondPlayerName = "Bob", TotalLengthOfSimulation = TimeSpan.FromDays(365) };
            var dummyStats = new SimulationStats
            {
                TotalSimulationTime = TimeSpan.FromHours(1),
                ShortestGame = TimeSpan.FromSeconds(10),
                Ties = 1000,
                Player1Wins = 522425,
                Player2Wins = 521542,
                WarsByDepth = new List<int> { 0, 100, 200, 300, 400, 500, 600, 700 },
            };
            var result = objectUnderTest.GetFinalStats(dummyGameParams, dummyStats);
            Assert.Equal(@"
Finished simulation of 365.00:00:00 of game play between Alice and Bob...

Simulation Time: 01:00:00
Shortest Game: 00:00:10
Total Games Played: 1,044,967
Ties: 1,000 (0.09570 %)
Alice Wins: 522,425 (49.99440 %)
Bob Wins: 521,542 (49.90990 %)
100 single war(s)
200 double war(s)
300 triple war(s)
400 quadruple war(s)
500 quintuple war(s)
600 sextuple war(s)
700 septuple war(s)

", result);
        }
    }
}
