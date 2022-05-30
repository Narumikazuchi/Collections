namespace Narumikazuchi.Collections;

/// <summary>
/// Represents a node in a <see cref="Trie{T}"/> data structure. Items of type <typeparamref name="TContent"/> can be attached to this <see cref="TrieNode{T}"/>.
/// </summary>
[DebuggerDisplay("{Value}")]
public sealed partial class TrieNode<TContent>
    where TContent : class
{
    /// <summary>
    /// Adds the elements of the specified collection to the <see cref="TrieNode{TContent}"/>.
    /// </summary>
    /// <param name="enumerable">The collection of items to add.</param>
    /// <exception cref="ArgumentNullException"/>
    public void AddRange<TEnumerable>([DisallowNull] TEnumerable enumerable)
        where TEnumerable : IEnumerable<TContent>
    {
        ArgumentNullException.ThrowIfNull(enumerable);

        foreach (TContent item in enumerable)
        {
            if (item is null)
            {
                continue;
            }
            m_Items.Add(item);
        }
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
    /// Finds the child-node with the specified value. Returns <see langword="null"/> if no <see cref=" TrieNode{T}"/> with the specified value exists.
    /// </summary>
    /// <param name="value">The value to lookup in the child-nodes of the <see cref="TrieNode{T}"/>.</param>
    [Pure]
    [return: MaybeNull]
    public TrieNode<TContent>? FindChildNode(Char value) => 
        m_Children.FirstOrDefault(n => n is not null &&
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
    public IStrongEnumerable<TrieNode<TContent>, CommonListEnumerator<TrieNode<TContent>>> Children =>
        m_Children;

    /// <summary>
    /// Gets the items associated with this <see cref="TrieNode{TContent}"/>.
    /// </summary>
    [NotNull]
    public IStrongEnumerable<TContent, Enumerator> Items => 
        new Enumerator(this);
}

// Non-Public
partial class TrieNode<TContent>
{
    internal TrieNode(Trie<TContent> trie, 
                      in Char value, 
                      TrieNode<TContent>? parent)
    {
        m_Trie = trie;
        m_Children = SortedCollection<TrieNode<TContent>>.Create(comparer: TrieNodeComparer<TContent>.Instance);
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
    internal readonly SortedCollection<TrieNode<TContent>> m_Children;

    private readonly Trie<TContent> m_Trie;
}

// ICollectionWithCount<T, U>
partial class TrieNode<TContent> : ICollectionWithCount<TContent, CommonHashSetEnumerator<TContent>>
{
    /// <inheritdoc/>
    public Int32 Count =>
        m_Items.Count;
}

// IEnumerable
partial class TrieNode<TContent> : IEnumerable
{
    IEnumerator IEnumerable.GetEnumerator() =>
        this.GetEnumerator();
}

// IEnumerable<T>
partial class TrieNode<TContent> : IEnumerable<TContent>
{
    IEnumerator<TContent> IEnumerable<TContent>.GetEnumerator() =>
        this.GetEnumerator();
}

// IModifyableCollection<T, U>
partial class TrieNode<TContent> : IModifyableCollection<TContent, CommonHashSetEnumerator<TContent>>
{
    /// <summary>
    /// Adds an object to the <see cref="TrieNode{TContent}"/>.
    /// </summary>
    /// <param name="item">The value to be added to the <see cref="TrieNode{TContent}"/>.</param>
    /// <returns><see langword="true"/> if the item was added; otherwise, <see langword="false"/></returns>
    /// <exception cref="ArgumentNullException"/>
    public Boolean Add([DisallowNull] TContent item)
    {
        ArgumentNullException.ThrowIfNull(item);

        return m_Items.Add(item);
    }

    /// <summary>
    /// Adds the elements of the specified collection to the <see cref="TrieNode{TContent}"/>.
    /// </summary>
    /// <param name="enumerable">The collection of items to add.</param>
    /// <exception cref="ArgumentNullException"/>
    public void AddRange<TEnumerator>([DisallowNull] IStrongEnumerable<TContent, TEnumerator> enumerable)
        where TEnumerator : struct, IStrongEnumerator<TContent>
    {
        ArgumentNullException.ThrowIfNull(enumerable);

        foreach (TContent item in enumerable)
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
    public Boolean Remove([DisallowNull] TContent item)
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
}

// IReadOnlyCollection<T, U>
partial class TrieNode<TContent> : IReadOnlyCollection<TContent, CommonHashSetEnumerator<TContent>>
{
    /// <summary>
    /// Determines whether the <see cref="TrieNode{TContent}"/> contains a specific value.
    /// </summary>
    /// <param name="item">The object to locate in the <see cref="TrieNode{TContent}"/>.</param>
    /// <returns><see langword="true"/> if <paramref name="item"/> is found in the <see cref="TrieNode{TContent}"/>; otherwise, <see langword="false"/></returns>
    /// <exception cref="ArgumentNullException"/>
    public Boolean Contains([DisallowNull] TContent item)
    {
        ArgumentNullException.ThrowIfNull(item);

        return m_Items.Contains(item: item);
    }

    /// <inheritdoc/>
    public void CopyTo([DisallowNull] TContent[] array)
    {
        ArgumentNullException.ThrowIfNull(array);

        m_Items.CopyTo(array);
    }
    /// <inheritdoc/>
    public void CopyTo([DisallowNull] TContent[] array,
                       Int32 destinationIndex)
    {
        ArgumentNullException.ThrowIfNull(array);
        destinationIndex.ThrowIfOutOfRange(0, Int32.MaxValue);

        m_Items.CopyTo(array: array,
                       arrayIndex: destinationIndex);
    }
}

// IStrongEnumerable<T, U>
partial class TrieNode<TContent> : IStrongEnumerable<TContent, CommonHashSetEnumerator<TContent>>
{
    /// <inheritdoc/>
    public CommonHashSetEnumerator<TContent> GetEnumerator() =>
        new(m_Items);
}

// ContentEnumerator
partial class TrieNode<TContent>
{
    /// <summary>
    /// An enumerator that iterates through the contents of the <see cref="TrieNode{TContent}"/>.
    /// </summary>
    public struct Enumerator :
        IStrongEnumerable<TContent, Enumerator>,
        IStrongEnumerator<TContent>
    {
        /// <summary>
        /// The default constructor for the <see cref="Enumerator"/> is not allowed.
        /// </summary>
        /// <exception cref="NotAllowed"></exception>
        public Enumerator()
        {
            throw new NotAllowed();
        }
        internal Enumerator(TrieNode<TContent> parent)
        {
            m_Parent = parent;
            m_ChildEnumerator = parent.Children.GetEnumerator();
            m_Child = null;
            m_Enumerator = default;
            m_State = null;
        }

        /// <inheritdoc/>
        public Boolean MoveNext()
        {
            if (!m_State.HasValue)
            {
                m_Enumerator = m_Parent.m_Items.GetEnumerator();
                m_State = 1;
            }

            while (m_State.Value == 1)
            {
                if (m_Enumerator.MoveNext())
                {
                    return true;
                }
                else if (m_Parent.m_Trie.ParentsKnowChildItems &&
                         m_ChildEnumerator.MoveNext())
                {
                    m_Child = m_ChildEnumerator.Current;
                    m_Enumerator = m_Child.m_Items.GetEnumerator();
                    continue;
                }
                else
                {
                    m_State = -1;
                    return false;
                }
            }

            m_State = -1;
            return false;
        }

        /// <inheritdoc/>
        public Enumerator GetEnumerator()
        {
            if (!m_State.HasValue)
            {
                return this;
            }
            else
            {
                return new(m_Parent);
            }
        }

        /// <inheritdoc/>
        public TContent Current =>
            m_Enumerator.Current;

        private readonly TrieNode<TContent> m_Parent;
        private readonly CommonListEnumerator<TrieNode<TContent>> m_ChildEnumerator;
        private TrieNode<TContent>? m_Child;
        private HashSet<TContent>.Enumerator m_Enumerator;
        private Int32? m_State;
    }
}