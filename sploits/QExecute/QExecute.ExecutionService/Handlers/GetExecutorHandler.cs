using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using HttpServer.Abstractions;
using QueenOfHearts.CoreLibrary.Models;
using QueenOfHearts.CoreLibrary.Serialization;
using QueenOfHearts.ExecutionService.Handlers.Requests;
using Vostok.Logging.Abstractions;

namespace QueenOfHearts.ExecutionService.Handlers
{
    internal class GetExecutorHandler : AuthorizedHandler<AuthorizedRequest>
    {
        public GetExecutorHandler(ILog log, IStateManager stateManager) : base(log, stateManager)
        {
        }

        public override HttpMethod Method => HttpMethod.Get;
        public override string Path => "/executor";

        protected override Task<HttpResponse> HandleAuthorizedInternal(AuthorizedRequest request, Executor executor)
        {
            return Task.FromResult(new HttpResponse(HttpStatusCode.OK, executor.ToJson()));
        }
    }
}