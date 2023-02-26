namespace Narumikazuchi.Collections;

public partial class TrieNode<TContent> : IHasCount
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