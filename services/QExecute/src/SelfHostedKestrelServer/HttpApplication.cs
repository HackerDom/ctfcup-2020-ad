using System;
using System.Net;
using System.Threading.Tasks;
using HttpServer.Abstractions;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Http.Features;
using Vostok.Logging.Abstractions;

namespace HttpServer
{
    internal class HttpApplication : IHttpApplication<ApplicationContext>
    {
        private readonly Func<HttpRequest, Task<HttpResponse>> _handler;
        private readonly ILog _log;

        public HttpApplication(Func<HttpRequest, Task<HttpResponse>> handler, ILog log)
        {
            _log = log;
            _handler = handler;
        }

        public ApplicationContext CreateContext(IFeatureCollection contextFeatures)
        {
            return new(contextFeatures, _log);
        }

        public async Task ProcessRequestAsync(ApplicationContext context)
        {
            try
            {
                var response = await _handler(context.Request);
                await context.RespondAsync(response);
            }
            catch (Exception e)
            {
                _log.Error(e);
                await context.RespondAsync(new HttpResponse(HttpStatusCode.InternalServerError, e.ToString()));
            }
        }

        public void DisposeContext(ApplicationContext context, Exception exception)
        {
            if (exception == null)
                return;

            _log.Error(exception);
        }
    }
}