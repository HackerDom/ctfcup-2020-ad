using System.Threading.Tasks;

namespace HttpServer.Abstractions
{
    public interface IHttpHandler
    {
        Task<HttpResponse> Handle(HttpRequest request);
    }
}