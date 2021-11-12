using System;
using NinetySixSimulator.Services.Models;

namespace NinetySixSimulator.Services;

public class TrackIndividualGameState : ITrackIndividualGameState
{
    public TimeSpan TimeElapsed { get; private set; } = TimeSpan.FromSeconds(0);
    public PlayerGameState FirstPlayerState { get; set; }
    public PlayerGameState SecondPlayerState { get; set; }
    public TimeSpan MaxDuration { get; private set; }

    public bool TimedOut { get; private set; } = false;

    public ISingleGameStats Stats { get; set; } = new SingleGameStats();

    public TrackIndividualGameState()
    {
        Random rnd = new Random();
        int maxDurationMinutes = rnd.Next(Constants.GamePlayParameters.MinGameLengthInMinutes,
            Constants.GamePlayParameters.MaxGameLengthInMinutes + 1);
        MaxDuration = TimeSpan.FromMinutes(maxDurationMinutes);

        var deck = new StandardDeck();
        deck.Cards.Shuffle();

        Tick(Constants.GamePlayParameters.TimeToShuffle);

        var player1 = new PlayerGameState();
        var player2 = new PlayerGameState();

        DealCardsToPlayer(player1, deck.Cards, 0, 26);
        DealCardsToPlayer(player2, deck.Cards, 26, 26);
        Tick(Constants.GamePlayParameters.TimeToDeal);

        FirstPlayerState = player1;
        SecondPlayerState = player2;
    }

    public ITrackIndividualGameState InitializeGameState()
    {
        return new TrackIndividualGameState();
    }

    private static void DealCardsToPlayer(PlayerGameState player, CardPile cardPile, int startIndex, int numCardsToDeal)
    {
        int cardsDealt = 0;
        var cards = cardPile.Cards;
        for (int i = startIndex; i < cards.Count && cardsDealt < numCardsToDeal; i++)
        {
            player.PlayPile.Cards.Add(new Card { Rank = cards[i].Rank, Suit = cards[i].Suit });
            cardsDealt++;
        }
    }

    public void Tick(TimeSpan time)
    {
        TimeElapsed += time;
        if (TimeElapsed > MaxDuration)
        {
            TimedOut = true;
        }
    }
}
