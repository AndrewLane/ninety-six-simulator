using Microsoft.Extensions.Logging;

namespace NinetySixSimulator.Services
{

    public class App : IApp
    {
        private readonly ILogger<App> _logger;

        public App(ILogger<App> logger)
        {
            _logger = logger;
        }

        public void Run()
        {
            _logger.LogInformation("Ready to play...");
        }
    }
}
