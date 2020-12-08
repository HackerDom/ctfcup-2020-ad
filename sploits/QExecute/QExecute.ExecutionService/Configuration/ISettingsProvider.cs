using HttpServer.Configuration;

namespace QueenOfHearts.ExecutionService.Configuration
{
    public interface ISettingsProvider
    {
        KestrelServerSettings KestrelServerSettings { get; }
        ApplicationSettings ApplicationSettings { get; }
    }
}