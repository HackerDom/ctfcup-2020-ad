using System.Net;
using HttpServer.Abstractions;
using QueenOfHearts.ExecutionService.OpLog.Operations;

namespace QueenOfHearts.ExecutionService.Handlers
{
    public static class OperationResultHelper
    {
        public static HttpResponse GetResponseFromResult(this OperationResult result, string content)
        {
            return result == OperationResult.Success
                ? HttpResponse.From(content)
                : new HttpResponse(HttpStatusCode.InternalServerError, "Can't apply operation");
        }

        public static HttpResponse GetResponseFromResult(this OperationResult result)
        {
            return result == OperationResult.Success
                ? HttpResponse.OK
                : new HttpResponse(HttpStatusCode.InternalServerError, "Can't apply operation");
        }
    }
}