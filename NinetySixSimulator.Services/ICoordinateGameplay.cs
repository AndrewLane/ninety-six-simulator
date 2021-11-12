using NinetySixSimulator.Services.Models;

namespace NinetySixSimulator.Services;

public interface ICoordinateGameplay
{
    ISimulationStats Play(GameParameters gameParams);
}
