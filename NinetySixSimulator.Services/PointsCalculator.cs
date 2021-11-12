using System.Collections.Generic;
using NinetySixSimulator.Services.Models;

namespace NinetySixSimulator.Services;

public class PointsCalculator : IPointsCalculator
{
    public int GetPoints(List<Card> cards)
    {
        int totalPoints = cards.Count;

        foreach (var card in cards)
        {
            switch (card.Rank)
            {
                case "A":
                    totalPoints += 5;
                    break;
                case "K":
                    totalPoints += 3;
                    break;
                case "Q":
                    totalPoints += 2;
                    break;
                case "J":
                    totalPoints += 1;
                    break;
            }
        }
        return totalPoints;
    }
}
