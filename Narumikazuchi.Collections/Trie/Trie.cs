namespace Narumikazuchi.Collections;

/// <summary>
/// Represents a very fast but memory costly word lookup data structure. Includes the attaching of objects of type <typeparamref name="TContent"/> to any individual <see cref="TrieNode{TContent}"/>.
/// </summary>
[DebuggerDisplay("Words: {_words} | Items: {RootNode.ChildItems.Count}")]
public sealed partial class Trie<TContent>
    where TContent : class
{
    /// <summary>
    /// Instantiates an empty <see cref="Trie{TContent}"/>.
    /// </summary>
    public Trie() :
        this(s_DefaultSeparators)
    { }
    /// <summary>
    /// Instantiates an empty <see cref="Trie{TContent}"/>.
    /// </summary>
    /// <exception cref="ArgumentNullException"/>
    public Trie([DisallowNull] IEnumerable<Char> separators)
    {
        ArgumentNullException.ThrowIfNull(separators);

        m_Root = new(trie: this,
                     value: '^',
                     parent: null);
        m_Separators = separators.ToArray();
    }

    /// <inheritdoc/>
    public Boolean Exists([DisallowNull] Func<String, Boolean> predicate)
    {
        ArgumentNullException.ThrowIfNull(predicate);

        foreach (String word in this.Traverse())
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
    [return: MaybeNull]
    public TrieNode<TContent>? Find([DisallowNull] Func<String, Boolean> predicate)
    {
        ArgumentNullException.ThrowIfNull(predicate);

        foreach (String word in this.Traverse())
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
    [return: NotNull]
    public IReadOnlyCollection<TrieNode<TContent>> FindAll([DisallowNull] Func<String, Boolean> predicate)
    {
        ArgumentNullException.ThrowIfNull(predicate);

        Collection<TrieNode<TContent>> result = new();
        foreach (String word in this.Traverse())
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
        return result;
    }

    /// <inheritdoc/>
    [return: NotNull]
    public IReadOnlyCollection<TrieNode<TContent>> FindExcept([DisallowNull] Func<String, Boolean> predicate)
    {
        ArgumentNullException.ThrowIfNull(predicate);

        Collection<TrieNode<TContent>> result = new();
        foreach (String word in this.Traverse())
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
        return result;
    }

    /// <inheritdoc/>
    [return: MaybeNull]
    public TrieNode<TContent>? FindLast([DisallowNull] Func<String, Boolean> predicate)
    {
        ArgumentNullException.ThrowIfNull(predicate);

        TrieNode<TContent>? result = null;
        foreach (String word in this.Traverse())
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
    public void Insert([DisallowNull] in String index,
                       [DisallowNull] TContent item)
    {
        ArgumentNullException.ThrowIfNull(item);

        this.InsertRange(index: index,
                         collection: new TContent[] { item });
    }

    /// <inheritdoc/>
    public void InsertRange([DisallowNull] in String index,
                            [DisallowNull] IEnumerable<TContent> collection)
    {
        ArgumentNullException.ThrowIfNull(index);
        ArgumentNullException.ThrowIfNull(collection);

        String[] words = index.ToLower()
                              .Split(separator: m_Separators,
                                     options: StringSplitOptions.RemoveEmptyEntries);
        foreach (String word in words)
        {
            TrieNode<TContent>? current = this.Find(prefix => prefix == word);
            if (current is null)
            {
                continue;
            }

            this.OnPropertyChanging(nameof(this.Count));
            lock (m_SyncRoot)
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
                    current.m_Children
                           .Add(key: word[i],
                                value: newNode);
                    current = newNode;
                }
                current.IsWord = true;
                foreach (TContent item in collection)
                {
                    if (item is null)
                    {
                        continue;
                    }
                    current.Add(item);
                }
            }
            this.OnPropertyChanged(nameof(this.Count));
            this.OnCollectionChanged(new(action: NotifyCollectionChangedAction.Add,
                                         changedItem: word));
        }
    }

    /// <inheritdoc/>
    public void Clear()
    {
        lock (m_SyncRoot)
        {
            m_Root.m_Children
                  .Clear();
            m_Words = 0;
        }
    }

    /// <inheritdoc/>
    public Boolean Remove([DisallowNull] String item)
    {
        ArgumentNullException.ThrowIfNull(item);

        Boolean result = false;
        String[] words = item.ToLower()
                             .Split(separator: m_Separators,
                                    options: StringSplitOptions.RemoveEmptyEntries);
        foreach (String word in words)
        {
            this.OnPropertyChanging(nameof(this.Count));
            lock (m_SyncRoot)
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
                        parent.m_Children
                              .Remove(node.Value);
                        node = parent;
                    }
                }
            }
            this.OnPropertyChanged(nameof(this.Count));
            this.OnCollectionChanged(new(action: NotifyCollectionChangedAction.Remove,
                                         changedItem: word));
        }
        return result;
    }

    /// <inheritdoc/>
    public Int32 RemoveAll([DisallowNull] Func<String, Boolean> predicate)
    {
        ArgumentNullException.ThrowIfNull(predicate);

        Collection<String> remove = new();
        foreach (String? word in this.Traverse())
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
    public IEnumerable<String> Traverse()
    {
        lock (m_SyncRoot)
        {
            foreach (TrieNode<TContent> child in m_Root.Children)
            {
                if (child is null)
                {
                    continue;
                }

                foreach (String word in this.TraverseInternal(parent: child))
                {
                    yield return word;
                }
            }
        }
        yield break;
    }

    /// <inheritdoc/>
    [NotNull]
    [Pure]
    public TrieNode<TContent> RootNode
    {
        get
        {
            lock (m_SyncRoot)
            {
                return m_Root;
            }
        }
    }

    /// <inheritdoc/>
    [Pure]
    public Boolean ParentsKnowChildItems
    {
        get;
        set;
    } = true;
}

// Non-Public
partial class Trie<TContent>
{
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
                             collection: Array.Empty<TContent>());
        }
    }

    private IEnumerable<String> TraverseInternal(TrieNode<TContent> parent) =>
        this.TraverseInternal(parent: parent,
                              wordStart: String.Empty);
    private IEnumerable<String> TraverseInternal(TrieNode<TContent> parent,
                                                 String? wordStart)
    {
        String start = wordStart + parent.Value
                                         .ToString();

        if (parent.IsLeaf ||
            parent.IsWord)
        {
            yield return start;
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
                yield return word;
            }
        }
        yield break;
    }

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private readonly Object m_SyncRoot = new();
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private readonly TrieNode<TContent> m_Root;
    private readonly Char[] m_Separators;
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private Int32 m_Words = 0;

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private static readonly Char[] s_DefaultSeparators = new Char[] { ' ', '.', ',', ';', '(', ')', '[', ']', '{', '}', '/', '\\', '-', '_' };

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private const String CANNOT_CREATE_FROM_EMPTY_COLLECTION = "Cannot create Trie from empty IEnumerable.";
}

// IEnumerable
partial class Trie<TContent> : IEnumerable
{
    IEnumerator IEnumerable.GetEnumerator() =>
        this.Traverse()
            .GetEnumerator();
}

// IEnumerable<T> - String
partial class Trie<TContent> : IEnumerable<String>
{
    IEnumerator<String> IEnumerable<String>.GetEnumerator() =>
        this.Traverse()
            .GetEnumerator();
}

// INotifyCollectionChanged
partial class Trie<TContent> : INotifyCollectionChanged
{
    /// <inheritdoc />
    public event NotifyCollectionChangedEventHandler? CollectionChanged;

    /// <summary>
    /// Raises the <see cref="CollectionChanged"/> event with the specified event args.
    /// </summary>
    private void OnCollectionChanged(NotifyCollectionChangedEventArgs eventArgs) =>
        this.CollectionChanged?
            .Invoke(sender: this,
                    e: eventArgs);
}

// INotifyPropertyChanging
partial class Trie<TContent> : INotifyPropertyChanging
{
    /// <inheritdoc />
    public event PropertyChangingEventHandler? PropertyChanging;

    /// <summary>
    /// Raises the <see cref="PropertyChanging"/> event with the specified event args.
    /// </summary>
    private void OnPropertyChanging(String propertyName) =>
        this.PropertyChanging?
            .Invoke(sender: this,
                    e: new(propertyName));
}

// INotifyPropertyChanged
partial class Trie<TContent> : INotifyPropertyChanged
{
    /// <inheritdoc />
    public event PropertyChangedEventHandler? PropertyChanged;

    /// <summary>
    /// Raises the <see cref="PropertyChanged"/> event with the specified event args.
    /// </summary>
    private void OnPropertyChanged(String propertyName) =>
        this.PropertyChanged?
            .Invoke(sender: this,
                    e: new(propertyName));
}

// IReadOnlyCollection<T>
partial class Trie<TContent> : IReadOnlyCollection<String>
{
    /// <inheritdoc />
    public Int32 Count =>
        m_Words;
}