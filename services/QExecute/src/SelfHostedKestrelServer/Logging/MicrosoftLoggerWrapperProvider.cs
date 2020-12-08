using Microsoft.Extensions.Logging;
using Vostok.Logging.Abstractions;

namespace HttpServer.Logging
{
    public class MicrosoftLoggerWrapperProvider : ILoggerProvider
    {
        private readonly ILog _log;

        public MicrosoftLoggerWrapperProvider(ILog log)
        {
            _log = log;
        }

        public void Dispose()
        {
        }

        public ILogger CreateLogger(string categoryName)
        {
            return new MicrosoftLoggerWrapper(_log);
        }
    }
}