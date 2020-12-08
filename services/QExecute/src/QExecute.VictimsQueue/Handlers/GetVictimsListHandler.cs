using System.Net.Http;
using System.Threading.Tasks;
using HttpServer.Abstractions;
using QueenOfHearts.CoreLibrary.Http;
using QueenOfHearts.CoreLibrary.Serialization;
using Vostok.Logging.Abstractions;

namespace QueenOfHearts.VictimsProvider.Handlers
{
    internal class GetVictimsListHandler : RequestHandlerBase
    {
        private readonly IVictimIndex _victimIndex;

        public GetVictimsListHandler(ILog log, IVictimIndex victimIndex) : base(log)
        {
            _victimIndex = victimIndex;
        }

        public override HttpMethod Method => HttpMethod.Get;
        public override string Path => "/victims";

        protected override async Task<HttpResponse> HandleInternal(HttpRequest request)
        {
            return HttpResponse.From(_victimIndex.Names.ToJson());
        }
    }
}