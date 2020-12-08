using System.Threading;
using HttpServer;
using HttpServer.Abstractions;
using QueenOfHearts.VictimsProvider.Configuration;
using Vostok.Logging.Abstractions;

namespace QueenOfHearts.VictimsProvider
{
    public class Application : IApplication
    {
        private readonly KestrelHttpServer _server;
        private readonly ISettingsProvider _settingsProvider;

        public Application(ISettingsProvider settingsProvider, IHttpHandler apiHandler, ILog log)
        {
            _settingsProvider = settingsProvider;
            _server = new KestrelHttpServer(apiHandler, log, settingsProvider.KestrelServerSettings);
        }

        public void Start()
        {
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