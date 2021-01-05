using System;

namespace NinetySixSimulator.ConsoleApp.Models
{
    public class Card
    {
        public string Rank { get; set; }
        public string Suit { get; set; }

        public override string ToString()
        {
            return $"{Rank}{Suit}";
        }

        public static bool operator <(Card first, Card second)
        {
            return first.rankToNumber() < second.rankToNumber();
        }

        public static bool operator >(Card first, Card second)
        {
            return first.rankToNumber() > second.rankToNumber();
        }

        public static bool operator ==(Card first, Card second)
        {
            return first.rankToNumber() == second.rankToNumber();
        }

        public static bool operator !=(Card first, Card second)
        {
            return first.rankToNumber() != second.rankToNumber();
        }


        private int rankToNumber()
        {
            switch (Rank)
            {
                case "A":
                    return 14;
                case "K":
                    return 13;
                case "Q":
                    return 12;
                case "J":
                    return 11;
                case "T":
                    return 10;
                default:
                    return Convert.ToInt32(Rank);
            }
        }
    }
}
