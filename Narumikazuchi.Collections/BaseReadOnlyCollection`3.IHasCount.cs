namespace Narumikazuchi.Collections;

public partial class BaseReadOnlyCollection<TElement, TCollection, TEnumerator> : IHasCount
{
    /// <inheritdoc/>
    public Int32 Count
    {
        get
        {
            return m_SectionEnd - m_SectionStart;
        }
    }
}