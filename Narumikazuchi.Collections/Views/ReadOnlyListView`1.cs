namespace Narumikazuchi.Collections;

/// <summary>
/// Represents a strongly-typed view of a collection of elements with a significantly faster enumerator to
/// improve iteration times for <see langword="foreach"/>-loops.
/// </summary>
/// <remarks>
/// The faster enumeration only works if the collection is used as-is or by using the
/// <see cref="IStrongEnumerable{TElement, TEnumerator}"/> interface. If you you plan on using 
/// any derivative of the <see cref="IEnumerable{T}"/> interface (i.e. <see cref="ICollection{T}"/>) 
/// in your code then the efficiency of the enumerator will be lost due to call virtualization in 
/// the compiler generated IL.
/// </remarks>
public readonly partial struct ReadOnlyListView<TElement>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ReadOnlyListView{TElement}"/> struct.
    /// </summary>
    public ReadOnlyListView()
    {
        m_Items = Array.Empty<TElement>();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ReadOnlyListView{TElement}"/> struct.
    /// </summary>
    public static ReadOnlyListView<TElement> Create() =>
        new();

    /// <summary>
    /// Initializes a new instance of the <see cref="ReadOnlyListView{TElement}"/> struct.
    /// </summary>
    /// <param name="items">The items that the resulting view should expose.</param>
    /// <exception cref="ArgumentNullException" />
    public static ReadOnlyListView<TElement> CreateFrom<TEnumerable>(
#if NETCOREAPP3_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        [DisallowNull]
#endif
        TEnumerable items)
            where TEnumerable : IReadOnlyList<TElement>
    {
#if NET6_0_OR_GREATER
        ArgumentNullException.ThrowIfNull(items);
#else
        if (items is null)
        {
            throw new ArgumentNullException(nameof(items));
        }
#endif

        return new(items);
    }

#pragma warning disable CS1591
    public static implicit operator ReadOnlyListView<TElement>(TElement[] source)
    {
        return new(source);
    }
#if NETCOREAPP3_1_OR_GREATER
    public static implicit operator ReadOnlyListView<TElement>(ImmutableArray<TElement> source)
    {
        return new(source);
    }
#endif
    public static implicit operator ReadOnlyListView<TElement>(List<TElement> source)
    {
        return new(source);
    }
#pragma warning restore
}

// Non-Public
partial struct ReadOnlyListView<TElement>
{
    internal ReadOnlyListView(IReadOnlyList<TElement> items)
    {
        m_Items = items;
    }

    internal readonly IReadOnlyList<TElement> m_Items;
}

// ICollectionWithCount<T, U>
partial struct ReadOnlyListView<TElement> : ICollectionWithCount<TElement, ReadOnlyListView<TElement>.Enumerator>
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
                return m_Items.Count;
            }
        }
    }
}

// IEnumerable
partial struct ReadOnlyListView<TElement> : IEnumerable
{
    IEnumerator IEnumerable.GetEnumerator() =>
        this.GetEnumerator();
}

// IEnumerable<T>
partial struct ReadOnlyListView<TElement> : IEnumerable<TElement>
{
    IEnumerator<TElement> IEnumerable<TElement>.GetEnumerator() =>
        this.GetEnumerator();
}

// IReadOnlyCollection<T, U>
partial struct ReadOnlyListView<TElement> : IReadOnlyCollection<TElement, ReadOnlyListView<TElement>.Enumerator>
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
            return m_Items.Contains(element);
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
            if (m_Items is TElement[] sourceArray)
            {
                Array.Copy(sourceArray: sourceArray,
                           destinationArray: array,
                           length: m_Items.Count);
            }
            else if (m_Items is List<TElement> list)
            {
                list.CopyTo(array);
            }
            else if (m_Items is ICollection<TElement> collectionT)
            {
                collectionT.CopyTo(array: array,
                                   arrayIndex: 0);
            }
            else if (m_Items is ICollection collection)
            {
                collection.CopyTo(array: array,
                                  index: 0);
            }
            else
            {
                Array.Copy(sourceArray: m_Items.ToArray(),
                           destinationArray: array,
                           length: m_Items.Count);
            }
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
            if (m_Items is TElement[] sourceArray)
            {
                Array.Copy(sourceArray: sourceArray,
                           sourceIndex: 0,
                           destinationArray: array,
                           destinationIndex: destinationIndex,
                           length: m_Items.Count);
            }
            else if (m_Items is List<TElement> list)
            {
                list.CopyTo(array: array,
                            index: 0,
                            arrayIndex: destinationIndex,
                            count: list.Count);
            }
            else if (m_Items is ICollection<TElement> collectionT)
            {
                collectionT.CopyTo(array: array,
                                   arrayIndex: destinationIndex);
            }
            else if (m_Items is ICollection collection)
            {
                collection.CopyTo(array: array,
                                  index: destinationIndex);
            }
            else
            {
                Array.Copy(sourceArray: m_Items.ToArray(),
                           sourceIndex: 0,
                           destinationArray: array,
                           destinationIndex: destinationIndex,
                           length: m_Items.Count);
            }
        }
    }
}

// IStrongEnumerable<T, U>
partial struct ReadOnlyListView<TElement> : IStrongEnumerable<TElement, ReadOnlyListView<TElement>.Enumerator>
{
    /// <inheritdoc/>
    public ReadOnlyListView<TElement>.Enumerator GetEnumerator() =>
        new(m_Items ?? Array.Empty<TElement>());
}

// __IReadOnlyCollection<T>
partial struct ReadOnlyListView<TElement> : __IReadOnlyCollection<TElement>
{
    Boolean __IReadOnlyCollection<TElement>.TryGetReadOnlyArray(
#if NETCOREAPP3_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        [NotNullWhen(true)]
#endif
        out TElement[]? array)
    {
        array = m_Items as TElement[] ?? default;
        return m_Items is TElement[];
    }

    Boolean __IReadOnlyCollection<TElement>.TryGetReadOnlyList(
#if NETCOREAPP3_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        [NotNullWhen(true)]
#endif
        out List<TElement>? list)
    {
        list = m_Items as List<TElement> ?? default;
        return m_Items is List<TElement>;
    }
}

// Enumerator
partial struct ReadOnlyListView<TElement>
{
    /// <summary>
    /// An enumerator that iterates through the <see cref="ReadOnlyListView{TElement}"/>.
    /// </summary>
    public struct Enumerator :
        IStrongEnumerable<TElement, ReadOnlyListView<TElement>.Enumerator>,
        IStrongEnumerator<TElement>,
        IEnumerator<TElement>
    {
        /// <summary>
        /// The default constructor for the <see cref="Enumerator"/> is not allowed.
        /// </summary>
        /// <exception cref="NotAllowed"></exception>
        public Enumerator()
        {
            throw new NotAllowed();
        }
        internal Enumerator(IReadOnlyList<TElement> source)
        {
            m_Elements = source;
            m_Index = -1;
        }

        /// <inheritdoc/>
        public Boolean MoveNext() =>
            ++m_Index < m_Elements.Count;

        /// <inheritdoc/>
        public Enumerator GetEnumerator()
        {
            if (m_Index == -1)
            {
                return this;
            }
            else
            {
                return new(m_Elements);
            }
        }

#if !NETCOREAPP3_1_OR_GREATER
        void IDisposable.Dispose()
        { }

        void IEnumerator.Reset()
        { }

        Object? IEnumerator.Current =>
            this.Current;

        IEnumerator IEnumerable.GetEnumerator() =>
            this.GetEnumerator();
        IEnumerator<TElement> IEnumerable<TElement>.GetEnumerator() =>
            this.GetEnumerator();
#endif

        /// <inheritdoc/>
        public TElement Current =>
            m_Elements[m_Index];

        private readonly IReadOnlyList<TElement> m_Elements;
        private Int32 m_Index;
    }
}