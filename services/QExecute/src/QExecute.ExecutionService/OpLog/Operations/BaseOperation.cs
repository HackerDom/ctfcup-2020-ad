using System;
using System.Threading.Tasks;
using QueenOfHearts.CoreLibrary.Serialization;

namespace QueenOfHearts.ExecutionService.OpLog.Operations
{
    internal abstract class BaseOperation : IOperation
    {
        private readonly TaskCompletionSource<OperationResult> _taskCompletionSource;

        protected BaseOperation()
        {
            _taskCompletionSource = new TaskCompletionSource<OperationResult>();
        }

        public abstract void Apply(Func<IState> stateProvider);

        public abstract void Serialize(IBinarySerializer serializer);

        public Task<OperationResult> WaitForCompletion()
        {
            return _taskCompletionSource.Task;
        }

        public void Complete(OperationResult operationResult)
        {
            _taskCompletionSource.SetResult(operationResult);
        }
    }
}