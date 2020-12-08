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
    internal class GetVictimsRequest : AuthorizedRequest
    {
        public string VictimName { get; set; }
    }

    internal class GetVictimHandler : AuthorizedHandler<GetVictimsRequest>
    {
        private readonly IVictimsStorageClient _victimsStorageClient;

        public GetVictimHandler(ILog log, IStateManager stateManager, IVictimsStorageClient victimsStorageClient) :
            base(log, stateManager)
        {
            _victimsStorageClient = victimsStorageClient;
        }

        public override HttpMethod Method => HttpMethod.Get;
        public override string Path => "/victim";


        protected override async Task<HttpResponse> HandleAuthorizedInternal(GetVictimsRequest request,
            Executor executor)
        {
            var victims = await _victimsStorageClient.GetVictim(request.VictimName);

            if (!victims.ExecutorId.Equals(request.ExecutorId))
                return new HttpResponse(HttpStatusCode.Forbidden, "Not your victim");

            return HttpResponse.From(victims.ToJson());
        }
    }
}