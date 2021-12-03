namespace Narumikazuchi.Collections.Abstract;

/// <summary>
/// Represents a strongly typed list of objects, which can be accessed by index. 
/// </summary>
public abstract partial class ListBase<TElement>
{
    /// <summary>
    /// Inserts the items from the specified collection into this <see cref="ListBase{T}"/> starting at the specified index.
    /// </summary>
    /// <param name="index">The index where to start inserting the new items.</param>
    /// <param name="collection">The collection of items to insert.</param>
    /// <exception cref="ArgumentNullException" />
    /// <exception cref="ArgumentOutOfRangeException" />
    /// <exception cref="NotAllowed" />
    public virtual void InsertRange(Int32 index, 
                                    [DisallowNull] IEnumerable<TElement?> collection)
    {
        ExceptionHelpers.ThrowIfArgumentNull(collection);
        if (this.IsReadOnly)
        {
            throw new NotAllowed(auxMessage: LIST_IS_READONLY);
        }
        if ((UInt32)index > (UInt32)this._size)
        {
            ArgumentOutOfRangeException ex = new(paramName: nameof(index),
                                                 message: INDEX_GREATER_THAN_COUNT);
            ex.Data.Add(key: "Index",
                        value: index);
            ex.Data.Add(key: "Collection Size",
                        value: this._size);
            throw ex;
        }

        if (collection is ICollection c)
        {
            Int32 count = c.Count;
            if (count > 0)
            {
                this.EnsureCapacity(capacity: this.Count + count);

                if (index < this.Count)
                {
                    lock (this._syncRoot)
                    {
                        Array.Copy(sourceArray: this._items, 
                                   sourceIndex: index, 
                                   destinationArray: this._items, 
                                   destinationIndex: index + count, 
                                   length: this._size - index);
                    }
                }

                if (ReferenceEquals(objA: this, 
                                    objB: c))
                {
                    lock (this._syncRoot)
                    {
                        // Copy first part of _items to insert location
                        Array.Copy(sourceArray: this._items,
                                   sourceIndex: 0,
                                   destinationArray: this._items,
                                   destinationIndex: index,
                                   length: index);
                        // Copy last part of _items back to inserted location
                        Array.Copy(sourceArray: this._items,
                                   sourceIndex: index + count,
                                   destinationArray: this._items,
                                   destinationIndex: index * 2,
                                   length: this._size - index);
                    }
                }
                else
                {
                    TElement[] insert = new TElement[count];
                    c.CopyTo(array: insert, 
                             index: 0);
                    lock (this._syncRoot)
                    {
                        insert.CopyTo(array: this._items, 
                                      index: index);
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
            using IEnumerator<TElement?> enumerator = collection.GetEnumerator();
            while (enumerator.MoveNext())
            {
                if (enumerator.Current is null)
                {
                    continue;
                }
                this.Insert(index: index++, 
                            item: enumerator.Current);
            }
        }
        lock (this._syncRoot)
        {
            this._version++;
        }
    }

    /// <summary>
    /// Moves the item at the given index one position in the specified direction in the <see cref="ListBase{T}"/>.
    /// </summary>
    public virtual Boolean MoveItem(in Int32 index,
                                    in ItemMoveDirection direction) =>
        this.MoveItem(index: index,
                      direction: direction,
                      positions: 1);

    /// <summary>
    /// Moves the item at the given index the given amount of positions in the specified direction in the <see cref="ListBase{T}"/>.
    /// </summary>
    public virtual Boolean MoveItem(in Int32 index,
                                    in ItemMoveDirection direction,
                                    Int32 positions)
    {
        TElement? tmp;
        if (direction == ItemMoveDirection.ToLowerIndex)
        {
            while (positions-- > 0)
            {
                if (index > 0)
                {
                    lock (this._syncRoot)
                    {
                        tmp = this[index];
                        this[index] = this[index - 1];
                        this[index - 1] = tmp;
                    }
                }
                else
                {
                    return false;
                }
            }
        }
        else
        {
            while (positions-- > 0)
            {
                if (index > -1 &&
                    index < this._size - 1)
                {
                    lock (this._syncRoot)
                    {
                        tmp = this[index];
                        this[index] = this[index + 1];
                        this[index + 1] = tmp;
                    }
                }
                else
                {
                    return false;
                }
            }
        }
        return true;
    }

    /// <summary>
    /// Moves the item one position in the specified direction in the <see cref="ListBase{T}"/>.
    /// </summary>
    public virtual Boolean MoveItem([AllowNull] in TElement? item,
                                    in ItemMoveDirection direction) =>
        this.MoveItem(item: item,
                      direction: direction,
                      positions: 1);

    /// <summary>
    /// Moves the item at the given index the given amount of positions in the specified direction in the <see cref="ListBase{T}"/>.
    /// </summary>
    public virtual Boolean MoveItem([AllowNull] in TElement? item,
                                    in ItemMoveDirection direction,
                                    Int32 positions)
    {
        Int32 index = this.IndexOf(item: item);
        if (index == -1)
        {
            return false;
        }
        if (direction == ItemMoveDirection.ToLowerIndex)
        {
            while (positions-- > 0)
            {
                if (index > 0)
                {
                    lock (this._syncRoot)
                    {
                        this[index] = this[index - 1];
                        this[index - 1] = item;
                    }
                    index--;
                }
                else
                {
                    return false;
                }
            }
        }
        else
        {
            while (positions-- > 0)
            {
                if (index > -1 &&
                    index < this._size - 1)
                {
                    lock (this._syncRoot)
                    {
                        this[index] = this[index + 1];
                        this[index + 1] = item;
                    }
                    index++;
                }
                else
                {
                    return false;
                }
            }
        }
        return true;
    }

    /// <summary>
    /// Removes a range of items from the <see cref="ListBase{T}"/>.
    /// </summary>
    /// <param name="index">The index of the first item to remove.</param>
    /// <param name="count">The amount of items to remove.</param>
    /// <exception cref="ArgumentException" />
    /// <exception cref="ArgumentOutOfRangeException" />
    /// <exception cref="IndexOutOfRangeException" />
    /// <exception cref="NotAllowed" />
    public virtual void RemoveRange(in Int32 index, 
                                    in Int32 count)
    {
        if (index < 0)
        {
            ArgumentOutOfRangeException ex = new(paramName: nameof(index),
                                                 message: START_INDEX_IS_NEGATIVE);
            ex.Data.Add(key: "Index",
                        value: index);
            ex.Data.Add(key: "Count",
                        value: count);
            ex.Data.Add(key: "Collection Size",
                        value: this._size);
            throw ex;
        }
        if (count < 0)
        {
            ArgumentOutOfRangeException ex = new(paramName: nameof(index),
                                                 message: COUNT_IS_NEGATIVE);
            ex.Data.Add(key: "Count",
                        value: count);
            throw ex;
        }
        if (this.IsReadOnly)
        {
            throw new NotAllowed(auxMessage: LIST_IS_READONLY);
        }
        if (this._size - index < count)
        {
            ArgumentException ex = new(message: COUNT_IS_GREATER_THAN_ITEMS);
            ex.Data.Add(key: "Index",
                        value: index);
            ex.Data.Add(key: "Count",
                        value: count);
            ex.Data.Add(key: "Collection Size",
                        value: this._size);
            throw ex;
        }

        lock (this._syncRoot)
        {
            if (count > 0)
            {
                Int32 i = this._size;
                this._size -= count;
                if (index < this._size)
                {
                    Array.Copy(sourceArray: this._items, 
                               sourceIndex: index + count, 
                               destinationArray: this._items, 
                               destinationIndex: index, 
                               length: this._size - index);
                }
                Array.Clear(array: this._items, 
                            index: this._size, 
                            length: count);
            }
            this._version++;
        }
    }

    /// <summary>
    /// Reverses the order of items in the entire <see cref="ListBase{T}"/>.
    /// </summary>
    /// <exception cref="ArgumentException" />
    /// <exception cref="ArgumentOutOfRangeException" />
    /// <exception cref="NotAllowed" />
    public virtual void Reverse() => 
        this.Reverse(index: 0, 
                     count: this._size);
    /// <summary>
    /// Reverses the order of items in the specified range in the <see cref="ListBase{T}"/>.
    /// </summary>
    /// <param name="index">The index of the first item in the range.</param>
    /// <param name="count">The length of the range to reverse.</param>
    /// <exception cref="ArgumentException" />
    /// <exception cref="ArgumentOutOfRangeException" />
    /// <exception cref="NotAllowed" />
    public virtual void Reverse(in Int32 index, 
                                in Int32 count)
    {
        if (index < 0)
        {
            ArgumentOutOfRangeException ex = new(paramName: nameof(index),
                                                 message: START_INDEX_IS_NEGATIVE);
            ex.Data.Add(key: "Index",
                        value: index);
            ex.Data.Add(key: "Count",
                        value: count);
            ex.Data.Add(key: "Collection Size",
                        value: this._size);
            throw ex;
        }
        if (count < 0)
        {
            ArgumentOutOfRangeException ex = new(paramName: nameof(index),
                                                 message: COUNT_IS_NEGATIVE);
            ex.Data.Add(key: "Count",
                        value: count);
            throw ex;
        }
        if (this.IsReadOnly)
        {
            throw new NotAllowed(auxMessage: LIST_IS_READONLY);
        }
        if (this._size - index < count)
        {
            ArgumentException ex = new(message: COUNT_IS_GREATER_THAN_ITEMS);
            ex.Data.Add(key: "Index",
                        value: index);
            ex.Data.Add(key: "Count",
                        value: count);
            ex.Data.Add(key: "Collection Size",
                        value: this._size);
            throw ex;
        }

        lock (this._syncRoot)
        {
            Array.Reverse(array: this._items, 
                          index: index, 
                          length: count);
            this._version++;
        }
    }

    /// <summary>
    /// Gets or sets the amount of items the <see cref="ListBase{T}"/> can hold without resizing itself.
    /// </summary>
    /// <exception cref="ArgumentOutOfRangeException" />
    /// <exception cref="NotAllowed" />
    [DebuggerBrowsable(DebuggerBrowsableState.Collapsed)]
    public virtual Int32 Capacity
    {
        get
        {
            lock (this._syncRoot)
            {
                return this._items.Length;
            }
        }
        set
        {
            if (this.IsFixedSize)
            {
                throw new NotAllowed(auxMessage: SIZE_IS_FIXED);
            }
            if (value < this._size)
            {
                throw new ArgumentOutOfRangeException(paramName: nameof(value),
                                                      message: CAPACITY_SMALLER_THAN_COUNT);
            }
            if (value != this.Capacity)
            {
                if (value > 0)
                {
                    TElement[] array = new TElement[value];
                    lock (this._syncRoot)
                    {
                        if (this._size > 0)
                        {
                            Array.Copy(sourceArray: this._items, 
                                       sourceIndex: 0, 
                                       destinationArray: array, 
                                       destinationIndex: 0, 
                                       length: this._size);
                        }
                        this._items = array;
                    }
                }
                else
                {
                    lock (this._syncRoot)
                    {
                        this._items = _emptyArray;
                    }
                }
            }
        }
    }
}

// Non-Public
partial class ListBase<TElement> : ReadOnlyListBase<TElement>
{
    /// <summary>
    /// Initializes a new empty instance of the <see cref="ListBase{T}"/> class.
    /// </summary>
    protected ListBase() : 
        base() 
    { }
    /// <summary>
    /// Initializes a new empty instance of the <see cref="ListBase{T}"/> class having the specified capacity.
    /// </summary>
    /// <param name="capacity">The capacity of this list.</param>
    /// <exception cref="ArgumentOutOfRangeException" />
    protected ListBase(in Int32 capacity)
    {
        if (capacity < 0)
        {
            throw new ArgumentOutOfRangeException(paramName: nameof(capacity), 
                                                  message: "The capacity of the list can't be negative.");
        }

        this._items = capacity == 0 
                            ? _emptyArray 
                            : new TElement[capacity];
    }
    /// <summary>
    /// Initializes a new instance of the <see cref="ListBase{T}"/> class having the specified collection of items.
    /// </summary>
    /// <param name="collection">The initial collection of items in this list.</param>
    /// <exception cref="ArgumentException" />
    /// <exception cref="ArgumentNullException" />
    /// <exception cref="NotAllowed" />
    protected ListBase([DisallowNull] IEnumerable<TElement?> collection) : 
        base(collection: collection,
             exactCapacity: false) 
    { }
    /// <summary>
    /// Initializes a new instance of the <see cref="ListBase{T}"/> class having the specified collection of items.
    /// </summary>
    /// <param name="collection">The initial collection of items in this list.</param>
    /// <param name="exactCapacity">Whether to resize the internal array to the exact size of the passed in collection.</param>
    /// <exception cref="ArgumentException" />
    /// <exception cref="ArgumentNullException" />
    /// <exception cref="NotAllowed" />
    protected ListBase([DisallowNull] IEnumerable<TElement?> collection,
                       Boolean exactCapacity) :
        base(collection: collection,
             exactCapacity: exactCapacity)
    { }

#pragma warning disable
    /// <summary>
    /// Error message, when the index parameter is greater than the amount of items in the collection.
    /// </summary>
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    protected const String INDEX_GREATER_THAN_COUNT = "The specified index is greater than the amount of items in the list.";
    /// <summary>
    /// Error message, when the new capacity is smaller than the amount of items in the collection.
    /// </summary>
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    protected const String CAPACITY_SMALLER_THAN_COUNT = "The specified capacity is smaller than the amount of items in the list.";
#pragma warning restore
}

// IContentAddable<T>
partial class ListBase<TElement> : IContentAddable<TElement?>
{
    /// <summary>
    /// Adds an object to the end of the <see cref="ListBase{T}"/>.
    /// </summary>
    /// <param name="item">The object to be added to the end of the <see cref="ListBase{T}"/>. The value can be null for reference types.</param>
    /// <exception cref="ArgumentNullException" />
    /// <exception cref="NotAllowed" />
    public virtual Boolean Add([AllowNull] TElement? item) =>
        IContentAddable<TElement?>.Add(this,
                                       item);

    /// <summary>
    /// Adds the elements of the specified collection to the end of the <see cref="ListBase{T}"/>.
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
partial class ListBase<TElement> : ICollection<TElement?>
{
    /// <summary>
    /// Adds an object to the end of the <see cref="ListBase{T}"/>.
    /// </summary>
    /// <param name="item">The object to be added to the end of the <see cref="ListBase{T}"/>. The value can be null for reference types.</param>
    /// <exception cref="ArgumentNullException" />
    /// <exception cref="NotAllowed" />
    void ICollection<TElement?>.Add([AllowNull] TElement? item) =>
        IContentAddable<TElement?>.Add(this,
                                       item);

    /// <inheritdoc />
    [Pure]
    public override Boolean IsReadOnly { get; } = false;
}

// IList<T>
partial class ListBase<TElement> : IList<TElement?>
{
    /// <summary>
    /// Inserts an element into the <see cref="ListBase{T}"/> at the specified index.
    /// </summary>
    /// <param name="index">The zero-based index at which item should be inserted.</param>
    /// <param name="item">The object to insert. The value can be null for reference types.</param>
    /// <exception cref="ArgumentOutOfRangeException" />
    /// <exception cref="InvalidCastException" />
    /// <exception cref="NotAllowed" />
    public virtual void Insert(Int32 index, 
                               [AllowNull] TElement? item)
    {
        if (this.IsReadOnly)
        {
            throw new NotAllowed(auxMessage: LIST_IS_READONLY);
        }
        if ((UInt32)index > (UInt32)this.Count)
        {
            ArgumentOutOfRangeException ex = new(paramName: nameof(index),
                                                 message: INDEX_GREATER_THAN_COUNT);
            ex.Data.Add(key: "Index",
                        value: index);
            ex.Data.Add(key: "Collection Size",
                        value: this._size);
            throw ex;
        }

        this.EnsureCapacity(capacity: this.Count + 1);
        lock (this._syncRoot)
        {

            if (index < this._size)
            {
                Array.Copy(sourceArray: this._items, 
                           sourceIndex: index, 
                           destinationArray: this._items, 
                           destinationIndex: index + 1, 
                           length: this._size - index);
            }
            this._items[index] = item;
            this._size++;
            this._version++;
        }
    }

    /// <summary>
    /// Removes the element at the specified index of the <see cref="ListBase{T}"/>.
    /// </summary>
    /// <param name="index">The zero-based index of the element to remove.</param>
    /// <exception cref="ArgumentOutOfRangeException" />
    /// <exception cref="NotAllowed" />
    public virtual void RemoveAt(Int32 index)
    {
        if (this.IsReadOnly)
        {
            throw new NotAllowed(auxMessage: LIST_IS_READONLY);
        }
        if ((UInt32)index > (UInt32)this.Count)
        {
            ArgumentOutOfRangeException ex = new(paramName: nameof(index),
                                                 message: INDEX_GREATER_THAN_COUNT);
            ex.Data.Add(key: "Index",
                        value: index);
            ex.Data.Add(key: "Collection Size",
                        value: this._size);
            throw ex;
        }

        lock (this._syncRoot)
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
        }
    }
    /// <inheritdoc />
    /// <exception cref="IndexOutOfRangeException" />
    /// <exception cref="NotAllowed" />
    [MaybeNull]
    public override TElement? this[Int32 index]
    {
        get => base[index];
        set
        {
            lock (this._syncRoot)
            {
                if ((UInt32)index >= (UInt32)this._size)
                {
                    throw new IndexOutOfRangeException();
                }
                this._items[index] = value;
            }
        }
    }
}

// IContentRemovable<T>
partial class ListBase<TElement> : IContentRemovable<TElement?>
{
    /// <summary>
    /// Removes all elements from the <see cref="ListBase{T}"/>.
    /// </summary>
    /// <exception cref="NotAllowed" />
    public virtual void Clear() =>
        IContentRemovable<TElement?>.Clear(this);

    /// <summary>
    /// Removes the first occurrence of a specific object from the <see cref="ListBase{T}"/>.
    /// </summary>
    /// <param name="item">The object to remove from the <see cref="ListBase{T}"/>. The value can be null for reference types.</param>
    /// <returns>
    /// <see langword="true"/> if item is successfully removed; otherwise, <see langword="false"/>.
    /// This method also returns <see langword="false"/> if item was not found in the original <see cref="ListBase{T}"/>.
    /// </returns>
    /// <exception cref="ArgumentNullException" />
    /// <exception cref="NotAllowed" />
    public virtual Boolean Remove([AllowNull] TElement? item) =>
        IContentRemovable<TElement?>.Remove(this,
                                            item);

    /// <summary>
    /// Removes all items from the <see cref="ListBase{T}"/> that match the specified condition.
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