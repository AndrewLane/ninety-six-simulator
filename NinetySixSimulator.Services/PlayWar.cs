﻿using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("NinetySixSimulator.Tests")]
[assembly: InternalsVisibleTo("ninety-six-simulator")]
[assembly: InternalsVisibleTo("DynamicProxyGenAssembly2, PublicKey=0024000004800000940000000602000000240000525341310004000001000100c547cac37abd99c8db225ef2f6c8a3602f3b3606cc9891605d02baa56104f4cfc0734aa39b93bf7852f7d9266654753cc297e7d2edfe0bac1cdcf9f717241550e0a7b191195b7667bb4f64bcb8e2121380fd1d9d46ad2d92d2d15605093924cceaf74c4861eff62abf69b9291ed0a340e113be11e6a7d3113e92484cf7045cc7")]
namespace NinetySixSimulator.Services;

internal class PlayWar : IPlayWar
{
    private readonly ILogger<PlayWar> _logger;

    public PlayWar(ILogger<PlayWar> logger)
    {
        _logger = logger;
    }
    public void War(ITrackIndividualGameState state, int warDepth = 0)
    {
        var firstPlayerCard = state.FirstPlayerState.PlayPile.Draw();
        var secondPlayerCard = state.SecondPlayerState.PlayPile.Draw();

        state.Tick(Constants.GamePlayParameters.TimeToPlayCard);

        _logger.LogDebug($"War: {firstPlayerCard} vs {secondPlayerCard}...");
        var warOnThisTurn = firstPlayerCard == secondPlayerCard;
        if (warOnThisTurn)
        {
            _logger.LogDebug($"War with depth {warDepth} on this turn.");
            (int firstPlayerFacedownCardsAlready, bool player1Shuffled) = ShuffleForWarIfNecessary(state.FirstPlayerState, 1);
            (int secondPlayerFacedownCardsAlready, bool player2Shuffled) = ShuffleForWarIfNecessary(state.SecondPlayerState, 2);
            if (player1Shuffled || player2Shuffled)
            {
                state.Tick(Constants.GamePlayParameters.TimeToShuffle);
            }

            if (PlayerHasEnoughCardsForWar(state.FirstPlayerState, firstPlayerFacedownCardsAlready) &&
                PlayerHasEnoughCardsForWar(state.SecondPlayerState, secondPlayerFacedownCardsAlready))
            {
                for (int i = 0; i < Constants.NumberOfFaceDownCardsForWar - firstPlayerFacedownCardsAlready; i++)
                {
                    state.FirstPlayerState.PlayedCards.Cards.Add(state.FirstPlayerState.PlayPile.Draw());
                }
                for (int i = 0; i < Constants.NumberOfFaceDownCardsForWar - secondPlayerFacedownCardsAlready; i++)
                {
                    state.SecondPlayerState.PlayedCards.Cards.Add(state.SecondPlayerState.PlayPile.Draw());
                }

                state.FirstPlayerState.PlayedCards.Cards.Add(firstPlayerCard);
                state.SecondPlayerState.PlayedCards.Cards.Add(secondPlayerCard);
                state.Tick(Constants.GamePlayParameters.TimeToPlayCardsFaceDownForAWar);
                War(state, warDepth + 1);
            }
            else
            {
                if (PlayerHasEnoughCardsForWar(state.FirstPlayerState, firstPlayerFacedownCardsAlready) == false &&
                    PlayerHasEnoughCardsForWar(state.SecondPlayerState, secondPlayerFacedownCardsAlready) == false)
                {
                    throw new Exception("This code path is almost statistically impossible with a shuffled, full deck of cards!");
                }
                if (PlayerHasEnoughCardsForWar(state.FirstPlayerState, firstPlayerFacedownCardsAlready) == false)
                {
                    _logger.LogDebug("Player 1 does not have enough cards for war...");
                    state.FirstPlayerState.CannotContinueBecauseCantPlayEnoughCardsForWar = true;
                }
                else
                {
                    _logger.LogDebug("Player 2 does not have enough cards for war...");
                    state.SecondPlayerState.CannotContinueBecauseCantPlayEnoughCardsForWar = true;
                }

            }
        }
        else if (firstPlayerCard > secondPlayerCard)
        {
            HandlePlayerWinning(state, warDepth, winner: 1, firstPlayerCard, secondPlayerCard);
        }
        else
        {
            HandlePlayerWinning(state, warDepth, winner: 2, firstPlayerCard, secondPlayerCard);
        }
    }

    private void HandlePlayerWinning(ITrackIndividualGameState state, int warDepth, int winner,
        Card firstCard, Card secondCard)
    {
        _logger.LogDebug($"Player {winner} wins");
        var loser = (3 - winner);
        var winnerState = (winner == 1) ? state.FirstPlayerState : state.SecondPlayerState;
        var loserState = (winner == 2) ? state.FirstPlayerState : state.SecondPlayerState;
        state.Tick(Constants.GamePlayParameters.TimeToGatherCards);
        if (warDepth > 0)
        {
            if (winner == 1)
            {
                state.Stats.Player1WarWinsByDepth[warDepth]++;
            }
            else
            {
                state.Stats.Player2WarWinsByDepth[warDepth]++;
            }

            foreach (var card in winnerState.PlayedCards.Cards)
            {
                _logger.LogDebug($"Player {winner} saves: {card}");
                winnerState.GatherPile.Cards.Add(card);
            }
            foreach (var card in loserState.PlayedCards.Cards)
            {
                _logger.LogDebug($"Player {loser} loses: {card}");
                winnerState.GatherPile.Cards.Add(card);
            }
            winnerState.PlayedCards.Cards.Clear();
            loserState.PlayedCards.Cards.Clear();

        }
        winnerState.GatherPile.Cards.Add(firstCard);
        winnerState.GatherPile.Cards.Add(secondCard);
    }

    public void Transition(ITrackIndividualGameState state)
    {
        var player1Shuffled = ShuffleForPlayerIfNecessary(state.FirstPlayerState, 1);
        var player2Shuffled = ShuffleForPlayerIfNecessary(state.SecondPlayerState, 2);

        if (player1Shuffled || player2Shuffled)
        {
            state.Tick(Constants.GamePlayParameters.TimeToShuffle);
        }

        if (_logger.IsEnabled(LogLevel.Trace))
        {
            _logger.LogTrace($"After transition, player 1 has {state.FirstPlayerState.PlayPile.Cards.Count} card(s) in their play pile");
            _logger.LogTrace($"After transition, player 1 has {state.FirstPlayerState.GatherPile.Cards.Count} card(s) in their gather pile");
            _logger.LogTrace($"After transition, player 1 has {state.FirstPlayerState.PlayedCards.Cards.Count} card(s) in their played cards pile");

            _logger.LogTrace($"After transition, player 2 has {state.SecondPlayerState.PlayPile.Cards.Count} card(s) in their play pile");
            _logger.LogTrace($"After transition, player 2 has {state.SecondPlayerState.GatherPile.Cards.Count} card(s) in their gather pile");
            _logger.LogTrace($"After transition, player 2 has {state.SecondPlayerState.PlayedCards.Cards.Count} card(s) in their played cards pile");
        }
        _logger.LogDebug($"Total time taken so far is {state.TimeElapsed.TotalSeconds} seconds");

        if (state.TimedOut)
        {
            _logger.LogDebug($"Game over after {state.TimeElapsed.TotalSeconds} seconds because max mins is {state.MaxDuration.TotalMinutes}");
        }
    }

    private bool ShuffleForPlayerIfNecessary(PlayerGameState playerGameState, int whichPlayer)
    {
        if (playerGameState.PlayPile.Cards.Any()) return false; // nothing to do if they have more cards to play

        _logger.LogDebug($"Moving {playerGameState.GatherPile.Cards.Count} card(s) from gather pile to play pile for player {whichPlayer}...");
        foreach (var card in playerGameState.GatherPile.Cards)
        {
            playerGameState.PlayPile.Cards.Add(new Card { Rank = card.Rank, Suit = card.Suit });
        }
        playerGameState.GatherPile.Cards.Clear();
        playerGameState.PlayPile.Shuffle();
        return true;
    }

    public bool PlayerHasLost(PlayerGameState playerState)
    {
        if (playerState.CannotContinueBecauseCantPlayEnoughCardsForWar)
        {
            return true;
        }
        if (playerState.GatherPile.Cards.Any() == false && playerState.PlayPile.Cards.Any() == false)
        {
            return true;
        }
        return false;
    }

    private static bool PlayerHasEnoughCardsForWar(PlayerGameState playerGameState, int numberOfCardsPlayedFaceDownAlready = 0)
    {
        // does the player have enough cards to put face down and then 1 to play for the war
        return playerGameState.PlayPile.Cards.Count >= Constants.NumberOfFaceDownCardsForWar + 1 - numberOfCardsPlayedFaceDownAlready;
    }

    private (int, bool) ShuffleForWarIfNecessary(PlayerGameState playerGameState, int whichPlayer)
    {
        if (PlayerHasEnoughCardsForWar(playerGameState)) return (0, false); // check if we can do nothing
        if (playerGameState.PlayPile.Cards.Count + playerGameState.GatherPile.Cards.Count >= Constants.NumberOfFaceDownCardsForWar + 1)
        {
            //we can handle just shuffling, so first move everything in the gather pile to the play pile (which could be nothing)
            int cardsToPutFaceDownFromPlayPile = playerGameState.PlayPile.Cards.Count;
            for (int i = 0; i < cardsToPutFaceDownFromPlayPile; i++)
            {
                playerGameState.PlayedCards.Cards.Add(playerGameState.PlayPile.Draw());
            }

            //now move the gather pile to the play pile and shuffle everything
            ShuffleForPlayerIfNecessary(playerGameState, whichPlayer);
            return (cardsToPutFaceDownFromPlayPile, true);
        }
        else
        {
            return (0, false);
        }
    }
}
