using System;
using System.Threading.Tasks;
using HttpServer.Abstractions;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Net.Http.Headers;
using Vostok.Logging.Abstractions;

namespace HttpServer
{
    internal class ApplicationContext
    {
        private readonly ILog _log;
        private readonly IHttpResponseFeature _responseFeature;

        public ApplicationContext(IFeatureCollection contextFeatures, ILog log)
        {
            _log = log;
            _responseFeature = contextFeatures.Get<IHttpResponseFeature>();
            Request = HttpRequest.ParseRequest(contextFeatures.Get<IHttpRequestFeature>());
        }

        public HttpRequest Request { get; }


        public async Task RespondAsync(HttpResponse response)
        {
            _responseFeature.StatusCode = (int) response.Code;
            _responseFeature.Headers = response.Headers;
            await SetContent(response);
        }

        private async Task SetContent(HttpResponse response)
        {
            try
            {
                if (response.Body == null)
                    return;

                await response.Body.CopyToAsync(_responseFeature.Body);
                _responseFeature.Headers[HeaderNames.ContentLength] = response.BodyLength.ToString();
            }
            catch (Exception error)
            {
                _log.Error(error);
            }
        }
    }
}