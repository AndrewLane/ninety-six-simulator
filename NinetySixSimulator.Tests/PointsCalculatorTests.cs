using NinetySixSimulator.ConsoleApp;
using NinetySixSimulator.ConsoleApp.Models;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace NinetySixSimulator.Tests
{
    public class PointsCalculatorTests
    {
        /// <summary>
        /// Calculating points when you have no cards should result in 0 points
        /// </summary>
        [Fact]
        public void NoCardsTest()
        {
            var objectUnderTest = new PointsCalculator();
            var result = objectUnderTest.GetPoints(Enumerable.Empty<Card>().ToList());
            Assert.True(result == 0);
        }

        /// <summary>
        /// A single card with no bonus points should result in a point total of 1.
        /// </summary>
        [Fact]
        public void SingleCardTest()
        {
            var objectUnderTest = new PointsCalculator();
            var result = objectUnderTest.GetPoints(new List<Card> { new Card { Rank = "2", Suit = Constants.Suits.Clubs } });
            Assert.True(result == 1);
        }

        /// <summary>
        /// The entire deck should be 96 points.
        /// </summary>
        [Fact]
        public void StandardDeckTest()
        {
            var objectUnderTest = new PointsCalculator();
            var entireDeck = new StandardDeck();
            var result = objectUnderTest.GetPoints(entireDeck.Cards);
            Assert.True(result == 96);
        }
    }
}
