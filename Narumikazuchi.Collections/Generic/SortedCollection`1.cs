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
        where TEnumerator : IEnumerator<TElement> =>
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
        where TEnumerator : IEnumerator<TElement>
    {
        ArgumentNullException.ThrowIfNull(collection);
        ArgumentNullException.ThrowIfNull(comparer);

        if (collection is IReadOnlyCollection<TElement> roc)
        {
            SortedCollection<TElement> result = new(capacity: roc.Count,
                                                    comparer: comparer);
            TEnumerator enumerator = collection.GetEnumerator();
            while (enumerator.MoveNext())
            {
                result.Add(enumerator.Current);
            }
            return result;
        }
        else if (collection is ICollection<TElement> ct)
        {
            SortedCollection<TElement> result = new(capacity: ct.Count,
                                                    comparer: comparer);
            TEnumerator enumerator = collection.GetEnumerator();
            while (enumerator.MoveNext())
            {
                result.Add(enumerator.Current);
            }
            return result;
        }
        else if (collection is ICollection c)
        {
            SortedCollection<TElement> result = new(capacity: c.Count,
                                                    comparer: comparer);
            TEnumerator enumerator = collection.GetEnumerator();
            while (enumerator.MoveNext())
            {
                result.Add(enumerator.Current);
            }
            return result;
        }
        else
        {
            SortedCollection<TElement> result = new(capacity: 4,
                                                    comparer: comparer);
            TEnumerator enumerator = collection.GetEnumerator();
            while (enumerator.MoveNext())
            {
                result.Add(enumerator.Current);
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

// ICollection
partial class SortedCollection<TElement> : ICollection
{
    Boolean ICollection.IsSynchronized =>
        ((ICollection)m_Elements).IsSynchronized;

    Object ICollection.SyncRoot =>
        ((ICollection)m_Elements).SyncRoot;

    void ICollection.CopyTo(Array array,
                            Int32 index) =>
        ((ICollection)m_Elements).CopyTo(array: array,
                                         index: index);
}

// ICollection<T>
partial class SortedCollection<TElement> : ICollection<TElement>
{

    /// <inheritdoc/>
    public void Add(TElement item)
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
                return;
            }
        }
        m_Elements.Add(item);
    }

    /// <inheritdoc/>
    public void Clear() =>
        m_Elements.Clear();

    /// <inheritdoc/>
    public Boolean Contains(TElement item) =>
        m_Elements.Contains(item);

    /// <inheritdoc/>
    public void CopyTo(TElement[] array,
                       Int32 arrayIndex) =>
        m_Elements.CopyTo(array: array,
                          arrayIndex: arrayIndex);

    /// <inheritdoc/>
    public Boolean Remove(TElement item) =>
        m_Elements.Remove(item);

    /// <inheritdoc/>
    public Boolean IsReadOnly { get; } = false;
}

// IReadOnlyCollection<T>
partial class SortedCollection<TElement> : IReadOnlyCollection<TElement>
{
    /// <inheritdoc/>
    public Int32 Count =>
        m_Elements.Count;
}

// IStrongEnumerable<T, U>
partial class SortedCollection<TElement> : IStrongEnumerable<TElement, List<TElement>.Enumerator>
{
    /// <inheritdoc/>
    public List<TElement>.Enumerator GetEnumerator() =>
        m_Elements.GetEnumerator();
}