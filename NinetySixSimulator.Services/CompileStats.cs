using System.Collections.Generic;
using NinetySixSimulator.Services.Models;

namespace NinetySixSimulator.Services;

public class CompileStats : ICompileStats
{
    public void UpdateStats(ISimulationStats statsSoFar, ISingleGameStats singleGameStats)
    {
        switch (singleGameStats.Winner)
        {
            case 1:
                statsSoFar.Player1Wins++;
                break;
            case 2:
                statsSoFar.Player2Wins++;
                break;
            default:
                statsSoFar.Ties++;
                break;
        }
        statsSoFar.Player1WinsNinetySixToZero += (singleGameStats.Player1WinsNinetySixToZero ? 1 : 0);
        statsSoFar.Player2WinsNinetySixToZero += (singleGameStats.Player2WinsNinetySixToZero ? 1 : 0);
        statsSoFar.TotalWinsNinetySixToZero += (singleGameStats.Player1WinsNinetySixToZero ? 1 : 0);
        statsSoFar.TotalWinsNinetySixToZero += (singleGameStats.Player2WinsNinetySixToZero ? 1 : 0);
        UpdateWarDepthWins(singleGameStats.Player1WarWinsByDepth, statsSoFar.Player1WarWinsByDepth);
        UpdateWarDepthWins(singleGameStats.Player1WarWinsByDepth, statsSoFar.WarsByDepth);
        UpdateWarDepthWins(singleGameStats.Player2WarWinsByDepth, statsSoFar.Player2WarWinsByDepth);
        UpdateWarDepthWins(singleGameStats.Player2WarWinsByDepth, statsSoFar.WarsByDepth);

        if (singleGameStats.SimulationTime < statsSoFar.ShortestGame)
        {
            statsSoFar.ShortestGame = singleGameStats.SimulationTime;
        }
    }

    void UpdateWarDepthWins(List<int> source, List<int> target)
    {
        for (int i = 0; i < source.Count; i++)
        {
            if (source[i] > 0)
            {
                target[i] += source[i];
            }
        }
    }
}
