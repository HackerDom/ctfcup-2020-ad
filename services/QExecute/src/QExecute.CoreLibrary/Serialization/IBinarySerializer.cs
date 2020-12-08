using System;
using System.Threading.Tasks;

namespace QueenOfHearts.CoreLibrary.Serialization
{
    public interface IBinarySerializer : IDisposable
    {
        void Write(int i);
        void Write(Guid g);
        void Write(string str);
        Task FlushAsync();
    }
}