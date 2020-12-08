using System;
using QueenOfHearts.CoreLibrary.Models;
using QueenOfHearts.CoreLibrary.Serialization;

namespace QueenOfHearts.ExecutionService.OpLog.Operations
{
    internal class AddCommandOperation : BaseOperation
    {
        private readonly string _commandName;
        private readonly string _apiKey;

        public AddCommandOperation(string commandName, string apiKey)
        {
            _commandName = commandName;
            _apiKey = apiKey;
        }

        public override void Apply(Func<IState> stateProvider)
        {
            if (stateProvider().CommandIndex.Contains(_commandName))
                throw new Exception($"Can't create command. Command with name {_commandName} already exist");

            var command = new Command(_commandName);
            stateProvider().CommandIndex.Put(_commandName, command);
            command.Admins.Add(_apiKey);
        }

        public override void Serialize(IBinarySerializer serializer)
        {
            serializer.Write(_commandName);
            serializer.Write(_apiKey);
        }

        public static AddCommandOperation Deserialize(IBinaryDeserializer binaryDeserializer)
        {
            var commandName = binaryDeserializer.ReadString();
            var executorId = binaryDeserializer.ReadString();
            return new AddCommandOperation(commandName, executorId);
        }
    }
}