namespace Narumikazuchi.Collections;

public partial class ObservableList<TElement> : IHasCount
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