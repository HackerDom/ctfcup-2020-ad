using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using HttpServer.Abstractions;
using QueenOfHearts.CoreLibrary.Http;
using QueenOfHearts.CoreLibrary.Serialization;
using Vostok.Logging.Abstractions;

namespace QueenOfHearts.ExecutionService.Handlers
{
    internal class GetCommandsListHandler : RequestHandlerBase
    {
        private readonly IStateManager _stateManager;

        public GetCommandsListHandler(ILog log, IStateManager stateManager) : base(log)
        {
            _stateManager = stateManager;
        }

        public override HttpMethod Method => HttpMethod.Get;
        public override string Path => "/commandList";

        protected override Task<HttpResponse> HandleInternal(HttpRequest request)
        {
            return Task.FromResult(new HttpResponse(HttpStatusCode.OK,
                _stateManager.ObtainState().CommandIndex.Keys.ToJson()));
        }
    }
}