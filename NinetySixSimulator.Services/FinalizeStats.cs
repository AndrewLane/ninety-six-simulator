namespace NinetySixSimulator.Services;

public class FinalizeStats : IFinalizeStats
{
    public string GetFinalStats(GameParameters gameParams, ISimulationStats stats)
    {
        var totalGames = stats.Player1Wins + stats.Player2Wins + stats.Ties;
        var finalResult = $@"
Finished simulation of {gameParams.TotalLengthOfSimulation:c} of game play between {gameParams.FirstPlayerName} and {gameParams.SecondPlayerName}...

Simulation Time: {stats.TotalSimulationTime:c}
Shortest Game: {stats.ShortestGame:c}
Total Games Played: {totalGames:n0}
Ties: {stats.Ties:n0} ({calculatePercentage(stats.Ties, totalGames)})
{gameParams.FirstPlayerName} Wins: {stats.Player1Wins:n0} ({calculatePercentage(stats.Player1Wins, totalGames)})
{gameParams.SecondPlayerName} Wins: {stats.Player2Wins:n0} ({calculatePercentage(stats.Player2Wins, totalGames)})
{getWarStats(stats)}
";
        return finalResult;
    }

    private string calculatePercentage(int numerator, int denominator)
    {
        var percent = ((double)numerator / (double)denominator) * 100;
        return $"{percent:n5} %";
    }

    private string getWarStats(ISimulationStats stats)
    {
        int maxWarDepth = 0;
        for (int i = stats.WarsByDepth.Count - 1; i >= 1; i--)
        {
            if (stats.WarsByDepth[i] > 0)
            {
                maxWarDepth = i;
                break;
            }
        }
        if (maxWarDepth == 0)
        {
            return "No wars";
        }
        var wars = new StringBuilder();
        for (int i = 1; i <= maxWarDepth; i++)
        {
            wars.AppendLine($"{stats.WarsByDepth[i]:n0} {translateDepth(i)} war(s)");
        }
        return wars.ToString();
    }

    private string translateDepth(int depth)
    {
        return depth switch
        {
            7 => "septuple",
            6 => "sextuple",
            5 => "quintuple",
            4 => "quadruple",
            3 => "triple",
            2 => "double",
            _ => "single",
        };
    }
}
