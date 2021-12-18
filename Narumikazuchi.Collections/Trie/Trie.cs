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
    public Trie() => 
        this._root = new(trie: this, 
                         value: '^', 
                         parent: null);

    /// <summary>
    /// Traverses through the <see cref="Trie{TContent}"/> and returns the inserted words in alphabetic order.
    /// </summary>
    /// <returns>An <see cref="IEnumerable"/> which iterates through all inserted words of this <see cref="Trie{TContent}"/></returns>
    public IEnumerable<String> Traverse()
    {
        lock (this._syncRoot)
        {
            foreach (TrieNode<TContent>? child in this._root.Children)
            {
                if (child is null)
                {
                    continue;
                }

                foreach (String word in this.TraverseInternal(child))
                {
                    yield return word;
                }
            }
        }
        yield break;
    }

    /// <summary>
    /// An array containing the default seperators used by the <see cref="Insert(in String, TContent?)"/> functions.
    /// </summary>
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public static readonly Char[] DefaultSeparators = new Char[] { ' ', '.', ',', ';', '(', ')', '[', ']', '{', '}', '/', '\\', '-', '_' };
}

// Non-Public
partial class Trie<TContent>
{
    internal Trie(IEnumerable<String> collection) : 
        this()
    {
        if (collection is null)
        {
            throw new ArgumentNullException(nameof(collection));
        }
        if (!collection.Any())
        {
            throw new ArgumentException(CANNOT_CREATE_FROM_EMPTY_COLLECTION);
        }

        foreach (String word in collection.Distinct())
        {
            this.Insert(index: word,
                        item: default);
        }
    }

    private IEnumerable<String> TraverseInternal(TrieNode<TContent> parent) =>
        this.TraverseInternal(parent: parent,
                              wordStart: String.Empty);
    private IEnumerable<String> TraverseInternal(TrieNode<TContent> parent,
                                                 String? wordStart)
    {
        String start = wordStart + parent.Value.ToString();

        if (parent.IsLeaf ||
            parent.IsWord)
        {
            yield return start;
        }

        foreach (TrieNode<TContent>? child in parent.Children)
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
    private readonly Object _syncRoot = new();
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private readonly TrieNode<TContent> _root;
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private Int32 _words = 0;

#pragma warning disable
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private const String CANNOT_CREATE_FROM_EMPTY_COLLECTION = "Cannot create Trie from empty IEnumerable.";
#pragma warning restore
}

// ICollection
partial class Trie<TContent> : ICollection
{
    void ICollection.CopyTo(Array array, 
                            Int32 index)
    {
        ExceptionHelpers.ThrowIfArgumentNull(array);

        foreach (String word in this.Traverse())
        {
            array.SetValue(index: index++,
                           value: word);
        }
    }
}

// IContentClearable
partial class Trie<TContent> : IContentClearable
{
    /// <inheritdoc/>
    public void Clear()
    {
        lock (this._syncRoot)
        {
            this._root.Children.Clear();
            this._words = 0;
        }
    }
}

// IContentInsertable<T>
partial class Trie<TContent> : IContentInsertable<String>
{
    void IContentInsertable<String>.Insert(in String index, 
                                           Object item)
    {
        if (item is TContent content)
        {
            this.Insert(index: index,
                        item: content);
            return;
        }
        throw new InvalidCastException();
    }
}

// IContentInsertable<T, U>
partial class Trie<TContent> : IContentInsertable<String, TContent?>
{
    /// <inheritdoc/>
    public void Insert([DisallowNull] in String index,
                       TContent? item) =>
        this.InsertRange(index: index,
                         collection: new TContent?[] { item });

    /// <inheritdoc/>
    public void InsertRange([DisallowNull] in String index, 
                            [DisallowNull] IEnumerable<TContent?> collection)
    {
        ExceptionHelpers.ThrowIfNullOrEmpty(index);
        ExceptionHelpers.ThrowIfArgumentNull(collection);

        String[] words = index.ToLower()
                              .Split(separator: DefaultSeparators,
                                     options: StringSplitOptions.RemoveEmptyEntries);
        foreach (String word in words)
        {
            TrieNode<TContent>? current = this.Find(predicate: (prefix) => prefix == word);
            if (current is null)
            {
                continue;
            }

            this.PropertyChanging?.Invoke(this,
                                          new(propertyName: nameof(this.Count)));
            lock (this._syncRoot)
            {
                if (current.Depth < word.Length)
                {
                    this._words++;
                }
                for (Int32 i = (Int32)current.Depth; i < word.Length; i++)
                {
                    TrieNode<TContent> newNode = new(trie: this,
                                                     value: word[i],
                                                     parent: current);
                    current.Children.Add(item: newNode);
                    current = newNode;
                }
                current.IsWord = true;
                foreach (TContent? item in collection)
                {
                    current.Add(item: item);
                }
            }
            this.PropertyChanged?.Invoke(this,
                                         new(propertyName: nameof(this.Count)));
            this.CollectionChanged?.Invoke(this,
                                           new(action: NotifyCollectionChangedAction.Add,
                                               changedItem: word));
        }
    }
}

// IContentRemovable
partial class Trie<TContent> : IContentRemovable
{
    Boolean IContentRemovable.Remove(Object item) =>
        item is String word &&
        this.Remove(item: word);
}

// IContentRemovable<T>
partial class Trie<TContent> : IContentRemovable<String>
{
    /// <inheritdoc/>
    public Boolean Remove([DisallowNull] String item)
    {
        Boolean result = false;
        String[] words = item.ToLower()
                             .Split(separator: DefaultSeparators,
                                    options: StringSplitOptions.RemoveEmptyEntries);
        foreach (String word in words)
        {
            this.PropertyChanging?.Invoke(this,
                                          new(propertyName: nameof(this.Count)));
            lock (this._syncRoot)
            {
                this._words--;
                TrieNode<TContent>? node = this.Find(predicate: (prefix) => prefix == word);
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
                        parent.Children.Remove(item: node);
                        node = parent;
                    }
                }
            }
            this.PropertyChanged?.Invoke(this,
                                         new(propertyName: nameof(this.Count)));
            this.CollectionChanged?.Invoke(this,
                                           new(action: NotifyCollectionChangedAction.Remove,
                                               changedItem: word));
        }
        return result;
    }

    /// <inheritdoc/>
    public Int32 RemoveAll([DisallowNull] Func<String, Boolean> predicate)
    {
        Collection<String> remove = new();
        foreach (String? word in this.Traverse())
        {
            if (predicate.Invoke(arg: word))
            {
                remove.Add(item: word);
            }
        }

        foreach (String? word in remove)
        {
            this.Remove(item: word);
        }
        return remove.Count;
    }
}

// IContentTree<T, U, V>
partial class Trie<TContent> : IContentTree<TrieNode<TContent>, Char, TContent>
{
    /// <inheritdoc/>
    [DebuggerBrowsable(DebuggerBrowsableState.Collapsed)]
    public Boolean ParentsKnowChildItems { get; set; } = true;
}

// IElementFinder<T, U>
partial class Trie<TContent> : IElementFinder<String, TrieNode<TContent>>
{
    /// <inheritdoc/>
    public Boolean Exists([DisallowNull] Func<String, Boolean> predicate)
    {
        ExceptionHelpers.ThrowIfArgumentNull(predicate);

        foreach (String word in this.Traverse())
        {
            String first = word.ToLower()
                               .Split(separator: DefaultSeparators,
                                      options: StringSplitOptions.RemoveEmptyEntries)[0];
            if (predicate.Invoke(arg: first))
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
        ExceptionHelpers.ThrowIfArgumentNull(predicate);

        foreach (String word in this.Traverse())
        {
            String first = word.ToLower()
                               .Split(separator: DefaultSeparators,
                                      options: StringSplitOptions.RemoveEmptyEntries)[0];
            if (predicate.Invoke(arg: first))
            {
                TrieNode<TContent> result = this._root;
                for (Int32 i = 0; i < word.Length; i++)
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
    public IElementContainer<TrieNode<TContent>> FindAll([DisallowNull] Func<String, Boolean> predicate)
    {
        ExceptionHelpers.ThrowIfArgumentNull(predicate);

        Collection<TrieNode<TContent>> result = new();
        foreach (String word in this.Traverse())
        {
            String first = word.ToLower()
                               .Split(separator: DefaultSeparators,
                                      options: StringSplitOptions.RemoveEmptyEntries)[0];
            if (predicate.Invoke(arg: first))
            {
                TrieNode<TContent> current = this._root;
                for (Int32 i = 0; i < word.Length; i++)
                {
                    TrieNode<TContent>? temp = current.FindChildNode(word[i]);
                    if (temp is null)
                    {
                        throw new NullReferenceException();
                    }
                    current = temp;
                }
                result.Add(item: current);
            }
        }
        return result.AsGenericElementContainer<Collection<TrieNode<TContent>>, TrieNode<TContent>>();
    }

    /// <inheritdoc/>
    [return: NotNull]
    public IElementContainer<TrieNode<TContent>> FindExcept([DisallowNull] Func<String, Boolean> predicate)
    {
        ExceptionHelpers.ThrowIfArgumentNull(predicate);

        Collection<TrieNode<TContent>> result = new();
        foreach (String word in this.Traverse())
        {
            String first = word.ToLower()
                               .Split(separator: DefaultSeparators,
                                      options: StringSplitOptions.RemoveEmptyEntries)[0];
            if (predicate.Invoke(arg: first))
            {
                continue;
            }
            TrieNode<TContent> current = this._root;
            for (Int32 i = 0; i < word.Length; i++)
            {
                TrieNode<TContent>? temp = current.FindChildNode(word[i]);
                if (temp is null)
                {
                    throw new NullReferenceException();
                }
                current = temp;
            }
            result.Add(item: current);
        }
        return result.AsGenericElementContainer<Collection<TrieNode<TContent>>, TrieNode<TContent>>();
    }

    /// <inheritdoc/>
    [return: MaybeNull]
    public TrieNode<TContent>? FindLast([DisallowNull] Func<String, Boolean> predicate)
    {
        ExceptionHelpers.ThrowIfArgumentNull(predicate);

        TrieNode<TContent>? result = null;
        foreach (String word in this.Traverse())
        {
            String first = word.ToLower()
                               .Split(separator: DefaultSeparators,
                                      options: StringSplitOptions.RemoveEmptyEntries)[0];
            if (predicate.Invoke(arg: first))
            {
                TrieNode<TContent> current = this._root;
                for (Int32 i = 0; i < word.Length; i++)
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
}

// IEnumerable
partial class Trie<TContent> : IEnumerable
{
    IEnumerator IEnumerable.GetEnumerator() =>
        ((IEnumerable<String>)this).GetEnumerator();
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
    /// <inheritdoc/>
    public event NotifyCollectionChangedEventHandler? CollectionChanged;
}

// INotifyPropertyChanged
partial class Trie<TContent> : INotifyPropertyChanged
{
    /// <inheritdoc/>
    public event PropertyChangedEventHandler? PropertyChanged;
}

// INotifyPropertyChanging
partial class Trie<TContent> : INotifyPropertyChanging
{
    /// <inheritdoc/>
    public event PropertyChangingEventHandler? PropertyChanging;
}

// IReadOnlyCollection<T>
partial class Trie<TContent> : IReadOnlyCollection<String>
{
    /// <inheritdoc/>
    [Pure]
    public Int32 Count
    {
        get
        {
            lock (this._syncRoot)
            {
                return this._words;
            }
        }
    }
}

// ISynchronized
partial class Trie<TContent> : ISynchronized
{
    /// <inheritdoc/>
    [Pure]
    public Boolean IsSynchronized { get; } = true;

    /// <inheritdoc/>
    [Pure]
    [NotNull]
    public Object SyncRoot =>
        this._syncRoot;
}

// ITree<T, U>
partial class Trie<TContent> : ITree<TrieNode<TContent>, Char>
{
    /// <inheritdoc/>
    [DebuggerBrowsable(DebuggerBrowsableState.Collapsed)]
    [NotNull]
    [Pure]
    public TrieNode<TContent> RootNode
    {
        get
        {
            lock (this._syncRoot)
            {
                return this._root;
            }
        }
    }
}