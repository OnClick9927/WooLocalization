using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WooLocalization
{
    [Serializable]
    internal class SerializableDictionary<TKey, TValue> :
        ISerializationCallbackReceiver,
        IDictionary<TKey, TValue>
    {
        [SerializeField] private List<SerializableKeyValuePair> list = new List<SerializableKeyValuePair>();

        [Serializable]
        private struct SerializableKeyValuePair
        {
            public TKey Key;
            public TValue Value;

            public SerializableKeyValuePair(TKey key, TValue value)
            {
                Key = key;
                Value = value;
            }
        }

        private Dictionary<TKey, int> _keyPositions;
        private KeyCollection _keys;
        private ValueCollection _values;
        private int _version;

        private Dictionary<TKey, int> KeyPositions
        {
            get
            {
                if (_keyPositions == null)
                    RebuildIndex();
                return _keyPositions;
            }
        }

        internal int Version
        {
            get
            {
                if (_keyPositions == null)
                    RebuildIndex();
                return _version;
            }
        }

        public SerializableDictionary()
        {
            _keyPositions = new Dictionary<TKey, int>();
        }

        public void OnBeforeSerialize() { }

        public void OnAfterDeserialize()
        {
            RebuildIndex();
        }

        private void RebuildIndex()
        {
            _keyPositions = new Dictionary<TKey, int>(list.Count);
            for (var i = 0; i < list.Count; i++)
                _keyPositions[list[i].Key] = i;
            _version++;
        }

        public TValue this[TKey key]
        {
            get => list[KeyPositions[key]].Value;
            set
            {
                int index;
                var pair = new SerializableKeyValuePair(key, value);
                if (KeyPositions.TryGetValue(key, out index))
                {
                    list[index] = pair;
                }
                else
                {
                    KeyPositions.Add(key, list.Count);
                    list.Add(pair);
                }
                _version++;
            }
        }

        public KeyCollection Keys => _keys ?? (_keys = new KeyCollection(this));
        public ValueCollection Values => _values ?? (_values = new ValueCollection(this));
        ICollection<TKey> IDictionary<TKey, TValue>.Keys => Keys;
        ICollection<TValue> IDictionary<TKey, TValue>.Values => Values;

        public void Add(TKey key, TValue value)
        {
            KeyPositions.Add(key, list.Count);
            list.Add(new SerializableKeyValuePair(key, value));
            _version++;
        }

        public bool ContainsKey(TKey key) => KeyPositions.ContainsKey(key);

        public bool Remove(TKey key)
        {
            int index;
            if (!KeyPositions.TryGetValue(key, out index))
                return false;

            KeyPositions.Remove(key);
            list.RemoveAt(index);
            for (var i = index; i < list.Count; i++)
                KeyPositions[list[i].Key] = i;
            _version++;
            return true;
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            int index;
            if (KeyPositions.TryGetValue(key, out index))
            {
                value = list[index].Value;
                return true;
            }

            value = default;
            return false;
        }

        public int Count => list.Count;
        public bool IsReadOnly => false;

        public void Add(KeyValuePair<TKey, TValue> item) => Add(item.Key, item.Value);

        public void Clear()
        {
            if (list.Count == 0)
                return;
            list.Clear();
            KeyPositions.Clear();
            _version++;
        }

        public bool Contains(KeyValuePair<TKey, TValue> item)
        {
            TValue value;
            return TryGetValue(item.Key, out value) && EqualityComparer<TValue>.Default.Equals(value, item.Value);
        }

        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            if (array == null)
                throw new ArgumentNullException(nameof(array));
            if (arrayIndex < 0 || arrayIndex > array.Length)
                throw new ArgumentOutOfRangeException(nameof(arrayIndex));
            if (array.Length - arrayIndex < list.Count)
                throw new ArgumentException("The destination array is too small.", nameof(array));

            for (var i = 0; i < list.Count; i++)
            {
                var entry = list[i];
                array[arrayIndex + i] = new KeyValuePair<TKey, TValue>(entry.Key, entry.Value);
            }
        }

        public bool Remove(KeyValuePair<TKey, TValue> item)
        {
            return Contains(item) && Remove(item.Key);
        }

        internal void CopyKeysTo(List<TKey> destination)
        {
            destination.Clear();
            if (destination.Capacity < list.Count)
                destination.Capacity = list.Count;
            for (var i = 0; i < list.Count; i++)
                destination.Add(list[i].Key);
        }

        public Enumerator GetEnumerator() => new Enumerator(this);
        IEnumerator<KeyValuePair<TKey, TValue>> IEnumerable<KeyValuePair<TKey, TValue>>.GetEnumerator() => GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public struct Enumerator : IEnumerator<KeyValuePair<TKey, TValue>>
        {
            private readonly SerializableDictionary<TKey, TValue> owner;
            private readonly int version;
            private int index;

            internal Enumerator(SerializableDictionary<TKey, TValue> owner)
            {
                this.owner = owner;
                version = owner.Version;
                index = -1;
            }

            public bool MoveNext()
            {
                ValidateVersion();
                if (index < owner.list.Count)
                    index++;
                return index < owner.list.Count;
            }

            public KeyValuePair<TKey, TValue> Current
            {
                get
                {
                    if (index < 0 || index >= owner.list.Count)
                        throw new InvalidOperationException();
                    var item = owner.list[index];
                    return new KeyValuePair<TKey, TValue>(item.Key, item.Value);
                }
            }

            object IEnumerator.Current => Current;

            public void Reset()
            {
                ValidateVersion();
                index = -1;
            }

            public void Dispose() { }

            private void ValidateVersion()
            {
                if (version != owner._version)
                    throw new InvalidOperationException("Collection was modified during enumeration.");
            }
        }

        public sealed class KeyCollection : ICollection<TKey>
        {
            private readonly SerializableDictionary<TKey, TValue> owner;

            internal KeyCollection(SerializableDictionary<TKey, TValue> owner)
            {
                this.owner = owner;
            }

            public int Count => owner.Count;
            public bool IsReadOnly => true;
            public bool Contains(TKey item) => owner.ContainsKey(item);

            public void CopyTo(TKey[] array, int arrayIndex)
            {
                ValidateCopyTo(array, arrayIndex, owner.Count);
                for (var i = 0; i < owner.list.Count; i++)
                    array[arrayIndex + i] = owner.list[i].Key;
            }

            public KeyEnumerator GetEnumerator() => new KeyEnumerator(owner);
            IEnumerator<TKey> IEnumerable<TKey>.GetEnumerator() => GetEnumerator();
            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
            void ICollection<TKey>.Add(TKey item) => throw new NotSupportedException();
            void ICollection<TKey>.Clear() => throw new NotSupportedException();
            bool ICollection<TKey>.Remove(TKey item) => throw new NotSupportedException();
        }

        public struct KeyEnumerator : IEnumerator<TKey>
        {
            private Enumerator enumerator;

            internal KeyEnumerator(SerializableDictionary<TKey, TValue> owner)
            {
                enumerator = new Enumerator(owner);
            }

            public bool MoveNext() => enumerator.MoveNext();
            public TKey Current => enumerator.Current.Key;
            object IEnumerator.Current => Current;
            public void Reset() => enumerator.Reset();
            public void Dispose() { }
        }

        public sealed class ValueCollection : ICollection<TValue>
        {
            private readonly SerializableDictionary<TKey, TValue> owner;

            internal ValueCollection(SerializableDictionary<TKey, TValue> owner)
            {
                this.owner = owner;
            }

            public int Count => owner.Count;
            public bool IsReadOnly => true;

            public bool Contains(TValue item)
            {
                var comparer = EqualityComparer<TValue>.Default;
                for (var i = 0; i < owner.list.Count; i++)
                {
                    if (comparer.Equals(owner.list[i].Value, item))
                        return true;
                }
                return false;
            }

            public void CopyTo(TValue[] array, int arrayIndex)
            {
                ValidateCopyTo(array, arrayIndex, owner.Count);
                for (var i = 0; i < owner.list.Count; i++)
                    array[arrayIndex + i] = owner.list[i].Value;
            }

            public ValueEnumerator GetEnumerator() => new ValueEnumerator(owner);
            IEnumerator<TValue> IEnumerable<TValue>.GetEnumerator() => GetEnumerator();
            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
            void ICollection<TValue>.Add(TValue item) => throw new NotSupportedException();
            void ICollection<TValue>.Clear() => throw new NotSupportedException();
            bool ICollection<TValue>.Remove(TValue item) => throw new NotSupportedException();
        }

        public struct ValueEnumerator : IEnumerator<TValue>
        {
            private Enumerator enumerator;

            internal ValueEnumerator(SerializableDictionary<TKey, TValue> owner)
            {
                enumerator = new Enumerator(owner);
            }

            public bool MoveNext() => enumerator.MoveNext();
            public TValue Current => enumerator.Current.Value;
            object IEnumerator.Current => Current;
            public void Reset() => enumerator.Reset();
            public void Dispose() { }
        }

        private static void ValidateCopyTo<T>(T[] array, int arrayIndex, int count)
        {
            if (array == null)
                throw new ArgumentNullException(nameof(array));
            if (arrayIndex < 0 || arrayIndex > array.Length)
                throw new ArgumentOutOfRangeException(nameof(arrayIndex));
            if (array.Length - arrayIndex < count)
                throw new ArgumentException("The destination array is too small.", nameof(array));
        }
    }
}
