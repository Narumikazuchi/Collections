namespace Narumikazuchi.Collections;

/// <summary>
/// Represents a strongly-typed collection of key-value pairs with a significantly faster enumerator to
/// improve iteration times for <see langword="foreach"/>-loops. The key-value pairs are sorted by using
/// the provided <see cref="IComparer{T}"/>.
/// </summary>
/// <remarks>
/// The faster enumeration only works if the collection is used as-is or by using the
/// <see cref="IStrongEnumerable{TElement, TEnumerator}"/> interface. If you you plan on using 
/// the <see cref="IEnumerable{T}"/> or any other derivative interface in your code then the 
/// efficiency of the enumerator will be lost due to call virtualization in the compiler generated IL.
/// </remarks>
public readonly partial struct ReadOnlySortedDictionary<TKey, TValue, TComparer, TEqualityComparer>
    where TKey : notnull
    where TComparer : IComparer<TKey>
    where TEqualityComparer : IEqualityComparer<TKey>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ReadOnlySortedDictionary{TKey, TValue, TComparer, TEqualityComparer}"/> struct.
    /// </summary>
    public ReadOnlySortedDictionary()
    {
        m_Buckets = Array.Empty<Int32>();
        m_Entries = Array.Empty<__DictionaryEntry<TKey, TValue>>();
        this.EqualityComparer = default!;
        this.Comparer = default!;
        this.Count = 0;
        this.Keys = new();
        this.Values = new();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ReadOnlySortedDictionary{TKey, TValue, TComparer, TEqualityComparer}"/> class.
    /// </summary>
    public static ReadOnlySortedDictionary<TKey, TValue, TComparer, TEqualityComparer> Create() =>
        new();
    /// <summary>
    /// Initializes a new instance of the <see cref="ReadOnlySortedDictionary{TKey, TValue, TComparer, TEqualityComparer}"/> struct.
    /// </summary>
    /// <param name="items">The items that the resulting collection shall hold.</param>
    /// <param name="equalityComparer">The comparer that will be used to compare two instances of type <typeparamref name="TKey"/> for equality.</param>
    /// <param name="comparer">The comparer that will be used to compare two instances of type <typeparamref name="TKey"/>.</param>
    /// <exception cref="ArgumentNullException" />
    public static ReadOnlySortedDictionary<TKey, TValue, TComparer, TEqualityComparer> CreateFrom<TEnumerable>([DisallowNull] TEnumerable items,
                                                                                                               [DisallowNull] TEqualityComparer equalityComparer,
                                                                                                               [DisallowNull] TComparer comparer)
        where TEnumerable : IEnumerable<KeyValuePair<TKey, TValue>>
    {
        ArgumentNullException.ThrowIfNull(items);
        ArgumentNullException.ThrowIfNull(equalityComparer);
        ArgumentNullException.ThrowIfNull(comparer);

        if (items is IReadOnlyCollection<KeyValuePair<TKey, TValue>> roc)
        {
            return new(items: roc.OrderBy(kv => kv.Key, comparer),
                       count: roc.Count,
                       equalityComparer: equalityComparer,
                       comparer: comparer);
        }
        else if (items is ICollection<KeyValuePair<TKey, TValue>> c)
        {
            return new(items: c.OrderBy(kv => kv.Key, comparer),
                       count: c.Count,
                       equalityComparer: equalityComparer,
                       comparer: comparer);
        }
        else
        {
            return new(items: items.OrderBy(kv => kv.Key, comparer),
                       count: items.Count(),
                       equalityComparer: equalityComparer,
                       comparer: comparer);
        }
    }
    /// <summary>
    /// Initializes a new instance of the <see cref="ReadOnlySortedDictionary{TKey, TValue, TComparer, TEqualityComparer}"/> struct.
    /// </summary>
    /// <param name="items">The items that the resulting collection shall hold.</param>
    /// <param name="equalityComparer">The comparer that will be used to compare two instances of type <typeparamref name="TKey"/> for equality.</param>
    /// <param name="comparer">The comparer that will be used to compare two instances of type <typeparamref name="TKey"/>.</param>
    /// <exception cref="ArgumentNullException" />
    public static ReadOnlySortedDictionary<TKey, TValue, TComparer, TEqualityComparer> CreateFrom<TEnumerable, TEnumerator>([DisallowNull] TEnumerable items,
                                                                                                                            [DisallowNull] TEqualityComparer equalityComparer,
                                                                                                                            [DisallowNull] TComparer comparer)
        where TEnumerator : struct, IStrongEnumerator<KeyValuePair<TKey, TValue>>
        where TEnumerable : IStrongEnumerable<KeyValuePair<TKey, TValue>, TEnumerator>
    {
        ArgumentNullException.ThrowIfNull(items);
        ArgumentNullException.ThrowIfNull(equalityComparer);
        ArgumentNullException.ThrowIfNull(comparer);

        if (items is ReadOnlySortedDictionary<TKey, TValue, TComparer, TEqualityComparer> readOnlySortedDictionary)
        {
            if (Equals(readOnlySortedDictionary.Comparer, comparer))
            {
                return ReadOnlySortedDictionary<TKey, TValue, TComparer, TEqualityComparer>.Clone(readOnlySortedDictionary);
            }
            else
            {
                List<KeyValuePair<TKey, TValue>> result = new();
                foreach (KeyValuePair<TKey, TValue> kv in items)
                {
                    result.Add(kv);
                }
                return new(items: result.OrderBy(kv => kv.Key, comparer)
                                        .ToArray(),
                           equalityComparer: equalityComparer,
                           comparer: comparer);
            }
        }
        else
        {
            List<KeyValuePair<TKey, TValue>> result = new();
            foreach (KeyValuePair<TKey, TValue> kv in items)
            {
                result.Add(kv);
            }
            return new(items: result.OrderBy(kv => kv.Key, comparer)
                                    .ToArray(),
                       equalityComparer: equalityComparer,
                       comparer: comparer);
        }
    }
}

// Non-Public
partial struct ReadOnlySortedDictionary<TKey, TValue, TComparer, TEqualityComparer>
{
    private ReadOnlySortedDictionary(Int32[] buckets,
                                     __DictionaryEntry<TKey, TValue>[] entries,
                                     TEqualityComparer equalityComparer,
                                     TComparer comparer,
                                     ReadOnlyCollection<TKey> keys,
                                     ReadOnlyCollection<TValue> values)
    {
        m_Buckets = buckets;
        m_Entries = entries;
        this.EqualityComparer = equalityComparer;
        this.Comparer = comparer;
        this.Count = keys.Count;
        this.Keys = keys;
        this.Values = values;
    }
    private ReadOnlySortedDictionary(IOrderedEnumerable<KeyValuePair<TKey, TValue>> items,
                                     in Int32 count,
                                     TEqualityComparer equalityComparer,
                                     TComparer comparer)
    {
        // Initialize
        Int32 size = (Int32)Primes.GetNext(count);
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
        this.Comparer = comparer;

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

        this.Count = index;
        this.Keys = ReadOnlyCollection<TKey>.CreateFrom(keys);
        this.Values = ReadOnlyCollection<TValue>.CreateFrom(values);
    }
    private ReadOnlySortedDictionary(KeyValuePair<TKey, TValue>[] items,
                                     TEqualityComparer equalityComparer,
                                     TComparer comparer)
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
        this.Comparer = comparer;

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

        keys.Sort(comparer);
        this.Count = index;
        this.Keys = ReadOnlyCollection<TKey>.CreateFrom(keys);
        this.Values = ReadOnlyCollection<TValue>.CreateFrom(values);
    }

    private Int32 FindEntry(TKey key)
    {
        ArgumentNullException.ThrowIfNull(key);

        if (m_Buckets is not null)
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
        }
        return -1;
    }

    private static ReadOnlySortedDictionary<TKey, TValue, TComparer, TEqualityComparer> Clone(ReadOnlySortedDictionary<TKey, TValue, TComparer, TEqualityComparer> dictionary) => 
        new(buckets: dictionary.m_Buckets,
            entries: dictionary.m_Entries,
            equalityComparer: dictionary.EqualityComparer,
            comparer: dictionary.Comparer,
            keys: dictionary.Keys,
            values: dictionary.Values);

    internal readonly Int32[] m_Buckets;
    internal readonly __DictionaryEntry<TKey, TValue>[] m_Entries;
}

// IReadOnlyCollection<T>
partial struct ReadOnlySortedDictionary<TKey, TValue, TComparer, TEqualityComparer> : ICollectionWithCount<KeyValuePair<TKey, TValue>, CommonDictionaryEnumerator<TKey, TValue>>
{
    /// <inheritdoc/>
    public Int32 Count { get; }
}

// IEnumerable
partial struct ReadOnlySortedDictionary<TKey, TValue, TComparer, TEqualityComparer> : IEnumerable
{
    IEnumerator IEnumerable.GetEnumerator() =>
        this.GetEnumerator();
}

// IEnumerable<T>
partial struct ReadOnlySortedDictionary<TKey, TValue, TComparer, TEqualityComparer> : IEnumerable<KeyValuePair<TKey, TValue>>
{
    IEnumerator<KeyValuePair<TKey, TValue>> IEnumerable<KeyValuePair<TKey, TValue>>.GetEnumerator() =>
        this.GetEnumerator();
}

// IReadOnlyDictionary<T, U>
partial struct ReadOnlySortedDictionary<TKey, TValue, TComparer, TEqualityComparer> : IReadOnlyLookup<TKey, TValue, CommonDictionaryEnumerator<TKey, TValue>, TEqualityComparer>
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

    /// <inheritdoc/>
    public TEqualityComparer EqualityComparer { get; }
}

// ISortedDictionary<T, U>
partial struct ReadOnlySortedDictionary<TKey, TValue, TComparer, TEqualityComparer> : ISortedDictionary<TKey, TValue, CommonDictionaryEnumerator<TKey, TValue>, TEqualityComparer, TComparer>
{
    /// <inheritdoc/>
    public TComparer Comparer { get; }
}

// IStrongEnumerable<T, U>
partial struct ReadOnlySortedDictionary<TKey, TValue, TComparer, TEqualityComparer> : IStrongEnumerable<KeyValuePair<TKey, TValue>, CommonDictionaryEnumerator<TKey, TValue>>
{
    /// <inheritdoc/>
    public CommonDictionaryEnumerator<TKey, TValue> GetEnumerator() =>
        new(elements: m_Entries,
            count: this.Count);
}

// __IReadOnlyDictionary<T, U>
partial struct ReadOnlySortedDictionary<TKey, TValue, TComparer, TEqualityComparer> : __IReadOnlyDictionary<TKey, TValue, TEqualityComparer>
{
    Int32 __IReadOnlyDictionary<TKey, TValue, TEqualityComparer>.Size =>
        m_Entries.Length;

    Int32[] __IReadOnlyDictionary<TKey, TValue, TEqualityComparer>.Buckets =>
        m_Buckets;

    __DictionaryEntry<TKey, TValue>[] __IReadOnlyDictionary<TKey, TValue, TEqualityComparer>.Entries =>
        m_Entries;

    TEqualityComparer __IReadOnlyDictionary<TKey, TValue, TEqualityComparer>.EqualityComparer =>
        this.EqualityComparer;

    ReadOnlyCollection<TKey> __IReadOnlyDictionary<TKey, TValue, TEqualityComparer>.Keys =>
        this.Keys;

    ReadOnlyCollection<TValue> __IReadOnlyDictionary<TKey, TValue, TEqualityComparer>.Values =>
        this.Values;
}