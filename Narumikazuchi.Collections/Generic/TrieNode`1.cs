namespace Narumikazuchi.Collections;

/// <summary>
/// Represents a node in a <see cref="Trie{T}"/> data structure. Items of type <typeparamref name="TContent"/> can be attached to this <see cref="TrieNode{T}"/>.
/// </summary>
[DebuggerDisplay("{Value}")]
public sealed partial class TrieNode<TContent>
    where TContent : class
{
    /// <summary>
    /// Adds an object to the <see cref="TrieNode{TContent}"/>.
    /// </summary>
    /// <param name="item">The value to be added to the <see cref="TrieNode{TContent}"/>.</param>
    /// <returns><see langword="true"/> if the item was added; otherwise, <see langword="false"/></returns>
    /// <exception cref="ArgumentNullException"/>
    public Boolean Add(TContent item)
    {
        ArgumentNullException.ThrowIfNull(item);

        return m_Items.Add(item);
    }

    /// <summary>
    /// Adds the elements of the specified collection to the <see cref="TrieNode{TContent}"/>.
    /// </summary>
    /// <param name="collection">The collection of items to add.</param>
    /// <exception cref="ArgumentNullException"/>
    public void AddRange([DisallowNull] IEnumerable<TContent> collection)
    {
        ArgumentNullException.ThrowIfNull(collection);

        foreach (TContent item in collection)
        {
            if (item is null)
            {
                continue;
            }
            m_Items.Add(item);
        }
    }

    /// <summary>
    /// Removes all elements from the <see cref="TrieNode{TContent}"/>.
    /// </summary>
    public void Clear() =>
        m_Items.Clear();

    /// <summary>
    /// Removes the first occurrence of the specified item from the <see cref="TrieNode{TContent}"/>.
    /// </summary>
    /// <param name="item">The item to remove.</param>
    /// <returns><see langword="true"/> if the item was found and removed; otherwise, <see langword="false"/></returns>
    /// <exception cref="ArgumentNullException"/>
    public Boolean Remove(TContent item)
    {
        ArgumentNullException.ThrowIfNull(item);

        if (m_Items.Contains(item: item))
        {
            return m_Items.Remove(item);
        }

        foreach (TrieNode<TContent> child in this.Children)
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

    /// <summary>
    /// Removes all objects from the <see cref="TrieNode{TContent}"/> that match the specified condition.
    /// </summary>
    /// <param name="predicate">The condition that objects need to meet to be deleted.</param>
    /// <returns>The amount of items removed</returns>
    /// <exception cref="ArgumentNullException"/>
    public Int32 RemoveAll([DisallowNull] Func<TContent, Boolean> predicate)
    {
        ArgumentNullException.ThrowIfNull(predicate);

        Collection<TContent> remove = new();
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
    /// Determines whether the <see cref="TrieNode{TContent}"/> contains a specific value.
    /// </summary>
    /// <param name="item">The object to locate in the <see cref="TrieNode{TContent}"/>.</param>
    /// <returns><see langword="true"/> if <paramref name="item"/> is found in the <see cref="TrieNode{TContent}"/>; otherwise, <see langword="false"/></returns>
    /// <exception cref="ArgumentNullException"/>
    public Boolean Contains(TContent item)
    {
        ArgumentNullException.ThrowIfNull(item);
        
        return m_Items.Contains(item: item);
    }

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
    [Pure]
    public Char Value { get; }

    /// <summary>
    /// Gets the parent of the current node. Should return <see langword="null"/> for root nodes.
    /// </summary>
    [Pure]
    [MaybeNull]
    public TrieNode<TContent>? Parent { get; }

    /// <summary>
    /// Gets the depth of this node in it's corresponding tree. Should be 0 for root nodes.
    /// </summary>
    [Pure]
    public UInt32 Depth { get; }

    /// <summary>
    /// Gets whether this <see cref="TrieNode{TContent}"/> has no more child-nodes.
    /// </summary>
    [Pure]
    public Boolean IsLeaf =>
        m_Children.Count == 0;

    /// <summary>
    /// Gets the child nodes of this <see cref="TrieNode{TContent}"/>.
    /// </summary>
    [NotNull]
    [Pure]
    public IEnumerable<TrieNode<TContent>> Children =>
        m_Children.Values;

    /// <summary>
    /// Gets the items associated with this <see cref="TrieNode{TContent}"/>.
    /// </summary>
    [NotNull]
    public IEnumerable<TContent> Items
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

// Non-Public
partial class TrieNode<TContent>
{
    internal TrieNode(Trie<TContent> trie, 
                      in Char value, 
                      TrieNode<TContent>? parent)
    {
        m_Trie = trie;
        m_Children = new();
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

    private static IEnumerable<TContent> ChildSelector(KeyValuePair<Char, TrieNode<TContent>> child)
    {
        if (child.Value is null)
        {
            return Array.Empty<TContent>();
        }
        return child.Value
                    .Items;
    }

    internal Boolean IsWord { get; set; }

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    internal IEnumerable<TContent> ChildItems => 
        m_Children.SelectMany(selector: ChildSelector)
                  .Union(second: m_Items);

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    internal readonly HashSet<TContent> m_Items;

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    internal readonly SortedList<Char, TrieNode<TContent>> m_Children;

    private readonly Trie<TContent> m_Trie;
}

// IStrongEnumerable<T, U>
partial class TrieNode<TContent> : IStrongEnumerable<TContent?, HashSet<TContent>.Enumerator>
{
    /// <inheritdoc/>
    public HashSet<TContent>.Enumerator GetEnumerator() =>
        m_Items.GetEnumerator();
}