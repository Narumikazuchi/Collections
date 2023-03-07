namespace Narumikazuchi.Collections;

/// <summary>
/// Represents a strongly typed list of objects of type <typeparamref name="TElement"/>, which reports changes and can be accessed by index. 
/// </summary>
public partial class SortedList<TElement, TComparer> : StrongEnumerable<TElement, CommonListEnumerator<TElement>>
    where TComparer : IComparer<TElement>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SortedList{TElement, TComparer}"/> class.
    /// </summary>
    /// <param name="items">The items that the resulting collection shall hold.</param>
    /// <param name="comparer">The comparer used for sorting.</param>
    /// <exception cref="ArgumentNullException" />
    static public SortedList<TElement, TComparer> CreateFrom<TEnumerable>([DisallowNull] TEnumerable items,
                                                                          [DisallowNull] TComparer comparer)
        where TEnumerable : IEnumerable<TElement>
    {
#if NET6_0_OR_GREATER
        ArgumentNullException.ThrowIfNull(items);
        ArgumentNullException.ThrowIfNull(comparer);
#else
        if (items is null)
        {
            throw new ArgumentNullException(nameof(items));
        }
        
        if (comparer is null)
        {
            throw new ArgumentNullException(nameof(comparer));
        }
#endif

        return new(items: new List<TElement>(items),
                   comparer: comparer);
    }

    /// <inheritdoc/>
    public sealed override CommonListEnumerator<TElement> GetEnumerator()
    {
        return new(m_Items);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SortedList{TElement, TComparer}"/> class.
    /// </summary>
    public SortedList([DisallowNull] TComparer comparer)
    {
#if NET6_0_OR_GREATER
        ArgumentNullException.ThrowIfNull(comparer);
#else
        if (comparer is null)
        {
            throw new ArgumentNullException(nameof(comparer));
        }
#endif

        m_Items = new();
        this.Comparer = comparer;
    }
}