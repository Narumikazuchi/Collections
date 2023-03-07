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
public sealed partial class ReadOnlySortedList<TElement, TComparer> : BaseReadOnlyCollection<TElement, TElement[], CommonArrayEnumerator<TElement>>
    where TComparer : IComparer<TElement>
{
#pragma warning disable CS1591
    static public implicit operator ReadOnlyList<TElement>(in ReadOnlySortedList<TElement, TComparer> source)
    {
        TElement[] items = new TElement[source.Count];
        Array.Copy(sourceArray: source.m_Items,
                   destinationArray: items,
                   length: source.Count);
        return new(items);
    }
#pragma warning restore

    /// <summary>
    /// Initializes a new instance of the <see cref="ReadOnlySortedList{TElement, TComparer}"/> type.
    /// </summary>
    /// <param name="items">The items that the resulting collection shall hold.</param>
    /// <param name="comparer">The comparer that will be used to compare two instances of type <typeparamref name="TElement"/>.</param>
    /// <exception cref="ArgumentNullException" />
    public static ReadOnlySortedList<TElement, TComparer> CreateFrom<TEnumerable>([DisallowNull] TEnumerable items,
                                                                                  [DisallowNull] TComparer comparer)
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

        if (items is ReadOnlySortedList<TElement, TComparer> other)
        {
            return new(items: other.m_Items,
                       comparer: comparer);
        }
        else if (items is IReadOnlyCollection<TElement> iReadOnlyCollection)
        {
            return new(items: iReadOnlyCollection.OrderBy(x => x, (TComparer)comparer)
                                                 .ToArray(),
                       comparer: comparer);
        }
        else if (items is ICollection<TElement> iCollection)
        {
            return new(items: iCollection.OrderBy(x => x, (TComparer)comparer)
                                         .ToArray(),
                       comparer: comparer);
        }
        else
        {
            return new(items: items.OrderBy(x => x, (TComparer)comparer)
                                   .ToArray(),
                       comparer: comparer);
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
    static public readonly ReadOnlySortedList<TElement, Comparer<TElement>> Empty = new(items: Array.Empty<TElement>(),
                                                                                        comparer: Comparer<TElement>.Default);

    internal ReadOnlySortedList(TElement[] items,
                                TComparer comparer)
        : base(items)
    {
        this.Comparer = comparer;
    }
}