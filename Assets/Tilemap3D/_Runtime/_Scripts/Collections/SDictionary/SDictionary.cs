using UnityEngine;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Tilemap3D.Collections
{
    [Serializable]
    public class SDictionary<K, V> : IDictionary<K, V>, ISerializationCallbackReceiver
    {
        [SerializeField, HideInInspector] private string keyTypeName;
        [SerializeField, HideInInspector] private string valueTypeName;
        [SerializeField, HideInInspector] private List<int> keyCollisions = new List<int>();
        [SerializeField] private List<Entry> entries = new List<Entry>();
        private Dictionary<K, V> dictionary = new Dictionary<K, V>();

        public static bool KeyTypeIsSerializable => typeof(K).IsSerializable;
        public static bool ValueTypeIsSerializable => typeof(V).IsSerializable;

        /// <summary>
        /// Serializable KeyValue class used as items in the dictionary. This is needed
        /// since the KeyValuePair in System.Collections.Generic isn't serializable.
        /// </summary>
        [Serializable]
        protected class Entry
        {
            public K key;
            public V value;
            public Entry(K key, V value)
            {
                this.key = key;
                this.value = value;
            }

            public override string ToString()
            {
                return "{" + key.ToString() + ", " + value.ToString() + "}";
            }
        }

        public SDictionary()
        {
            InitializeKeyValueTypes();
        }

        private void InitializeKeyValueTypes()
        {
            Type kType = typeof(K);
            Type vType = typeof(V);
            if (keyTypeName == null || (!keyTypeName.Equals(kType) || !valueTypeName.Equals(vType)))
            {
                keyTypeName = kType.IsPrimitive || kType.Equals(typeof(string)) ? kType.Name.ToLower() : kType.Name;
                valueTypeName = vType.IsPrimitive || vType.Equals(typeof(string)) ? vType.Name.ToLower() : vType.Name;
            }
        }

        public V this[K key]
        {
            get
            {
                return dictionary[key];
            }
            set
            {
                dictionary[key] = value;
            }
        }

        public ICollection<K> Keys
        {
            get => dictionary.Keys;
        }

        public ICollection<V> Values
        {
            get => dictionary.Values;
        }

        public int Count
        {
            get => dictionary.Count;
        }

        public bool IsReadOnly { get; set; }

        public IEqualityComparer<K> Comparer => dictionary.Comparer;

        public IEnumerator<KeyValuePair<K, V>> GetEnumerator()
        {
            return dictionary.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return dictionary.GetEnumerator();
        }

        /// <summary>
        /// Called after Unity tries to deserialize this object.
        /// </summary>
        public void OnAfterDeserialize()
        {
            // dictionary is not serialized, only the list is. Therefore, we must rebuild the dictionary from the list of entries.
            keyCollisions = new List<int>();
            dictionary = new Dictionary<K, V>(entries.Count);
            for (int i = 0; i < entries.Count; i++)
            {
                if (entries[i].key != null)
                {
                    if (!dictionary.ContainsKey(entries[i].key))
                    {
                        dictionary.Add(entries[i].key, entries[i].value);
                    }
                    else
                        keyCollisions.Add(i);
                }
            }
        }

        /// <summary>
        /// Called before Unity tries to serialize this object.
        /// </summary>
        public void OnBeforeSerialize()
        {
            InitializeKeyValueTypes();

            // serialize the dictionary (nothing to do because all dictionary operations also affect the list which is auto serialized)
        }

        private Entry CreateEntry(K key, V value)
        {
            return new Entry(key, value);
        }

        public void Add(K key, V value)
        {
            dictionary.Add(key, value);
            Entry entry = CreateEntry(key, value);
            entries.Add(entry);
        }

        public void Add(KeyValuePair<K, V> item)
        {
            dictionary.Add(item.Key, item.Value);
            Entry entry = CreateEntry(item.Key, item.Value);
            entries.Add(entry);
        }

        public void Clear()
        {
            dictionary.Clear();
            entries.Clear();
        }

        public bool Contains(KeyValuePair<K, V> item)
        {
            if (dictionary.TryGetValue(item.Key, out V value))
            {
                return EqualityComparer<V>.Default.Equals(value, item.Value);
            }
            
            return false;
        }

        public bool ContainsKey(K key)
        {
            return dictionary.ContainsKey(key);
        }

        public bool ContainsValue(V value)
        {
            return dictionary.ContainsValue(value);
        }

        public void CopyTo(KeyValuePair<K, V>[] array, int arrayIndex = 0)
        {
            if (array == null)
                throw new ArgumentException("The array cannot be null.");
            if (arrayIndex < 0)
                throw new ArgumentOutOfRangeException("The starting array index cannot be negative.");
            if (array.Length - arrayIndex < dictionary.Count)
                throw new ArgumentException("The destination array has fewer elements than the collection.");

            foreach (var pair in dictionary)
            {
                array[arrayIndex] = pair;
                arrayIndex++;
            }
        }

        public bool Remove(K key)
        {
            if (dictionary.Remove(key))
            {
                entries.RemoveAll((entry) => EqualityComparer<K>.Default.Equals(entry.key, key));

                return true;
            }

            return false;
        }

        public bool Remove(KeyValuePair<K, V> item)
        {
            return Remove(item.Key);
        }

        public bool TryGetValue(K key, out V value)
        {
            return dictionary.TryGetValue(key, out value);
        }

        public override string ToString()
        {
            return ToString(false);
        }

        public string ToString(bool pretty = false)
        {
            StringBuilder strBuilder = new StringBuilder();

            strBuilder.Append("{ ");
            int count = 0;
            foreach (K key in Keys)
            {
                strBuilder.Append((count == 0 ? "" : ", ") +
                    (pretty ? "\n\t" : "(") + key + " : " + this[key] + (pretty ? "" : ")")
                );
                count++;
            }
            strBuilder.Append(pretty ? "\n}" : " }");

            return strBuilder.ToString();
        }
    }
}
