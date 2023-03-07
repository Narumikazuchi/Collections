namespace Narumikazuchi.Collections;

/// <summary>
/// Represents a strongly-typed readonly collection of elements with a significantly faster enumerator to
/// improve iteration times for <see langword="foreach"/>-loops.
/// </summary>
/// <remarks>
/// The faster enumeration only works if the collection is used as-is or by using the
/// <see cref="IStrongEnumerable{TElement, TEnumerator}"/> interface. If you you plan on using 
/// any derivative of the <see cref="IEnumerable{T}"/> interface (i.e. <see cref="ICollection{T}"/>) 
/// in your code then the efficiency of the enumerator will be lost due to call virtualization in 
/// the compiler generated IL.
/// </remarks>
public sealed partial class ReadOnlyList<TElement> : BaseReadOnlyCollection<TElement, TElement[], CommonArrayEnumerator<TElement>>
{
#pragma warning disable CS1591 // Missing comments
    static public implicit operator ReadOnlyList<TElement>(TElement[] source)
    {
        TElement[] items = new TElement[source.Length];
        Array.Copy(sourceArray: source,
                   destinationArray: items,
                   length: source.Length);
        return new(items);
    }

#if NET6_0_OR_GREATER
    static public implicit operator ReadOnlyList<TElement>(ImmutableArray<TElement> source)
    {
        TElement[] items = new TElement[source.Length];
        Array.Copy(sourceArray: source.ToArray(),
                   destinationArray: items,
                   length: source.Length);
        return new(items);
    }

#endif

    static public implicit operator ReadOnlyList<TElement>(List<TElement> source)
    {
        TElement[] items = new TElement[source.Count];
        Array.Copy(sourceArray: source.ToArray(),
                   destinationArray: items,
                   length: source.Count);
        return new(items);
    }

    static public implicit operator ReadOnlyList<TElement>(HashSet<TElement> source)
    {
        TElement[] items = new TElement[source.Count];
        Array.Copy(sourceArray: source.ToArray(),
                   destinationArray: items,
                   length: source.Count);
        return new(items);
    }

    static public implicit operator ReadOnlySortedList<TElement, Comparer<TElement>>(ReadOnlyList<TElement> source)
    {
        TElement[] items = new TElement[source.Count];
        Array.Copy(sourceArray: source.ToArray(),
                   destinationArray: items,
                   length: source.Count);
        return new(items: items,
                   comparer: Comparer<TElement>.Default);
    }
#pragma warning restore

    /// <summary>
    /// Initializes a new instance of the <see cref="ReadOnlyList{TElement}"/> type.
    /// </summary>
    /// <param name="items">The items that the resulting collection shall hold.</param>
    /// <exception cref="ArgumentNullException" />
    static public ReadOnlyList<TElement> CreateFrom<TEnumerable>([DisallowNull] TEnumerable items)
        where TEnumerable : IEnumerable<TElement>
    {
#if NET6_0_OR_GREATER
        ArgumentNullException.ThrowIfNull(items);
#else
        if (items is null)
        {
            throw new ArgumentNullException(nameof(items));
        }
#endif

        if (items is TElement[] array)
        {
            TElement[] elements = new TElement[array.Length];
            Array.Copy(sourceArray: array,
                       destinationArray: elements,
                       length: array.Length);
            return new(elements);
        }
#if NET6_0_OR_GREATER
        else if (items is ImmutableArray<TElement> immutableArray)
        {
            TElement[] elements = new TElement[immutableArray.Length];
            immutableArray.CopyTo(elements);
            return new(elements);
        }
#endif
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
        else if (items is IHasCount counted)
        {
            TElement[] elements = new TElement[counted.Count];
            Int32 index = 0;
            foreach (TElement element in items)
            {
                elements[index++] = element;
            }

            return new(elements);
        }
        else
        {
            return new(items.ToArray());
        }
    }

    /// <inheritdoc/>
    public sealed override CommonArrayEnumerator<TElement> GetEnumerator()
    {
        return new(m_Items);
    }

    /// <summary>
    /// Represents an empty <see cref="ReadOnlyList{TElement}"/>.
    /// </summary>
    static public readonly ReadOnlyList<TElement> Empty = new(Array.Empty<TElement>());

    internal ReadOnlyList(TElement[] items)
        : base(items)
    { }
}