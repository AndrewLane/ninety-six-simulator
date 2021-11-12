using System;
using System.IO;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NinetySixSimulator.Services;
using Serilog;

namespace NinetySixSimulator.ConsoleApp;

class Program
{
    static void Main()
    {
        Console.WriteLine("Hello Ninety-Six Simulator!");

        var builder = new ConfigurationBuilder();
        BuildConfig(builder);

        Log.Logger = new LoggerConfiguration()
            .ReadFrom.Configuration(builder.Build())
            .CreateLogger();

        Log.Logger.Information("App starting...");

        var host = Host.CreateDefaultBuilder()
            .ConfigureServices((context, services) =>
            {
                services.AddTransient<IApp, App>();
                services.AddSingleton<IDateTime, SystemDateTime>();
                services.AddSingleton<ICoordinateGameplay, CoordinateGameplay>();
                services.AddSingleton<IPlayWar, PlayWar>();
                services.AddSingleton<IPointsCalculator, PointsCalculator>();
                services.AddSingleton<ITrackIndividualGameState, TrackIndividualGameState>();
                services.AddSingleton<ICompileStats, CompileStats>();
                services.AddSingleton<IFinalizeStats, FinalizeStats>();
            })
            .UseSerilog()
            .Build();

        var svc = ActivatorUtilities.CreateInstance<App>(host.Services);
        svc.Run();
    }

    static void BuildConfig(IConfigurationBuilder builder)
    {
        builder.SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production"}.json", optional: true)
            .AddEnvironmentVariables();
    }
}
