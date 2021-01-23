using NinetySixSimulator.Services.Models;

namespace NinetySixSimulator.Services
{
    public interface IPlayWar
    {
        void War(ITrackIndividualGameState state, int warDepth = 0);
        void Transition(ITrackIndividualGameState state);
        bool PlayerHasLost(PlayerGameState playerState);
    }
}
