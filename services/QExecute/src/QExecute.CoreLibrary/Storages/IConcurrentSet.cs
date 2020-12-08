using System.Collections.Generic;

namespace QueenOfHearts.CoreLibrary.Storages
{
    public interface IConcurrentSet<TKey> : ICollection<TKey>
    {
        bool Add(TKey key);
        bool Contains(TKey key);
        bool Remove(TKey key);
    }
}