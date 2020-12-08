using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace QueenOfHearts.CoreLibrary.Storages
{
    public class ConcurrentSet<TKey> : IConcurrentSet<TKey>
    {
        private readonly ConcurrentDictionary<TKey, byte> _set;

        public ConcurrentSet()
        {
            _set = new ConcurrentDictionary<TKey, byte>();
        }

        public int Count => _set.Count;
        public bool IsReadOnly => false;

        public bool Add(TKey key)
        {
            return _set.TryAdd(key, 0);
        }

        public void Clear()
        {
            _set.Clear();
        }

        void ICollection<TKey>.Add(TKey item)
        {
            _set[item ?? throw new ArgumentNullException(nameof(item))] = 0;
        }

        public bool Contains(TKey key)
        {
            return _set.ContainsKey(key ?? throw new ArgumentNullException(nameof(key)));
        }

        public void CopyTo(TKey[] array, int arrayIndex)
        {
            _set.ToArray().CopyTo(array, arrayIndex);
        }

        public bool Remove(TKey key)
        {
            return _set.TryRemove(key, out _);
        }

        public IEnumerator<TKey> GetEnumerator()
        {
            return _set.Keys.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}