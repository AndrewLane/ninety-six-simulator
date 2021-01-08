using System.Collections.Generic;

namespace NinetySixSimulator.ConsoleApp.Models
{
    public class StandardDeck
    {
        public List<Card> Cards { get; private set; } = new List<Card>();

        public StandardDeck()
        {
            var suits = new string[] { Constants.Suits.Clubs, Constants.Suits.Diamonds, Constants.Suits.Hearts, Constants.Suits.Spades };
            for (int i = 1; i <= 13; i++)
            {
                foreach (var suit in suits)
                {
                    Cards.Add(new Card { Rank = TranslateNumberToRank(i), Suit = suit });
                }
            }
        }

        private static string TranslateNumberToRank(int number)
        {
            if (number < 2)
            {
                return "A";
            }
            if (number < 10)
            {
                return number.ToString();
            }
            return number switch
            {
                10 => "T",
                11 => "J",
                12 => "Q",
                _ => "K",
            };
        }
    }
}
