using System.Net;
using System.Threading.Tasks;
using HttpServer.Abstractions;
using QueenOfHearts.CoreLibrary.Http;
using QueenOfHearts.CoreLibrary.Models;
using QueenOfHearts.CoreLibrary.Serialization;
using QueenOfHearts.ExecutionService.Handlers.Requests;
using Vostok.Logging.Abstractions;

namespace QueenOfHearts.ExecutionService.Handlers
{
    internal abstract class AuthorizedHandler<TValue> : RequestHandlerBase where TValue : AuthorizedRequest
    {
        private readonly IStateManager _stateManager;

        protected AuthorizedHandler(ILog log, IStateManager stateManager) : base(log)
        {
            _stateManager = stateManager;
        }

        protected override async Task<HttpResponse> HandleInternal(HttpRequest request)
        {
            var authorizedRequest = await request.Body.FromJsonAsync<TValue>();
            if (!_stateManager.ObtainState().ExecutorIndex.TryGet(authorizedRequest.ExecutorId, out var executor))
                return new HttpResponse(HttpStatusCode.NotFound, $"Executor {authorizedRequest.ExecutorId} not found");

            if (!executor.ExecutorApiKey.Equals(authorizedRequest.ExecutorApiKey))
                return new HttpResponse(HttpStatusCode.Forbidden, "Invalid api key");

            return await HandleAuthorizedInternal(authorizedRequest, executor);
        }

        protected abstract Task<HttpResponse> HandleAuthorizedInternal(TValue request, Executor executor);
    }
}