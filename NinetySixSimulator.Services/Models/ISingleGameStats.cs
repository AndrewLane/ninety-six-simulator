using System;
using System.Collections.Generic;

namespace NinetySixSimulator.Services.Models
{
    public interface ISingleGameStats
    {
        List<int> Player1WarWinsByDepth { get; set; }
        List<int> Player2WarWinsByDepth { get; set; }
        TimeSpan SimulationTime { get; set; }
        bool Player1WinsNinetySixToZero { get; set; }
        bool Player2WinsNinetySixToZero { get; set; }
        int Winner { get; set; }
    }
}
