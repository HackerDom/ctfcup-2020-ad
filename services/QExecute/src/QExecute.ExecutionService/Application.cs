using System.Threading;
using HttpServer;
using HttpServer.Abstractions;
using QueenOfHearts.ExecutionService.Configuration;
using Vostok.Logging.Abstractions;

namespace QueenOfHearts.ExecutionService
{
    public class Application : IApplication
    {
        private readonly KestrelHttpServer _server;
        private readonly ISettingsProvider _settingsProvider;
        private readonly IStateManager _stateManagers;

        public Application(ISettingsProvider settingsProvider, IHttpHandler apiHandler, ILog log,
            IStateManager stateManagers)
        {
            _settingsProvider = settingsProvider;
            _stateManagers = stateManagers;
            _server = new KestrelHttpServer(apiHandler, log, settingsProvider.KestrelServerSettings);
        }

        public void Start()
        {
            _stateManagers.Initialize();
            _server.Start(_settingsProvider.ApplicationSettings.Port);
            Thread.Sleep(-1);
        }

        public void Stop()
        {
            _server.Stop();
        }

        public void Dispose()
        {
            _server?.Dispose();
        }
    }
}