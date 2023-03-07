namespace Narumikazuchi.Collections;

public partial class Trie<TContent>
{
    private Trie(Char[] separators)
    {
#if NET6_0_OR_GREATER
        ArgumentNullException.ThrowIfNull(separators);
#else
        if (separators is null)
        {
            throw new ArgumentNullException(nameof(separators));
        }
#endif

        m_Root = new(trie: this,
                     value: '^',
                     parent: null);
        m_Separators = separators;
    }
    internal Trie(IEnumerable<String> collection) :
        this()
    {
        if (!collection.Any())
        {
            throw new ArgumentException(CANNOT_CREATE_FROM_EMPTY_COLLECTION);
        }

        foreach (String word in collection.Distinct())
        {
            this.InsertRange(index: word,
                             enumerable: Array.Empty<TContent>());
        }
    }

    private ReadOnlyList<String> TraverseInternal(TrieNode<TContent> parent)
    {
        return this.TraverseInternal(parent: parent,
                                     wordStart: String.Empty);
    }

    private ReadOnlyList<String> TraverseInternal(TrieNode<TContent> parent,
                                                  String wordStart)
    {
        List<String> words = new();
        String start = wordStart + parent.Value.ToString();

        if (parent.IsLeaf ||
            parent.IsWord)
        {
            words.Add(start);
        }

        foreach (TrieNode<TContent> child in parent.Children)
        {
            if (child is null)
            {
                continue;
            }

            foreach (String word in this.TraverseInternal(parent: child,
                                                          wordStart: start))
            {
                words.Add(word);
            }
        }

        return ReadOnlyList<String>.CreateFrom<List<String>>(words);
    }

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    static private readonly Char[] s_DefaultSeparators = new Char[] { ' ', '.', ',', ';', '(', ')', '[', ']', '{', '}', '/', '\\', '-', '_' };

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private readonly Mutex m_Mutex = new();
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private readonly TrieNode<TContent> m_Root;
    private readonly Char[] m_Separators;
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private Int32 m_Words = 0;

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private const String CANNOT_CREATE_FROM_EMPTY_COLLECTION = "Cannot create Trie from empty IEnumerable.";
}