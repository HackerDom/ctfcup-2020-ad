using System;

namespace QueenOfHearts.CoreLibrary.Serialization
{
    public interface IBinaryDeserializer : IDisposable
    {
        int ReadInt32();
        string ReadString();
        Guid ReadGuid();
    }
}