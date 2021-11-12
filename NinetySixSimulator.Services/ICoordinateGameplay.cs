namespace NinetySixSimulator.Services;

public interface ICoordinateGameplay
{
    ISimulationStats Play(GameParameters gameParams);
}
