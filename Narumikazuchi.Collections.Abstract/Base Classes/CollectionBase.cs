namespace Narumikazuchi.Collections.Abstract;

/// <summary>
/// Represents a strongly typed collection of objects. 
/// </summary>
public abstract partial class CollectionBase<TElement>
{
    /// <summary>
    /// Reverses the order of items in the entire <see cref="CollectionBase{T}"/>.
    /// </summary>
    /// <exception cref="ArgumentException" />
    /// <exception cref="ArgumentOutOfRangeException" />
    /// <exception cref="NotAllowed" />
    public virtual void Reverse()
    {
        if (this.IsReadOnly)
        {
            throw new NotAllowed(auxMessage: COLLECTION_IS_READONLY);
        }

        lock (this._syncRoot)
        {
            Array.Reverse(array: this._items, 
                          index: 0, 
                          length: this._size);
            this._version++;
        }
    }
}

// Non-Public
partial class CollectionBase<TElement> : ReadOnlyCollectionBase<TElement>
{
    /// <summary>
    /// Initializes a new empty instance of the <see cref="CollectionBase{T}"/> class.
    /// </summary>
    protected CollectionBase() : 
        base() 
    { }
    /// <summary>
    /// Initializes a new instance of the <see cref="CollectionBase{T}"/> class having the specified collection of items.
    /// </summary>
    /// <param name="collection">The initial collection of items in this list.</param>
    /// <exception cref="ArgumentException" />
    /// <exception cref="ArgumentNullException" />
    /// <exception cref="NotAllowed" />
    protected CollectionBase([DisallowNull] IEnumerable<TElement?> collection) : 
        base(collection: collection,
             exactCapacity: false) 
    { }
    /// <summary>
    /// Initializes a new instance of the <see cref="CollectionBase{T}"/> class having the specified collection of items.
    /// </summary>
    /// <param name="collection">The initial collection of items in this list.</param>
    /// <param name="exactCapacity">Whether to resize the internal array to the exact size of the passed in collection.</param>
    /// <exception cref="ArgumentException" />
    /// <exception cref="ArgumentNullException" />
    /// <exception cref="NotAllowed" />
    protected CollectionBase([DisallowNull] IEnumerable<TElement?> collection,
                             Boolean exactCapacity) :
        base(collection: collection,
             exactCapacity: exactCapacity)
    { }

#pragma warning disable
    /// <summary>
    /// Error message, when trying to write to a readonly list.
    /// </summary>
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    protected const String COLLECTION_IS_READONLY = "This collection is readonly and can't be written to.";
#pragma warning restore
}

// IContentAddable<T>
partial class CollectionBase<TElement> : IContentAddable<TElement?>
{
    /// <inheritdoc />
    /// <exception cref="ArgumentNullException" />
    /// <exception cref="NotAllowed" />
    public virtual Boolean Add([AllowNull] TElement? item) =>
        IContentAddable<TElement?>.Add(this,
                            item);

    /// <summary>
    /// Adds the elements of the specified collection to the end of the <see cref="CollectionBase{T}"/>.
    /// </summary>
    /// <param name="collection">The collection of items to add.</param>
    /// <exception cref="ArgumentNullException" />
    /// <exception cref="ArgumentOutOfRangeException" />
    /// <exception cref="NotAllowed" />
    public virtual void AddRange([DisallowNull] IEnumerable<TElement?> collection) =>
        IContentAddable<TElement?>.AddRange(this,
                                            collection);
}

// ICollection<T>
partial class CollectionBase<TElement> : ICollection<TElement?>
{
    void ICollection<TElement?>.Add([AllowNull] TElement? item) =>
        IContentAddable<TElement?>.Add(this,
                            item);

    /// <inheritdoc />
    [Pure]
    public override Boolean IsReadOnly { get; } = false;
}

// IContentRemovable<T>
partial class CollectionBase<TElement> : IContentRemovable<TElement?>
{
    /// <summary>
    /// Removes all elements from the <see cref="CollectionBase{T}"/>.
    /// </summary>
    /// <exception cref="NotAllowed" />
    public virtual void Clear() =>
        IContentRemovable<TElement?>.Clear(this);

    /// <inheritdoc />
    /// <exception cref="ArgumentNullException" />
    /// <exception cref="NotAllowed" />
    public virtual Boolean Remove([AllowNull] TElement? item) =>
        IContentRemovable<TElement?>.Remove(this,
                                            item);

    /// <inheritdoc />
    /// <exception cref="ArgumentNullException" />
    /// <exception cref="IndexOutOfRangeException" />
    /// <exception cref="NotAllowed" />
    public virtual Int32 RemoveAll([DisallowNull] Func<TElement?, Boolean> predicate) =>
        IContentRemovable<TElement?>.RemoveAll(this,
                                               predicate);
}