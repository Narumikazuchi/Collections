namespace Narumikazuchi.Collections;

/// <summary>
/// Represents a strongly-typed collection of elements with a significantly faster enumerator to
/// improve iteration times for <see langword="foreach"/>-loops.
/// </summary>
/// <remarks>
/// The faster enumeration only works if the collection is used as-is or by using the
/// <see cref="IStrongEnumerable{TElement, TEnumerator}"/> interface. If you you plan on using 
/// the <see cref="IEnumerable{T}"/> or any other derivative interface in your code then the 
/// efficiency of the enumerator will be lost due to call virtualization in the compiler generated IL.
/// </remarks>
public readonly partial struct ReadOnlyCollection<TElement>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ReadOnlyCollection{TElement}"/> struct.
    /// </summary>
    public ReadOnlyCollection()
    {
        m_Items = Array.Empty<TElement>();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ReadOnlyCollection{TElement}"/> class.
    /// </summary>
    public static ReadOnlyCollection<TElement> Create() =>
        new();

    /// <summary>
    /// Initializes a new instance of the <see cref="ReadOnlyCollection{TElement}"/> struct.
    /// </summary>
    /// <param name="items">The items that the resulting collection shall hold.</param>
    /// <exception cref="ArgumentNullException" />
    public static ReadOnlyCollection<TElement> CreateFrom([DisallowNull] IEnumerable<TElement> items)
    {
        ArgumentNullException.ThrowIfNull(items);

        if (items is TElement[] a)
        {
            TElement[] elements = new TElement[a.Length];
            Array.Copy(sourceArray: a,
                       destinationArray: elements,
                       length: a.Length);
            return new(elements);
        }
        else if (items is ImmutableArray<TElement> ia)
        {
            TElement[] elements = new TElement[ia.Length];
            Int32 index = 0;
            while (index < ia.Length)
            {
                elements[index] = ia[index++];
            }
            return new(elements);
        }
        else if (items is List<TElement> list)
        {
            return new(list.ToArray());
        }
        else if (items is IReadOnlyList<TElement> rol)
        {
            TElement[] elements = new TElement[rol.Count];
            Int32 index = 0;
            while (index < rol.Count)
            {
                elements[index] = rol[index++];
            }
            return new(elements);
        }
        else if (items is IList<TElement> l)
        {
            TElement[] elements = new TElement[l.Count];
            Int32 index = 0;
            while (index < l.Count)
            {
                elements[index] = l[index++];
            }
            return new(elements);
        }
        else
        {
            return new(items.ToArray());
        }
    }
    /// <summary>
    /// Initializes a new instance of the <see cref="ReadOnlyCollection{TElement}"/> struct.
    /// </summary>
    /// <param name="items">The items that the resulting collection shall hold.</param>
    /// <exception cref="ArgumentNullException" />
    public static ReadOnlyCollection<TElement> CreateFrom<TEnumerator>([DisallowNull] IStrongEnumerable<TElement, TEnumerator> items)
        where TEnumerator : struct, IStrongEnumerator<TElement>
    {
        ArgumentNullException.ThrowIfNull(items);

        if (items is ReadOnlyCollection<TElement> readOnlyCollection)
        {
            TElement[] elements = new TElement[readOnlyCollection.Count];
            Array.Copy(sourceArray: readOnlyCollection.m_Items,
                       destinationArray: elements,
                       length: readOnlyCollection.Count);
            return new(elements);
        }
        else if (items is ReadOnlySortedCollection<TElement> readOnlySortedCollection)
        {
            TElement[] elements = new TElement[readOnlySortedCollection.Count];
            Array.Copy(sourceArray: readOnlySortedCollection.m_Items,
                       destinationArray: elements,
                       length: readOnlySortedCollection.Count);
            return new(elements);
        }
        else if (items is ReadOnlyList<TElement> readOnlyList)
        {
            TElement[] elements = new TElement[readOnlyList.Count];
            Array.Copy(sourceArray: readOnlyList.m_Items,
                       destinationArray: elements,
                       length: readOnlyList.Count);
            return new(elements);
        }
        else if (items is ReadOnlySortedList<TElement> readOnlySortedList)
        {
            TElement[] elements = new TElement[readOnlySortedList.Count];
            Array.Copy(sourceArray: readOnlySortedList.m_Items,
                       destinationArray: elements,
                       length: readOnlySortedList.Count);
            return new(elements);
        }
        else if (items is IReadOnlyCollection<TElement> roc)
        {
            TElement[] elements = new TElement[roc.Count];
            TEnumerator enumerator = items.GetEnumerator();
            Int32 index = 0;
            while (enumerator.MoveNext())
            {
                elements[index++] = enumerator.Current;
            }
            return new(elements);
        }
        else if (items is ICollection<TElement> c)
        {
            TElement[] elements = new TElement[c.Count];
            TEnumerator enumerator = items.GetEnumerator();
            Int32 index = 0;
            while (enumerator.MoveNext())
            {
                elements[index++] = enumerator.Current;
            }
            return new(elements);
        }
        else
        {
            List<TElement> list = new();
            TEnumerator enumerator = items.GetEnumerator();
            while (enumerator.MoveNext())
            {
                list.Add(enumerator.Current);
            }
            return new(list.ToArray());
        }
    }
}

// Non-Public
partial struct ReadOnlyCollection<TElement>
{
    internal ReadOnlyCollection(TElement[] items)
    {
        m_Items = items;
    }

    internal readonly TElement[] m_Items;
}

// IStrongEnumerable<T, U>
partial struct ReadOnlyCollection<TElement> : IStrongEnumerable<TElement, CommonArrayEnumerator<TElement>>
{
    /// <inheritdoc/>
    public CommonArrayEnumerator<TElement> GetEnumerator() =>
        new(m_Items);
}

// IReadOnlyCollection<T>
partial struct ReadOnlyCollection<TElement> : IReadOnlyCollection<TElement>
{
    /// <inheritdoc/>
    public Int32 Count =>
        m_Items.Length;
}