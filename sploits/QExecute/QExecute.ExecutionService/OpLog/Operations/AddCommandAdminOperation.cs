using System;
using QueenOfHearts.CoreLibrary.Serialization;

namespace QueenOfHearts.ExecutionService.OpLog.Operations
{
    internal class AddCommandAdminOperation : BaseOperation
    {
        public readonly string ApiKey;
        public readonly string CommandName;

        public AddCommandAdminOperation(string commandName, string apiKey)
        {
            CommandName = commandName;
            ApiKey = apiKey;
        }

        public override void Apply(Func<IState> stateProvider)
        {
            if (!stateProvider().CommandIndex.TryGet(CommandName, out var command))
                throw new Exception($"Can't add admin. Cant' find command {CommandName}");

            command.Admins.Add(ApiKey);
        }

        public override void Serialize(IBinarySerializer serializer)
        {
            serializer.Write(CommandName);
            serializer.Write(ApiKey);
        }

        public static AddCommandAdminOperation Deserialize(IBinaryDeserializer binaryDeserializer)
        {
            var commandName = binaryDeserializer.ReadString();
            var apiKey = binaryDeserializer.ReadString();
            return new AddCommandAdminOperation(commandName, apiKey);
        }
    }
}