using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using HttpServer.Abstractions;
using Vostok.Logging.Abstractions;

namespace QueenOfHearts.CoreLibrary.Http
{
    public class CompositeApiHandler : IHttpHandler
    {
        private readonly Dictionary<string, IHttpHandler> _handlersByPath;
        private readonly ILog _log;

        public CompositeApiHandler(IEnumerable<IApiHandler> pathHandlers, ILog log)
        {
            _log = log;
            _handlersByPath = pathHandlers.ToDictionary(handler => GetHandlerKey(handler.Method.Method, handler.Path),
                handler => (IHttpHandler) handler);
        }

        public async Task<HttpResponse> Handle(HttpRequest request)
        {
            var key = GetHandlerKey(request.Method.Method, request.Path);
            if (_handlersByPath.ContainsKey(key))
                return await _handlersByPath[key].Handle(request);

            var notFoundMessage = $"{request.Path} not found";
            _log.Error(notFoundMessage);
            return new HttpResponse(HttpStatusCode.NotFound, notFoundMessage);
        }

        private string GetHandlerKey(string method, string path)
        {
            return $"{method}_{path}";
        }
    }
}