using System;
using System.Collections.Generic;

namespace NinetySixSimulator.Services.Models
{
    public interface ISimulationStats
    {
        int Player1Wins { get; set; }
        List<int> Player1WarWinsByDepth { get; set; }
        List<int> Player2WarWinsByDepth { get; set; }
        List<int> WarsByDepth { get; set; }
        int Player2Wins { get; set; }
        int Ties { get; set; }
        TimeSpan ShortestGame { get; set; }
        TimeSpan TotalSimulationTime { get; set; }
        int Player1WinsNinetySixToZero { get; set; }
        int Player2WinsNinetySixToZero { get; set; }
        int TotalWinsNinetySixToZero { get; set; }
    }
}
