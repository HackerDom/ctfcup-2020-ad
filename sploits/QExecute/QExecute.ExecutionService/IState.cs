using QueenOfHearts.CoreLibrary.Models;
using QueenOfHearts.CoreLibrary.Storages;

namespace QueenOfHearts.ExecutionService
{
    public interface IState
    {
        ISerializableIndex<string, Executor> ExecutorIndex { get; }
        ISerializableIndex<string, Command> CommandIndex { get; }
    }
}