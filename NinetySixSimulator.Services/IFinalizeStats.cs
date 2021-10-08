using NinetySixSimulator.Services.Models;

namespace NinetySixSimulator.Services
{
    public interface IFinalizeStats
    {
        string GetFinalStats(GameParameters gameParams, ISimulationStats stats);
    }
}
