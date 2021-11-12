namespace NinetySixSimulator.Tests;

public class CompileStatsTests
{
    [Fact]
    public void UpdateStatsTest()
    {
        var objectUnderTest = new CompileStats();
        var statsSoFar = new SimulationStats();
        var longPlayer1WinStats = new SingleGameStats
        {
            SimulationTime = TimeSpan.FromHours(1),
            Winner = 1,
            Player1WinsNinetySixToZero = true,
            Player2WinsNinetySixToZero = false,
            Player1WarWinsByDepth = new List<int>() { 0, 10, 9, 8, 7, 6, 5, 4 },
            Player2WarWinsByDepth = new List<int>() { 0, 10, 10, 10, 10, 10, 10, 10 },
        };
        var quickPlayer2WinsStats = new SingleGameStats
        {
            SimulationTime = TimeSpan.FromMinutes(1),
            Winner = 2,
            Player1WinsNinetySixToZero = false,
            Player2WinsNinetySixToZero = false,
            Player1WarWinsByDepth = new List<int>() { 0, 0, 0, 0, 0, 0, 0, 0 },
            Player2WarWinsByDepth = new List<int>() { 0, 0, 0, 0, 0, 0, 0, 0 },
        };
        var tie = new SingleGameStats
        {
            SimulationTime = TimeSpan.FromMinutes(5),
            Winner = 0,
            Player1WinsNinetySixToZero = false,
            Player2WinsNinetySixToZero = false,
            Player1WarWinsByDepth = new List<int>() { 0, 3, 2, 1, 0, 0, 0, 0 },
            Player2WarWinsByDepth = new List<int>() { 0, 3, 2, 1, 0, 0, 0, 0 },
        };
        objectUnderTest.UpdateStats(statsSoFar, longPlayer1WinStats);
        objectUnderTest.UpdateStats(statsSoFar, quickPlayer2WinsStats);
        objectUnderTest.UpdateStats(statsSoFar, tie);

        Assert.True(statsSoFar.Player1Wins == 1);
        Assert.True(statsSoFar.Player2Wins == 1);
        Assert.True(statsSoFar.Ties == 1);
        Assert.True(statsSoFar.Player1WinsNinetySixToZero == 1);
        Assert.True(statsSoFar.Player2WinsNinetySixToZero == 0);
        Assert.True(statsSoFar.ShortestGame == TimeSpan.FromMinutes(1));
        Assert.True(statsSoFar.TotalWinsNinetySixToZero == 1);
        Assert.Equal(statsSoFar.WarsByDepth, new List<int>() { 0, 26, 23, 20, 17, 16, 15, 14 });
        Assert.Equal(statsSoFar.Player1WarWinsByDepth, new List<int>() { 0, 13, 11, 9, 7, 6, 5, 4 });
        Assert.Equal(statsSoFar.Player2WarWinsByDepth, new List<int>() { 0, 13, 12, 11, 10, 10, 10, 10 });
    }
}
