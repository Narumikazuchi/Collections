namespace Narumikazuchi.Collections;

public partial class ReadOnlyDictionary<TKey, TValue, TEqualityComparer>
{
    internal ReadOnlyDictionary(KeyValuePair<TKey, TValue>[] items,
                                TEqualityComparer equalityComparer)
    {
        // Initialize
        Int32 size = (Int32)Primes.GetNext(items.Length);
        m_Buckets = new Int32[size];
        Int32 index;
        for (index = 0;
             index < size;
             index++)
        {
            m_Buckets[index] = -1;
        }

        m_Entries = new __DictionaryEntry<TKey, TValue>[size];
        this.EqualityComparer = equalityComparer;

        // Adding
        List<TKey> keys = new();
        List<TValue> values = new();
        index = 0;
        foreach (KeyValuePair<TKey, TValue> item in items)
        {
            Int32 hashcode = this.EqualityComparer.GetHashCode(item.Key) & 0x7FFFFFFF;
            Int32 bucket = hashcode % m_Buckets.Length;

            Boolean add = true;
            for (Int32 i = m_Buckets[bucket];
                 i > -1;
                 i = m_Entries[i].Next)
            {
                if (m_Entries[i].HashCode == hashcode &&
                    this.EqualityComparer.Equals(x: item.Key,
                                                 y: m_Entries[i].Key))
                {
                    add = false;
                    break;
                }
            }

            if (!add)
            {
                continue;
            }

            keys.Add(item.Key);
            values.Add(item.Value);

            m_Entries[index].HashCode = hashcode;
            m_Entries[index].Next = m_Buckets[bucket];
            m_Entries[index].Key = item.Key;
            m_Entries[index].Value = item.Value;
            m_Buckets[bucket] = index;
            index++;
        }

        this.Keys = ReadOnlyList<TKey>.CreateFrom<List<TKey>>(keys);
        this.Values = ReadOnlyList<TValue>.CreateFrom<List<TValue>>(values);
    }
#if NETCOREAPP3_1_OR_GREATER
    internal ReadOnlyDictionary(ImmutableArray<KeyValuePair<TKey, TValue>> items,
                                TEqualityComparer equalityComparer)
    {
        // Initialize
        Int32 size = (Int32)Primes.GetNext(items.Length);
        m_Buckets = new Int32[size];
        Int32 index;
        for (index = 0;
             index < size;
             index++)
        {
            m_Buckets[index] = -1;
        }

        m_Entries = new __DictionaryEntry<TKey, TValue>[size];
        this.EqualityComparer = equalityComparer;

        // Adding
        List<TKey> keys = new();
        List<TValue> values = new();
        index = 0;
        foreach (KeyValuePair<TKey, TValue> item in items)
        {
            Int32 hashcode = this.EqualityComparer.GetHashCode(item.Key) & 0x7FFFFFFF;
            Int32 bucket = hashcode % m_Buckets.Length;

            Boolean add = true;
            for (Int32 i = m_Buckets[bucket];
                 i > -1;
                 i = m_Entries[i].Next)
            {
                if (m_Entries[i].HashCode == hashcode &&
                    this.EqualityComparer.Equals(x: item.Key,
                                                 y: m_Entries[i].Key))
                {
                    add = false;
                    break;
                }
            }

            if (!add)
            {
                continue;
            }

            keys.Add(item.Key);
            values.Add(item.Value);

            m_Entries[index].HashCode = hashcode;
            m_Entries[index].Next = m_Buckets[bucket];
            m_Entries[index].Key = item.Key;
            m_Entries[index].Value = item.Value;
            m_Buckets[bucket] = index;
            index++;
        }
        
        this.Keys = ReadOnlyList<TKey>.CreateFrom<List<TKey>>(keys);
        this.Values = ReadOnlyList<TValue>.CreateFrom<List<TValue>>(values);
    }
#endif
    internal ReadOnlyDictionary(Dictionary<TKey, TValue> items,
                                TEqualityComparer equalityComparer)
    {
        // Initialize
        Int32 size = (Int32)Primes.GetNext(items.Count);
        m_Buckets = new Int32[size];
        Int32 index;
        for (index = 0;
             index < size;
             index++)
        {
            m_Buckets[index] = -1;
        }

        m_Entries = new __DictionaryEntry<TKey, TValue>[size];
        this.EqualityComparer = equalityComparer;

        // Adding
        index = 0;
        foreach (KeyValuePair<TKey, TValue> item in items)
        {
            Int32 hashcode = this.EqualityComparer.GetHashCode(item.Key) & 0x7FFFFFFF;
            Int32 bucket = hashcode % m_Buckets.Length;

            m_Entries[index].HashCode = hashcode;
            m_Entries[index].Next = m_Buckets[bucket];
            m_Entries[index].Key = item.Key;
            m_Entries[index].Value = item.Value;
            m_Buckets[bucket] = index;
            index++;
        }

        this.Keys = ReadOnlyList<TKey>.CreateFrom<Dictionary<TKey, TValue>.KeyCollection>(items.Keys);
        this.Values = ReadOnlyList<TValue>.CreateFrom<Dictionary<TKey, TValue>.ValueCollection>(items.Values);
    }

    private Int32 FindEntry(TKey key)
    {
        Int32 hashCode = this.EqualityComparer.GetHashCode(key) & 0x7FFFFFFF;
        for (Int32 index = m_Buckets[hashCode % m_Buckets.Length];
             index > -1;
             index = m_Entries[index].Next)
        {
            if (m_Entries[index].HashCode == hashCode &&
                this.EqualityComparer.Equals(x: m_Entries[index].Key,
                                             y: key))
            {
                return index;
            }
        }

        return -1;
    }

    internal readonly Int32[] m_Buckets;
    internal readonly __DictionaryEntry<TKey, TValue>[] m_Entries;
}