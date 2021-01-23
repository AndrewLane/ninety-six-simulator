using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using Moq;
using NinetySixSimulator.Services;
using NinetySixSimulator.Services.Models;
using NinetySixSimulator.Tests.Utils;
using Xunit;

namespace NinetySixSimulator.Tests
{
    public class PlayWarTests
    {
        [Fact]
        public void PlayerHasLostTestWithNoCardsAtAll()
        {
            var objectUnderTest = new PlayWar(new Mock<ILogger<PlayWar>>().Object);
            var result = objectUnderTest.PlayerHasLost(new PlayerGameState { });
            Assert.True(result == true);
        }

        [Fact]
        public void PlayerHasLostTestWhenCannotCompleteAWar()
        {
            var objectUnderTest = new PlayWar(new Mock<ILogger<PlayWar>>().Object);
            var result = objectUnderTest.PlayerHasLost(new PlayerGameState { CannotContinueBecauseCantPlayEnoughCardsForWar = true });
            Assert.True(result == true);
        }

        [Fact]
        public void PlayerHasLostTestWhenTheyStillHavePlayPileCards()
        {
            var objectUnderTest = new PlayWar(new Mock<ILogger<PlayWar>>().Object);
            var result = objectUnderTest.PlayerHasLost(new PlayerGameState { PlayPile = new CardPile { Cards = new List<Card> { new Card() } } });
            Assert.True(result == false);
        }

        [Fact]
        public void PlayerHasLostTestWhenTheyStillHaveGatherPileCards()
        {
            var objectUnderTest = new PlayWar(new Mock<ILogger<PlayWar>>().Object);
            var result = objectUnderTest.PlayerHasLost(new PlayerGameState { GatherPile = new CardPile { Cards = new List<Card> { new Card() } } });
            Assert.True(result == false);
        }

        [Fact]
        public void PlayerHasLostTestWhenStartingWithRealGameStates()
        {
            var objectUnderTest = new PlayWar(new Mock<ILogger<PlayWar>>().Object);
            var realGameState = new TrackIndividualGameState().InitializeGameState();

            Assert.True(objectUnderTest.PlayerHasLost(realGameState.FirstPlayerState) == false);
            Assert.True(objectUnderTest.PlayerHasLost(realGameState.SecondPlayerState) == false);
        }

        [Fact]
        public void TransitionTestNoTimePassesWithNoShuffling()
        {
            var objectUnderTest = new PlayWar(new Mock<ILogger<PlayWar>>().Object);

            var realGameState = new TrackIndividualGameState().InitializeGameState();
            var timeElapsedSoFar = realGameState.TimeElapsed;
            objectUnderTest.Transition(realGameState);

            Assert.True(realGameState.TimeElapsed == timeElapsedSoFar);
        }

        [Fact]
        public void TransitionTestShufflingTakesTimeAndCanTimeOutGame()
        {
            var loggerDouble = new LoggerDouble<PlayWar>();

            var firstPlayerState = new PlayerGameState
            {
                GatherPile = new CardPile { Cards = new List<Card> { new Card { Rank = "K", Suit = Constants.Suits.Hearts } } },
                PlayPile = new CardPile { Cards = new List<Card> { } },
                PlayedCards = new CardPile { Cards = new List<Card> { } }
            };
            var secondPlayerState = new PlayerGameState
            {
                GatherPile = new CardPile { Cards = new List<Card> { new Card { Rank = "3", Suit = Constants.Suits.Clubs } } },
                PlayPile = new CardPile { Cards = new List<Card> { } },
                PlayedCards = new CardPile { Cards = new List<Card> { } }
            };

            var gameStateMock = new Mock<ITrackIndividualGameState>();
            gameStateMock.Setup(mock => mock.FirstPlayerState).Returns(firstPlayerState);
            gameStateMock.Setup(mock => mock.SecondPlayerState).Returns(secondPlayerState);
            var maxDuration = TimeSpan.FromMinutes(5);
            gameStateMock.Setup(mock => mock.MaxDuration).Returns(maxDuration);
            var timeElapsedInGame = new TimeSpan(hours: 0, minutes: 4, seconds: 59);
            gameStateMock.Setup(mock => mock.TimeElapsed).Returns(() => timeElapsedInGame);
            gameStateMock.Setup(mock => mock.Tick(It.IsAny<TimeSpan>()))
                .Callback<TimeSpan>((timeTaken) => timeElapsedInGame += timeTaken);
            gameStateMock.Setup(mock => mock.TimedOut).Returns(() => timeElapsedInGame > maxDuration);

            var objectUnderTest = new PlayWar(loggerDouble);

            //ensure we haven't timed out yet and the simulation will make us time out
            Assert.True(gameStateMock.Object.TimedOut == false);

            objectUnderTest.Transition(gameStateMock.Object);
            gameStateMock.Verify(mock => mock.Tick(Constants.GamePlayParameters.TimeToShuffle), Times.Once());
            Assert.True(loggerDouble.TraceEntries.Count() == 6);
            Assert.True(loggerDouble.HasBeenLogged(LogLevel.Trace, "After transition, player 1 has 1 card(s) in their play pile"));
            Assert.True(loggerDouble.HasBeenLogged(LogLevel.Trace, "After transition, player 1 has 0 card(s) in their gather pile"));
            Assert.True(loggerDouble.HasBeenLogged(LogLevel.Trace, "After transition, player 1 has 0 card(s) in their played cards pile"));
            Assert.True(loggerDouble.HasBeenLogged(LogLevel.Trace, "After transition, player 2 has 1 card(s) in their play pile"));
            Assert.True(loggerDouble.HasBeenLogged(LogLevel.Trace, "After transition, player 2 has 0 card(s) in their gather pile"));
            Assert.True(loggerDouble.HasBeenLogged(LogLevel.Trace, "After transition, player 2 has 0 card(s) in their played cards pile"));
            Assert.True(loggerDouble.DebugEntries.Count() == 4);
            Assert.True(loggerDouble.HasBeenLogged(LogLevel.Debug, "Moving 1 card(s) from gather pile to play pile for player 1..."));
            Assert.True(loggerDouble.HasBeenLogged(LogLevel.Debug, "Moving 1 card(s) from gather pile to play pile for player 2..."));
            Assert.True(loggerDouble.HasBeenLogged(LogLevel.Debug, "Total time taken so far is 314 seconds"));
            Assert.True(loggerDouble.HasBeenLogged(LogLevel.Debug, "Game over after 314 seconds because max mins is 5"));
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        public void WarTestSimpleCase(int winner)
        {
            var loggerDouble = new LoggerDouble<PlayWar>();

            var winningCard = new Card { Rank = "K", Suit = Constants.Suits.Hearts };
            var losingCard = new Card { Rank = "3", Suit = Constants.Suits.Clubs };

            var firstPlayerState = new PlayerGameState
            {
                PlayPile = new CardPile { Cards = new List<Card> { winner == 1 ? winningCard : losingCard } },
                GatherPile = new CardPile { Cards = new List<Card> { } },
                PlayedCards = new CardPile { Cards = new List<Card> { } }
            };
            var secondPlayerState = new PlayerGameState
            {
                PlayPile = new CardPile { Cards = new List<Card> { winner == 2 ? winningCard : losingCard } },
                GatherPile = new CardPile { Cards = new List<Card> { } },
                PlayedCards = new CardPile { Cards = new List<Card> { } }
            };

            var gameStateMock = new Mock<ITrackIndividualGameState>();
            gameStateMock.Setup(mock => mock.FirstPlayerState).Returns(firstPlayerState);
            gameStateMock.Setup(mock => mock.SecondPlayerState).Returns(secondPlayerState);

            var objectUnderTest = new PlayWar(loggerDouble);

            objectUnderTest.War(gameStateMock.Object);

            gameStateMock.Verify(mock => mock.Tick(Constants.GamePlayParameters.TimeToPlayCard), Times.Once());
            gameStateMock.Verify(mock => mock.Tick(Constants.GamePlayParameters.TimeToGatherCards), Times.Once());

            Assert.True(loggerDouble.HasBeenLogged(LogLevel.Debug, $"Player {winner} wins"));
            if (winner == 1)
            {
                Assert.True(loggerDouble.HasBeenLogged(LogLevel.Debug, $"War: {winningCard} vs {losingCard}..."));
            }
            else
            {
                Assert.True(loggerDouble.HasBeenLogged(LogLevel.Debug, $"War: {losingCard} vs {winningCard}..."));
            }
            Assert.True(loggerDouble.DebugEntries.Count() == 2);
            Assert.True(firstPlayerState.GatherPile.Cards.Count == (winner == 1 ? 2 : 0));
            Assert.True(firstPlayerState.PlayedCards.Cards.Count == 0);
            Assert.True(firstPlayerState.PlayPile.Cards.Count == 0);
            Assert.True(secondPlayerState.GatherPile.Cards.Count == (winner == 2 ? 2 : 0));
            Assert.True(secondPlayerState.PlayedCards.Cards.Count == 0);
            Assert.True(secondPlayerState.PlayPile.Cards.Count == 0);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        public void WarTestWinnerAfterSingleDepthWar(int winner)
        {
            var loggerDouble = new LoggerDouble<PlayWar>();

            var winningCard = new Card { Rank = "K", Suit = Constants.Suits.Hearts };
            var losingCard = new Card { Rank = "3", Suit = Constants.Suits.Clubs };

            var firstPlayerState = new PlayerGameState
            {
                PlayPile = new CardPile { Cards = new List<Card> { winner == 1 ? winningCard : losingCard } },
                GatherPile = new CardPile { Cards = new List<Card> { } },
                PlayedCards = new CardPile
                {
                    Cards = new List<Card> {
                        new Card { Rank = "A", Suit = Constants.Suits.Clubs},
                        new Card { Rank = "A", Suit = Constants.Suits.Spades},
                        new Card { Rank = "A", Suit = Constants.Suits.Hearts},
                    }
                }
            };
            var secondPlayerState = new PlayerGameState
            {
                PlayPile = new CardPile { Cards = new List<Card> { winner == 2 ? winningCard : losingCard } },
                GatherPile = new CardPile { Cards = new List<Card> { } },
                PlayedCards = new CardPile
                {
                    Cards = new List<Card> {
                        new Card { Rank = "2", Suit = Constants.Suits.Clubs},
                        new Card { Rank = "2", Suit = Constants.Suits.Spades},
                        new Card { Rank = "2", Suit = Constants.Suits.Hearts},
                    }
                }
            };

            var gameStateMock = new Mock<ITrackIndividualGameState>();
            gameStateMock.Setup(mock => mock.FirstPlayerState).Returns(firstPlayerState);
            gameStateMock.Setup(mock => mock.SecondPlayerState).Returns(secondPlayerState);

            var objectUnderTest = new PlayWar(loggerDouble);

            objectUnderTest.War(gameStateMock.Object, warDepth: 1);

            gameStateMock.Verify(mock => mock.Tick(Constants.GamePlayParameters.TimeToPlayCard), Times.Once());
            gameStateMock.Verify(mock => mock.Tick(Constants.GamePlayParameters.TimeToGatherCards), Times.Once());

            Assert.True(loggerDouble.HasBeenLogged(LogLevel.Debug, $"Player {winner} wins"));
            if (winner == 1)
            {
                Assert.True(loggerDouble.HasBeenLogged(LogLevel.Debug, $"War: {winningCard} vs {losingCard}..."));
            }
            else
            {
                Assert.True(loggerDouble.HasBeenLogged(LogLevel.Debug, $"War: {losingCard} vs {winningCard}..."));
            }
            var player1verb = (winner == 1 ? "saves" : "loses");
            var player2verb = (winner == 2 ? "saves" : "loses");

            Assert.True(loggerDouble.HasBeenLogged(LogLevel.Debug, $"Player 1 {player1verb}: A♣"));
            Assert.True(loggerDouble.HasBeenLogged(LogLevel.Debug, $"Player 1 {player1verb}: A♥"));
            Assert.True(loggerDouble.HasBeenLogged(LogLevel.Debug, $"Player 1 {player1verb}: A♠"));
            Assert.True(loggerDouble.HasBeenLogged(LogLevel.Debug, $"Player 2 {player2verb}: 2♣"));
            Assert.True(loggerDouble.HasBeenLogged(LogLevel.Debug, $"Player 2 {player2verb}: 2♥"));
            Assert.True(loggerDouble.HasBeenLogged(LogLevel.Debug, $"Player 2 {player2verb}: 2♠"));
            Assert.True(loggerDouble.DebugEntries.Count() == 8);
            Assert.True(firstPlayerState.GatherPile.Cards.Count == (winner == 1 ? 8 : 0));
            Assert.True(firstPlayerState.PlayedCards.Cards.Count == 0);
            Assert.True(firstPlayerState.PlayPile.Cards.Count == 0);
            Assert.True(secondPlayerState.GatherPile.Cards.Count == (winner == 2 ? 8 : 0));
            Assert.True(secondPlayerState.PlayedCards.Cards.Count == 0);
            Assert.True(secondPlayerState.PlayPile.Cards.Count == 0);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        public void WarTestSimpleWar(int winner)
        {
            var loggerDouble = new LoggerDouble<PlayWar>();

            var winningCard = new Card { Rank = "K", Suit = Constants.Suits.Hearts };
            var losingCard = new Card { Rank = "3", Suit = Constants.Suits.Clubs };

            var firstPlayerState = new PlayerGameState
            {
                PlayPile = new CardPile { Cards = new List<Card> { 
                    new Card { Rank = "5", Suit = Constants.Suits.Diamonds },
                    new Card { Rank = "A", Suit = Constants.Suits.Clubs },
                    new Card { Rank = "7", Suit = Constants.Suits.Spades },
                    new Card { Rank = "4", Suit = Constants.Suits.Diamonds },
                    winner == 1 ? winningCard : losingCard
                } },
                GatherPile = new CardPile { Cards = new List<Card> { } },
                PlayedCards = new CardPile { Cards = new List<Card> { } },
            };
            var secondPlayerState = new PlayerGameState
            {
                PlayPile = new CardPile { Cards = new List<Card> {
                    new Card { Rank = "5", Suit = Constants.Suits.Hearts },
                    new Card { Rank = "2", Suit = Constants.Suits.Clubs },
                    new Card { Rank = "Q", Suit = Constants.Suits.Hearts },
                    new Card { Rank = "J", Suit = Constants.Suits.Diamonds },
                    winner == 2 ? winningCard : losingCard
                } },
                GatherPile = new CardPile { Cards = new List<Card> { } },
                PlayedCards = new CardPile { Cards = new List<Card> { } },
            };

            var gameStateMock = new Mock<ITrackIndividualGameState>();
            gameStateMock.Setup(mock => mock.FirstPlayerState).Returns(firstPlayerState);
            gameStateMock.Setup(mock => mock.SecondPlayerState).Returns(secondPlayerState);

            var objectUnderTest = new PlayWar(loggerDouble);

            objectUnderTest.War(gameStateMock.Object);

            gameStateMock.Verify(mock => mock.Tick(Constants.GamePlayParameters.TimeToPlayCard), Times.Exactly(2));
            gameStateMock.Verify(mock => mock.Tick(Constants.GamePlayParameters.TimeToPlayCardsFaceDownForAWar), Times.Once());
            gameStateMock.Verify(mock => mock.Tick(Constants.GamePlayParameters.TimeToShuffle), Times.Never());
            gameStateMock.Verify(mock => mock.Tick(Constants.GamePlayParameters.TimeToGatherCards), Times.Once());

            Assert.True(loggerDouble.HasBeenLogged(LogLevel.Debug, $"Player {winner} wins"));
            Assert.True(loggerDouble.HasBeenLogged(LogLevel.Debug, $"War: 5♦ vs 5♥..."));
            if (winner == 1)
            {
                Assert.True(loggerDouble.HasBeenLogged(LogLevel.Debug, $"War: {winningCard} vs {losingCard}..."));
            }
            else
            {
                Assert.True(loggerDouble.HasBeenLogged(LogLevel.Debug, $"War: {losingCard} vs {winningCard}..."));
            }
            var player1verb = (winner == 1 ? "saves" : "loses");
            var player2verb = (winner == 2 ? "saves" : "loses");

            Assert.True(loggerDouble.HasBeenLogged(LogLevel.Debug, "War with depth 0 on this turn."));
            Assert.True(loggerDouble.HasBeenLogged(LogLevel.Debug, $"Player 1 {player1verb}: 5♦"));
            Assert.True(loggerDouble.HasBeenLogged(LogLevel.Debug, $"Player 1 {player1verb}: A♣"));
            Assert.True(loggerDouble.HasBeenLogged(LogLevel.Debug, $"Player 1 {player1verb}: 7♠"));
            Assert.True(loggerDouble.HasBeenLogged(LogLevel.Debug, $"Player 1 {player1verb}: 4♦"));
            Assert.True(loggerDouble.HasBeenLogged(LogLevel.Debug, $"Player 2 {player2verb}: 5♥"));
            Assert.True(loggerDouble.HasBeenLogged(LogLevel.Debug, $"Player 2 {player2verb}: 2♣"));
            Assert.True(loggerDouble.HasBeenLogged(LogLevel.Debug, $"Player 2 {player2verb}: Q♥"));
            Assert.True(loggerDouble.HasBeenLogged(LogLevel.Debug, $"Player 2 {player2verb}: J♦"));
            Assert.True(loggerDouble.DebugEntries.Count() == 12);
            Assert.True(firstPlayerState.GatherPile.Cards.Count == (winner == 1 ? 10 : 0));
            Assert.True(firstPlayerState.PlayedCards.Cards.Count == 0);
            Assert.True(firstPlayerState.PlayPile.Cards.Count == 0);
            Assert.True(secondPlayerState.GatherPile.Cards.Count == (winner == 2 ? 10 : 0));
            Assert.True(secondPlayerState.PlayedCards.Cards.Count == 0);
            Assert.True(secondPlayerState.PlayPile.Cards.Count == 0);
        }
    }
}
