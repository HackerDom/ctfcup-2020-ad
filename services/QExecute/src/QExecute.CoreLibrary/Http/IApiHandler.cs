using System.Net.Http;
using HttpServer.Abstractions;

namespace QueenOfHearts.CoreLibrary.Http
{
    public interface IApiHandler : IHttpHandler
    {
        HttpMethod Method { get; }
        string Path { get; }
    }
}