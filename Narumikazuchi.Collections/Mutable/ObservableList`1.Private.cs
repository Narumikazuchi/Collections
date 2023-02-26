namespace Narumikazuchi.Collections;

public partial class ObservableList<TElement>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ObservableList{TElement}"/> class.
    /// </summary>
    /// <param name="items">The items that the resulting collection shall hold.</param>
    /// <exception cref="ArgumentNullException" />
    protected ObservableList(NotNull<List<TElement>> items)
    {
        m_Items = items;
    }

    internal readonly List<TElement> m_Items;
}