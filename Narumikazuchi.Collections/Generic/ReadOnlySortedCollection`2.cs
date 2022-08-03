namespace Narumikazuchi.Collections;

/// <summary>
/// Represents a strongly-typed collection of elements with a significantly faster enumerator to
/// improve iteration times for <see langword="foreach"/>-loops.
/// </summary>
/// <remarks>
/// The faster enumeration only works if the collection is used as-is or by using the
/// <see cref="IStrongEnumerable{TElement, TEnumerator}"/> interface. If you you plan on using 
/// the <see cref="IEnumerable{T}"/> or <see cref="IReadOnlyCollection{T}"/> interface in your 
/// code then the efficiency of the enumerator will be lost due to call virtualization in the 
/// compiler generated IL.
/// </remarks>
public readonly partial struct ReadOnlySortedCollection<TElement, TComparer>
    where TComparer : IComparer<TElement>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ReadOnlySortedCollection{TElement, TComparer}"/> struct.
    /// </summary>
    public ReadOnlySortedCollection()
    {
        m_Items = Array.Empty<TElement>();
        this.Comparer = default!;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ReadOnlySortedCollection{TElement, TComparer}"/> class.
    /// </summary>
    public static ReadOnlySortedCollection<TElement, TComparer> Create() =>
        new();

    /// <summary>
    /// Initializes a new instance of the <see cref="ReadOnlySortedCollection{TElement, TComparer}"/> struct.
    /// </summary>
    /// <param name="items">The items that the resulting collection shall hold.</param>
    /// <param name="comparer">The comparer that will be used to compare two instances of type <typeparamref name="TElement"/>.</param>
    /// <exception cref="ArgumentNullException" />
    public static ReadOnlySortedCollection<TElement, TComparer> CreateFrom<TEnumerable>([DisallowNull] TEnumerable items,
                                                                                        [DisallowNull] TComparer comparer)
        where TEnumerable : IEnumerable<TElement>
    {
        ArgumentNullException.ThrowIfNull(items);
        ArgumentNullException.ThrowIfNull(comparer);

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
    /// Initializes a new instance of the <see cref="ReadOnlySortedCollection{TElement, TComparer}"/> struct.
    /// </summary>
    /// <param name="items">The items that the resulting collection shall hold.</param>
    /// <param name="comparer">The comparer that will be used to compare two instances of type <typeparamref name="TElement"/>.</param>
    /// <exception cref="ArgumentNullException" />
    public static ReadOnlySortedCollection<TElement, TComparer> CreateFrom<TEnumerable, TEnumerator>([DisallowNull] TEnumerable items,
                                                                                                     [DisallowNull] TComparer comparer)
        where TEnumerator : struct, IStrongEnumerator<TElement>
        where TEnumerable : IStrongEnumerable<TElement, TEnumerator>
    {
        ArgumentNullException.ThrowIfNull(items);
        ArgumentNullException.ThrowIfNull(comparer);

        if (items is ISortedCollection<TElement, TEnumerator, TComparer> iSortedCollection)
        {
            if (Equals(iSortedCollection.Comparer, comparer))
            {
                if (iSortedCollection is __IReadOnlyCollection<TElement> iReadOnlyCollectionT &&
                    iReadOnlyCollectionT.TryGetReadOnlyArray(out TElement[]? array))
                {
                    return new(items: array,
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
    public static implicit operator ReadOnlyCollection<TElement>(in ReadOnlySortedCollection<TElement, TComparer> source)
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
partial struct ReadOnlySortedCollection<TElement, TComparer>
{
    internal ReadOnlySortedCollection(TElement[] items,
                                      TComparer comparer)
    {
        m_Items = items;
        this.Comparer = comparer;
    }
    internal ReadOnlySortedCollection(IOrderedEnumerable<TElement> items,
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
partial struct ReadOnlySortedCollection<TElement, TComparer> : ICollectionWithCount<TElement, CommonArrayEnumerator<TElement>>
{
    /// <inheritdoc/>
    public Int32 Count =>
        m_Items.Length;
}

// IEnumerable
partial struct ReadOnlySortedCollection<TElement, TComparer> : IEnumerable
{
    IEnumerator IEnumerable.GetEnumerator() =>
        this.GetEnumerator();
}

// IEnumerable<T>
partial struct ReadOnlySortedCollection<TElement, TComparer> : IEnumerable<TElement>
{
    IEnumerator<TElement> IEnumerable<TElement>.GetEnumerator() =>
        this.GetEnumerator();
}

// IReadOnlyCollection<T>
partial struct ReadOnlySortedCollection<TElement, TComparer> : IReadOnlyCollection<TElement, CommonArrayEnumerator<TElement>>
{
    /// <inheritdoc/>
    public Boolean Contains(TElement element) =>
        Array.IndexOf(array: m_Items,
                      value: element) > -1;

    /// <inheritdoc/>
    public void CopyTo([DisallowNull] TElement[] array)
    {
        ArgumentNullException.ThrowIfNull(array);

        Array.Copy(sourceArray: m_Items,
                   destinationArray: array,
                   length: m_Items.Length);
    }
    /// <inheritdoc/>
    public void CopyTo([DisallowNull] TElement[] array,
                       Int32 destinationIndex)
    {
        ArgumentNullException.ThrowIfNull(array);
        destinationIndex.ThrowIfOutOfRange(0, Int32.MaxValue);

        Array.Copy(sourceArray: m_Items,
                   sourceIndex: 0,
                   destinationArray: array,
                   destinationIndex: destinationIndex,
                   length: m_Items.Length);
    }
}

// ISortedCollection<T, U>
partial struct ReadOnlySortedCollection<TElement, TComparer> : ISortedCollection<TElement, CommonArrayEnumerator<TElement>, TComparer>
{
    /// <inheritdoc/>
    public TComparer Comparer { get; }
}

// IStrongEnumerable<T, U>
partial struct ReadOnlySortedCollection<TElement, TComparer> : IStrongEnumerable<TElement, CommonArrayEnumerator<TElement>>
{
    /// <inheritdoc/>
    public CommonArrayEnumerator<TElement> GetEnumerator() =>
        new(m_Items);
}

// __IReadOnlyCollection<T>
partial struct ReadOnlySortedCollection<TElement, TComparer> : __IReadOnlyCollection<TElement>
{
    Boolean __IReadOnlyCollection<TElement>.TryGetReadOnlyArray([NotNullWhen(true)] out TElement[]? array)
    {
        array = m_Items;
        return true;
    }

    Boolean __IReadOnlyCollection<TElement>.TryGetReadOnlyList([NotNullWhen(true)] out List<TElement>? list)
    {
        list = default;
        return false;
    }
}