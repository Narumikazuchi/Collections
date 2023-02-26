namespace Narumikazuchi.Collections;

public partial class SortedList<TElement, TComparer> : IHasCount
{
    /// <inheritdoc/>
    public Int32 Count
    {
        get
        {
            return m_Items.Count;
        }
    }
}