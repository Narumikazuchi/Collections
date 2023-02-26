namespace Narumikazuchi.Collections;

public partial class TrieNodeComparer<TContent> : IComparer<TrieNode<TContent>>
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