using HttpServer.Configuration;

namespace QueenOfHearts.ExecutionService.Configuration
{
    internal class SettingsProvider : ISettingsProvider
    {
        public KestrelServerSettings KestrelServerSettings { get; } = new();
        public ApplicationSettings ApplicationSettings { get; } = new();
    }
}