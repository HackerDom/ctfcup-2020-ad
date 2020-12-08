using System;
using System.Threading.Tasks;
using QueenOfHearts.CoreLibrary.Serialization;

namespace QueenOfHearts.ExecutionService.OpLog.Operations
{
    internal interface IOperation
    {
        void Apply(Func<IState> stareProvider);
        void Serialize(IBinarySerializer serializer);
        Task<OperationResult> WaitForCompletion();
        void Complete(OperationResult operationResult);
    }
}