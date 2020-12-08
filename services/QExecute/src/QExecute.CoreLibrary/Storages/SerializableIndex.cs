using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using QueenOfHearts.CoreLibrary.Serialization;

namespace QueenOfHearts.CoreLibrary.Storages
{
    public class GenericIndex
    {
        public static SerializableIndex<TKey, TValue> Deserialize<TKey, TValue>(IBinaryDeserializer binaryDeserializer,
            Func<IBinaryDeserializer, TKey> keySelector,
            Func<IBinaryDeserializer, TValue> valueSelector)
        {
            var index = new ConcurrentDictionary<TKey, TValue>();
            var count = binaryDeserializer.ReadInt32();
            for (var i = 0; i < count; i++)
            {
                var key = keySelector(binaryDeserializer);
                var value = valueSelector(binaryDeserializer);
                index[key] = value;
            }

            return new SerializableIndex<TKey, TValue>(index);
        }
    }

    public class SerializableIndex<TKey, TValue> : GenericIndex, ISerializableIndex<TKey, TValue>
    {
        private readonly ConcurrentDictionary<TKey, TValue> _index;

        public SerializableIndex()
        {
            _index = new ConcurrentDictionary<TKey, TValue>();
        }

        public SerializableIndex(ConcurrentDictionary<TKey, TValue> index)
        {
            _index = index;
        }

        public IEnumerable<TKey> Keys => _index.Keys;

        public void Put(TKey key, TValue value)
        {
            _index[key] = value;
        }

        public bool TryGet(TKey key, out TValue value)
        {
            return _index.TryGetValue(key, out value);
        }

        public bool Contains(TKey key)
        {
            return _index.ContainsKey(key);
        }

        public void Serialize(IBinarySerializer binarySerializer, Action<TKey> keySerializer,
            Action<TValue> valueSerializer)
        {
            var indexCopy = _index.ToArray();
            binarySerializer.Write(indexCopy.Length);
            foreach (var value in indexCopy)
            {
                keySerializer(value.Key);
                valueSerializer(value.Value);
            }
        }
    }
}