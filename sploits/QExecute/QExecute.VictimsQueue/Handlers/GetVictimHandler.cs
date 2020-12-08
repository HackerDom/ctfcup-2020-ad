using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using HttpServer.Abstractions;
using QueenOfHearts.CoreLibrary.Http;
using QueenOfHearts.CoreLibrary.Serialization;
using Vostok.Logging.Abstractions;

namespace QueenOfHearts.VictimsProvider.Handlers
{
    internal class GetVictimRequest
    {
        public string VictimName { get; set; }
    }

    internal class GetVictimHandler : RequestHandlerBase
    {
        private readonly IVictimIndex _victimIndex;

        public GetVictimHandler(ILog log, IVictimIndex victimIndex) : base(log)
        {
            _victimIndex = victimIndex;
        }

        public override HttpMethod Method => HttpMethod.Get;
        public override string Path => "/victim";

        protected override async Task<HttpResponse> HandleInternal(HttpRequest request)
        {
            var getVictim = await request.Body.FromJsonAsync<GetVictimRequest>();

            return !_victimIndex.TryGetVictim(getVictim.VictimName, out var victim)
                ? new HttpResponse(HttpStatusCode.NotFound, "Victim not found")
                : HttpResponse.From(victim.ToJson());
        }
    }
}