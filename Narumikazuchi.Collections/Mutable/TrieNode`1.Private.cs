namespace Narumikazuchi.Collections;

public partial class TrieNode<TContent>
{
    internal TrieNode(Trie<TContent> trie,
                      Char value,
                      TrieNode<TContent>? parent)
    {
        m_Trie = trie;
        m_Children = new SortedList<TrieNode<TContent>, TrieNodeComparer<TContent>>(TrieNodeComparer<TContent>.Instance);
        m_Items = new();
        this.Value = Char.ToLower(c: value);
        this.Parent = parent;
        if (parent is null)
        {
            this.Depth = 0;
            return;
        }

        this.Depth = parent.Depth + 1;
    }

    internal Boolean IsWord { get; set; }

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    internal readonly HashSet<TContent> m_Items;

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    internal readonly SortedList<TrieNode<TContent>, TrieNodeComparer<TContent>> m_Children;

    private readonly Trie<TContent> m_Trie;
}