namespace Narumikazuchi.Collections;

/// <summary>
/// Represents a strongly typed collection of sorted objects of type <typeparamref name="TElement"/>.
/// </summary>
public partial class SortedCollection<TElement, TComparer>
    where TComparer : IComparer<TElement>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SortedCollection{TElement, TComparer}"/> class.
    /// </summary>
    /// <param name="capacity">The initial capacity of the collection.</param>
    /// <param name="comparer">The comparer that will be used to compare two instances of type <typeparamref name="TElement"/>.</param>
    /// <exception cref="ArgumentNullException"/>
    public static SortedCollection<TElement, TComparer> Create(in Int32 capacity,
#if NETCOREAPP3_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        [DisallowNull]
#endif
        TComparer comparer) =>
            new(capacity: capacity,
                comparer: comparer);

    /// <summary>
    /// Initializes a new instance of the <see cref="SortedCollection{TElement, TComparer}"/> class.
    /// </summary>
    /// <param name="collection">The collection of items that this collection will initially hold.</param>
    /// <param name="comparer">The comparer that will be used to compare two instances of type <typeparamref name="TElement"/>.</param>
    /// <exception cref="ArgumentNullException"/>
    public static SortedCollection<TElement, TComparer> CreateFrom<TEnumerable>(
#if NETCOREAPP3_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        [DisallowNull]
#endif
        TEnumerable collection,
#if NETCOREAPP3_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        [DisallowNull]
#endif
        TComparer comparer)
            where TEnumerable : IEnumerable<TElement>
    {
#if NET6_0_OR_GREATER
        ArgumentNullException.ThrowIfNull(collection);
        ArgumentNullException.ThrowIfNull(comparer);
#else
        if (collection is null)
        {
            throw new ArgumentNullException(nameof(collection));
        }

        if (comparer is null)
        {
            throw new ArgumentNullException(nameof(comparer));
        }
#endif

        List<TElement> elements = new(collection);
        return new(collection: elements,
                   comparer: comparer);
    }
    /// <summary>
    /// Initializes a new instance of the <see cref="SortedCollection{TElement, TComparer}"/> class.
    /// </summary>
    /// <param name="collection">The collection of items that this collection will initially hold.</param>
    /// <param name="comparer">The comparer that will be used to compare two instances of type <typeparamref name="TElement"/>.</param>
    /// <exception cref="ArgumentNullException"/>
    public static SortedCollection<TElement, TComparer> CreateFrom<TEnumerable, TEnumerator>(
#if NETCOREAPP3_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        [DisallowNull]
#endif
        TEnumerable collection,
#if NETCOREAPP3_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        [DisallowNull]
#endif
        TComparer comparer)
            where TEnumerator : struct, IStrongEnumerator<TElement>
            where TEnumerable : IStrongEnumerable<TElement, TEnumerator>
    {
#if NET6_0_OR_GREATER
        ArgumentNullException.ThrowIfNull(collection);
        ArgumentNullException.ThrowIfNull(comparer);
#else
        if (collection is null)
        {
            throw new ArgumentNullException(nameof(collection));
        }

        if (comparer is null)
        {
            throw new ArgumentNullException(nameof(comparer));
        }
#endif

        if (collection is ICollectionWithCount<TElement, TEnumerator> counted)
        {
            SortedCollection<TElement, TComparer> result = new(capacity: counted.Count,
                                                               comparer: comparer);
            foreach (TElement element in collection)
            {
                result.Add(element);
            }
            return result;
        }
        else
        {
            SortedCollection<TElement, TComparer> result = new(capacity: 4,
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
partial class SortedCollection<TElement, TComparer>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SortedCollection{TElement, TComparer}"/> class.
    /// </summary>
    /// <param name="capacity">The initial capacity of the collection.</param>
    /// <param name="comparer">The comparer that will be used to compare two instances of type <typeparamref name="TElement"/>.</param>
    /// <exception cref="ArgumentNullException"/>
    protected SortedCollection(in Int32 capacity,
#if NETCOREAPP3_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        [DisallowNull]
#endif
        TComparer comparer)
    {
#if NET6_0_OR_GREATER
        ArgumentNullException.ThrowIfNull(comparer);
#else
        if (comparer is null)
        {
            throw new ArgumentNullException(nameof(comparer));
        }
#endif

        m_Elements = new(capacity);
        this.Comparer = comparer;
    }
    /// <summary>
    /// Initializes a new instance of the <see cref="SortedCollection{TElement, TComparer}"/> class.
    /// </summary>
    /// <param name="collection">The collection of items that this collection will initially hold.</param>
    /// <param name="comparer">The comparer that will be used to compare two instances of type <typeparamref name="TElement"/>.</param>
    /// <exception cref="ArgumentNullException"/>
    protected SortedCollection(
#if NETCOREAPP3_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        [DisallowNull]
#endif
        List<TElement> collection,
#if NETCOREAPP3_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        [DisallowNull]
#endif
        TComparer comparer)
    {
#if NET6_0_OR_GREATER
        ArgumentNullException.ThrowIfNull(collection);
        ArgumentNullException.ThrowIfNull(comparer);
#else
        if (collection is null)
        {
            throw new ArgumentNullException(nameof(collection));
        }

        if (comparer is null)
        {
            throw new ArgumentNullException(nameof(comparer));
        }
#endif

        m_Elements = collection;
        this.Comparer = comparer;
        m_Elements.Sort(comparer);
    }

    internal readonly List<TElement> m_Elements;
}

// IReadOnlyCollection<T>
partial class SortedCollection<TElement, TComparer> : ICollectionWithCount<TElement, CommonListEnumerator<TElement>>
{
    /// <inheritdoc/>
    public Int32 Count =>
        m_Elements.Count;
}

// IEnumerable
partial class SortedCollection<TElement, TComparer> : IEnumerable
{
    IEnumerator IEnumerable.GetEnumerator() =>
        this.GetEnumerator();
}

// IEnumerable<T>
partial class SortedCollection<TElement, TComparer> : IEnumerable<TElement>
{
    IEnumerator<TElement> IEnumerable<TElement>.GetEnumerator() =>
        this.GetEnumerator();
}

// ICollection<T>
partial class SortedCollection<TElement, TComparer> : IModifyableCollection<TElement, CommonListEnumerator<TElement>>
{
    /// <inheritdoc/>
    public Boolean Add(TElement item)
    {
        for (Int32 i = 0;
             i < m_Elements.Count;
             i++)
        {
            if (this.Comparer.Compare(x: m_Elements[i],
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
    public void AddRange<TEnumerable, TEnumerator>(
#if NETCOREAPP3_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        [DisallowNull]
#endif
        TEnumerable enumerable)
            where TEnumerator : struct, IStrongEnumerator<TElement>
            where TEnumerable : IStrongEnumerable<TElement, TEnumerator>
    {
#if NET6_0_OR_GREATER
        ArgumentNullException.ThrowIfNull(enumerable);
#else
        if (enumerable is null)
        {
            throw new ArgumentNullException(nameof(enumerable));
        }
#endif

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
partial class SortedCollection<TElement, TComparer> : IReadOnlyCollection<TElement, CommonListEnumerator<TElement>>
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
partial class SortedCollection<TElement, TComparer> : ISortedCollection<TElement, CommonListEnumerator<TElement>, TComparer>
{
    /// <inheritdoc/>
    public TComparer Comparer { get; }
}

// IStrongEnumerable<T, U>
partial class SortedCollection<TElement, TComparer> : IStrongEnumerable<TElement, CommonListEnumerator<TElement>>
{
    /// <inheritdoc/>
    public CommonListEnumerator<TElement> GetEnumerator() =>
        new(m_Elements);
}