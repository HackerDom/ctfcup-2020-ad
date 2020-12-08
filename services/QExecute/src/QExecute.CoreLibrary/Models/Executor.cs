using QueenOfHearts.CoreLibrary.Serialization;

namespace QueenOfHearts.CoreLibrary.Models
{
    public class Executor : ISerializable
    {
        public Executor(string executorId, string executorApiKey)
        {
            ExecutorId = executorId;
            ExecutorApiKey = executorApiKey;
        }

        public string ExecutorId { get; }
        public string ExecutorApiKey { get; }

        public void Serialize(IBinarySerializer serializer)
        {
            serializer.Write(ExecutorId);
            serializer.Write(ExecutorApiKey);
        }

        public static Executor Deserialize(IBinaryDeserializer binaryDeserializer)
        {
            var id = binaryDeserializer.ReadString();
            var apiKey = binaryDeserializer.ReadString();
            return new Executor(id, apiKey);
        }
    }
}