using System;
using System.Net.Http;
using System.Threading.Tasks;
using HttpServer.Abstractions;
using QueenOfHearts.CoreLibrary.Http;
using QueenOfHearts.CoreLibrary.Serialization;
using QueenOfHearts.ExecutionService.Handlers.Requests;
using QueenOfHearts.ExecutionService.OpLog;
using QueenOfHearts.ExecutionService.OpLog.Operations;
using Vostok.Logging.Abstractions;

namespace QueenOfHearts.ExecutionService.Handlers
{
    internal class AddExecutorHandler : RequestHandlerBase
    {
        private readonly IOperationApplier _operationApplier;

        public AddExecutorHandler(ILog log, IOperationApplier operationApplier) : base(log)
        {
            _operationApplier = operationApplier;
        }

        public override HttpMethod Method => HttpMethod.Put;
        public override string Path => "/executor";

        protected override async Task<HttpResponse> HandleInternal(HttpRequest request)
        {
            var addExecutor = await request.Body.FromJsonAsync<AuthorizedRequest>();
            var addExecutorOperation = new AddExecutorOperation(addExecutor.ExecutorId, addExecutor.ExecutorApiKey);
            _operationApplier.EnqueueOperation(addExecutorOperation);
            var result = await addExecutorOperation.WaitForCompletion();
            return HttpResponse.OK;
        }
    }
}