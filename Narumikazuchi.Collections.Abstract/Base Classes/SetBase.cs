namespace Narumikazuchi.Collections.Abstract;

/// <summary>
/// Represents a strongly typed collection where every object is only contained once. The procedure to check whether the object is already in the <see cref="SetBase{TElement}"/> can be specified
/// with a corresponding <see cref="IEqualityComparer{T}"/> object or a <see cref="EqualityComparison{T}"/> delegate.
/// </summary>
public abstract partial class SetBase<TElement>
{ }

// Non-Public
partial class SetBase<TElement> : ReadOnlySetBase<TElement>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SetBase{TElement}"/> class.
    /// </summary>
    protected SetBase() :
        base()
    { }
    /// <summary>
    /// Initializes a new instance of the <see cref="SetBase{TElement}"/> class containing the specified collection of items.
    /// </summary>
    /// <param name="collection">The collection of items in this list.</param>
    /// <param name="exactCapacity">Whether to resize the internal array to the exact size of the passed in collection.</param>
    /// <exception cref="ArgumentException" />
    /// <exception cref="ArgumentNullException" />
    /// <exception cref="NotAllowed" />
    protected SetBase([DisallowNull] IEnumerable<TElement?> collection,
                      Boolean exactCapacity) :
        base(collection: collection,
             comparer: null,
             exactCapacity: exactCapacity)
    { }
    /// <summary>
    /// Initializes a new instance of the <see cref="SetBase{TElement}"/> class containing the specified collection of items.
    /// </summary>
    /// <param name="collection">The collection of items in this list.</param>
    /// <param name="comparison">The delegate that is used to compare two instances of <typeparamref name="TElement"/>.</param>
    /// <param name="exactCapacity">Whether to resize the internal array to the exact size of the passed in collection.</param>
    /// <exception cref="ArgumentException" />
    /// <exception cref="ArgumentNullException" />
    /// <exception cref="NotAllowed" />
    protected SetBase([DisallowNull] IEnumerable<TElement?> collection,
                      [DisallowNull] EqualityComparison<TElement?> comparison,
                      Boolean exactCapacity) :
         base(collection: collection,
              comparer: new __FuncEqualityComparer<TElement?>(comparison),
              exactCapacity: exactCapacity)
    { }
    /// <summary>
    /// Initializes a new instance of the <see cref="SetBase{TElement}"/> class containing the specified collection of items.
    /// </summary>
    /// <param name="collection">The collection of items in this list.</param>
    /// <param name="comparer">The comparer that is used to compare two instances of <typeparamref name="TElement"/>.</param>
    /// <param name="exactCapacity">Whether to resize the internal array to the exact size of the passed in collection.</param>
    /// <exception cref="ArgumentException" />
    /// <exception cref="ArgumentNullException" />
    /// <exception cref="NotAllowed" />
    protected SetBase([DisallowNull] IEnumerable<TElement?> collection,
                      [AllowNull] IEqualityComparer<TElement?>? comparer,
                      Boolean exactCapacity) :
        base(collection: collection,
              comparer: comparer,
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
partial class SetBase<TElement> : IContentAddable<TElement?>
{
    /// <inheritdoc/>
    /// <exception cref="ArgumentNullException"/>
    /// <exception cref="NotAllowed"/>
    public virtual Boolean Add([AllowNull] TElement? item) =>
        !this.Contains(item) &&
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
                                            collection.Except(this));
}

// ICollection<T>
partial class SetBase<TElement> : ICollection<TElement?>
{
    /// <exception cref="ArgumentNullException" />
    /// <exception cref="NotAllowed" />
    void ICollection<TElement?>.Add([AllowNull] TElement? item) =>
        this.Add(item: item);
}

// IContentRemovable<T>
partial class SetBase<TElement> : IContentRemovable<TElement?>
{
    /// <summary>
    /// Removes all elements from the <see cref="CollectionBase{T}"/>.
    /// </summary>
    /// <exception cref="NotAllowed" />
    public virtual void Clear() =>
        IContentRemovable<TElement?>.Clear(this);

    /// <summary>
    /// Removes the first occurrence of a specific object from the <see cref="CollectionBase{T}"/>.
    /// </summary>
    /// <param name="item">The object to remove from the <see cref="CollectionBase{T}"/>. The value can be null for reference types.</param>
    /// <returns>
    /// <see langword="true"/> if item is successfully removed; otherwise, <see langword="false"/>.
    /// This method also returns <see langword="false"/> if item was not found in the original <see cref="CollectionBase{T}"/>.
    /// </returns>
    /// <exception cref="ArgumentNullException" />
    /// <exception cref="NotAllowed" />
    public virtual Boolean Remove([AllowNull] TElement? item) =>
        IContentRemovable<TElement?>.Remove(this,
                                            item);

    /// <summary>
    /// Removes all items from the <see cref="CollectionBase{T}"/> that match the specified condition.
    /// </summary>
    /// <param name="predicate">The condition to determine if an item should be removed.</param>
    /// <returns>The number of items removed from the list</returns>
    /// <exception cref="ArgumentNullException" />
    /// <exception cref="IndexOutOfRangeException" />
    /// <exception cref="NotAllowed" />
    public virtual Int32 RemoveAll([DisallowNull] Func<TElement?, Boolean> predicate) =>
        IContentRemovable<TElement?>.RemoveAll(this,
                                               predicate);
}

// ISet<T>
partial class SetBase<TElement> : ISet<TElement?>
{
    /// <inheritdoc/>
    /// <exception cref="ArgumentNullException"/>
    public virtual void ExceptWith([DisallowNull] IEnumerable<TElement?> other)
    {
        ExceptionHelpers.ThrowIfArgumentNull(other);
        if (this.IsReadOnly)
        {
            throw new NotAllowed(auxMessage: COLLECTION_IS_READONLY);
        }

        if (this.Count == 0)
        {
            return;
        }

        if (other == this)
        {
            this.Clear();
            return;
        }

        foreach (TElement? item in other)
        {
            this.Remove(item: item);
        }
    }

    /// <inheritdoc/>
    /// <exception cref="ArgumentNullException"/>
    public virtual void IntersectWith([DisallowNull] IEnumerable<TElement?> other)
    {
        ExceptionHelpers.ThrowIfArgumentNull(other);
        if (this.IsReadOnly)
        {
            throw new NotAllowed(auxMessage: COLLECTION_IS_READONLY);
        }

        if (other == this)
        {
            return;
        }

        if (!other.Any())
        {
            this.Clear();
            return;
        }

        BitArray arr = new(length: this._size);

        foreach (TElement? item in other)
        {
            Int32 index = Array.IndexOf(array: this._items,
                                        value: item);
            if (index >= 0)
            {
                arr.Set(index: index,
                        value: true);
            }
        }

        for (Int32 i = this._size - 1; i > 0; i--)
        {
            if (!arr.Get(index: i))
            {
                lock (this._syncRoot)
                {
                    this._size--;
                    if (i < this._size)
                    {
                        Array.Copy(sourceArray: this._items,
                                   sourceIndex: i + 1,
                                   destinationArray: this._items,
                                   destinationIndex: i,
                                   length: this._size - i);
                    }
                    this._items[this._size] = default;
                    this._version++;
                }
            }
        }
    }

    /// <inheritdoc/>
    /// <exception cref="ArgumentNullException"/>
    public virtual void SymmetricExceptWith([DisallowNull] IEnumerable<TElement?> other)
    {
        ExceptionHelpers.ThrowIfArgumentNull(other);
        if (this.IsReadOnly)
        {
            throw new NotAllowed(auxMessage: COLLECTION_IS_READONLY);
        }

        if (this.Count == 0)
        {
            this.UnionWith(other: other);
            return;
        }

        if (other == this)
        {
            this.Clear();
            return;
        }

        Int32 current = this._size;
        BitArray toRemove = new(length: current);
        List<Int32> added = new();

        foreach (TElement? item in other)
        {
            Int32 index = Array.IndexOf(array: this._items,
                                        value: item);
            if (index >= 0 &&
                !added.Contains(item: index))
            {
                toRemove.Set(index: index,
                             value: true);
                continue;
            }
            added.Add(item: this._size);
            this.Add(item: item);
        }

        for (Int32 i = current - 1; i > 0; i--)
        {
            if (toRemove.Get(index: i))
            {
                lock (this._syncRoot)
                {
                    this._size--;
                    if (i < this._size)
                    {
                        Array.Copy(sourceArray: this._items,
                                   sourceIndex: i + 1,
                                   destinationArray: this._items,
                                   destinationIndex: i,
                                   length: this._size - i);
                    }
                    this._items[this._size] = default;
                    this._version++;
                }
            }
        }
    }

    /// <inheritdoc/>
    /// <exception cref="ArgumentNullException"/>
    public virtual void UnionWith([DisallowNull] IEnumerable<TElement?> other)
    {
        ExceptionHelpers.ThrowIfArgumentNull(other);
        if (this.IsReadOnly)
        {
            throw new NotAllowed(auxMessage: COLLECTION_IS_READONLY);
        }

        if (other == this)
        {
            return;
        }

        foreach (TElement? item in other)
        {
            this.Add(item: item);
        }
    }
}