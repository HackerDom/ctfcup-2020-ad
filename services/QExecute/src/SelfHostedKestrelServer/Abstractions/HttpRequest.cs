using System.IO;
using System.Net.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;

namespace HttpServer.Abstractions
{
    public class HttpRequest
    {
        private HttpRequest(QueryString query, HttpMethod method, string path, Stream body)
        {
            Query = query;
            Method = method;
            Path = path;
            Body = body;
        }

        public QueryString Query { get; }
        public HttpMethod Method { get; }
        public string Path { get; }
        public Stream Body { get; }

        public static HttpRequest ParseRequest(IHttpRequestFeature _requestFeature)
        {
            return new(QueryString.FromUriComponent(_requestFeature.QueryString),
                new HttpMethod(_requestFeature.Method),
                _requestFeature.Path,
                _requestFeature.Body);
        }
    }
}