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
public readonly partial struct ReadOnlySortedCollection<TElement>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ReadOnlySortedCollection{TElement}"/> struct.
    /// </summary>
    public ReadOnlySortedCollection()
    {
        m_Items = Array.Empty<TElement>();
        m_Comparer = Comparer<TElement>.Default;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ReadOnlySortedCollection{TElement}"/> class.
    /// </summary>
    public static ReadOnlySortedCollection<TElement> Create() =>
        new();

    /// <summary>
    /// Initializes a new instance of the <see cref="ReadOnlySortedCollection{TElement}"/> struct.
    /// </summary>
    /// <param name="items">The items that the resulting collection shall hold.</param>
    /// <exception cref="ArgumentNullException" />
    public static ReadOnlySortedCollection<TElement> CreateFrom([DisallowNull] IEnumerable<TElement> items) =>
        CreateFrom(items: items,
                   comparer: Comparer<TElement>.Default);
    /// <summary>
    /// Initializes a new instance of the <see cref="ReadOnlySortedCollection{TElement}"/> struct.
    /// </summary>
    /// <param name="items">The items that the resulting collection shall hold.</param>
    /// <param name="comparer">The comparer that will be used to compare two instances of type <typeparamref name="TElement"/>.</param>
    /// <exception cref="ArgumentNullException" />
    public static ReadOnlySortedCollection<TElement> CreateFrom([DisallowNull] IEnumerable<TElement> items,
                                                                [DisallowNull] IComparer<TElement> comparer)
    {
        ArgumentNullException.ThrowIfNull(items);
        ArgumentNullException.ThrowIfNull(comparer);

        if (items is IReadOnlyCollection<TElement> roc)
        {
            return new(items: roc.OrderBy(x => x, comparer),
                       count: roc.Count,
                       comparer: comparer);
        }
        else if (items is ICollection<TElement> c)
        {
            return new(items: c.OrderBy(x => x, comparer),
                       count: c.Count,
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
    /// Initializes a new instance of the <see cref="ReadOnlySortedCollection{TElement}"/> struct.
    /// </summary>
    /// <param name="items">The items that the resulting collection shall hold.</param>
    /// <exception cref="ArgumentNullException" />
    public static ReadOnlySortedCollection<TElement> CreateFrom<TEnumerator>([DisallowNull] IStrongEnumerable<TElement, TEnumerator> items)
        where TEnumerator : struct, IStrongEnumerator<TElement> =>
            CreateFrom(items: items,
                       comparer: Comparer<TElement>.Default);
    /// <summary>
    /// Initializes a new instance of the <see cref="ReadOnlySortedCollection{TElement}"/> struct.
    /// </summary>
    /// <param name="items">The items that the resulting collection shall hold.</param>
    /// <param name="comparer">The comparer that will be used to compare two instances of type <typeparamref name="TElement"/>.</param>
    /// <exception cref="ArgumentNullException" />
    public static ReadOnlySortedCollection<TElement> CreateFrom<TEnumerator>([DisallowNull] IStrongEnumerable<TElement, TEnumerator> items,
                                                                             [DisallowNull] IComparer<TElement> comparer)
        where TEnumerator : struct, IStrongEnumerator<TElement>
    {
        ArgumentNullException.ThrowIfNull(items);
        ArgumentNullException.ThrowIfNull(comparer);

        if (items is ReadOnlySortedCollection<TElement> readOnlySortedCollection)
        {
            if (readOnlySortedCollection.m_Comparer == comparer)
            {
                TElement[] elements = new TElement[readOnlySortedCollection.Count];
                Array.Copy(sourceArray: readOnlySortedCollection.m_Items,
                           destinationArray: elements,
                           length: elements.Length);
                return new(items: elements,
                           comparer: readOnlySortedCollection.m_Comparer);
            }
            else
            {
                return new(items: items.OrderBy(x => x, comparer),
                           count: items.Count(),
                           comparer: comparer);
            }
        }
        else if (items is IReadOnlyCollection<TElement> roc)
        {
            return new(items: roc.OrderBy(x => x, comparer),
                       count: roc.Count,
                       comparer: comparer);
        }
        else if (items is ICollection<TElement> c)
        {
            return new(items: c.OrderBy(x => x, comparer),
                       count: c.Count,
                       comparer: comparer);
        }
        else
        {
            return new(items: items.OrderBy(x => x, comparer),
                       count: items.Count(),
                       comparer: comparer);
        }
    }

#pragma warning disable CS1591
    public static implicit operator ReadOnlyCollection<TElement>(in ReadOnlySortedCollection<TElement> source)
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
partial struct ReadOnlySortedCollection<TElement>
{
    internal ReadOnlySortedCollection(TElement[] items,
                                      IComparer<TElement> comparer)
    {
        m_Items = items;
        m_Comparer = comparer;
    }
    internal ReadOnlySortedCollection(IOrderedEnumerable<TElement> items,
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

// IStrongEnumerable<T, U>
partial struct ReadOnlySortedCollection<TElement> : IStrongEnumerable<TElement, CommonArrayEnumerator<TElement>>
{
    /// <inheritdoc/>
    public CommonArrayEnumerator<TElement> GetEnumerator() =>
        new(m_Items);
}

// IReadOnlyCollection<T>
partial struct ReadOnlySortedCollection<TElement> : IReadOnlyCollection<TElement>
{
    /// <inheritdoc/>
    public Int32 Count =>
        m_Items.Length;
}