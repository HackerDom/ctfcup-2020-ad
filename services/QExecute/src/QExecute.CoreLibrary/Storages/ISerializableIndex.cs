using System;
using System.Collections.Generic;
using QueenOfHearts.CoreLibrary.Serialization;

namespace QueenOfHearts.CoreLibrary.Storages
{
    public interface ISerializableIndex<TKey, TValue>
    {
        IEnumerable<TKey> Keys { get; }
        void Put(TKey key, TValue value);
        bool TryGet(TKey key, out TValue value);
        bool Contains(TKey key);

        void Serialize(IBinarySerializer binarySerializer, Action<TKey> keySerializer,
            Action<TValue> valueSerializer);
    }
}