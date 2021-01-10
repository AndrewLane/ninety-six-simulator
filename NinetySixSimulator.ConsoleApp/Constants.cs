using System;

namespace NinetySixSimulator.ConsoleApp
{
    public static class Constants
    {
        public static class Suits
        {
            public const string Spades = "♠";
            public const string Hearts = "♥";
            public const string Diamonds = "♦";
            public const string Clubs = "♣";
        }

        public static class GamePlayParameters
        {
            public static readonly TimeSpan TimeToPlayCard = TimeSpan.FromSeconds(1);
            public static readonly TimeSpan TimeToShuffle = TimeSpan.FromSeconds(15);
            public static readonly TimeSpan TimeToCountScore = TimeSpan.FromSeconds(60);
            public static readonly TimeSpan TimeToPlayCardsFaceDownForAWar = TimeSpan.FromSeconds(3);
            public static readonly TimeSpan TimeToDeal = TimeSpan.FromSeconds(26);
        }

        public const int NumberOfFaceDownCardsForWar = 3;
    }
}
