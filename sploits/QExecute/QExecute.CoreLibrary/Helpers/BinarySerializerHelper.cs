using System;
using System.Collections.Generic;
using QueenOfHearts.CoreLibrary.Serialization;

namespace QueenOfHearts.CoreLibrary.Helpers
{
    public static class BinarySerializerHelper
    {
        public static TCollection DeserializeCollection<TValue, TCollection>(
            this IBinaryDeserializer binaryDeserializer, Func<TCollection> collectionInitializer, Func<TValue> selector)
            where TCollection : ICollection<TValue>
        {
            var count = binaryDeserializer.ReadInt32();
            var array = collectionInitializer();
            for (var i = 0; i < count; i++) array.Add(selector());
            return array;
        }

        public static void SerializeCollection<TValue>(this IBinarySerializer binarySerializer,
            ICollection<TValue> collection, Action<TValue> onItemSerialize)
        {
            binarySerializer.Write(collection.Count);
            foreach (var value in collection) onItemSerialize(value);
        }
    }
}