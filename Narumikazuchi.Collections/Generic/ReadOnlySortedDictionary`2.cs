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
#pragma warning disable CS0282
public readonly partial struct ReadOnlySortedDictionary<TKey, TValue>
    where TKey : notnull
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ReadOnlySortedDictionary{TKey, TValue}"/> struct.
    /// </summary>
    public ReadOnlySortedDictionary()
    {
        m_Buckets = Array.Empty<Int32>();
        m_Entries = Array.Empty<__DictionaryEntry<TKey, TValue>>();
        m_EqualityComparer = EqualityComparer<TKey>.Default;
        m_Comparer = Comparer<TKey>.Default;
        this.Count = 0;
        this.Keys = new();
        this.Values = new();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ReadOnlySortedDictionary{TKey, TValue}"/> class.
    /// </summary>
    public static ReadOnlySortedDictionary<TKey, TValue> Create() =>
        new();
    /// <summary>
    /// Initializes a new instance of the <see cref="ReadOnlySortedDictionary{TKey, TValue}"/> struct.
    /// </summary>
    /// <param name="items">The items that the resulting collection shall hold.</param>
    /// <exception cref="ArgumentNullException" />
    public static ReadOnlySortedDictionary<TKey, TValue> CreateFrom([DisallowNull] IEnumerable<KeyValuePair<TKey, TValue>> items) =>
        CreateFrom(items: items,
                   equalityComparer: EqualityComparer<TKey>.Default,
                   comparer: Comparer<TKey>.Default);
    /// <summary>
    /// Initializes a new instance of the <see cref="ReadOnlySortedDictionary{TKey, TValue}"/> struct.
    /// </summary>
    /// <param name="items">The items that the resulting collection shall hold.</param>
    /// <param name="equalityComparer">The comparer that will be used to compare two instances of type <typeparamref name="TKey"/> for equality.</param>
    /// <exception cref="ArgumentNullException" />
    public static ReadOnlySortedDictionary<TKey, TValue> CreateFrom([DisallowNull] IEnumerable<KeyValuePair<TKey, TValue>> items,
                                                                    [DisallowNull] IEqualityComparer<TKey> equalityComparer) =>
        CreateFrom(items: items,
                   equalityComparer: equalityComparer,
                   comparer: Comparer<TKey>.Default);
    /// <summary>
    /// Initializes a new instance of the <see cref="ReadOnlySortedDictionary{TKey, TValue}"/> struct.
    /// </summary>
    /// <param name="items">The items that the resulting collection shall hold.</param>
    /// <param name="equalityComparer">The comparer that will be used to compare two instances of type <typeparamref name="TKey"/> for equality.</param>
    /// <param name="comparer">The comparer that will be used to compare two instances of type <typeparamref name="TKey"/>.</param>
    /// <exception cref="ArgumentNullException" />
    public static ReadOnlySortedDictionary<TKey, TValue> CreateFrom([DisallowNull] IEnumerable<KeyValuePair<TKey, TValue>> items,
                                                                    [DisallowNull] IEqualityComparer<TKey> equalityComparer,
                                                                    [DisallowNull] IComparer<TKey> comparer)
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
    /// Initializes a new instance of the <see cref="ReadOnlySortedDictionary{TKey, TValue}"/> struct.
    /// </summary>
    /// <param name="items">The items that the resulting collection shall hold.</param>
    /// <exception cref="ArgumentNullException" />
    public static ReadOnlySortedDictionary<TKey, TValue> CreateFrom<TEnumerator>([DisallowNull] IStrongEnumerable<KeyValuePair<TKey, TValue>, TEnumerator> items)
        where TEnumerator : IEnumerator<KeyValuePair<TKey, TValue>> =>
            CreateFrom(items: items,
                       equalityComparer: EqualityComparer<TKey>.Default,
                       comparer: Comparer<TKey>.Default);
    /// <summary>
    /// Initializes a new instance of the <see cref="ReadOnlySortedDictionary{TKey, TValue}"/> struct.
    /// </summary>
    /// <param name="items">The items that the resulting collection shall hold.</param>
    /// <param name="equalityComparer">The comparer that will be used to compare two instances of type <typeparamref name="TKey"/> for equality.</param>
    /// <exception cref="ArgumentNullException" />
    public static ReadOnlySortedDictionary<TKey, TValue> CreateFrom<TEnumerator>([DisallowNull] IStrongEnumerable<KeyValuePair<TKey, TValue>, TEnumerator> items,
                                                                                 [DisallowNull] IEqualityComparer<TKey> equalityComparer)
        where TEnumerator : IEnumerator<KeyValuePair<TKey, TValue>> =>
            CreateFrom(items: items,
                       equalityComparer: equalityComparer,
                       comparer: Comparer<TKey>.Default);
    /// <summary>
    /// Initializes a new instance of the <see cref="ReadOnlySortedDictionary{TKey, TValue}"/> struct.
    /// </summary>
    /// <param name="items">The items that the resulting collection shall hold.</param>
    /// <param name="equalityComparer">The comparer that will be used to compare two instances of type <typeparamref name="TKey"/> for equality.</param>
    /// <param name="comparer">The comparer that will be used to compare two instances of type <typeparamref name="TKey"/>.</param>
    /// <exception cref="ArgumentNullException" />
    public static ReadOnlySortedDictionary<TKey, TValue> CreateFrom<TEnumerator>([DisallowNull] IStrongEnumerable<KeyValuePair<TKey, TValue>, TEnumerator> items,
                                                                                 [DisallowNull] IEqualityComparer<TKey> equalityComparer,
                                                                                 [DisallowNull] IComparer<TKey> comparer)
        where TEnumerator : IEnumerator<KeyValuePair<TKey, TValue>>
    {
        ArgumentNullException.ThrowIfNull(items);
        ArgumentNullException.ThrowIfNull(equalityComparer);
        ArgumentNullException.ThrowIfNull(comparer);


        if (items is ReadOnlySortedDictionary<TKey, TValue> readOnlySortedDictionary)
        {
            if (readOnlySortedDictionary.m_Comparer == comparer)
            {
                return new(buckets: readOnlySortedDictionary.m_Buckets,
                           entries: readOnlySortedDictionary.m_Entries,
                           equalityComparer: readOnlySortedDictionary.m_EqualityComparer,
                           comparer: comparer,
                           keys: readOnlySortedDictionary.Keys,
                           values: readOnlySortedDictionary.Values);
            }
            else
            {
                return new(items: items.OrderBy(kv => kv.Key, comparer),
                           count: items.Count(),
                           equalityComparer: equalityComparer,
                           comparer: comparer);
            }
        }
        //else if (items is ReadOnlyDictionary<TKey, TValue> readOnlyDictionary)
        //{
        //    return new(items: readOnlyDictionary.OrderBy(kv => kv.Key, comparer),
        //               count: readOnlyDictionary.Count,
        //               equalityComparer: equalityComparer);
        //}
        //else if (items is ReadOnlyCollection<KeyValuePair<TKey, TValue>> readOnlyCollection)
        //{
        //    return new(items: readOnlyCollection.OrderBy(kv => kv.Key, comparer),
        //               count: readOnlyCollection.Count,
        //               equalityComparer: equalityComparer);
        //}
        //else if (items is ReadOnlySortedCollection<KeyValuePair<TKey, TValue>> readOnlySortedCollection)
        //{
        //    return new(items: readOnlySortedCollection.m_Items.OrderBy(kv => kv.Key, comparer),
        //               count: readOnlySortedCollection.Count,
        //               equalityComparer: equalityComparer);
        //}
        //else if (items is ReadOnlyList<KeyValuePair<TKey, TValue>> readOnlyList)
        //{
        //    return new(items: readOnlyList.m_Items.OrderBy(kv => kv.Key, comparer),
        //               count: readOnlyList.Count,
        //               equalityComparer: equalityComparer);
        //}
        //else if (items is ReadOnlySortedList<KeyValuePair<TKey, TValue>> readOnlySortedList)
        //{
        //    return new(items: readOnlySortedList.m_Items.OrderBy(kv => kv.Key, comparer),
        //               count: readOnlySortedList.Count,
        //               equalityComparer: equalityComparer);
        //}
        else
        {
            return new(items: items.OrderBy(kv => kv.Key, comparer),
                       count: items.Count(),
                       equalityComparer: equalityComparer,
                       comparer: comparer);
        }
    }

    /// <summary>
    /// Gets a read-only list of keys present in the <see cref="ReadOnlySortedDictionary{TKey, TValue}"/>.
    /// </summary>
    public ReadOnlySortedList<TKey> Keys { get; }

    /// <summary>
    /// Gets a read-only list of values present in the <see cref="ReadOnlySortedDictionary{TKey, TValue}"/>.
    /// </summary>
    public ReadOnlyList<TValue> Values { get; }
}

// Non-Public
partial struct ReadOnlySortedDictionary<TKey, TValue>
{
    private ReadOnlySortedDictionary(Int32[] buckets,
                                     __DictionaryEntry<TKey, TValue>[] entries,
                                     IEqualityComparer<TKey> equalityComparer,
                                     IComparer<TKey> comparer,
                                     in ReadOnlySortedList<TKey> keys,
                                     in ReadOnlyList<TValue> values)
    {
        m_Buckets = buckets;
        m_Entries = entries;
        m_EqualityComparer = equalityComparer;
        m_Comparer = comparer;
        this.Count = keys.Count;
        this.Keys = keys;
        this.Values = values;
    }
    private ReadOnlySortedDictionary(IOrderedEnumerable<KeyValuePair<TKey, TValue>> items,
                                     in Int32 count,
                                     IEqualityComparer<TKey> equalityComparer,
                                     IComparer<TKey> comparer)
    {
        // Initialize
        Int32 size = Primes.GetNext(count);
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
        m_Comparer = comparer;

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
        this.Keys = ReadOnlySortedList<TKey>.CreateFrom(keys);
        this.Values = ReadOnlyList<TValue>.CreateFrom(values);
    }
    //private ReadOnlySortedDictionary(ReadOnlySortedCollection<KeyValuePair<TKey, TValue>> items,
    //                                 IEqualityComparer<TKey> equalityComparer,
    //                                 IComparer<TKey> comparer)
    //{
    //    // Initialize
    //    Int32 size = Primes.GetNext(items.Count);
    //    m_Buckets = new Int32[size];
    //    Int32 index;
    //    for (index = 0;
    //         index < size;
    //         index++)
    //    {
    //        m_Buckets[index] = -1;
    //    }
    //    m_Entries = new __DictionaryEntry<TKey, TValue>[size];
    //    m_EqualityComparer = equalityComparer;
    //    m_Comparer = comparer;

    //    // Adding
    //    List<TKey> keys = new();
    //    List<TValue> values = new();
    //    index = 0;
    //    foreach (KeyValuePair<TKey, TValue> item in items)
    //    {
    //        Int32 hashcode = m_EqualityComparer.GetHashCode(item.Key) & 0x7FFFFFFF;
    //        Int32 bucket = hashcode % m_Buckets.Length;

    //        Boolean add = true;
    //        for (Int32 i = m_Buckets[bucket];
    //             i > -1;
    //             i = m_Entries[i].Next)
    //        {
    //            if (m_Entries[i].HashCode == hashcode &&
    //                m_EqualityComparer.Equals(x: item.Key,
    //                                          y: m_Entries[i].Key))
    //            {
    //                add = false;
    //                break;
    //            }
    //        }

    //        if (!add)
    //        {
    //            continue;
    //        }

    //        keys.Add(item.Key);
    //        values.Add(item.Value);

    //        m_Entries[index].HashCode = hashcode;
    //        m_Entries[index].Next = m_Buckets[bucket];
    //        m_Entries[index].Key = item.Key;
    //        m_Entries[index].Value = item.Value;
    //        m_Buckets[bucket] = index;
    //        index++;
    //    }

    //    this.Count = index;
    //    this.Keys = ReadOnlySortedList<TKey>.CreateFrom(keys);
    //    this.Values = ReadOnlyList<TValue>.CreateFrom(values);
    //}
    //private ReadOnlySortedDictionary(ReadOnlySortedList<KeyValuePair<TKey, TValue>> items,
    //                                 IEqualityComparer<TKey> equalityComparer,
    //                                 IComparer<TKey> comparer)
    //{
    //    // Initialize
    //    Int32 size = Primes.GetNext(items.Count);
    //    m_Buckets = new Int32[size];
    //    Int32 index;
    //    for (index = 0;
    //         index < size;
    //         index++)
    //    {
    //        m_Buckets[index] = -1;
    //    }
    //    m_Entries = new __DictionaryEntry<TKey, TValue>[size];
    //    m_EqualityComparer = equalityComparer;
    //    m_Comparer = comparer;

    //    // Adding
    //    List<TKey> keys = new();
    //    List<TValue> values = new();
    //    index = 0;
    //    foreach (KeyValuePair<TKey, TValue> item in items)
    //    {
    //        Int32 hashcode = m_EqualityComparer.GetHashCode(item.Key) & 0x7FFFFFFF;
    //        Int32 bucket = hashcode % m_Buckets.Length;

    //        Boolean add = true;
    //        for (Int32 i = m_Buckets[bucket];
    //             i > -1;
    //             i = m_Entries[i].Next)
    //        {
    //            if (m_Entries[i].HashCode == hashcode &&
    //                m_EqualityComparer.Equals(x: item.Key,
    //                                          y: m_Entries[i].Key))
    //            {
    //                add = false;
    //                break;
    //            }
    //        }

    //        if (!add)
    //        {
    //            continue;
    //        }

    //        keys.Add(item.Key);
    //        values.Add(item.Value);

    //        m_Entries[index].HashCode = hashcode;
    //        m_Entries[index].Next = m_Buckets[bucket];
    //        m_Entries[index].Key = item.Key;
    //        m_Entries[index].Value = item.Value;
    //        m_Buckets[bucket] = index;
    //        index++;
    //    }

    //    this.Count = index;
    //    this.Keys = ReadOnlySortedList<TKey>.CreateFrom(keys);
    //    this.Values = ReadOnlyList<TValue>.CreateFrom(values);
    //}

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

    internal readonly Int32[] m_Buckets;
    internal readonly __DictionaryEntry<TKey, TValue>[] m_Entries;
    internal readonly IEqualityComparer<TKey> m_EqualityComparer;
    internal readonly IComparer<TKey> m_Comparer;
}

// IReadOnlyCollection<T>
partial struct ReadOnlySortedDictionary<TKey, TValue> : IReadOnlyCollection<KeyValuePair<TKey, TValue>>
{
    /// <inheritdoc/>
    public Int32 Count { get; }
}

// IReadOnlyDictionary<T, U>
partial struct ReadOnlySortedDictionary<TKey, TValue> : IReadOnlyDictionary<TKey, TValue>
{
    /// <inheritdoc/>
    public Boolean ContainsKey(TKey key) =>
        this.FindEntry(key) > -1;

    /// <inheritdoc/>
    public Boolean TryGetValue(TKey key,
                               [MaybeNullWhen(false)] out TValue value)
    {
        Int32 index = this.FindEntry(key);
        if (index > -1)
        {
            value = m_Entries[index].Value;
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

    IEnumerable<TKey> IReadOnlyDictionary<TKey, TValue>.Keys =>
        this.Keys;

    IEnumerable<TValue> IReadOnlyDictionary<TKey, TValue>.Values =>
        this.Values;
}

// IStrongEnumerable<T, U>
partial struct ReadOnlySortedDictionary<TKey, TValue> : IStrongEnumerable<KeyValuePair<TKey, TValue>, CommonDictionaryEnumerator<TKey, TValue>>
{
    /// <inheritdoc/>
    public CommonDictionaryEnumerator<TKey, TValue> GetEnumerator() =>
        new(m_Entries);
}