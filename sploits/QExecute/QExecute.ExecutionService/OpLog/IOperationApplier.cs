using QueenOfHearts.ExecutionService.OpLog.Operations;

namespace QueenOfHearts.ExecutionService.OpLog
{
    internal interface IOperationApplier
    {
        void EnqueueOperation(IOperation operation);
    }
}