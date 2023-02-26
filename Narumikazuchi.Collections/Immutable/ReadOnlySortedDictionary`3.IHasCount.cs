namespace Narumikazuchi.Collections;

public partial class ReadOnlySortedDictionary<TKey, TValue, TEqualityComparer> : IHasCount
{
    /// <inheritdoc/>
    public Int32 Count
    {
        get
        {
            return m_Entries.Length;
        }
    }
}