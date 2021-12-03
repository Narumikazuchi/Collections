namespace Narumikazuchi.Collections.Abstract;

/// <summary>
/// Represents a collection whose underlying item storage is an array.
/// </summary>
[DebuggerDisplay("Count = {_size}")]
public abstract partial class ArrayBasedCollection<TElement>
{ }

// Non-Public
partial class ArrayBasedCollection<TElement>
{
    private protected ArrayBasedCollection() { }

    /// <summary>
    /// Checks whether the specified object can be added to the collection.
    /// </summary>
    /// <param name="value">The object to check.</param>
    /// <returns><see langword="true"/> if the object can be added to the list; else <see langword="false"/></returns>
    [Pure]
    protected static Boolean IsCompatibleObject([AllowNull] Object? value) =>
        value is TElement || 
        (value is null && 
        default(TElement) is null);

    /// <summary>
    /// Expands the underlying array to fit the specified number of items, if necessary.
    /// </summary>
    /// <param name="capacity">The number of items to fit into this collection.</param>
    /// <exception cref="NotAllowed" />
    protected void EnsureCapacity(in Int32 capacity) =>
        ICollectionExpandable<TElement>.EnsureCapacity(this,
                                                       capacity);

    /// <summary>
    /// Statically allocates an empty array to use for every <see cref="ArrayBasedCollection{T}"/> using the type <typeparamref name="TElement"/>.
    /// </summary>
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    [NotNull]
    protected static readonly TElement?[] _emptyArray = Array.Empty<TElement?>();

    /// <summary>
    /// The internal mutex for synchronizing multi-threaded access.
    /// </summary>
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    [NotNull]
    protected readonly Object _syncRoot = new();
    /// <summary>
    /// Internally manages and contains the items for the <see cref="ArrayBasedCollection{T}"/>.
    /// </summary>
    [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
    [NotNull]
    protected TElement?[] _items = new TElement?[1];
    /// <summary>
    /// Internally manages and contains the actual amount of items in the <see cref="ArrayBasedCollection{T}"/>.
    /// </summary>
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    protected Int32 _size = 0;
    /// <summary>
    /// Keeps track of changes done to the collection, preventing invalid operations during enumeration.
    /// </summary>
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    protected Int32 _version = 0;

#pragma warning disable
    /// <summary>
    /// Represents the default capacity for a new <see cref="ArrayBasedCollection{T}"/>.
    /// </summary>
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    protected const Int32 DEFAULTCAPACITY = 4;
    /// <summary>
    /// Represents the max. size to which the internal array can expand to.
    /// </summary>
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    protected const Int32 MAXARRAYSIZE = 0X7FEFFFFF;
    /// <summary>
    /// Error message, when trying to change _item size while <see cref="IsFixedSize"/> is <see langword="true"/>.
    /// </summary>
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    protected const String SIZE_IS_FIXED = "The capacity for the collection can't be changed, since its size is fixed.";
    /// <summary>
    /// Error message, when the amount of items in the collection have changed during enumeration.
    /// </summary>
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    protected const String COLLECTION_CHANGED = "The collection changed during enumeration.";
#pragma warning restore
}

// IBackendContainer<T>
partial class ArrayBasedCollection<TElement> : IBackingContainer<TElement?[]>
{
    [NotNull]
    TElement?[] IBackingContainer<TElement?[]>.Items
    {
        get => this._items;
        set => this._items = value;
    }

    Int32 IBackingContainer<TElement?[]>.ItemCount
    {
        get => this._size;
        set => this._size = value;
    }
}

// ICollection
partial class ArrayBasedCollection<TElement> : ICollection
{
    /// <inheritdoc/>
    [Pure]
    public abstract void CopyTo([DisallowNull] Array array, 
                                Int32 index);

    /// <inheritdoc/>
    [Pure]
    public virtual Int32 Count
    {
        get
        {
            lock (this._syncRoot)
            {
                return this._size;
            }
        }
    }
}

// ICollectionExpandable<T>
partial class ArrayBasedCollection<TElement> : ICollectionExpandable<TElement>
{
    void ICollectionExpandable<TElement>.EnsureCapacity(in Int32 capacity) =>
        this.EnsureCapacity(capacity);
}

// ICollectionImmutability
partial class ArrayBasedCollection<TElement> : ICollectionImmutability
{
    /// <inheritdoc/>
    [Pure]
    public virtual Boolean IsFixedSize => false;
}

// IConvertToArray<T>
partial class ArrayBasedCollection<TElement> : IConvertToArray<TElement?[]>
{
    /// <inheritdoc/>
    [Pure]
    [return: NotNull]
    public virtual TElement?[] ToArray()
    {
        lock (this._syncRoot)
        {
            if (this._size == 0)
            {
                return Array.Empty<TElement?>();
            }

            TElement?[] array = new TElement?[this._size];
            Array.Copy(sourceArray: this._items,
                       sourceIndex: 0,
                       destinationArray: array,
                       destinationIndex: 0,
                       length: this._size);
            return array;
        }
    }
}

// IElementContainer
partial class ArrayBasedCollection<TElement> : IElementContainer
{
    [Pure]
    Boolean IElementContainer.Contains(Object? item) =>
        item is TElement element &&
        this.Contains(element);
}

// IElementContainer<T>
partial class ArrayBasedCollection<TElement> : IElementContainer<TElement?>
{
    /// <inheritdoc/>
    [Pure]
    public virtual Boolean Contains([AllowNull] TElement? item)
    {
        lock (this._syncRoot)
        {
            return Array.IndexOf(array: this._items,
                                 value: item) > -1;
        }
    }
}

// IEnumerable<T>
partial class ArrayBasedCollection<TElement> : IEnumerable<TElement?>
{
    /// <inheritdoc />
    [Pure]
    [return: NotNull]
    public IEnumerator<TElement?> GetEnumerator()
    {
        lock (this._syncRoot)
        {
            Int32 v = this._version;
            for (Int32 i = 0; i < this._size; i++)
            {
                if (this._version != v)
                {
                    NotAllowed ex = new(auxMessage: COLLECTION_CHANGED);
                    ex.Data.Add(key: "Index",
                                value: i);
                    ex.Data.Add(key: "Fixed Version",
                                value: v);
                    ex.Data.Add(key: "Altered Version",
                                value: this._version);
                    throw ex;
                }
                yield return this._items[i];
            }
        }
    }

    /// <inheritdoc />
    [Pure]
    [return: NotNull]
    IEnumerator IEnumerable.GetEnumerator() =>
        this.GetEnumerator();
}

// ISynchronized
partial class ArrayBasedCollection<TElement> : ISynchronized
{
    /// <inheritdoc />
    [Pure]
    public virtual Boolean IsSynchronized => true;

    /// <inheritdoc />
    [Pure]
    [NotNull]
    public virtual Object SyncRoot => this._syncRoot;
}

// IVersioned
partial class ArrayBasedCollection<TElement> : IVersioned
{
    Int32 IVersioned.Version 
    { 
        get => this._version;
        set => this._version = value; 
    }
}