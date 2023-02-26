namespace Narumikazuchi.Collections;

public partial class SortedList<TElement, TComparer>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SortedList{TElement, TComparer}"/> class.
    /// </summary>
    /// <param name="items">The items that the resulting collection shall hold.</param>
    /// <param name="comparer">The comparer used for sorting.</param>
    /// <exception cref="ArgumentNullException" />
    protected SortedList(NotNull<List<TElement>> items,
                         NotNull<TComparer> comparer)
    {
        m_Items = items;
        this.Comparer = comparer;
    }

    internal readonly List<TElement> m_Items;
}