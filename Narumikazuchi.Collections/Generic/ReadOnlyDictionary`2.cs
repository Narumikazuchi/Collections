namespace Narumikazuchi.Collections;

/// <summary>
/// Represents a strongly-typed collection of key-value pairs with a significantly faster enumerator to
/// improve iteration times for <see langword="foreach"/>-loops.
/// </summary>
/// <remarks>
/// The faster enumeration only works if the collection is used as-is or by using the
/// <see cref="IStrongEnumerable{TElement, TEnumerator}"/> interface. If you you plan on using 
/// the <see cref="IEnumerable{T}"/> or any other derivative interface in your code then the 
/// efficiency of the enumerator will be lost due to call virtualization in the compiler generated IL.
/// </remarks>
public readonly partial struct ReadOnlyDictionary<TKey, TValue>
    where TKey : notnull
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ReadOnlyDictionary{TKey, TValue}"/> struct.
    /// </summary>
    public ReadOnlyDictionary()
    {
        m_Buckets = Array.Empty<Int32>();
        m_Entries = Array.Empty<__DictionaryEntry<TKey, TValue>>();
        m_EqualityComparer = EqualityComparer<TKey>.Default;
        this.Count = 0;
        this.Keys = new();
        this.Values = new();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ReadOnlyDictionary{TKey, TValue}"/> class.
    /// </summary>
    public static ReadOnlyDictionary<TKey, TValue> Create() =>
        new();

    /// <summary>
    /// Initializes a new instance of the <see cref="ReadOnlyDictionary{TKey, TValue}"/> struct.
    /// </summary>
    /// <param name="items">The items that the resulting collection shall hold.</param>
    /// <exception cref="ArgumentNullException" />
    public static ReadOnlyDictionary<TKey, TValue> CreateFrom([DisallowNull] IEnumerable<KeyValuePair<TKey, TValue>> items) =>
        CreateFrom(items: items,
                   equalityComparer: EqualityComparer<TKey>.Default);
    /// <summary>
    /// Initializes a new instance of the <see cref="ReadOnlyDictionary{TKey, TValue}"/> struct.
    /// </summary>
    /// <param name="items">The items that the resulting collection shall hold.</param>
    /// <param name="equalityComparer">The comparer that will be used to compare two instances of type <typeparamref name="TKey"/> for equality.</param>
    /// <exception cref="ArgumentNullException" />
    public static ReadOnlyDictionary<TKey, TValue> CreateFrom([DisallowNull] IEnumerable<KeyValuePair<TKey, TValue>> items,
                                                              [DisallowNull] IEqualityComparer<TKey> equalityComparer)
    {
        ArgumentNullException.ThrowIfNull(items);
        ArgumentNullException.ThrowIfNull(equalityComparer);

        if (items is KeyValuePair<TKey, TValue>[] array)
        {
            return new(items: array,
                       equalityComparer: equalityComparer);
        }
        else if (items is ImmutableArray<KeyValuePair<TKey, TValue>> immutableArray)
        {
            return new(items: immutableArray,
                       equalityComparer: equalityComparer);
        }
        else if (items is List<KeyValuePair<TKey, TValue>> list)
        {
            return new(items: list.ToArray(),
                       equalityComparer: equalityComparer);
        }
        else if (items is Dictionary<TKey, TValue> dictionary)
        {
            return new(items: dictionary,
                       equalityComparer: equalityComparer);
        }
        else if (items is ICollection<KeyValuePair<TKey, TValue>> iCollection)
        {
            KeyValuePair<TKey, TValue>[] elements = new KeyValuePair<TKey, TValue>[iCollection.Count];
            iCollection.CopyTo(elements, 0);
            return new(items: elements,
                       equalityComparer: equalityComparer);
        }
        else if (items is IReadOnlyList<KeyValuePair<TKey, TValue>> iReadOnlyList)
        {
            KeyValuePair<TKey, TValue>[] elements = new KeyValuePair<TKey, TValue>[iReadOnlyList.Count];
            Int32 index = 0;
            while (index < iReadOnlyList.Count)
            {
                elements[index] = iReadOnlyList[index++];
            }
            return new(items: elements,
                       equalityComparer: equalityComparer);
        }
        else
        {
            return new(items: items.ToArray(),
                       equalityComparer: equalityComparer);
        }
    }
    /// <summary>
    /// Initializes a new instance of the <see cref="ReadOnlyDictionary{TKey, TValue}"/> struct.
    /// </summary>
    /// <param name="items">The items that the resulting collection shall hold.</param>
    /// <exception cref="ArgumentNullException" />
    public static ReadOnlyDictionary<TKey, TValue> CreateFrom<TEnumerator>([DisallowNull] IStrongEnumerable<KeyValuePair<TKey, TValue>, TEnumerator> items)
        where TEnumerator : struct, IStrongEnumerator<KeyValuePair<TKey, TValue>> =>
            CreateFrom(items: items,
                       equalityComparer: EqualityComparer<TKey>.Default);
    /// <summary>
    /// Initializes a new instance of the <see cref="ReadOnlyDictionary{TKey, TValue}"/> struct.
    /// </summary>
    /// <param name="items">The items that the resulting collection shall hold.</param>
    /// <param name="equalityComparer">The comparer that will be used to compare two instances of type <typeparamref name="TKey"/> for equality.</param>
    /// <exception cref="ArgumentNullException" />
    public static ReadOnlyDictionary<TKey, TValue> CreateFrom<TEnumerator>([DisallowNull] IStrongEnumerable<KeyValuePair<TKey, TValue>, TEnumerator> items,
                                                                           [DisallowNull] IEqualityComparer<TKey> equalityComparer)
        where TEnumerator : struct, IStrongEnumerator<KeyValuePair<TKey, TValue>>
    {
        ArgumentNullException.ThrowIfNull(items);
        ArgumentNullException.ThrowIfNull(equalityComparer);

        if (items is ReadOnlyDictionary<TKey, TValue> readOnlyDictionary)
        {
            return ReadOnlyDictionary<TKey, TValue>.Clone(readOnlyDictionary);
        }
        else if (items is ReadOnlySortedDictionary<TKey, TValue> readOnlySortedDictionary)
        {
            return ReadOnlyDictionary<TKey, TValue>.Clone(readOnlySortedDictionary);
        }
        else if (items is IReadOnlyLookup<TKey, TValue, TEnumerator> iReadOnlyLookup)
        {
            KeyValuePair<TKey, TValue>[] elements = new KeyValuePair<TKey, TValue>[iReadOnlyLookup.Count];
            Int32 index = 0;
            foreach (KeyValuePair<TKey, TValue> kv in iReadOnlyLookup)
            {
                elements[index++] = kv;
            }
            return new(items: elements,
                       equalityComparer: equalityComparer);
        }
        else if (items is IReadOnlyCollection<KeyValuePair<TKey, TValue>, TEnumerator> iReadOnlyCollection)
        {
            KeyValuePair<TKey, TValue>[] elements = new KeyValuePair<TKey, TValue>[iReadOnlyCollection.Count];
            iReadOnlyCollection.CopyTo(elements);
            return new(items: elements,
                       equalityComparer: equalityComparer);
        }
        else
        {
            List<KeyValuePair<TKey, TValue>> list = new();
            foreach (KeyValuePair<TKey, TValue> kv in items)
            {
                list.Add(kv);
            }
            return new(items: list.ToArray(),
                       equalityComparer: equalityComparer);
        }
    }
}

// Non-Public
partial struct ReadOnlyDictionary<TKey, TValue>
{
    internal ReadOnlyDictionary(Int32[] buckets,
                                __DictionaryEntry<TKey, TValue>[] entries,
                                IEqualityComparer<TKey> equalityComparer,
                                ReadOnlyCollection<TKey> keys,
                                ReadOnlyCollection<TValue> values)
    {
        m_Buckets = buckets;
        m_Entries = entries;
        m_EqualityComparer = equalityComparer;
        this.Count = keys.Count;
        this.Keys = keys;
        this.Values = values;
    }
    internal ReadOnlyDictionary(KeyValuePair<TKey, TValue>[] items,
                                IEqualityComparer<TKey> equalityComparer)
    {
        // Initialize
        Int32 size = Primes.GetNext(items.Length);
        m_Buckets = new Int32[size];
        Int32 index;
        for (index = 0;
             index < size;
             index++)
        {
            m_Buckets[index] = -1;
        }
        m_Entries = new __DictionaryEntry<TKey, TValue>[size];
        m_EqualityComparer = equalityComparer;

        // Adding
        List<TKey> keys = new();
        List<TValue> values = new();
        index = 0;
        foreach (KeyValuePair<TKey, TValue> item in items)
        {
            Int32 hashcode = m_EqualityComparer.GetHashCode(item.Key) & 0x7FFFFFFF;
            Int32 bucket = hashcode % m_Buckets.Length;

            Boolean add = true;
            for (Int32 i = m_Buckets[bucket];
                 i > -1;
                 i = m_Entries[i].Next)
            {
                if (m_Entries[i].HashCode == hashcode &&
                    m_EqualityComparer.Equals(x: item.Key,
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

        this.Count = index;
        this.Keys = ReadOnlyCollection<TKey>.CreateFrom(keys);
        this.Values = ReadOnlyCollection<TValue>.CreateFrom(values);
    }
    internal ReadOnlyDictionary(ImmutableArray<KeyValuePair<TKey, TValue>> items,
                                IEqualityComparer<TKey> equalityComparer)
    {
        // Initialize
        Int32 size = Primes.GetNext(items.Length);
        m_Buckets = new Int32[size];
        Int32 index;
        for (index = 0;
             index < size;
             index++)
        {
            m_Buckets[index] = -1;
        }
        m_Entries = new __DictionaryEntry<TKey, TValue>[size];
        m_EqualityComparer = equalityComparer;

        // Adding
        List<TKey> keys = new();
        List<TValue> values = new();
        index = 0;
        foreach (KeyValuePair<TKey, TValue> item in items)
        {
            Int32 hashcode = m_EqualityComparer.GetHashCode(item.Key) & 0x7FFFFFFF;
            Int32 bucket = hashcode % m_Buckets.Length;

            Boolean add = true;
            for (Int32 i = m_Buckets[bucket];
                 i > -1;
                 i = m_Entries[i].Next)
            {
                if (m_Entries[i].HashCode == hashcode &&
                    m_EqualityComparer.Equals(x: item.Key,
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

        this.Count = index;
        this.Keys = ReadOnlyCollection<TKey>.CreateFrom(keys);
        this.Values = ReadOnlyCollection<TValue>.CreateFrom(values);
    }
    internal ReadOnlyDictionary(Dictionary<TKey, TValue> items,
                                IEqualityComparer<TKey> equalityComparer)
    {
        // Initialize
        Int32 size = Primes.GetNext(items.Count);
        m_Buckets = new Int32[size];
        Int32 index;
        for (index = 0;
             index < size;
             index++)
        {
            m_Buckets[index] = -1;
        }
        m_Entries = new __DictionaryEntry<TKey, TValue>[size];
        m_EqualityComparer = equalityComparer;

        // Adding
        index = 0;
        foreach (KeyValuePair<TKey, TValue> item in items)
        {
            Int32 hashcode = m_EqualityComparer.GetHashCode(item.Key) & 0x7FFFFFFF;
            Int32 bucket = hashcode % m_Buckets.Length;

            m_Entries[index].HashCode = hashcode;
            m_Entries[index].Next = m_Buckets[bucket];
            m_Entries[index].Key = item.Key;
            m_Entries[index].Value = item.Value;
            m_Buckets[bucket] = index;
            index++;
        }

        this.Count = index;
        this.Keys = ReadOnlyCollection<TKey>.CreateFrom(items.Keys);
        this.Values = ReadOnlyCollection<TValue>.CreateFrom(items.Values);
    }

    private Int32 FindEntry(TKey key)
    {
        ArgumentNullException.ThrowIfNull(key);

        if (m_Buckets is not null)
        {
            Int32 hashCode = m_EqualityComparer.GetHashCode(key) & 0x7FFFFFFF;
            for (Int32 index = m_Buckets[hashCode % m_Buckets.Length];
                 index > -1;
                 index = m_Entries[index].Next)
            {
                if (m_Entries[index].HashCode == hashCode &&
                    m_EqualityComparer.Equals(x: m_Entries[index].Key,
                                              y: key))
                {
                    return index;
                }
            }
        }
        return -1;
    }

    private static ReadOnlyDictionary<TKey, TValue> Clone<TEnumerable>(TEnumerable lookup)
        where TEnumerable : __IReadOnlyDictionary<TKey, TValue> => 
            new(buckets: lookup.Buckets,
                entries: lookup.Entries,
                equalityComparer: lookup.EqualityComparer,
                keys: lookup.Keys,
                values: lookup.Values);

    internal readonly Int32[] m_Buckets;
    internal readonly __DictionaryEntry<TKey, TValue>[] m_Entries;
    internal readonly IEqualityComparer<TKey> m_EqualityComparer;
}

// IEnumerable
partial struct ReadOnlyDictionary<TKey, TValue> : IEnumerable
{
    IEnumerator IEnumerable.GetEnumerator() =>
        this.GetEnumerator();
}

// IEnumerable<T>
partial struct ReadOnlyDictionary<TKey, TValue> : IEnumerable<KeyValuePair<TKey, TValue>>
{
    IEnumerator<KeyValuePair<TKey, TValue>> IEnumerable<KeyValuePair<TKey, TValue>>.GetEnumerator() =>
        this.GetEnumerator();
}

// IReadOnlyCollection<T>
partial struct ReadOnlyDictionary<TKey, TValue> : ICollectionWithCount<KeyValuePair<TKey, TValue>, CommonDictionaryEnumerator<TKey, TValue>>
{
    /// <inheritdoc/>
    public Int32 Count { get; }
}

// IReadOnlyDictionary<T, U>
partial struct ReadOnlyDictionary<TKey, TValue> : IReadOnlyLookup<TKey, TValue, CommonDictionaryEnumerator<TKey, TValue>>
{
    /// <inheritdoc/>
    public Boolean ContainsKey([DisallowNull] TKey key) =>
        this.FindEntry(key) > -1;

    /// <inheritdoc/>
    public Boolean ContainsValue(TValue value) =>
        this.Values.Contains(value);

    /// <inheritdoc/>
    public Boolean TryGetValue([DisallowNull] TKey key,
                               [NotNullWhen(true)] out TValue? value)
    {
        Int32 index = this.FindEntry(key);
        if (index > -1)
        {
            value = m_Entries[index].Value!;
            return true;
        }
        value = default;
        return false;
    }

    /// <inheritdoc/>
    public TValue this[TKey key]
    {
        get
        {
            Int32 index = this.FindEntry(key);
            if (index > -1)
            {
                return m_Entries[index].Value;
            }
            throw new KeyNotFoundException();
        }
    }

    /// <inheritdoc/>
    public ReadOnlyCollection<TKey> Keys { get; }

    /// <inheritdoc/>
    public ReadOnlyCollection<TValue> Values { get; }
}

// IStrongEnumerable<T, U>
partial struct ReadOnlyDictionary<TKey, TValue> : IStrongEnumerable<KeyValuePair<TKey, TValue>, CommonDictionaryEnumerator<TKey, TValue>>
{
    /// <inheritdoc/>
    public CommonDictionaryEnumerator<TKey, TValue> GetEnumerator() =>
        new(m_Entries);
}

// __IReadOnlyDictionary<T, U>
partial struct ReadOnlyDictionary<TKey, TValue> : __IReadOnlyDictionary<TKey, TValue>
{
    Int32 __IReadOnlyDictionary<TKey, TValue>.Size =>
        m_Entries.Length;

    Int32[] __IReadOnlyDictionary<TKey, TValue>.Buckets =>
        m_Buckets;

    __DictionaryEntry<TKey, TValue>[] __IReadOnlyDictionary<TKey, TValue>.Entries =>
        m_Entries;

    IEqualityComparer<TKey> __IReadOnlyDictionary<TKey, TValue>.EqualityComparer =>
        m_EqualityComparer;

    ReadOnlyCollection<TKey> __IReadOnlyDictionary<TKey, TValue>.Keys =>
        this.Keys;

    ReadOnlyCollection<TValue> __IReadOnlyDictionary<TKey, TValue>.Values =>
        this.Values;
}