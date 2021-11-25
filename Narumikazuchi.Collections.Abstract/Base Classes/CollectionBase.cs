namespace Narumikazuchi.Collections.Abstract;

/// <summary>
/// Represents a strongly typed collection of objects. 
/// </summary>
public abstract partial class CollectionBase<TElement> : ReadOnlyCollectionBase<TElement>
{
    /// <summary>
    /// Adds the elements of the specified collection to the end of the <see cref="CollectionBase{T}"/>.
    /// </summary>
    /// <param name="collection">The collection of items to add.</param>
    /// <exception cref="ArgumentNullException" />
    /// <exception cref="ArgumentOutOfRangeException" />
    /// <exception cref="NotAllowed" />
    public virtual void AddRange([DisallowNull] IEnumerable<TElement> collection)
    {
        ExceptionHelpers.ThrowIfArgumentNull(collection);
        if (this.IsReadOnly)
        {
            throw new NotAllowed(auxMessage: COLLECTION_IS_READONLY);
        }

        if (collection is ICollection<TElement> c)
        {
            Int32 count = c.Count;
            if (count > 0)
            {
                this.EnsureCapacity(capacity: this.Count + count);

                if (ReferenceEquals(objA: this, 
                                    objB: c))
                {
                    lock (this._syncRoot)
                    {
                        Array.Copy(sourceArray: this._items, 
                                   sourceIndex: 0, 
                                   destinationArray: this._items, 
                                   destinationIndex: this._size, 
                                   length: this._size);
                    }
                }
                else
                {
                    TElement[] insert = new TElement[count];
                    c.CopyTo(array: insert, 
                             arrayIndex: 0);
                    lock (this._syncRoot)
                    {
                        insert.CopyTo(array: this._items,
                                      index: this._size);
                    }
                }

                lock (this._syncRoot)
                {
                    this._size += count;
                }
            }
        }
        else
        {
            using IEnumerator<TElement> enumerator = collection.GetEnumerator();
            while (enumerator.MoveNext())
            {
                if (enumerator.Current is null)
                {
                    continue;
                }
                this.Add(item: enumerator.Current);
            }
        }
        lock (this._syncRoot)
        {
            this._version++;
        }
    }

    /// <summary>
    /// Removes all items from the <see cref="CollectionBase{T}"/> that match the specified condition.
    /// </summary>
    /// <param name="predicate">The condition to determine if an item should be removed.</param>
    /// <returns>The number of items removed from the list</returns>
    /// <exception cref="ArgumentNullException" />
    /// <exception cref="IndexOutOfRangeException" />
    /// <exception cref="NotAllowed" />
    public virtual Int32 RemoveAll([DisallowNull] Func<TElement?, Boolean> predicate)
    {
        ExceptionHelpers.ThrowIfArgumentNull(predicate);
        if (this.IsReadOnly)
        {
            throw new NotAllowed(auxMessage: COLLECTION_IS_READONLY);
        }

        lock (this._syncRoot)
        {
            Int32 v = this._version;
            Int32 free = 0;
            while (free < this._size &&
                   !predicate.Invoke(arg: this._items[free]))
            {
                if (this._version != v)
                {
                    NotAllowed ex = new(auxMessage: COLLECTION_CHANGED);
                    ex.Data.Add(key: "Fixed Version",
                                value: v);
                    ex.Data.Add(key: "Altered Version",
                                value: this._version);

                    throw ex;
                }
                free++;
            }
            if (free >= this._size)
            {
                return 0;
            }

            Int32 current = free + 1;
            while (current < this._size)
            {
                if (this._version != v)
                {
                    NotAllowed ex = new(auxMessage: COLLECTION_CHANGED);
                    ex.Data.Add(key: "Fixed Version",
                                value: v);
                    ex.Data.Add(key: "Altered Version",
                                value: this._version);
                    throw ex;
                }
                while (current < this._size &&
                       predicate(arg: this._items[current]))
                {
                    current++;
                }

                if (current < this._size)
                {
                    this._items[free++] = this._items[current++];
                }
            }

            Array.Clear(array: this._items, 
                        index: free, 
                        length: this._size - free);
            Int32 result = this._size - free;
            this._size = free;
            this._version++;
            return result;
        }
    }

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
partial class CollectionBase<TElement>
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
    protected CollectionBase([DisallowNull] IEnumerable<TElement> collection) : 
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
    protected CollectionBase([DisallowNull] IEnumerable<TElement> collection,
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

// ICollection
partial class CollectionBase<TElement> : ICollection<TElement>
{
    /// <summary>
    /// Copies the entire <see cref="CollectionBase{T}"/> to a compatible one-dimensional <see cref="Array"/>, 
    /// starting at the specified index of the target array.
    /// </summary>
    /// <param name="array">
    /// The one-dimensional <see cref="Array"/> that is the destination of the elements copied
    /// from <see cref="CollectionBase{T}"/>. The <see cref="Array"/> must have zero-based indexing.</param>
    /// <param name="index">The zero-based index in array at which copying begins.</param>
    /// <exception cref="ArgumentException" />
    /// <exception cref="ArgumentNullException" />
    /// <exception cref="ArgumentOutOfRangeException" />
    [Pure]
    public virtual void CopyTo([DisallowNull] Array array, 
                               Int32 index)
    {
        ExceptionHelpers.ThrowIfArgumentNull(array);

        if (array is TElement[] arr)
        {
            this.CopyTo(array: arr, 
                        index: index);
        }
    }

    /// <summary>
    /// Adds an object to the end of the <see cref="CollectionBase{T}"/>.
    /// </summary>
    /// <param name="item">The object to be added to the end of the <see cref="CollectionBase{T}"/>. The value can be null for reference types.</param>
    /// <exception cref="ArgumentNullException" />
    /// <exception cref="NotAllowed" />
    public virtual void Add([DisallowNull] TElement item)
    {
        ExceptionHelpers.ThrowIfArgumentNull(item);
        if (this.IsReadOnly)
        {
            throw new NotAllowed(auxMessage: COLLECTION_IS_READONLY);
        }

        this.EnsureCapacity(capacity: this.Count + 1);
        lock (this._syncRoot)
        {
            this._items[this._size++] = item;
            this._version++;
        }
    }

    /// <summary>
    /// Removes all elements from the <see cref="CollectionBase{T}"/>.
    /// </summary>
    /// <exception cref="NotAllowed" />
    public virtual void Clear()
    {
        if (this.IsReadOnly)
        {
            throw new NotAllowed(auxMessage: COLLECTION_IS_READONLY);
        }

        lock (this._syncRoot)
        {
            if (this._size > 0)
            {
                Array.Clear(array: this._items, 
                            index: 0, 
                            length: this._size);
                this._size = 0;
            }
            this._version++;
        }
    }

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
    public virtual Boolean Remove([DisallowNull] TElement item)
    {
        ExceptionHelpers.ThrowIfArgumentNull(item);
        if (this.IsReadOnly)
        {
            throw new NotAllowed(auxMessage: COLLECTION_IS_READONLY);
        }

        lock (this._syncRoot)
        {
            Int32 index = Array.IndexOf(array: this._items, 
                                        value: item);
            if (index > -1)
            {
                this._size--;
                if (index < this._size)
                {
                    Array.Copy(sourceArray: this._items, 
                               sourceIndex: index + 1, 
                               destinationArray: this._items, 
                               destinationIndex: index, 
                               length: this._size - index);
                }
                this._items[this._size] = default;
                this._version++;
                return true;
            }
            return false;
        }
    }

    /// <inheritdoc />
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public virtual Boolean IsSynchronized => true;

    /// <inheritdoc />
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public override Boolean IsReadOnly { get; } = false;
}