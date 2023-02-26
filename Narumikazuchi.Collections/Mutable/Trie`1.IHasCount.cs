namespace Narumikazuchi.Collections;

public partial class Trie<TContent> : IHasCount
{
    /// <inheritdoc />
    public Int32 Count
    {
        get
        {
            return m_Words;
        }
    }
}