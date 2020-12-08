using HttpServer.Configuration;

namespace QueenOfHearts.VictimsProvider.Configuration
{
    internal class SettingsProvider : ISettingsProvider
    {
        public KestrelServerSettings KestrelServerSettings { get; } = new();
        public ApplicationSettings ApplicationSettings { get; } = new();
    }
}