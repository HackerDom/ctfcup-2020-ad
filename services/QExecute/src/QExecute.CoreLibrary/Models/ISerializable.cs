using QueenOfHearts.CoreLibrary.Serialization;

namespace QueenOfHearts.CoreLibrary.Models
{
    internal interface ISerializable
    {
        void Serialize(IBinarySerializer serializer);
    }
}