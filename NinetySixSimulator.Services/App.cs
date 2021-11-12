using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NinetySixSimulator.Services.Models;

namespace NinetySixSimulator.Services;

public class App : IApp
{
    private readonly IConfiguration _config;
    private readonly ILogger<App> _logger;
    private readonly ICoordinateGameplay _gamePlayCoordinator;
    private readonly IFinalizeStats _statsGatherer;

    public App(IConfiguration config, ILogger<App> logger, ICoordinateGameplay gamePlayCoordinator, IFinalizeStats statsGatherer)
    {
        _config = config;
        _logger = logger;
        _gamePlayCoordinator = gamePlayCoordinator;
        _statsGatherer = statsGatherer;
    }

    public void Run()
    {
        _logger.LogInformation("Ready to play...");
        var gameParams = new GameParameters
        {
            FirstPlayerName = _config.GetValue<string>("Player1Name") ?? "Player 1",
            SecondPlayerName = _config.GetValue<string>("Player2Name") ?? "Player 2",
            TotalLengthOfSimulation = TimeSpan.FromMinutes(_config.GetValue<int?>("SimulationMaxMinutes") ?? (60 * 24))
        };
        var stats = _gamePlayCoordinator.Play(gameParams);
        Console.WriteLine(_statsGatherer.GetFinalStats(gameParams, stats));
    }
}
