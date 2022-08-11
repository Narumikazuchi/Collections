namespace Narumikazuchi.Collections;

/// <summary>
/// Represents a strongly-typed collection of elements with a significantly faster enumerator to
/// improve iteration times for <see langword="foreach"/>-loops. The elements of this collection
/// can be accessed by index.
/// </summary>
/// <remarks>
/// The faster enumeration only works if the collection is used as-is or by using the
/// <see cref="IStrongEnumerable{TElement, TEnumerator}"/> interface. If you you plan on using 
/// the <see cref="IEnumerable{T}"/> or any other derivative interface in your code then the 
/// efficiency of the enumerator will be lost due to call virtualization in the compiler generated IL.
/// </remarks>
public readonly partial struct ReadOnlyList<TElement>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ReadOnlyList{TElement}"/> struct.
    /// </summary>
    public ReadOnlyList()
    {
        m_Items = Array.Empty<TElement>();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ReadOnlyList{TElement}"/> class.
    /// </summary>
    public static ReadOnlyList<TElement> Create() =>
        new();

    /// <summary>
    /// Initializes a new instance of the <see cref="ReadOnlyList{TElement}"/> struct.
    /// </summary>
    /// <param name="items">The items that the resulting collection shall hold.</param>
    /// <exception cref="ArgumentNullException" />
    public static ReadOnlyList<TElement> CreateFrom<TEnumerable>([DisallowNull] TEnumerable items)
        where TEnumerable : IEnumerable<TElement>
    {
        ArgumentNullException.ThrowIfNull(items);

        if (items is TElement[] array)
        {
            TElement[] elements = new TElement[array.Length];
            Array.Copy(sourceArray: array,
                       destinationArray: elements,
                       length: array.Length);
            return new(elements);
        }
        else if (items is ImmutableArray<TElement> immutableArray)
        {
            TElement[] elements = new TElement[immutableArray.Length];
            immutableArray.CopyTo(elements);
            return new(elements);
        }
        else if (items is List<TElement> list)
        {
            TElement[] elements = new TElement[list.Count];
            list.CopyTo(elements);
            return new(elements);
        }
        else if (items is ICollection<TElement> iCollectionT)
        {
            TElement[] elements = new TElement[iCollectionT.Count];
            iCollectionT.CopyTo(array: elements,
                                arrayIndex: 0);
            return new(elements);
        }
        else if (items is ICollection iCollection)
        {
            TElement[] elements = new TElement[iCollection.Count];
            iCollection.CopyTo(array: elements,
                               index: 0);
            return new(elements);
        }
        else if (items is IReadOnlyList<TElement> iReadOnlyList)
        {
            TElement[] elements = new TElement[iReadOnlyList.Count];
            Int32 index = 0;
            while (index < iReadOnlyList.Count)
            {
                elements[index] = iReadOnlyList[index++];
            }
            return new(elements);
        }
        else
        {
            return new(items.ToArray());
        }
    }
    /// <summary>
    /// Initializes a new instance of the <see cref="ReadOnlyList{TElement}"/> struct.
    /// </summary>
    /// <param name="items">The items that the resulting collection shall hold.</param>
    /// <exception cref="ArgumentNullException" />
    public static ReadOnlyList<TElement> CreateFrom<TEnumerable, TEnumerator>([DisallowNull] TEnumerable items)
        where TEnumerator : struct, IStrongEnumerator<TElement>
        where TEnumerable : IStrongEnumerable<TElement, TEnumerator>
    {
        ArgumentNullException.ThrowIfNull(items);

        if (items is ICollectionWithCount<TElement, TEnumerator> counted)
        {
            TElement[] elements = new TElement[counted.Count];
            if (counted is __IReadOnlyCollection<TElement> iReadOnlyCollectionT &&
                iReadOnlyCollectionT.TryGetReadOnlyArray(out TElement[]? array))
            {
                return new(array);
            }
            if (items is IReadOnlyCollection<TElement, TEnumerator> iReadOnlyCollection)
            {
                iReadOnlyCollection.CopyTo(elements);
                return new(elements);
            }

            Int32 index = 0;
            foreach (TElement element in items)
            {
                elements[index++] = element;
            }
            return new(elements);
        }
        else
        {
            List<TElement> list = new();
            foreach (TElement element in items)
            {
                list.Add(element);
            }
            return new(list.ToArray());
        }
    }

#pragma warning disable CS1591
    public static implicit operator ReadOnlyList<TElement>(TElement[] source)
    {
        TElement[] items = new TElement[source.Length];
        Array.Copy(sourceArray: source,
                   destinationArray: items,
                   length: source.Length);
        return new(items);
    }
    public static implicit operator ReadOnlyList<TElement>(in ImmutableArray<TElement> source)
    {
        TElement[] items = new TElement[source.Length];
        Array.Copy(sourceArray: source.ToArray(),
                   destinationArray: items,
                   length: source.Length);
        return new(items);
    }
    public static implicit operator ReadOnlyList<TElement>(List<TElement> source)
    {
        TElement[] items = new TElement[source.Count];
        Array.Copy(sourceArray: source.ToArray(),
                   destinationArray: items,
                   length: source.Count);
        return new(items);
    }
    public static implicit operator ReadOnlyList<TElement>(HashSet<TElement> source)
    {
        TElement[] items = new TElement[source.Count];
        Array.Copy(sourceArray: source.ToArray(),
                   destinationArray: items,
                   length: source.Count);
        return new(items);
    }
#pragma warning restore
}

// Non-Public
partial struct ReadOnlyList<TElement>
{
    internal ReadOnlyList(TElement[] items)
    {
        m_Items = items;
    }

    internal readonly TElement[] m_Items;
}

// IReadOnlyCollection<T>
partial struct ReadOnlyList<TElement> : ICollectionWithCount<TElement, CommonArrayEnumerator<TElement>>
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
partial struct ReadOnlyList<TElement> : IEnumerable
{
    IEnumerator IEnumerable.GetEnumerator() =>
        this.GetEnumerator();
}

// IEnumerable<T>
partial struct ReadOnlyList<TElement> : IEnumerable<TElement>
{
    IEnumerator<TElement> IEnumerable<TElement>.GetEnumerator() =>
        this.GetEnumerator();
}

// IReadOnlyList<T>
partial struct ReadOnlyList<TElement> : ICollectionWithReadIndexer<TElement, CommonArrayEnumerator<TElement>>
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
}

// IReadOnlyCollection<T>
partial struct ReadOnlyList<TElement> : IReadOnlyCollection<TElement, CommonArrayEnumerator<TElement>>
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
    public void CopyTo([DisallowNull] TElement[] array)
    {
        ArgumentNullException.ThrowIfNull(array);

        if (m_Items is not null)
        {
            Array.Copy(sourceArray: m_Items,
                       destinationArray: array,
                       length: m_Items.Length);
        }
    }
    /// <inheritdoc/>
    public void CopyTo([DisallowNull] TElement[] array,
                       Int32 destinationIndex)
    {
        ArgumentNullException.ThrowIfNull(array);
        destinationIndex.ThrowIfOutOfRange(0, Int32.MaxValue);

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

// IStrongEnumerable<T, U>
partial struct ReadOnlyList<TElement> : IStrongEnumerable<TElement, CommonArrayEnumerator<TElement>>
{
    /// <inheritdoc/>
    public CommonArrayEnumerator<TElement> GetEnumerator() =>
        new(m_Items ?? Array.Empty<TElement>());
}

// __IReadOnlyCollection<T>
partial struct ReadOnlyList<TElement> : __IReadOnlyCollection<TElement>
{
    Boolean __IReadOnlyCollection<TElement>.TryGetReadOnlyArray([NotNullWhen(true)] out TElement[]? array)
    {
        array = m_Items ?? Array.Empty<TElement>();
        return true;
    }

    Boolean __IReadOnlyCollection<TElement>.TryGetReadOnlyList([NotNullWhen(true)] out List<TElement>? list)
    {
        list = default;
        return false;
    }
}