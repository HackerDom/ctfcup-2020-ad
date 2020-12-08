using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Mime;
using Microsoft.AspNetCore.Server.Kestrel.Core.Internal.Http;
using Microsoft.Extensions.Primitives;

namespace HttpServer.Abstractions
{
    public class HttpResponse
    {
        public static HttpResponse OK = new(HttpStatusCode.OK);

        public HttpResponse(HttpStatusCode code, string content) : this(code, new StringContent(content),
            content.Length)
        {
        }

        public HttpResponse(HttpStatusCode code, byte[] content) : this(code, new ByteArrayContent(content),
            content.Length)
        {
        }


        public HttpResponse(HttpStatusCode code, HttpContent content, int bodyLength)
        {
            Body = content;
            Headers = new HttpResponseHeaders();
            BodyLength = bodyLength;
            Code = code;
            foreach (var contentHeader in Body.Headers)
                Headers.TryAdd(contentHeader.Key, new StringValues(contentHeader.Value.ToArray()));
        }

        public HttpResponse(HttpStatusCode code, HttpContent body = null, int bodyLength = 0,
            HttpResponseHeaders headers = null, ContentType contentType = null)
        {
            Code = code;
            Body = body;
            Headers = headers;
            BodyLength = bodyLength;
        }

        public HttpStatusCode Code { get; }
        public HttpContent Body { get; }
        public HttpResponseHeaders Headers { get; }
        public int BodyLength { get; }

        public static HttpResponse From(HttpContent content)
        {
            return new(HttpStatusCode.OK, content);
        }

        public static HttpResponse From(byte[] content)
        {
            return new(HttpStatusCode.OK, content);
        }

        public static HttpResponse From(string content)
        {
            return new(HttpStatusCode.OK, content);
        }
    }
}