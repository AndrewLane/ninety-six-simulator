using System;
using System.Collections.Generic;

namespace NinetySixSimulator.Services.Models
{

    public class SingleGameStats : ISingleGameStats
    {
        public List<int> Player1WarWinsByDepth { get; set; } = new List<int>() { 0, 0, 0, 0, 0, 0, 0, 0 };
        public List<int> Player2WarWinsByDepth { get; set; } = new List<int>() { 0, 0, 0, 0, 0, 0, 0, 0 };
        public TimeSpan SimulationTime { get; set; }
        public bool Player1WinsNinetySixToZero { get; set; }
        public bool Player2WinsNinetySixToZero { get; set; }
        public int Winner { get; set; }
    }
}
