using NinetySixSimulator.Services.Models;

namespace NinetySixSimulator.Services;

public interface ICompileStats
{
    void UpdateStats(ISimulationStats statsSoFar, ISingleGameStats singleGameStats);
}
