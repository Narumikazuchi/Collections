namespace Narumikazuchi.Collections;

/// <summary>
/// Represents a very fast but memory costly word lookup data structure. Includes the attaching of objects of type <typeparamref name="TContent"/> to any individual <see cref="TrieNode{TContent}"/>.
/// </summary>
[DebuggerDisplay("Words: {_words} | Items: {RootNode.ChildItems.Count}")]
public sealed partial class Trie<TContent> : StrongEnumerable<String, CommonArrayEnumerator<String>>
    where TContent : class
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Trie{TContent}"/> type.
    /// </summary>
    /// <param name="separators">The separators used to split inserted words.</param>
    /// <exception cref="ArgumentNullException" />
    static public Trie<TContent> CreateFrom<TEnumerable>(
#if NETCOREAPP3_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        [DisallowNull]
#endif
        NotNull<TEnumerable> separators)
            where TEnumerable : IEnumerable<Char>
    {
        TEnumerable source = separators;
        return new(source);
    }

    /// <summary>
    /// Instantiates an empty <see cref="Trie{TContent}"/>.
    /// </summary>
    public Trie() :
        this(s_DefaultSeparators)
    { }

    /// <inheritdoc/>
    public override CommonArrayEnumerator<String> GetEnumerator()
    {
        return this.TraverseInternal(m_Root)
                   .GetEnumerator();
    }

    /// <inheritdoc/>
    public Boolean Exists(
#if NETCOREAPP3_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        [DisallowNull]
#endif
        Func<String, Boolean> predicate)
    {
#if NET6_0_OR_GREATER
        ArgumentNullException.ThrowIfNull(predicate);
#else
        if (predicate is null)
        {
            throw new ArgumentNullException(nameof(predicate));
        }
#endif

        foreach (String word in this)
        {
            String first = word.ToLower()
                               .Split(separator: m_Separators,
                                      options: StringSplitOptions.RemoveEmptyEntries)[0];
            if (predicate.Invoke(first))
            {
                return true;
            }
        }

        return false;
    }

    /// <inheritdoc/>
#if NETCOREAPP3_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
    [return: MaybeNull]
#endif
    public MaybeNull<TrieNode<TContent>> Find(
#if NETCOREAPP3_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        [DisallowNull]
#endif
        Func<String, Boolean> predicate)
    {
#if NET6_0_OR_GREATER
        ArgumentNullException.ThrowIfNull(predicate);
#else
        if (predicate is null)
        {
            throw new ArgumentNullException(nameof(predicate));
        }
#endif

        foreach (String word in this)
        {
            String first = word.ToLower()
                               .Split(separator: m_Separators,
                                      options: StringSplitOptions.RemoveEmptyEntries)[0];
            if (predicate.Invoke(first))
            {
                TrieNode<TContent> result = m_Root;
                for (Int32 i = 0;
                     i < word.Length;
                     i++)
                {
                    TrieNode<TContent>? temp = result.FindChildNode(word[i]);
                    if (temp is null)
                    {
                        throw new NullReferenceException();
                    }

                    result = temp;
                }

                return result;
            }
        }

        return null;
    }

    /// <inheritdoc/>
    public NotNull<ReadOnlyList<TrieNode<TContent>>> FindAll(
#if NETCOREAPP3_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        [DisallowNull]
#endif
        Func<String, Boolean> predicate)
    {
#if NET6_0_OR_GREATER
        ArgumentNullException.ThrowIfNull(predicate);
#else
        if (predicate is null)
        {
            throw new ArgumentNullException(nameof(predicate));
        }
#endif

        List<TrieNode<TContent>> result = new();
        foreach (String word in this)
        {
            String first = word.ToLower()
                               .Split(separator: m_Separators,
                                      options: StringSplitOptions.RemoveEmptyEntries)[0];
            if (predicate.Invoke(first))
            {
                TrieNode<TContent> current = m_Root;
                for (Int32 i = 0;
                     i < word.Length;
                     i++)
                {
                    TrieNode<TContent>? temp = current.FindChildNode(word[i]);
                    if (temp is null)
                    {
                        throw new NullReferenceException();
                    }

                    current = temp;
                }

                result.Add(current);
            }
        }

        return ReadOnlyList<TrieNode<TContent>>.CreateFrom<List<TrieNode<TContent>>>(result);
    }

    /// <inheritdoc/>
    public NotNull<ReadOnlyList<TrieNode<TContent>>> FindExcept(
#if NETCOREAPP3_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        [DisallowNull]
#endif
        Func<String, Boolean> predicate)
    {
#if NET6_0_OR_GREATER
        ArgumentNullException.ThrowIfNull(predicate);
#else
        if (predicate is null)
        {
            throw new ArgumentNullException(nameof(predicate));
        }
#endif

        List<TrieNode<TContent>> result = new();
        foreach (String word in this)
        {
            String first = word.ToLower()
                               .Split(separator: m_Separators,
                                      options: StringSplitOptions.RemoveEmptyEntries)[0];
            if (predicate.Invoke(first))
            {
                continue;
            }
            TrieNode<TContent> current = m_Root;
            for (Int32 i = 0;
                 i < word.Length;
                 i++)
            {
                TrieNode<TContent>? temp = current.FindChildNode(word[i]);
                if (temp is null)
                {
                    throw new NullReferenceException();
                }

                current = temp;
            }

            result.Add(current);
        }

        return ReadOnlyList<TrieNode<TContent>>.CreateFrom<List<TrieNode<TContent>>>(result);
    }

    /// <inheritdoc/>
#if NETCOREAPP3_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
    [return: MaybeNull]
#endif
    public MaybeNull<TrieNode<TContent>> FindLast(
#if NETCOREAPP3_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        [DisallowNull]
#endif
        Func<String, Boolean> predicate)
    {
#if NET6_0_OR_GREATER
        ArgumentNullException.ThrowIfNull(predicate);
#else
        if (predicate is null)
        {
            throw new ArgumentNullException(nameof(predicate));
        }
#endif

        TrieNode<TContent>? result = null;
        foreach (String word in this)
        {
            String first = word.ToLower()
                               .Split(separator: m_Separators,
                                      options: StringSplitOptions.RemoveEmptyEntries)[0];
            if (predicate.Invoke(first))
            {
                TrieNode<TContent> current = m_Root;
                for (Int32 i = 0;
                     i < word.Length;
                     i++)
                {
                    TrieNode<TContent>? temp = current.FindChildNode(word[i]);
                    if (temp is null)
                    {
                        throw new NullReferenceException();
                    }

                    current = temp;
                }

                result = current;
            }
        }

        return result;
    }

    /// <inheritdoc/>
    public void Insert(
#if NETCOREAPP3_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        [DisallowNull]
#endif
        NotNullOrEmpty<String> index,
#if NETCOREAPP3_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        [DisallowNull]
#endif
        NotNull<TContent> item)
    {
        this.InsertRange<TContent[]>(index: index,
                                     enumerable: new TContent[] { item });
    }

    /// <inheritdoc/>
    public void InsertRange<TEnumerable>(
#if NETCOREAPP3_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        [DisallowNull]
#endif
        NotNullOrEmpty<String> index,
#if NETCOREAPP3_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        [DisallowNull]
#endif
        NotNull<TEnumerable> enumerable)
            where TEnumerable : IEnumerable<TContent>
    {
        String insert = index;
        TEnumerable source = enumerable;
        String[] words = insert.ToLower()
                               .Split(separator: m_Separators,
                                      options: StringSplitOptions.RemoveEmptyEntries);
        foreach (String word in words)
        {
            TrieNode<TContent>? current = this.Find(prefix => prefix == word);
            if (current is null)
            {
                continue;
            }

            ((INotifyPropertyChangingHelper)this).OnPropertyChanging(nameof(this.Count));
            lock (m_Mutex)
            {
                if (current.Depth < word.Length)
                {
                    m_Words++;
                }

                for (Int32 i = (Int32)current.Depth;
                     i < word.Length;
                     i++)
                {
                    TrieNode<TContent> newNode = new(trie: this,
                                                     value: word[i],
                                                     parent: current);
                    current.m_Children.Add(newNode);
                    current = newNode;
                }

                current.IsWord = true;
                foreach (TContent item in source)
                {
                    if (item is null)
                    {
                        continue;
                    }

                    current.Add(item);
                }
            }

            ((INotifyPropertyChangedHelper)this).OnPropertyChanged(nameof(this.Count));
            ((INotifyCollectionChangedHelper)this).OnCollectionChanged(new NotifyCollectionChangedEventArgs(action: NotifyCollectionChangedAction.Add,
                                                                                                            changedItem: word));
        }
    }

    /// <inheritdoc/>
    public void Clear()
    {
        lock (m_Mutex)
        {
            m_Root.m_Children.Clear();
            m_Words = 0;
        }
    }

    /// <inheritdoc/>
    public Boolean Remove(
#if NETCOREAPP3_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        [DisallowNull]
#endif
        NotNullOrEmpty<String> item)
    {
        String remove = item;
        Boolean result = false;
        String[] words = remove.ToLower()
                               .Split(separator: m_Separators,
                                      options: StringSplitOptions.RemoveEmptyEntries);
        foreach (String word in words)
        {
            ((INotifyPropertyChangingHelper)this).OnPropertyChanging(nameof(this.Count));
            lock (m_Mutex)
            {
                m_Words--;
                TrieNode<TContent>? node = this.Find(prefix => prefix == word);
                if (node is null)
                {
                    continue;
                }

                if (node.Depth == word.Length)
                {
                    node.IsWord = false;
                    result = true;
                    while (node.IsLeaf)
                    {
                        TrieNode<TContent>? parent = node.Parent;
                        if (parent is null)
                        {
                            break;
                        }

                        parent.m_Children.Remove(node);
                        node = parent;
                    }
                }
            }

            ((INotifyPropertyChangedHelper)this).OnPropertyChanged(nameof(this.Count));
            ((INotifyCollectionChangedHelper)this).OnCollectionChanged(new NotifyCollectionChangedEventArgs(action: NotifyCollectionChangedAction.Remove,
                                                                                                            changedItem: word));
        }
        return result;
    }

    /// <inheritdoc/>
    public Int32 RemoveAll(
#if NETCOREAPP3_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        [DisallowNull]
#endif
        Func<String, Boolean> predicate)
    {
#if NET6_0_OR_GREATER
        ArgumentNullException.ThrowIfNull(predicate);
#else
        if (predicate is null)
        {
            throw new ArgumentNullException(nameof(predicate));
        }
#endif

        List<String> remove = new();
        foreach (String? word in this)
        {
            if (predicate.Invoke(word))
            {
                remove.Add(word);
            }
        }

        Int32 skipped = 0;
        foreach (String? word in remove)
        {
            if (!this.Remove(word))
            {
                ++skipped;
            }
        }

        return remove.Count - skipped;
    }

    /// <summary>
    /// Traverses through the <see cref="Trie{TContent}"/> and returns the inserted words in alphabetic order.
    /// </summary>
    /// <returns>An <see cref="IEnumerable"/> which iterates through all inserted words of this <see cref="Trie{TContent}"/></returns>
    public NotNull<ReadOnlyList<String>> Traverse()
    {
        return this.TraverseInternal(m_Root);
    }

    /// <inheritdoc/>
#if NETCOREAPP3_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
    [NotNull]
#endif
    public NotNull<TrieNode<TContent>> RootNode
    {
        get
        {
            lock (m_Mutex)
            {
                return m_Root;
            }
        }
    }

    /// <inheritdoc/>
    public Boolean ParentsKnowChildItems
    {
        get;
        set;
    } = true;
}