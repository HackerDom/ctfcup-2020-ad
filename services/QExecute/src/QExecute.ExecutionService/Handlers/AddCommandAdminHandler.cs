using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using HttpServer.Abstractions;
using QueenOfHearts.CoreLibrary.Models;
using QueenOfHearts.ExecutionService.Handlers.Requests;
using QueenOfHearts.ExecutionService.OpLog;
using QueenOfHearts.ExecutionService.OpLog.Operations;
using Vostok.Logging.Abstractions;

namespace QueenOfHearts.ExecutionService.Handlers
{
    internal class AddCommandAdminHandler : AuthorizedHandler<AddCommandAdminRequest>
    {
        private readonly IOperationApplier _operationApplier;
        private readonly IStateManager _stateManager;

        public AddCommandAdminHandler(ILog log, IStateManager stateManager, IOperationApplier operationApplier) : base(
            log, stateManager)
        {
            _stateManager = stateManager;
            _operationApplier = operationApplier;
        }

        public override HttpMethod Method => HttpMethod.Put;
        public override string Path => "/addAdmin";

        protected override async Task<HttpResponse> HandleAuthorizedInternal(AddCommandAdminRequest request,
            Executor executor)
        {
            if (!_stateManager.ObtainState().CommandIndex.TryGet(request.CommandName, out var command))
                return new HttpResponse(HttpStatusCode.NotFound, $"{request.CommandName} not found");

            if (!command.Admins.Contains(request.ExecutorApiKey))
                return new HttpResponse(HttpStatusCode.Forbidden, $"Not admin of {request.CommandName}");

            var addCommandAdminOperation = new AddCommandAdminOperation(request.CommandName, request.AdminApiKey);
            _operationApplier.EnqueueOperation(addCommandAdminOperation);
            var result = await addCommandAdminOperation.WaitForCompletion();
            return result.GetResponseFromResult();
        }
    }
}