using System;
using System.Net.Http;
using System.Threading.Tasks;
using HttpServer.Abstractions;
using Vostok.Logging.Abstractions;

namespace QueenOfHearts.CoreLibrary.Http
{
    public abstract class RequestHandlerBase : IApiHandler
    {
        private readonly ILog _log;

        protected RequestHandlerBase(ILog log)
        {
            _log = log;
        }

        public async Task<HttpResponse> Handle(HttpRequest request)
        {
            var start = DateTime.UtcNow;
            _log.Info($"Start processing {request.Method} {request.Path}");
            var result = await HandleInternal(request);
            _log.Info($"Finished {request.Method} {request.Path} with {result.Code}, at {DateTime.UtcNow - start}");
            return result;
        }

        public abstract HttpMethod Method { get; }
        public abstract string Path { get; }

        protected abstract Task<HttpResponse> HandleInternal(HttpRequest request);
    }
}