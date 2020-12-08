using QueenOfHearts.CoreLibrary.Helpers;
using QueenOfHearts.CoreLibrary.Serialization;
using QueenOfHearts.CoreLibrary.Storages;

namespace QueenOfHearts.CoreLibrary.Models
{
    public class Command : ISerializable
    {
        public Command(string name)
        {
            Name = name;
            Admins = new ConcurrentSet<string>();
        }

        private Command(string name, ConcurrentSet<string> admins)
        {
            Name = name;
            Admins = admins;
        }

        public string Name { get; set; }
        public ConcurrentSet<string> Admins { get; set; }

        public void Serialize(IBinarySerializer serializer)
        {
            serializer.Write(Name);
            serializer.SerializeCollection(Admins, serializer.Write);
        }

        public static Command Deserialize(IBinaryDeserializer binaryDeserializer)
        {
            var name = binaryDeserializer.ReadString();
            var admins = binaryDeserializer.DeserializeCollection(() => new ConcurrentSet<string>(), binaryDeserializer.ReadString);
            return new Command(name, admins);
        }
    }
}