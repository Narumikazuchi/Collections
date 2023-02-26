namespace Narumikazuchi.Collections;

/// <summary>
/// Provides the default functionality for comparing two <see cref="TrieNode{TContent}"/> objects.
/// </summary>
public sealed partial class TrieNodeComparer<TContent>
    where TContent : class
{
    /// <summary>
    /// Gets the singleton instance of this <see cref="TrieNodeComparer{TContent}"/> class.
    /// </summary>
    static public TrieNodeComparer<TContent> Instance
    {
        get
        {
            return s_Instance.Value;
        }
    }

    private TrieNodeComparer()
    { }

    static private readonly Lazy<TrieNodeComparer<TContent>> s_Instance = new(valueFactory: () => new TrieNodeComparer<TContent>(),
                                                                              mode: LazyThreadSafetyMode.ExecutionAndPublication);
}