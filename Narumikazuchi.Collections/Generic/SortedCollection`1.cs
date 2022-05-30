namespace Narumikazuchi.Collections;

/// <summary>
/// Represents a strongly typed collection of sorted objects of type <typeparamref name="TElement"/>.
/// </summary>
public partial class SortedCollection<TElement>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SortedCollection{TElement}"/> class.
    /// </summary>
    /// <exception cref="ArgumentNullException"/>
    public static SortedCollection<TElement> Create() =>
        new(capacity: 4,
            comparer: Comparer<TElement>.Default);
    /// <summary>
    /// Initializes a new instance of the <see cref="SortedCollection{TElement}"/> class.
    /// </summary>
    /// <param name="capacity">The initial capacity of the collection.</param>
    /// <exception cref="ArgumentNullException"/>
    public static SortedCollection<TElement> Create(in Int32 capacity) =>
        new(capacity: capacity,
            comparer: Comparer<TElement>.Default);
    /// <summary>
    /// Initializes a new instance of the <see cref="SortedCollection{TElement}"/> class.
    /// </summary>
    /// <param name="comparer">The comparer that will be used to compare two instances of type <typeparamref name="TElement"/>.</param>
    /// <exception cref="ArgumentNullException"/>
    public static SortedCollection<TElement> Create([DisallowNull] IComparer<TElement> comparer) =>
        new(capacity: 4,
            comparer: comparer);
    /// <summary>
    /// Initializes a new instance of the <see cref="SortedCollection{TElement}"/> class.
    /// </summary>
    /// <param name="capacity">The initial capacity of the collection.</param>
    /// <param name="comparer">The comparer that will be used to compare two instances of type <typeparamref name="TElement"/>.</param>
    /// <exception cref="ArgumentNullException"/>
    public static SortedCollection<TElement> Create(in Int32 capacity,
                                                    [DisallowNull] IComparer<TElement> comparer) =>
        new(capacity: capacity,
            comparer: comparer);

    /// <summary>
    /// Initializes a new instance of the <see cref="SortedCollection{TElement}"/> class.
    /// </summary>
    /// <param name="collection">The collection of items that this collection will initially hold.</param>
    /// <exception cref="ArgumentNullException"/>
    public static SortedCollection<TElement> CreateFrom([DisallowNull] IEnumerable<TElement> collection) =>
        new(collection: collection,
            comparer: Comparer<TElement>.Default);
    /// <summary>
    /// Initializes a new instance of the <see cref="SortedCollection{TElement}"/> class.
    /// </summary>
    /// <param name="collection">The collection of items that this collection will initially hold.</param>
    /// <param name="comparer">The comparer that will be used to compare two instances of type <typeparamref name="TElement"/>.</param>
    /// <exception cref="ArgumentNullException"/>
    public static SortedCollection<TElement> CreateFrom([DisallowNull] IEnumerable<TElement> collection,
                                                        [DisallowNull] IComparer<TElement> comparer) =>
        new(collection: collection,
            comparer: comparer);
    /// <summary>
    /// Initializes a new instance of the <see cref="SortedCollection{TElement}"/> class.
    /// </summary>
    /// <param name="collection">The collection of items that this collection will initially hold.</param>
    /// <exception cref="ArgumentNullException"/>
    public static SortedCollection<TElement> CreateFrom<TEnumerator>([DisallowNull] IStrongEnumerable<TElement, TEnumerator> collection)
        where TEnumerator : struct, IStrongEnumerator<TElement> =>
            CreateFrom(collection: collection,
                       comparer: Comparer<TElement>.Default);
    /// <summary>
    /// Initializes a new instance of the <see cref="SortedCollection{TElement}"/> class.
    /// </summary>
    /// <param name="collection">The collection of items that this collection will initially hold.</param>
    /// <param name="comparer">The comparer that will be used to compare two instances of type <typeparamref name="TElement"/>.</param>
    /// <exception cref="ArgumentNullException"/>
    public static SortedCollection<TElement> CreateFrom<TEnumerator>([DisallowNull] IStrongEnumerable<TElement, TEnumerator> collection,
                                                                     [DisallowNull] IComparer<TElement> comparer)
        where TEnumerator : struct, IStrongEnumerator<TElement>
    {
        ArgumentNullException.ThrowIfNull(collection);
        ArgumentNullException.ThrowIfNull(comparer);

        if (collection is ICollectionWithCount<TElement, TEnumerator> counted)
        {
            SortedCollection<TElement> result = new(capacity: counted.Count,
                                                    comparer: comparer);
            foreach (TElement element in collection)
            {
                result.Add(element);
            }
            return result;
        }
        else
        {
            SortedCollection<TElement> result = new(capacity: 4,
                                                    comparer: comparer);
            foreach (TElement element in collection)
            {
                result.Add(element);
            }
            return result;
        }
    }
}

// Non-Public
partial class SortedCollection<TElement>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SortedCollection{TElement}"/> class.
    /// </summary>
    /// <param name="capacity">The initial capacity of the collection.</param>
    /// <param name="comparer">The comparer that will be used to compare two instances of type <typeparamref name="TElement"/>.</param>
    /// <exception cref="ArgumentNullException"/>
    protected SortedCollection(in Int32 capacity,
                               [DisallowNull] IComparer<TElement> comparer)
    {
        ArgumentNullException.ThrowIfNull(comparer);

        m_Elements = new(capacity);
        m_Comparer = comparer;
    }
    /// <summary>
    /// Initializes a new instance of the <see cref="SortedCollection{TElement}"/> class.
    /// </summary>
    /// <param name="collection">The collection of items that this collection will initially hold.</param>
    /// <param name="comparer">The comparer that will be used to compare two instances of type <typeparamref name="TElement"/>.</param>
    /// <exception cref="ArgumentNullException"/>
    protected SortedCollection([DisallowNull] IEnumerable<TElement> collection,
                               [DisallowNull] IComparer<TElement> comparer)
    {
        ArgumentNullException.ThrowIfNull(collection);
        ArgumentNullException.ThrowIfNull(comparer);

        m_Elements = new(collection);
        m_Elements.Sort(comparer);
        m_Comparer = comparer;
    }

    internal readonly List<TElement> m_Elements;
    internal readonly IComparer<TElement> m_Comparer;
}

// IReadOnlyCollection<T>
partial class SortedCollection<TElement> : ICollectionWithCount<TElement, CommonListEnumerator<TElement>>
{
    /// <inheritdoc/>
    public Int32 Count =>
        m_Elements.Count;
}

// IEnumerable
partial class SortedCollection<TElement> : IEnumerable
{
    IEnumerator IEnumerable.GetEnumerator() =>
        this.GetEnumerator();
}

// IEnumerable<T>
partial class SortedCollection<TElement> : IEnumerable<TElement>
{
    IEnumerator<TElement> IEnumerable<TElement>.GetEnumerator() =>
        this.GetEnumerator();
}

// ICollection<T>
partial class SortedCollection<TElement> : IModifyableCollection<TElement, CommonListEnumerator<TElement>>
{
    /// <inheritdoc/>
    public Boolean Add(TElement item)
    {
        for (Int32 i = 0;
             i < m_Elements.Count;
             i++)
        {
            if (m_Comparer.Compare(x: m_Elements[i],
                                   y: item) < 0)
            {
                m_Elements.Insert(index: i,
                                  item: item);
                return true;
            }
        }
        m_Elements.Add(item);
        return true;
    }

    /// <inheritdoc/>
    public void AddRange<TEnumerator>([DisallowNull] IStrongEnumerable<TElement, TEnumerator> enumerable)
        where TEnumerator : struct, IStrongEnumerator<TElement>
    {
        ArgumentNullException.ThrowIfNull(enumerable);

        foreach (TElement item in enumerable)
        {
            this.Add(item);
        }
    }

    /// <inheritdoc/>
    public void Clear() =>
        m_Elements.Clear();

    /// <inheritdoc/>
    public Boolean Remove(TElement item) =>
        m_Elements.Remove(item);
}

// IReadOnlyCollection<T>
partial class SortedCollection<TElement> : IReadOnlyCollection<TElement, CommonListEnumerator<TElement>>
{
    /// <inheritdoc/>
    public Boolean Contains(TElement item) =>
        m_Elements.Contains(item);

    /// <inheritdoc/>
    public void CopyTo(TElement[] array) =>
        m_Elements.CopyTo(array: array);
    /// <inheritdoc/>
    public void CopyTo(TElement[] array,
                       Int32 arrayIndex) =>
        m_Elements.CopyTo(array: array,
                          arrayIndex: arrayIndex);
}

// IStrongEnumerable<T, U>
partial class SortedCollection<TElement> : IStrongEnumerable<TElement, CommonListEnumerator<TElement>>
{
    /// <inheritdoc/>
    public CommonListEnumerator<TElement> GetEnumerator() =>
        new(m_Elements);
}