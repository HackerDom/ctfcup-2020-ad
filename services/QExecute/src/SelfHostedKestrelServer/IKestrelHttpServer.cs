using System;
using Microsoft.AspNetCore.Server.Kestrel.Core.Internal.Http;

namespace HttpServer
{
    public interface IKestrelHttpServer : IDisposable
    {
        void Start(int port, HttpScheme scheme = HttpScheme.Http);
        void Start(string prefix);
        void Stop();
    }
}