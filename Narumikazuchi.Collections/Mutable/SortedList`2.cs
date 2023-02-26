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
    static public SortedList<TElement, TComparer> CreateFrom<TEnumerable>(
#if NETCOREAPP3_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        [DisallowNull]
#endif
        NotNull<TEnumerable> items,
#if NETCOREAPP3_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        [DisallowNull]
#endif
        NotNull<TComparer> comparer)
            where TEnumerable : IEnumerable<TElement>
    {
        return new(items: new List<TElement>((TEnumerable)items),
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
    public SortedList(NotNull<TComparer> comparer)
    {
        m_Items = new();
        this.Comparer = comparer;
    }
}