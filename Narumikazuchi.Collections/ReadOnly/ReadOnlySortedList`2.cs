namespace Narumikazuchi.Collections;

/// <summary>
/// Represents a strongly-typed collection of elements with a significantly faster enumerator to
/// improve iteration times for <see langword="foreach"/>-loops. The elements of this collection
/// can be accessed by index and are sorted upon instatiation using either the default comparer
/// or the provided one.
/// </summary>
/// <remarks>
/// The faster enumeration only works if the collection is used as-is or by using the
/// <see cref="IStrongEnumerable{TElement, TEnumerator}"/> interface. If you you plan on using 
/// the <see cref="IEnumerable{T}"/> or <see cref="IReadOnlyCollection{T}"/> interface in your 
/// code then the efficiency of the enumerator will be lost due to call virtualization in the 
/// compiler generated IL.
/// </remarks>
public readonly partial struct ReadOnlySortedList<TElement, TComparer>
    where TComparer : IComparer<TElement>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ReadOnlySortedList{TElement, TComparer}"/> struct.
    /// </summary>
    public ReadOnlySortedList()
    {
        m_Items = Array.Empty<TElement>();
        this.Comparer = default!;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ReadOnlySortedList{TElement, TComparer}"/> class.
    /// </summary>
    public static ReadOnlySortedList<TElement, TComparer> Create() =>
        new();

    /// <summary>
    /// Initializes a new instance of the <see cref="ReadOnlySortedList{TElement, TComparer}"/> struct.
    /// </summary>
    /// <param name="items">The items that the resulting collection shall hold.</param>
    /// <param name="comparer">The comparer that will be used to compare two instances of type <typeparamref name="TElement"/>.</param>
    /// <exception cref="ArgumentNullException" />
    public static ReadOnlySortedList<TElement, TComparer> CreateFrom<TEnumerable>(
#if NETCOREAPP3_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        [DisallowNull]
#endif
        TEnumerable items,
#if NETCOREAPP3_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        [DisallowNull]
#endif
        TComparer comparer)
            where TEnumerable : IEnumerable<TElement>
    {
#if NET6_0_OR_GREATER
        ArgumentNullException.ThrowIfNull(items);
        ArgumentNullException.ThrowIfNull(comparer);
#else
        if (items is null)
        {
            throw new ArgumentNullException(nameof(items));
        }

        if (comparer is null)
        {
            throw new ArgumentNullException(nameof(comparer));
        }
#endif

        if (items is IReadOnlyCollection<TElement> iReadOnlyCollection)
        {
            return new(items: iReadOnlyCollection.OrderBy(x => x, comparer),
                       count: iReadOnlyCollection.Count,
                       comparer: comparer);
        }
        else if (items is ICollection<TElement> iCollection)
        {
            return new(items: iCollection.OrderBy(x => x, comparer),
                       count: iCollection.Count,
                       comparer: comparer);
        }
        else
        {
            return new(items: items.OrderBy(x => x, comparer),
                       count: items.Count(),
                       comparer: comparer);
        }
    }
    /// <summary>
    /// Initializes a new instance of the <see cref="ReadOnlySortedList{TElement, TComparer}"/> struct.
    /// </summary>
    /// <param name="items">The items that the resulting collection shall hold.</param>
    /// <param name="comparer">The comparer that will be used to compare two instances of type <typeparamref name="TElement"/>.</param>
    /// <exception cref="ArgumentNullException" />
    public static ReadOnlySortedList<TElement, TComparer> CreateFrom<TEnumerable, TEnumerator>(
#if NETCOREAPP3_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        [DisallowNull]
#endif
        TEnumerable items,
#if NETCOREAPP3_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        [DisallowNull]
#endif
        TComparer comparer)
            where TEnumerator : struct, IStrongEnumerator<TElement>
            where TEnumerable : IStrongEnumerable<TElement, TEnumerator>
    {
#if NET6_0_OR_GREATER
        ArgumentNullException.ThrowIfNull(items);
        ArgumentNullException.ThrowIfNull(comparer);
#else
        if (items is null)
        {
            throw new ArgumentNullException(nameof(items));
        }

        if (comparer is null)
        {
            throw new ArgumentNullException(nameof(comparer));
        }
#endif

        if (items is ISortedCollection<TElement, TEnumerator, TComparer> iSortedCollection)
        {
            if (Equals(iSortedCollection.Comparer, comparer))
            {
                if (iSortedCollection is __IReadOnlyCollection<TElement> iReadOnlyCollectionT &&
                    iReadOnlyCollectionT.TryGetReadOnlyArray(out TElement[]? array))
                {
                    return new(items: array!,
                               comparer: comparer);
                }
                else
                {
                    TElement[] elements = new TElement[iSortedCollection.Count];
                    iSortedCollection.CopyTo(elements, 0);
                    return new(items: elements.OrderBy(x => x, comparer)
                                              .ToArray(),
                               comparer: comparer);
                }
            }
        }
        if (items is IReadOnlyCollection<TElement, TEnumerator> iReadOnlyCollection)
        {
            TElement[] elements = new TElement[iReadOnlyCollection.Count];
            iReadOnlyCollection.CopyTo(elements, 0);
            return new(items: elements.OrderBy(x => x, comparer)
                                      .ToArray(),
                       comparer: comparer);
        }
        else
        {
            List<TElement> result = new();
            foreach (TElement element in items)
            {
                result.Add(element);
            }
            result.Sort(comparer);
            return new(items: result.ToArray(),
                       comparer: comparer);
        }
    }

#pragma warning disable CS1591
    public static implicit operator ReadOnlyList<TElement>(in ReadOnlySortedList<TElement, TComparer> source)
    {
        TElement[] items = new TElement[source.Count];
        Array.Copy(sourceArray: source.m_Items,
                   destinationArray: items,
                   length: source.Count);
        return new(items);
    }
#pragma warning restore
}

// Non-Public
partial struct ReadOnlySortedList<TElement, TComparer>
{
    internal ReadOnlySortedList(TElement[] items,
                                TComparer comparer)
    {
        m_Items = items;
        this.Comparer = comparer;
    }
    internal ReadOnlySortedList(IOrderedEnumerable<TElement> items,
                                TComparer comparer,
                                in Int32 count)
    {
        m_Items = new TElement[count];
        this.Comparer = comparer;

        Int32 index = 0;
        foreach (TElement item in items)
        {
            m_Items[index++] = item;
        }
    }

    internal readonly TElement[] m_Items;
}

// IReadOnlyCollection<T>
partial struct ReadOnlySortedList<TElement, TComparer> : ICollectionWithCount<TElement, CommonArrayEnumerator<TElement>>
{
    /// <inheritdoc/>
    public Int32 Count
    {
        get
        {
            if (m_Items is null)
            {
                return 0;
            }
            else
            {
                return m_Items.Length;
            }
        }
    }
}

// IEnumerable
partial struct ReadOnlySortedList<TElement, TComparer> : IEnumerable
{
    IEnumerator IEnumerable.GetEnumerator() =>
        this.GetEnumerator();
}

// IEnumerable<T>
partial struct ReadOnlySortedList<TElement, TComparer> : IEnumerable<TElement>
{
    IEnumerator<TElement> IEnumerable<TElement>.GetEnumerator() =>
        this.GetEnumerator();
}

// IReadOnlyList<T>
partial struct ReadOnlySortedList<TElement, TComparer> : ICollectionWithReadIndexer<TElement, CommonArrayEnumerator<TElement>>
{
    /// <inheritdoc/>
    public Int32 IndexOf(TElement element)
    {
        if (m_Items is null)
        {
            return -1;
        }
        else
        {
            return Array.IndexOf(array: m_Items,
                                 value: element);
        }
    }

    /// <inheritdoc/>
    public TElement this[Int32 index]
    {
        get
        {
            if (m_Items is null)
            {
                throw new ArgumentOutOfRangeException(paramName: nameof(index));
            }
            else
            {
                return m_Items[index];
            }
        }
    }
#if NETCOREAPP3_1_OR_GREATER
    /// <inheritdoc/>
    public TElement this[Index index]
    {
        get
        {
            if (m_Items is null)
            {
                throw new ArgumentOutOfRangeException(paramName: nameof(index));
            }
            else
            {
                return m_Items[index];
            }
        }
    }
    /// <inheritdoc/>
    public ImmutableArray<TElement> this[Range range]
    {
        get
        {
            if (m_Items is null)
            {
                throw new ArgumentOutOfRangeException(paramName: nameof(range));
            }
            else
            {
                return m_Items[range].ToImmutableArray();
            }
        }
    }
#endif
}

// IReadOnlyCollection<T>
partial struct ReadOnlySortedList<TElement, TComparer> : IReadOnlyCollection<TElement, CommonArrayEnumerator<TElement>>
{
    /// <inheritdoc/>
    public Boolean Contains(TElement element)
    {
        if (m_Items is null)
        {
            return false;
        }
        else
        {
            return Array.IndexOf(array: m_Items,
                                 value: element) > -1;
        }
    }

    /// <inheritdoc/>
    public void CopyTo(
#if NETCOREAPP3_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        [DisallowNull]
#endif
        TElement[] array)
    {
#if NET6_0_OR_GREATER
        ArgumentNullException.ThrowIfNull(array);
#else
        if (array is null)
        {
            throw new ArgumentNullException(nameof(array));
        }
#endif

        if (m_Items is not null)
        {
            Array.Copy(sourceArray: m_Items,
                       destinationArray: array,
                       length: m_Items.Length);
        }
    }
    /// <inheritdoc/>
    public void CopyTo(
#if NETCOREAPP3_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        [DisallowNull]
#endif
        TElement[] array,
        Int32 destinationIndex)
    {
#if NET6_0_OR_GREATER
        ArgumentNullException.ThrowIfNull(array);
#else
        if (array is null)
        {
            throw new ArgumentNullException(nameof(array));
        }
#endif
#if NETCOREAPP3_1_OR_GREATER
        destinationIndex.ThrowIfOutOfRange(0, Int32.MaxValue);
#else
        if (destinationIndex is < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(destinationIndex));
        }
#endif

        if (m_Items is not null)
        {
            Array.Copy(sourceArray: m_Items,
                       sourceIndex: 0,
                       destinationArray: array,
                       destinationIndex: destinationIndex,
                       length: m_Items.Length);
        }
    }
}

// ISortedCollection<T, U>
partial struct ReadOnlySortedList<TElement, TComparer> : ISortedCollection<TElement, CommonArrayEnumerator<TElement>, TComparer>
{
    /// <inheritdoc/>
    public TComparer Comparer { get; }
}

// IStrongEnumerable<T, U>
partial struct ReadOnlySortedList<TElement, TComparer> : IStrongEnumerable<TElement, CommonArrayEnumerator<TElement>>
{
    /// <inheritdoc/>
    public CommonArrayEnumerator<TElement> GetEnumerator() =>
        new(m_Items ?? Array.Empty<TElement>());
}

// __IReadOnlyCollection<T>
partial struct ReadOnlySortedList<TElement, TComparer> : __IReadOnlyCollection<TElement>
{
    Boolean __IReadOnlyCollection<TElement>.TryGetReadOnlyArray(
#if NETCOREAPP3_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        [NotNullWhen(true)]
#endif
        out TElement[]? array)
    {
        array = m_Items ?? Array.Empty<TElement>();
        return true;
    }

    Boolean __IReadOnlyCollection<TElement>.TryGetReadOnlyList(
#if NETCOREAPP3_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        [NotNullWhen(true)]
#endif
        out List<TElement>? list)
    {
        list = default;
        return false;
    }
}