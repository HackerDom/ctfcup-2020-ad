using QueenOfHearts.CoreLibrary.Models;
using QueenOfHearts.CoreLibrary.Storages;

namespace QueenOfHearts.ExecutionService
{
    internal class State : IState
    {
        public State(ISerializableIndex<string, Executor> executorIndex,
            ISerializableIndex<string, Command> commandIndex)
        {
            ExecutorIndex = executorIndex;
            CommandIndex = commandIndex;
        }

        public ISerializableIndex<string, Executor> ExecutorIndex { get; }
        public ISerializableIndex<string, Command> CommandIndex { get; }
    }
}