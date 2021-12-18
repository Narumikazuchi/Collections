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
        this.Children.FirstOrDefault(n => n is not null &&
                                          n.Value == Char.ToLower(value));

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
    internal TrieNode(Trie<TContent> trie, 
                      in Char value, 
                      TrieNode<TContent>? parent)
    {
        this._trie = trie;
        this._children = new(AreNodesEqual,
                             CompareNodes);
        this._items = new ObservableSet<TContent>(AreItemsEqual);
        this.Value = Char.ToLower(value);
        this.Parent = parent;
        this.Depth = parent is null 
                        ? 0 
                        : parent.Depth + 1;
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
        return ReferenceEquals(left,
                               right);
    }

    private static Boolean AreNodesEqual(TrieNode<TContent> left, 
                                         TrieNode<TContent> right) =>
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
        return left.Value.CompareTo(right.Value);
    }

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    internal Boolean IsWord { get; set; }
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    internal IEnumerable<TContent?> ChildItems => 
        this._children.SelectMany<TrieNode<TContent>?, TContent?>(child => child is null
                                                                            ? Array.Empty<TContent?>()
                                                                            : child.Items)
                      .Union(this._items);

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    internal readonly ObservableSet<TContent> _items;
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private readonly NodeCollection<TrieNode<TContent>, Char> _children;
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private readonly Trie<TContent> _trie;
}

// ICollection
partial class TrieNode<TContent> : ICollection
{
    Int32 ICollection.Count =>
        this._items.Count;

    Boolean ICollection.IsSynchronized =>
        this._items.IsSynchronized;

    Object ICollection.SyncRoot =>
        this._items.SyncRoot;

    void ICollection.CopyTo(Array array, 
                            Int32 index) =>
        ((ICollection)this._items).CopyTo(array: array,
                                          index: index);
}

// IContentAddable<T>
partial class TrieNode<TContent> : IContentAddable<TContent?>
{
    /// <inheritdoc/>
    public Boolean Add(TContent? item) =>
        this._items.Add(item);

    /// <inheritdoc/>
    public void AddRange([DisallowNull] IEnumerable<TContent?> collection)
    {
        ExceptionHelpers.ThrowIfArgumentNull(collection);

        foreach (TContent? item in collection)
        {
            this._items.Add(item);
        }
    }
}

// IContentAddable<T>
partial class TrieNode<TContent> : IContentClearable
{
    /// <inheritdoc/>
    public void Clear() =>
        this._items.Clear();
}

// IContentAddable<T>
partial class TrieNode<TContent> : IContentRemovable
{
    Boolean IContentRemovable.Remove(Object item) =>
        item is TContent content &&
        this.Remove(item: content);
}

// IContentAddable<T>
partial class TrieNode<TContent> : IContentRemovable<TContent?>
{
    /// <inheritdoc/>
    public Boolean Remove(TContent? item)
    {
        if (this._items.Contains(item: item))
        {
            return this._items.Remove(item: item);
        }
        foreach (TrieNode<TContent>? child in this.Children)
        {
            if (child is null)
            {
                continue;
            }
            if (child.Contains(item: item))
            {
                return child.Remove(item: item);
            }
        }

        return false;
    }

    /// <inheritdoc/>
    public Int32 RemoveAll([DisallowNull] Func<TContent?, Boolean> predicate)
    {
        ExceptionHelpers.ThrowIfArgumentNull(predicate);

        Collection<TContent?> remove = new();
        foreach (TContent? item in this._items)
        {
            if (predicate.Invoke(arg: item))
            {
                remove.Add(item: item);
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
                if (predicate.Invoke(arg: item))
                {
                    remove.Add(item: item);
                }
            }
        }

        foreach (TContent? item in remove)
        {
            this.Remove(item: item);
        }
        return remove.Count;
    }
}

// IContentAddable<T>
partial class TrieNode<TContent> : IElementContainer
{
    Boolean IElementContainer.Contains(Object? item) =>
        item is TContent content &&
        this.Contains(item: content);
}

// IContentAddable<T>
partial class TrieNode<TContent> : IElementContainer<TContent?>
{
    /// <inheritdoc/>
    public Boolean Contains(TContent? item) =>
        this._items.Contains(item: item);
}

// IEnumerable
partial class TrieNode<TContent> : IEnumerable
{
    IEnumerator IEnumerable.GetEnumerator() =>
        ((IEnumerable<TContent?>)this._items).GetEnumerator();
}

// IEnumerable<T>
partial class TrieNode<TContent> : IEnumerable<TContent?>
{
    IEnumerator<TContent?> IEnumerable<TContent?>.GetEnumerator() =>
        ((IEnumerable<TContent?>)this._items).GetEnumerator();
}

// IReadOnlyCollection<T>
partial class TrieNode<TContent> : IReadOnlyCollection<TContent?>
{
    Int32 IReadOnlyCollection<TContent?>.Count =>
        this._items.Count;
}

// Non-Public
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
    public Boolean IsLeaf => this._children.Count == 0;
    /// <inheritdoc/>
    [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
    [NotNull]
    [Pure]
    public NodeCollection<TrieNode<TContent>, Char> Children => this._children;

    /// <inheritdoc/>
    [DebuggerBrowsable(DebuggerBrowsableState.Collapsed)]
    [NotNull]
    public IEnumerable<TContent?> Items =>
        this._trie.ParentsKnowChildItems
                ? this.ChildItems
                : this._items;
}