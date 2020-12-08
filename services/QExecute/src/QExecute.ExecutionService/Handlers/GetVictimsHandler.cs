using System.Net.Http;
using System.Threading.Tasks;
using HttpServer.Abstractions;
using QueenOfHearts.CoreLibrary.Models;
using QueenOfHearts.CoreLibrary.Serialization;
using QueenOfHearts.ExecutionService.Handlers.Requests;
using Vostok.Logging.Abstractions;

namespace QueenOfHearts.ExecutionService.Handlers
{
    internal class GetVictimsHandler : AuthorizedHandler<AuthorizedRequest>
    {
        private readonly IVictimsStorageClient _victimsStorageClient;

        public GetVictimsHandler(ILog log, IStateManager stateManager, IVictimsStorageClient victimsStorageClient) :
            base(log, stateManager)
        {
            _victimsStorageClient = victimsStorageClient;
        }

        public override HttpMethod Method => HttpMethod.Get;
        public override string Path => "/victims";


        protected override async Task<HttpResponse> HandleAuthorizedInternal(AuthorizedRequest request,
            Executor executor)
        {
            var victims = await _victimsStorageClient.GetVictims();
            return HttpResponse.From(victims.ToJson());
        }
    }
}