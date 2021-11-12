using System.Collections.Generic;
using NinetySixSimulator.Services;
using NinetySixSimulator.Services.Models;
using Xunit;

namespace NinetySixSimulator.Tests;

public class CardTests
{
    [Fact]
    public void CardEqualityTest()
    {
        var tenOfClubs = new Card { Rank = "T", Suit = Constants.Suits.Clubs };
        var tenOfHearts = new Card { Rank = "T", Suit = Constants.Suits.Hearts };
        Assert.True(tenOfClubs == tenOfHearts);
    }

    [Fact]
    public void CardInequalityTest()
    {
        var tenOfClubs = new Card { Rank = "T", Suit = Constants.Suits.Clubs };
        var nineOfHearts = new Card { Rank = "9", Suit = Constants.Suits.Hearts };
        Assert.True(tenOfClubs != nineOfHearts);
    }

    [Fact]
    public void TestComparisons()
    {
        var ascendingArray = new List<Card>
            {
                new Card { Rank = "2", Suit = Constants.Suits.Clubs },
                new Card { Rank = "3", Suit = Constants.Suits.Clubs },
                new Card { Rank = "4", Suit = Constants.Suits.Clubs },
                new Card { Rank = "5", Suit = Constants.Suits.Clubs },
                new Card { Rank = "6", Suit = Constants.Suits.Clubs },
                new Card { Rank = "7", Suit = Constants.Suits.Clubs },
                new Card { Rank = "8", Suit = Constants.Suits.Clubs },
                new Card { Rank = "9", Suit = Constants.Suits.Clubs },
                new Card { Rank = "T", Suit = Constants.Suits.Clubs },
                new Card { Rank = "J", Suit = Constants.Suits.Clubs },
                new Card { Rank = "Q", Suit = Constants.Suits.Clubs },
                new Card { Rank = "K", Suit = Constants.Suits.Clubs },
                new Card { Rank = "A", Suit = Constants.Suits.Clubs }
            };
        for (int i = 0; i < ascendingArray.Count - 1; i++)
        {
            Assert.True(ascendingArray[i] < ascendingArray[i + 1]);
            Assert.True(ascendingArray[i + 1] > ascendingArray[i]);
        }
    }

    [Fact]
    public void TestToString()
    {
        var aceOfSpades = new Card { Rank = "A", Suit = Constants.Suits.Spades };
        Assert.True(aceOfSpades.ToString() == "A♠");
    }
}
