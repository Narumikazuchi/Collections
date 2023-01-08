namespace Narumikazuchi.Collections;

/// <summary>
/// Provides the default functionality for comparing two <see cref="TrieNode{TContent}"/> objects.
/// </summary>
public sealed partial class TrieNodeComparer<TContent>
    where TContent : class
{
    internal TrieNodeComparer()
    { }

    /// <summary>
    /// Gets the singleton instance of this <see cref="TrieNodeComparer{TContent}"/> class.
    /// </summary>
    public static TrieNodeComparer<TContent> Instance { get; } = new();
}

partial class TrieNodeComparer<TContent> : IComparer<TrieNode<TContent>>
{
    /// <inheritdoc/>
    public Int32 Compare(TrieNode<TContent>? left,
                         TrieNode<TContent>? right)
    {
        if (left is null)
        {
            if (right is null)
            {
                return 0;
            }
            else
            {
                return -1;
            }
        }
        else if (right is null)
        {
            return 1;
        }
        else
        {
            return left.Value.CompareTo(right.Value);
        }
    }
}