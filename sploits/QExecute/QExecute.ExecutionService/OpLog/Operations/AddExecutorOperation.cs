using System;
using QueenOfHearts.CoreLibrary.Models;
using QueenOfHearts.CoreLibrary.Serialization;

namespace QueenOfHearts.ExecutionService.OpLog.Operations
{
    internal class AddExecutorOperation : BaseOperation
    {
        public AddExecutorOperation(string executorId, string apiKey)
        {
            ExecutorId = executorId;
            ApiKey = apiKey;
        }

        public string ExecutorId { get; }
        public string ApiKey { get; }

        public override void Apply(Func<IState> stareProvider)
        {
            var serializableIndex = stareProvider().ExecutorIndex;
            serializableIndex.Put(ExecutorId, new Executor(ExecutorId, ApiKey));
        }

        public override void Serialize(IBinarySerializer serializer)
        {
            serializer.Write(ExecutorId);
            serializer.Write(ApiKey);
        }

        public static AddExecutorOperation Deserialize(IBinaryDeserializer binaryDeserializer)
        {
            var executorId = binaryDeserializer.ReadString();
            var apiKey = binaryDeserializer.ReadString();
            return new AddExecutorOperation(executorId, apiKey);
        }
    }
}