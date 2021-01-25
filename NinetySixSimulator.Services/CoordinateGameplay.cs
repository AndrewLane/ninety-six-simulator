using System;
using System.Linq;
using Microsoft.Extensions.Logging;
using NinetySixSimulator.Services.Models;

namespace NinetySixSimulator.Services
{
    internal class CoordinateGameplay : ICoordinateGameplay
    {
        private readonly ILogger<CoordinateGameplay> _logger;
        private readonly IPlayWar _gamePlay;
        private readonly IPointsCalculator _pointsCalculator;
        private readonly ITrackIndividualGameState _individualGameStateTracker;
        private readonly IDateTime _dateTime;

        public CoordinateGameplay(ILogger<CoordinateGameplay> logger, IPlayWar gamePlay, 
            IPointsCalculator pointsCalculator, ITrackIndividualGameState individualGameStateTracker,
            IDateTime dateTime)
        {
            _logger = logger;
            _gamePlay = gamePlay;
            _pointsCalculator = pointsCalculator;
            _individualGameStateTracker = individualGameStateTracker;
            _dateTime = dateTime;
        }

        public void Play(GameParameters gameParams)
        {
            int player1Wins = 0;
            int player2Wins = 0;
            int ties = 0;
            bool doneWithAllGames = false;

            var totalElapsedTime = new TimeSpan();
            var startofSimulation = _dateTime.Now;

            while (!doneWithAllGames)
            {
                var game = _individualGameStateTracker.InitializeGameState();

                _logger.LogDebug($"Game will last a max of {game.MaxDuration.TotalMinutes} minutes...");
                if (_logger.IsEnabled(LogLevel.Trace))
                {
                    foreach (var card in game.FirstPlayerState.PlayPile.Cards)
                    {
                        _logger.LogTrace($"{gameParams.FirstPlayerName} has {card}");
                    }
                    foreach (var card in game.SecondPlayerState.PlayPile.Cards)
                    {
                        _logger.LogTrace($"{gameParams.SecondPlayerName} has {card}");
                    }
                }

                bool doneWithIndividualGame = false;
                while (!doneWithIndividualGame)
                {
                    _gamePlay.War(game);
                    doneWithIndividualGame = _gamePlay.PlayerHasLost(game.FirstPlayerState) || _gamePlay.PlayerHasLost(game.SecondPlayerState);
                    if (!doneWithIndividualGame)
                    {
                        _gamePlay.Transition(game);
                    }
                    if (game.TimedOut)
                    {
                        doneWithIndividualGame = true;
                    }
                }
                totalElapsedTime += game.TimeElapsed;

                int winner;

                if (_gamePlay.PlayerHasLost(game.FirstPlayerState))
                {
                    winner = 2;
                }
                else if (_gamePlay.PlayerHasLost(game.SecondPlayerState))
                {
                    winner = 1;
                }
                else if (game.TimedOut)
                {
                    _logger.LogDebug($"Game has timed out after {game.TimeElapsed.TotalSeconds} seconds.");
                    var player1Cards = game.FirstPlayerState.PlayPile.Cards.Concat(game.FirstPlayerState.GatherPile.Cards)
                                                                           .Concat(game.FirstPlayerState.PlayedCards.Cards);
                    var player2Cards = game.SecondPlayerState.PlayPile.Cards.Concat(game.SecondPlayerState.GatherPile.Cards)
                                                                           .Concat(game.SecondPlayerState.PlayedCards.Cards);
                    if (player1Cards.Count() + player2Cards.Count() != 52)
                    {
                        throw new Exception("Unexpected total cards after game. " +
                                            $"{gameParams.FirstPlayerName} {player1Cards.Count()}, {gameParams.SecondPlayerName} {player2Cards.Count()}.");
                    }
                    var player1Points = _pointsCalculator.GetPoints(player1Cards.ToList());
                    var player2Points = _pointsCalculator.GetPoints(player2Cards.ToList());
                    if (player1Points + player2Points != 96)
                    {
                        throw new Exception("Unexpected total points after game. " +
                                            $"{gameParams.FirstPlayerName} {player1Points}, {gameParams.SecondPlayerName} {player2Points}.");
                    }
                    if (player1Points > player2Points)
                    {
                        winner = 1;
                    }
                    else if (player2Points > player1Points)
                    {
                        winner = 2;
                    }
                    else
                    {
                        winner = 0; // tie!!!
                    }
                }
                else
                {
                    throw new Exception("Unexpected game ending...no winner and no time out!");
                }
                _logger.LogDebug(winner > 0 ? $"Player {winner} has won this game!" : "This game is a tie.");
                switch (winner)
                {
                    case 2:
                        player2Wins++;
                        break;
                    case 1:
                        player1Wins++;
                        break;
                    default:
                        ties++;
                        break;
                }


                _logger.LogDebug($"{gameParams.FirstPlayerName} has {player1Wins} wins, " +
                                 $"{gameParams.SecondPlayerName} has {player2Wins} wins, {ties} ties");
                doneWithAllGames = totalElapsedTime >= gameParams.TotalLengthOfSimulation;
            }

            var simulationTime = _dateTime.Now - startofSimulation;
            _logger.LogInformation($"Simulation took {simulationTime.TotalSeconds} seconds...");
        }
    }
}
