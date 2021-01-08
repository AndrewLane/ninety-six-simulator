using System.Collections.Generic;
using System.Linq;
using NinetySixSimulator.ConsoleApp;
using NinetySixSimulator.ConsoleApp.Models;
using Xunit;

namespace NinetySixSimulator.Tests
{
    public class StandardDeckTests
    {
        [Fact]
        public void TestStandardDeck()
        {
            var deck = new StandardDeck();
            Assert.True(deck.Cards.Count == 52);
            foreach (var suit in new List<string> { Constants.Suits.Clubs, Constants.Suits.Diamonds, Constants.Suits.Hearts, Constants.Suits.Spades })
            {
                Assert.True(deck.Cards.Where(item => item.Suit == suit).Count() == 13);
            }
            foreach (var rank in new List<string> { "2", "3", "4", "5", "6", "7", "8", "9", "T", "J", "Q", "K", "A" })
            {
                Assert.True(deck.Cards.Where(item => item.Rank == rank).Count() == 4);
            }
        }
    }
}
