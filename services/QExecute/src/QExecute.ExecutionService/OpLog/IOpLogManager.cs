using System.Collections.Generic;
using System.Threading.Tasks;
using QueenOfHearts.ExecutionService.OpLog.Operations;

namespace QueenOfHearts.ExecutionService.OpLog
{
    internal interface IOpLogManager
    {
        long LastReadeOffset { get; }
        IEnumerable<IOperation> Read(long fromOffset = 0);
        Task Write<TOperation>(TOperation operation) where TOperation : IOperation;
        Task Lock();
        void Release();
        void Delete();
    }
}