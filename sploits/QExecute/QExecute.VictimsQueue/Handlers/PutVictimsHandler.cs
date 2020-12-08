using System.Net.Http;
using System.Threading.Tasks;
using HttpServer.Abstractions;
using QueenOfHearts.CoreLibrary.Http;
using QueenOfHearts.CoreLibrary.Models;
using QueenOfHearts.CoreLibrary.Serialization;
using Vostok.Logging.Abstractions;

namespace QueenOfHearts.VictimsProvider.Handlers
{
    internal class PutVictimsHandler : RequestHandlerBase
    {
        private readonly IVictimIndex _victimIndex;

        public PutVictimsHandler(ILog log, IVictimIndex victimIndex) : base(log)
        {
            _victimIndex = victimIndex;
        }

        public override HttpMethod Method => HttpMethod.Put;
        public override string Path => "/victim";

        protected override async Task<HttpResponse> HandleInternal(HttpRequest request)
        {
            var victim = await request.Body.FromJsonAsync<Victim>();
            _victimIndex.Set(victim);
            return HttpResponse.OK;
            ;
        }
    }
}