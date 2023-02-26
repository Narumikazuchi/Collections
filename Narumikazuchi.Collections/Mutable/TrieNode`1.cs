namespace Narumikazuchi.Collections;

/// <summary>
/// Represents a node in a <see cref="Trie{T}"/> data structure. Items of type <typeparamref name="TContent"/> can be attached to this <see cref="TrieNode{T}"/>.
/// </summary>
[DebuggerDisplay("{Value}")]
public sealed partial class TrieNode<TContent> : StrongEnumerable<TContent, CommonHashSetEnumerator<TContent>>
    where TContent : class
{
    /// <inheritdoc/>
    public override CommonHashSetEnumerator<TContent> GetEnumerator()
    {
        return new CommonHashSetEnumerator<TContent>(m_Items);
    }

    /// <summary>
    /// Removes all objects from the <see cref="TrieNode{TContent}"/> that match the specified condition.
    /// </summary>
    /// <param name="predicate">The condition that objects need to meet to be deleted.</param>
    /// <returns>The amount of items removed</returns>
    /// <exception cref="ArgumentNullException"/>
    public Int32 RemoveAll(
#if NETCOREAPP3_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        [DisallowNull]
#endif
        Func<TContent, Boolean> predicate)
    {
#if NET6_0_OR_GREATER
        ArgumentNullException.ThrowIfNull(predicate);
#else
        if (predicate == null)
        {
            throw new ArgumentNullException(nameof(predicate));
        }
#endif

        List<TContent> remove = new();
        foreach (TContent item in m_Items)
        {
            if (item is null)
            {
                continue;
            }
            if (predicate.Invoke(item))
            {
                remove.Add(item);
            }
        }

        foreach (TrieNode<TContent> child in this.Children)
        {
            if (child is null)
            {
                continue;
            }

            foreach (TContent item in child.Items)
            {
                if (item is null)
                {
                    continue;
                }
                if (predicate.Invoke(item))
                {
                    remove.Add(item);
                }
            }
        }

        Int32 skipped = 0;
        foreach (TContent item in remove)
        {
            if (!this.Remove(item))
            {
                ++skipped;
            }
        }
        return remove.Count - skipped;
    }

    /// <summary>
    /// Finds the child-node with the specified value. Returns <see langword="null"/> if no <see cref=" TrieNode{T}"/> with the specified value exists.
    /// </summary>
    /// <param name="value">The value to lookup in the child-nodes of the <see cref="TrieNode{T}"/>.</param>
#if NETCOREAPP3_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
    [return: MaybeNull]
#endif
    public TrieNode<TContent>? FindChildNode(Char value)
    {
        return m_Children.FirstOrDefault(n => n is not null &&
                                              n.Value == Char.ToLower(c: value));
    }

    /// <inheritdoc/>
#if NETCOREAPP3_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
    [return: NotNull]
#endif
    public override String ToString()
    {
        StringBuilder builder = new();
        TrieNode<TContent>? current = this;
        do
        {
            builder.Insert(index: 0, 
                           value: current.Value);
            current = current.Parent;
        } while (current is not null &&
                 current.Parent is not null);
        return builder.ToString();
    }

    /// <summary>
    /// Gets the <see cref="Char"/> value of this <see cref="TrieNode{TContent}"/>.
    /// </summary>
    public Char Value { get; }

    /// <summary>
    /// Gets the parent of the current node. Should return <see langword="null"/> for root nodes.
    /// </summary>
#if NETCOREAPP3_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
    [MaybeNull]
#endif
    public TrieNode<TContent>? Parent { get; }

    /// <summary>
    /// Gets the depth of this node in it's corresponding tree. Should be 0 for root nodes.
    /// </summary>
    public UInt32 Depth { get; }

    /// <summary>
    /// Gets whether this <see cref="TrieNode{TContent}"/> has no more child-nodes.
    /// </summary>
    public Boolean IsLeaf
    {
        get
        {
            return m_Children.Count == 0;
        }
    }

    /// <summary>
    /// Gets the child nodes of this <see cref="TrieNode{TContent}"/>.
    /// </summary>
#if NETCOREAPP3_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
    [NotNull]
#endif
    public StrongEnumerable<TrieNode<TContent>, CommonListEnumerator<TrieNode<TContent>>> Children
    {
        get
        {
            return m_Children;
        }
    }

    /// <summary>
    /// Gets the items associated with this <see cref="TrieNode{TContent}"/>.
    /// </summary>
#if NETCOREAPP3_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
    [NotNull]
#endif
    public Enumerator Items
    {
        get
        {
            return new Enumerator(this);
        }
    }
}