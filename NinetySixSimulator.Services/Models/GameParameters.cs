using System;

namespace NinetySixSimulator.Services.Models
{
    public class GameParameters
    {
        public string FirstPlayerName { get; set; }
        public string SecondPlayerName { get; set; }
        public TimeSpan TotalLengthOfSimulation { get; set; }
    }
}
