namespace Narumikazuchi.Collections;

/// <summary>
/// Represents a node in a <see cref="Trie{T}"/> data structure. Items of type <typeparamref name="TContent"/> can be attached to this <see cref="TrieNode{T}"/>.
/// </summary>
[DebuggerDisplay("{Value}")]
public sealed partial class TrieNode<TContent>
{
    /// <summary>
    /// Finds the child-node with the specified value. Returns <see langword="null"/> if no <see cref=" TrieNode{T}"/> with the specified value exists.
    /// </summary>
    /// <param name="value">The value to lookup in the child-nodes of the <see cref="TrieNode{T}"/>.</param>
    [Pure]
    [return: MaybeNull]
    public TrieNode<TContent>? FindChildNode(Char value) => 
        this.Children
            .FirstOrDefault(n => n is not null &&
                                 n.Value == Char.ToLower(c: value));

    /// <inheritdoc/>
    [return: MaybeNull]
    public override String? ToString()
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
}

// Non-Public
partial class TrieNode<TContent>
{
    internal TrieNode(Trie<TContent> trie!!, 
                      in Char value, 
                      TrieNode<TContent>? parent)
    {
        m_Trie = trie;
        m_Children = new(equality: AreNodesEqual,
                         comparison: CompareNodes);
        m_Items = new ObservableSet<TContent>(comparison: AreItemsEqual);
        this.Value = Char.ToLower(c: value);
        this.Parent = parent;
        if (parent is null)
        {
            this.Depth = 0;
            return;
        }
        this.Depth = parent.Depth + 1;
    }

    private static Boolean AreItemsEqual(TContent? left, 
                                         TContent? right)
    {
        if (left is null)
        {
            return right is null;
        }
        if (right is null)
        {
            return false;
        }
        if (left is IEquatable<TContent> eq)
        {
            return eq.Equals(right);
        }
        return ReferenceEquals(objA: left,
                               objB: right);
    }

    private static Boolean AreNodesEqual(TrieNode<TContent> left!!, 
                                         TrieNode<TContent> right!!) =>
        left.Value == right.Value;

    private static Int32 CompareNodes(TrieNode<TContent>? left,
                                      TrieNode<TContent>? right)
    {
        if (left is null)
        {
            if (right is null)
            {
                return 0;
            }
            return 1;
        }
        if (right is null)
        {
            return -1;
        }
        return left.Value
                   .CompareTo(value: right.Value);
    }

    private static IEnumerable<TContent?> ChildSelector(TrieNode<TContent>? child)
    {
        if (child is null)
        {
            return Array.Empty<TContent?>();
        }
        return child.Items;
    }

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    internal Boolean IsWord { get; set; }
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    internal IEnumerable<TContent?> ChildItems => 
        m_Children.SelectMany(selector: ChildSelector)
                  .Union(second: m_Items);

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    internal readonly ObservableSet<TContent> m_Items;
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private readonly NodeCollection<TrieNode<TContent>, Char> m_Children;
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private readonly Trie<TContent> m_Trie;
}

// ICollection
partial class TrieNode<TContent> : ICollection
{
    Int32 ICollection.Count =>
        m_Items.Count;

    Boolean ICollection.IsSynchronized =>
        m_Items.IsSynchronized;

    Object ICollection.SyncRoot =>
        m_Items.SyncRoot;

    void ICollection.CopyTo(Array array!!, 
                            Int32 index) =>
        ((ICollection)m_Items).CopyTo(array: array,
                                      index: index);
}

// IContentAddable<T>
partial class TrieNode<TContent> : IContentAddable<TContent?>
{
    /// <inheritdoc/>
    public Boolean Add(TContent? item) =>
        m_Items.Add(item);

    /// <inheritdoc/>
    public void AddRange([DisallowNull] IEnumerable<TContent?> collection!!)
    {
        foreach (TContent? item in collection)
        {
            m_Items.Add(item);
        }
    }
}

// IContentClearable
partial class TrieNode<TContent> : IContentClearable
{
    /// <inheritdoc/>
    public void Clear() =>
        m_Items.Clear();
}

// IContentRemovable
partial class TrieNode<TContent> : IContentRemovable
{
    Boolean IContentRemovable.Remove(Object? item) =>
        item is TContent content &&
        this.Remove(content);
}

// IContentRemovable<T>
partial class TrieNode<TContent> : IContentRemovable<TContent?>
{
    /// <inheritdoc/>
    public Boolean Remove(TContent? item)
    {
        if (m_Items.Contains(item: item))
        {
            return m_Items.Remove(item);
        }

        foreach (TrieNode<TContent>? child in this.Children)
        {
            if (child is null)
            {
                continue;
            }
            if (child.Contains(item: item))
            {
                return child.Remove(item);
            }
        }

        return false;
    }

    /// <inheritdoc/>
    public Int32 RemoveAll([DisallowNull] Func<TContent?, Boolean> predicate!!)
    {
        Collection<TContent?> remove = new();
        foreach (TContent? item in m_Items)
        {
            if (predicate.Invoke(item))
            {
                remove.Add(item);
            }
        }

        foreach (TrieNode<TContent>? child in this.Children)
        {
            if (child is null)
            {
                continue;
            }

            foreach (TContent? item in child.Items)
            {
                if (predicate.Invoke(item))
                {
                    remove.Add(item);
                }
            }
        }

        Int32 skipped = 0;
        foreach (TContent? item in remove)
        {
            if (!this.Remove(item))
            {
                ++skipped;
            }
        }
        return remove.Count - skipped;
    }
}

// IElementContainer
partial class TrieNode<TContent> : IElementContainer
{
    Boolean IElementContainer.Contains(Object? item) =>
        item is TContent content &&
        this.Contains(item: content);
}

// IElementContainer<T>
partial class TrieNode<TContent> : IElementContainer<TContent?>
{
    /// <inheritdoc/>
    public Boolean Contains(TContent? item) =>
        m_Items.Contains(item: item);
}

// IEnumerable
partial class TrieNode<TContent> : IEnumerable
{
    IEnumerator IEnumerable.GetEnumerator() =>
        ((IEnumerable<TContent?>)m_Items).GetEnumerator();
}

// IEnumerable<T>
partial class TrieNode<TContent> : IEnumerable<TContent?>
{
    IEnumerator<TContent?> IEnumerable<TContent?>.GetEnumerator() =>
        ((IEnumerable<TContent?>)m_Items).GetEnumerator();
}

// IReadOnlyCollection<T>
partial class TrieNode<TContent> : IReadOnlyCollection<TContent?>
{
    Int32 IReadOnlyCollection<TContent?>.Count =>
        m_Items.Count;
}

// IContentTreeNode<T, U, V>
partial class TrieNode<TContent> : IContentTreeNode<TrieNode<TContent>, Char, TContent> 
    where TContent : class
{
    /// <inheritdoc/>
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    [Pure]
    public Char Value { get; }
    /// <inheritdoc/>
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    [Pure]
    [MaybeNull]
    public TrieNode<TContent>? Parent { get; }
    /// <inheritdoc/>
    [DebuggerBrowsable(DebuggerBrowsableState.Collapsed)]
    [Pure]
    public UInt32 Depth { get; }
    /// <inheritdoc/>
    [DebuggerBrowsable(DebuggerBrowsableState.Collapsed)]
    [Pure]
    public Boolean IsLeaf => 
        m_Children.Count == 0;
    /// <inheritdoc/>
    [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
    [NotNull]
    [Pure]
    public NodeCollection<TrieNode<TContent>, Char> Children => 
        m_Children;

    /// <inheritdoc/>
    [DebuggerBrowsable(DebuggerBrowsableState.Collapsed)]
    [NotNull]
    public IEnumerable<TContent?> Items
    {
        get
        {
            if (m_Trie.ParentsKnowChildItems)
            {
                return this.ChildItems;
            }
            return m_Items;
        }
    }
}