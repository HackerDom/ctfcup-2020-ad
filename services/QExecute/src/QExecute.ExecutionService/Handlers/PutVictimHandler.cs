using System.Net.Http;
using System.Threading.Tasks;
using HttpServer.Abstractions;
using QueenOfHearts.CoreLibrary.Models;
using QueenOfHearts.ExecutionService.Handlers.Requests;
using Vostok.Logging.Abstractions;

namespace QueenOfHearts.ExecutionService.Handlers
{
    internal class AddVictimRequest : AuthorizedRequest
    {
        public string VictimName { get; set; }
        public string InformerName { get; set; }
    }

    internal class AddVictimHandler : AuthorizedHandler<AddVictimRequest>
    {
        private readonly IVictimsStorageClient _victimsStorageClient;

        public AddVictimHandler(ILog log, IStateManager stateManager, IVictimsStorageClient victimsStorageClient) :
            base(log, stateManager)
        {
            _victimsStorageClient = victimsStorageClient;
        }

        public override HttpMethod Method => HttpMethod.Put;
        public override string Path => "/victim";

        protected override async Task<HttpResponse> HandleAuthorizedInternal(AddVictimRequest request,
            Executor executor)
        {
            var victim = new Victim
            {
                VictimName = request.VictimName,
                ExecutorId = request.ExecutorId,
                InformerName = request.InformerName
            };
            
            await _victimsStorageClient.PutVictim(victim);

            return HttpResponse.OK;
        }
    }
}