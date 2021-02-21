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
        private readonly ICompileStats _statsCompiler;

        public CoordinateGameplay(ILogger<CoordinateGameplay> logger, IPlayWar gamePlay,
            IPointsCalculator pointsCalculator, ITrackIndividualGameState individualGameStateTracker,
            IDateTime dateTime, ICompileStats statsCompiler)
        {
            _logger = logger;
            _gamePlay = gamePlay;
            _pointsCalculator = pointsCalculator;
            _individualGameStateTracker = individualGameStateTracker;
            _dateTime = dateTime;
            _statsCompiler = statsCompiler;
        }

        public ISimulationStats Play(GameParameters gameParams)
        {
            var overallStats = new SimulationStats();
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
                bool timeout = false;
                bool player1HasLost = false;
                bool player2HasLost = false;
                while (!doneWithIndividualGame)
                {
                    _gamePlay.War(game);
                    player1HasLost = _gamePlay.PlayerHasLost(game.FirstPlayerState);
                    player2HasLost = _gamePlay.PlayerHasLost(game.SecondPlayerState);
                    doneWithIndividualGame = player1HasLost || player2HasLost;
                    if (player1HasLost || player2HasLost)
                    {
                        game.Stats.Player1WinsNinetySixToZero = player2HasLost;
                        game.Stats.Player2WinsNinetySixToZero = player1HasLost;
                    }
                    else
                    {
                        _gamePlay.Transition(game);
                        if (game.TimedOut)
                        {
                            doneWithIndividualGame = true;
                            timeout = true;
                        }

                    }
                }
                totalElapsedTime += game.TimeElapsed;
                game.Stats.SimulationTime = game.TimeElapsed;

                int winner;

                if (player1HasLost)
                {
                    winner = 2;
                }
                else if (player2HasLost)
                {
                    winner = 1;
                }
                else if (timeout)
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
                game.Stats.Winner = winner;
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
                _statsCompiler.UpdateStats(overallStats, game.Stats);
                doneWithAllGames = totalElapsedTime >= gameParams.TotalLengthOfSimulation;
            }

            var simulationTime = _dateTime.Now - startofSimulation;
            _logger.LogInformation($"Simulation took {simulationTime.TotalSeconds} seconds...");
            overallStats.TotalSimulationTime = simulationTime;
            return overallStats;
        }
    }
}
