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
    /// Initializes a new instance of the <see cref="ReadOnlySortedList{TElement}"/> struct.
    /// </summary>
    /// <param name="items">The items that the resulting collection shall hold.</param>
    /// <exception cref="ArgumentNullException" />
    public static ReadOnlySortedList<TElement> CreateFrom<TEnumerator>([DisallowNull] IStrongEnumerable<TElement, TEnumerator> items)
        where TEnumerator : IEnumerator<TElement> =>
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
        where TEnumerator : IEnumerator<TElement>
    {
        ArgumentNullException.ThrowIfNull(items);
        ArgumentNullException.ThrowIfNull(comparer);

        if (items is ReadOnlySortedList<TElement> readOnlySortedList)
        {
            if (readOnlySortedList.m_Comparer == comparer)
            {
                TElement[] elements = new TElement[readOnlySortedList.Count];
                Array.Copy(sourceArray: readOnlySortedList.m_Items,
                           destinationArray: elements,
                           length: elements.Length);
                return new(items: elements,
                           comparer: readOnlySortedList.m_Comparer);
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

// IStrongEnumerable<T, U>
partial struct ReadOnlySortedList<TElement> : IStrongEnumerable<TElement, CommonArrayEnumerator<TElement>>
{
    /// <inheritdoc/>
    public CommonArrayEnumerator<TElement> GetEnumerator() =>
        new(m_Items);
}

// IReadOnlyCollection<T>
partial struct ReadOnlySortedList<TElement> : IReadOnlyCollection<TElement>
{
    /// <inheritdoc/>
    public Int32 Count =>
        m_Items.Length;
}

// IReadOnlyList<T>
partial struct ReadOnlySortedList<TElement> : IReadOnlyList<TElement>
{
    /// <inheritdoc/>
    public TElement this[Int32 index] =>
        m_Items[index];
}