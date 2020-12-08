using HttpServer.Configuration;

namespace QueenOfHearts.VictimsProvider.Configuration
{
    public interface ISettingsProvider
    {
        KestrelServerSettings KestrelServerSettings { get; }
        ApplicationSettings ApplicationSettings { get; }
    }
}