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
public readonly partial struct ReadOnlySortedList<TElement>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ReadOnlySortedList{TElement}"/> struct.
    /// </summary>
    public ReadOnlySortedList()
    {
        m_Items = Array.Empty<TElement>();
        m_Comparer = Comparer<TElement>.Default;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ReadOnlySortedList{TElement}"/> class.
    /// </summary>
    public static ReadOnlySortedList<TElement> Create() =>
        new();

    /// <summary>
    /// Initializes a new instance of the <see cref="ReadOnlySortedList{TElement}"/> struct.
    /// </summary>
    /// <param name="items">The items that the resulting collection shall hold.</param>
    /// <exception cref="ArgumentNullException" />
    public static ReadOnlySortedList<TElement> CreateFrom([DisallowNull] IEnumerable<TElement> items) =>
        CreateFrom(items: items,
                   comparer: Comparer<TElement>.Default);
    /// <summary>
    /// Initializes a new instance of the <see cref="ReadOnlySortedList{TElement}"/> struct.
    /// </summary>
    /// <param name="items">The items that the resulting collection shall hold.</param>
    /// <param name="comparer">The comparer that will be used to compare two instances of type <typeparamref name="TElement"/>.</param>
    /// <exception cref="ArgumentNullException" />
    public static ReadOnlySortedList<TElement> CreateFrom([DisallowNull] IEnumerable<TElement> items,
                                                          [DisallowNull] IComparer<TElement> comparer)
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
    /// Initializes a new instance of the <see cref="ReadOnlySortedList{TElement}"/> struct.
    /// </summary>
    /// <param name="items">The items that the resulting collection shall hold.</param>
    /// <exception cref="ArgumentNullException" />
    public static ReadOnlySortedList<TElement> CreateFrom<TEnumerator>([DisallowNull] IStrongEnumerable<TElement, TEnumerator> items)
        where TEnumerator : struct, IStrongEnumerator<TElement> =>
            CreateFrom(items: items,
                       comparer: Comparer<TElement>.Default);
    /// <summary>
    /// Initializes a new instance of the <see cref="ReadOnlySortedList{TElement}"/> struct.
    /// </summary>
    /// <param name="items">The items that the resulting collection shall hold.</param>
    /// <param name="comparer">The comparer that will be used to compare two instances of type <typeparamref name="TElement"/>.</param>
    /// <exception cref="ArgumentNullException" />
    public static ReadOnlySortedList<TElement> CreateFrom<TEnumerator>([DisallowNull] IStrongEnumerable<TElement, TEnumerator> items,
                                                                       [DisallowNull] IComparer<TElement> comparer)
        where TEnumerator : struct, IStrongEnumerator<TElement>
    {
        ArgumentNullException.ThrowIfNull(items);
        ArgumentNullException.ThrowIfNull(comparer);

        if (items is ISortedCollection<TElement, TEnumerator> iSortedCollection)
        {
            if (iSortedCollection.Comparer == comparer)
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
    public static implicit operator ReadOnlyList<TElement>(in ReadOnlySortedList<TElement> source)
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
partial struct ReadOnlySortedList<TElement>
{
    internal ReadOnlySortedList(TElement[] items,
                                IComparer<TElement> comparer)
    {
        m_Items = items;
        m_Comparer = comparer;
    }
    internal ReadOnlySortedList(IOrderedEnumerable<TElement> items,
                                IComparer<TElement> comparer,
                                in Int32 count)
    {
        m_Items = new TElement[count];
        m_Comparer = comparer;

        Int32 index = 0;
        foreach (TElement item in items)
        {
            m_Items[index++] = item;
        }
    }

    internal readonly TElement[] m_Items;
    internal readonly IComparer<TElement> m_Comparer;
}

// IReadOnlyCollection<T>
partial struct ReadOnlySortedList<TElement> : ICollectionWithCount<TElement, CommonArrayEnumerator<TElement>>
{
    /// <inheritdoc/>
    public Int32 Count =>
        m_Items.Length;
}

// IEnumerable
partial struct ReadOnlySortedList<TElement> : IEnumerable
{
    IEnumerator IEnumerable.GetEnumerator() =>
        this.GetEnumerator();
}

// IEnumerable<T>
partial struct ReadOnlySortedList<TElement> : IEnumerable<TElement>
{
    IEnumerator<TElement> IEnumerable<TElement>.GetEnumerator() =>
        this.GetEnumerator();
}

// IReadOnlyList<T>
partial struct ReadOnlySortedList<TElement> : ICollectionWithReadIndexer<TElement, CommonArrayEnumerator<TElement>>
{
    /// <inheritdoc/>
    public Int32 IndexOf(TElement element) =>
        Array.IndexOf(array: m_Items,
                      value: element);

    /// <inheritdoc/>
    public TElement this[Int32 index] =>
        m_Items[index];
    /// <inheritdoc/>
    public TElement this[Index index] =>
        m_Items[index];
}

// IReadOnlyCollection<T>
partial struct ReadOnlySortedList<TElement> : IReadOnlyCollection<TElement, CommonArrayEnumerator<TElement>>
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
partial struct ReadOnlySortedList<TElement> : ISortedCollection<TElement, CommonArrayEnumerator<TElement>>
{
    /// <inheritdoc/>
    public IComparer<TElement> Comparer =>
        m_Comparer;
}

// IStrongEnumerable<T, U>
partial struct ReadOnlySortedList<TElement> : IStrongEnumerable<TElement, CommonArrayEnumerator<TElement>>
{
    /// <inheritdoc/>
    public CommonArrayEnumerator<TElement> GetEnumerator() =>
        new(m_Items);
}

// __IReadOnlyCollection<T>
partial struct ReadOnlySortedList<TElement> : __IReadOnlyCollection<TElement>
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