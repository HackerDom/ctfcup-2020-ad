using System;
using HttpServer.Helpers;
using Microsoft.Extensions.Logging;
using Vostok.Logging.Abstractions;
using LogLevel = Microsoft.Extensions.Logging.LogLevel;

namespace HttpServer.Logging
{
    internal class MicrosoftLoggerWrapper : ILogger
    {
        private readonly ILog _log;

        public MicrosoftLoggerWrapper(ILog log)
        {
            _log = log;
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception,
            Func<TState, Exception, string> formatter)
        {
            switch (logLevel)
            {
                case LogLevel.Debug:
                    // _log.Debug(exception, formatter(state, exception));
                    break;
                case LogLevel.Information:
                    _log.Info(exception, formatter(state, exception));
                    break;
                case LogLevel.Warning:
                    _log.Warn(exception, formatter(state, exception));
                    break;
                case LogLevel.Critical:
                    _log.Fatal(exception, formatter(state, exception));
                    break;
                case LogLevel.Error:
                    _log.Error(exception, formatter(state, exception));
                    break;
            }
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            switch (logLevel)
            {
                case LogLevel.Trace:
                case LogLevel.None:
                    return false;
                default:
                    return true;
            }
        }

        public IDisposable BeginScope<TState>(TState state)
        {
            return DefaultDisposable.Defult;
        }
    }
}