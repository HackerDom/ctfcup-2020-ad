using System;
using System.Collections.Generic;
using System.Threading;
using HttpServer.Abstractions;
using HttpServer.Configuration;
using HttpServer.Logging;
using Microsoft.AspNetCore.Hosting.Internal;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.AspNetCore.Server.Kestrel.Core.Internal.Http;
using Microsoft.AspNetCore.Server.Kestrel.Transport.Sockets;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Vostok.Commons.Time;
using Vostok.Logging.Abstractions;

namespace HttpServer
{
    public class KestrelHttpServer : IKestrelHttpServer
    {
        private readonly IHttpHandler _handler;
        private readonly ILog _log;
        private readonly KestrelServerSettings _serverSettings;

        private readonly object _syncObject;
        private CancellationTokenSource _cts;

        private KestrelServer _kestrelServer;

        public KestrelHttpServer(IHttpHandler handler, ILog log, KestrelServerSettings serverSettings)
        {
            _handler = handler;
            _serverSettings = serverSettings;
            _log = log.ForContext(GetType());

            _syncObject = new object();
        }

        public bool IsRunning { get; private set; }

        public void Start(int port, HttpScheme scheme = HttpScheme.Http)
        {
            Start($"{scheme.ToString().ToLower()}://+:{port}/");
        }

        public void Start(string prefix)
        {
            lock (_syncObject)
            {
                if (IsRunning)
                    return;

                _kestrelServer = CreateKestrelServer(prefix);
                _kestrelServer.Features.Get<IServerAddressesFeature>().Addresses.Add(prefix);
                _cts = new CancellationTokenSource();
                _kestrelServer.StartAsync(new HttpApplication(_handler.Handle, _log), _cts.Token).Wait();

                IsRunning = true;

                _log.Info($"Http server started on {prefix}");
            }
        }

        public void Stop()
        {
            lock (_syncObject)
            {
                if (!IsRunning)
                    return;

                _cts.Cancel();
                _kestrelServer.Dispose();
                _kestrelServer = null;

                IsRunning = false;
            }

            _log.Info("Http server stopped");
        }

        public void Dispose()
        {
            Stop();
        }

        public static KestrelHttpServer StartNew(string prefix, IHttpHandler handler,
            KestrelServerSettings serverSettings, ILog log)
        {
            var server = new KestrelHttpServer(handler, log, serverSettings);
            server.Start(prefix);
            return server;
        }

        public static KestrelHttpServer StartNew(int port, HttpScheme scheme, IHttpHandler handler,
            KestrelServerSettings serverSettings, ILog log)
        {
            var server = new KestrelHttpServer(handler, log, serverSettings);
            server.Start(port, scheme);
            return server;
        }

        private KestrelServer CreateKestrelServer(string prefix)
        {
            var options = new KestrelServerOptions
            {
                AddServerHeader = false,
                Limits =
                {
                    KeepAliveTimeout = 2.Minutes(),
                    MaxRequestHeaderCount = 200,
                    MaxRequestHeadersTotalSize = 128 * 1024,
                    MaxRequestLineSize = 256 * 1024
                }
            };

            SetupHttps(prefix, options);

            var optionsWrapper = new OptionsWrapper<KestrelServerOptions>(options);


            var loggerFactory = new LoggerFactory();

            loggerFactory.AddProvider(new MicrosoftLoggerWrapperProvider(_log));

            var lifeTime = new ApplicationLifetime(loggerFactory.CreateLogger<ApplicationLifetime>());

            var socketTransportFactory = CreatSocketTransportFactory(lifeTime, loggerFactory);
            return new KestrelServer(optionsWrapper, socketTransportFactory, loggerFactory);
        }

        private SocketTransportFactory CreatSocketTransportFactory(ApplicationLifetime lifeTime,
            LoggerFactory loggerFactory)
        {
            var configureOptions = new ConfigureOptions<SocketTransportOptions>(transportOptions =>
                transportOptions.IOQueueCount = _serverSettings.IOQueueCount);
            var optionsFactory = new OptionsFactory<SocketTransportOptions>(new[] {configureOptions},
                new List<IPostConfigureOptions<SocketTransportOptions>>());
            var optionsManager = new OptionsManager<SocketTransportOptions>(optionsFactory);
            var socketTransportFactory = new SocketTransportFactory(optionsManager, lifeTime, loggerFactory);
            return socketTransportFactory;
        }

        private void SetupHttps(string prefix, KestrelServerOptions options)
        {
            if (prefix.StartsWith("https"))
            {
                if (_serverSettings.SslCertificate == null)
                    throw new InvalidOperationException(
                        "Can't use https scheme without server-side SSL certificate specified in server settings.");

                options.ConfigureHttpsDefaults(adapterOptions =>
                    adapterOptions.ServerCertificate = _serverSettings.SslCertificate);
            }
        }
    }
}