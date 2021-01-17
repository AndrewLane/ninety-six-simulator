using System.Collections.Generic;
using System.Text;
using NinetySixSimulator.Services;
using NinetySixSimulator.Services.Models;
using Xunit;

namespace NinetySixSimulator.Tests
{
    public class CardPileTests
    {
        [Fact]
        public void ShuffleTestForFullDeck()
        {
            HashSet<string> allStates = new HashSet<string>();
            var objectUnderTest = new StandardDeck().Cards;
            var initialState = cardPileRepresentation(objectUnderTest);
            allStates.Add(initialState);
            int numberOfShuffles = 1000;
            for (int i = 0; i < numberOfShuffles; i++)
            {
                objectUnderTest.Shuffle();
                allStates.Add(cardPileRepresentation(objectUnderTest));
            }

            //make sure that we never ever see the deck in a state we've seen before after we shuffle
            Assert.True(allStates.Count == numberOfShuffles + 1);
        }

        [Fact]
        public void ShuffleTestForJustThreeCards()
        {
            HashSet<string> allStates = new HashSet<string>();
            var objectUnderTest = new CardPile
            {
                Cards = new List<Card> {
                    new Card {Rank = "2", Suit = Constants.Suits.Clubs },
                    new Card {Rank = "3", Suit = Constants.Suits.Spades },
                    new Card {Rank = "2", Suit = Constants.Suits.Hearts }
                }
            };
            var initialState = cardPileRepresentation(objectUnderTest);
            allStates.Add(initialState);
            int numberOfShuffles = 10000;
            for (int i = 0; i < numberOfShuffles; i++)
            {
                objectUnderTest.Shuffle();
                allStates.Add(cardPileRepresentation(objectUnderTest));
            }

            //with that many shuffles, we should see all 6 possible states that 3 cards can be in
            Assert.True(allStates.Count == 6);
        }

        [Fact]
        public void TestDraw()
        {
            var objectUnderTest = new CardPile
            {
                Cards = new List<Card> {
                    new Card {Rank = "2", Suit = Constants.Suits.Clubs },
                    new Card {Rank = "2", Suit = Constants.Suits.Spades }
                }
            };
            var result = objectUnderTest.Draw();
            Assert.True(objectUnderTest.Cards.Count == 1);
            Assert.True(result.Rank == "2" && result.Suit == Constants.Suits.Clubs);
        }

        private static string cardPileRepresentation(CardPile cards)
        {
            var str = new StringBuilder();
            foreach (var card in cards.Cards)
            {
                str.Append(card.ToString());
            }
            return str.ToString();
        }
    }
}
