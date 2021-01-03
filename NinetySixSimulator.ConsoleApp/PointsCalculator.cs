using NinetySixSimulator.ConsoleApp.Models;
using System.Collections.Generic;
using System.Linq;

namespace NinetySixSimulator.ConsoleApp
{
    public class PointsCalculator
    {
        public int GetPoints(List<Card> cards)
        {
            int totalPoints = cards.Count();

            foreach (var card in cards)
            {
                switch (card.Rank)
                {
                    case "A":
                        totalPoints += 1;
                        break;
                    case "K":
                        totalPoints += 2;
                        break;
                    case "Q":
                        totalPoints += 3;
                        break;
                    case "J":
                        totalPoints += 5;
                        break;

                }
            }
            return totalPoints;
        }
    }
}
