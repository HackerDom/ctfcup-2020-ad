using System.Net.Http;
using System.Threading.Tasks;
using HttpServer.Abstractions;
using QueenOfHearts.CoreLibrary.Http;
using QueenOfHearts.CoreLibrary.Models;
using QueenOfHearts.CoreLibrary.Serialization;
using QueenOfHearts.ExecutionService.Handlers.Requests;
using QueenOfHearts.ExecutionService.OpLog;
using QueenOfHearts.ExecutionService.OpLog.Operations;
using Vostok.Logging.Abstractions;

namespace QueenOfHearts.ExecutionService.Handlers
{
    internal class AddCommandHandler : RequestHandlerBase
    {
        private readonly IOperationApplier _operationApplier;

        public AddCommandHandler(IStateManager stateManager, ILog log, IOperationApplier operationApplier) : base(log)
        {
            _operationApplier = operationApplier;
        }

        public override HttpMethod Method => HttpMethod.Put;
        public override string Path => "/command";

        protected override async Task<HttpResponse> HandleInternal(HttpRequest request)
        {
            var addCommand = await request.Body.FromJsonAsync<AddCommandRequest>();
            var addCommandOperation = new AddCommandOperation(addCommand.CommandName, addCommand.ExecutorApiKey);
            _operationApplier.EnqueueOperation(addCommandOperation);
            var result = await addCommandOperation.WaitForCompletion();
            return result.GetResponseFromResult();
        }
    }
}