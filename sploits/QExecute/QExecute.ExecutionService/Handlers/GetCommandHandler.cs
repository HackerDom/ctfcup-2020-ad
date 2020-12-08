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
    internal class GetCommandHandler : AuthorizedHandler<GetCommandRequest>
    {
        private readonly IStateManager _stateManager;

        public GetCommandHandler(ILog log, IStateManager stateManager) : base(log, stateManager)
        {
            _stateManager = stateManager;
        }

        public override HttpMethod Method => HttpMethod.Get;
        public override string Path => "/command";

        protected override async Task<HttpResponse> HandleAuthorizedInternal(GetCommandRequest request,
            Executor executor)
        {
            if (!_stateManager.ObtainState().CommandIndex.TryGet(request.CommandName, out var command))
                return new HttpResponse(HttpStatusCode.NotFound, $"{request.CommandName} not found");

            if (!command.Admins.Contains(request.ExecutorApiKey))
                return new HttpResponse(HttpStatusCode.Forbidden, $"Not admin of {request.CommandName}");

            return new HttpResponse(HttpStatusCode.OK, command.ToJson());
        }
    }
}