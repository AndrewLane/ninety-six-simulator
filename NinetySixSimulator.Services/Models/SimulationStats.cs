namespace NinetySixSimulator.Services.Models;

public class SimulationStats : ISimulationStats
{

    public int Player1Wins { get; set; } = 0;
    public List<int> Player1WarWinsByDepth { get; set; } = new List<int>() { 0, 0, 0, 0, 0, 0, 0, 0 };
    public List<int> Player2WarWinsByDepth { get; set; } = new List<int>() { 0, 0, 0, 0, 0, 0, 0, 0 };
    public List<int> WarsByDepth { get; set; } = new List<int>() { 0, 0, 0, 0, 0, 0, 0, 0 };
    public int Player2Wins { get; set; } = 0;
    public int Ties { get; set; } = 0;
    public TimeSpan ShortestGame { get; set; } = TimeSpan.MaxValue;
    public TimeSpan TotalSimulationTime { get; set; } = TimeSpan.FromSeconds(0);
    public int Player1WinsNinetySixToZero { get; set; }
    public int Player2WinsNinetySixToZero { get; set; }
    public int TotalWinsNinetySixToZero { get; set; }
}
