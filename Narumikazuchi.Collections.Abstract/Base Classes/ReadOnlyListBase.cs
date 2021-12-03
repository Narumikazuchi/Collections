namespace Narumikazuchi.Collections.Abstract;

/// <summary>
/// Represents a strongly typed list of objects, which can be accessed by index. 
/// </summary>
public abstract partial class ReadOnlyListBase<TElement>
{
    /// <summary>
    /// Copies a section of the <see cref="ReadOnlyListBase{T}"/> to the specified one-dimensional array.
    /// The section starts at the specified index, entails the specified count of items and begins inserting them
    /// at the specified starting index into the array.
    /// </summary>
    /// <param name="index">The index of the first item of this list to copy.</param>
    /// <param name="count">The amount of items to copy from this list.</param>
    /// <param name="array">An array with a fitting size to copy the items into.</param>
    /// <param name="arrayIndex">The index at which to start inserting items into the array.</param>
    /// <exception cref="ArgumentException" />
    /// <exception cref="ArgumentNullException" />
    /// <exception cref="ArgumentOutOfRangeException" />
    [Pure]
    public virtual void CopyTo(in Int32 index, 
                               in Int32 count, 
                               [DisallowNull] TElement[] array, 
                               in Int32 arrayIndex)
    {
        ExceptionHelpers.ThrowIfArgumentNull(array);
        if (array.Rank != 1)
        {
            throw new ArgumentException(message: MULTI_DIMENSIONAL_ARRAYS);
        }

        lock (this._syncRoot)
        {
            Array.Copy(sourceArray: this._items, 
                       sourceIndex: index, 
                       destinationArray: array, 
                       destinationIndex: arrayIndex, 
                       length: count);
        }
    }
}

// Non-Public
partial class ReadOnlyListBase<TElement> : ReadOnlyCollectionBase<TElement>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ReadOnlyListBase{T}"/> class.
    /// </summary>
    protected ReadOnlyListBase() : 
        base() 
    { }
    /// <summary>
    /// Initializes a new instance of the <see cref="ReadOnlyListBase{T}"/> class containing the specified collection of items.
    /// </summary>
    /// <param name="collection">The collection of items in this list.</param>
    /// <param name="exactCapacity">Whether to resize the internal array to the exact size of the passed in collection.</param>
    /// <exception cref="ArgumentException" />
    /// <exception cref="ArgumentNullException" />
    /// <exception cref="NotAllowed" />
    protected ReadOnlyListBase([DisallowNull] IEnumerable<TElement?> collection,
                               Boolean exactCapacity) : 
        base(collection: collection,
             exactCapacity: exactCapacity) 
    { }

#pragma warning disable
    /// <summary>
    /// Error message, when trying to write to a readonly list.
    /// </summary>
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    protected const String LIST_IS_READONLY = "This list is readonly and can't be written to.";
    /// <summary>
    /// Error message, when trying to write to a readonly list.
    /// </summary>
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    protected const String START_INDEX_IS_NEGATIVE = "The start index can't be negative.";
    /// <summary>
    /// Error message, when trying to write to a readonly list.
    /// </summary>
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    protected const String COUNT_IS_NEGATIVE = "The count can't be negative.";
    /// <summary>
    /// Error message, when trying to write to a readonly list.
    /// </summary>
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    protected const String COUNT_IS_GREATER_THAN_ITEMS = "The specified count is greater than the available number of items.";
    /// <summary>
    /// Error message, when the index parameter is greater than the amount of items in the collection.
    /// </summary>
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    protected const String INDEX_GREATER_THAN_COUNT = "The specified index is greater than the amount of items in the list.";
#pragma warning restore
}

// IBinarySearchable<T, U>
partial class ReadOnlyListBase<TElement> : IBinarySearchable<Int32, TElement?>
{
    /// <inheritdoc/>
    /// <exception cref="ArgumentException" />
    /// <exception cref="ArgumentNullException" />
    /// <exception cref="ArgumentOutOfRangeException" />
    /// <exception cref="NotAllowed" />
    [Pure]
    public virtual Int32 BinarySearch([AllowNull] TElement? item) =>
        this.BinarySearch(startIndex: 0,
                          endIndex: this._size,
                          item: item,
                          comparer: null);
    /// <inheritdoc/>
    /// <exception cref="ArgumentException" />
    /// <exception cref="ArgumentNullException" />
    /// <exception cref="ArgumentOutOfRangeException" />
    /// <exception cref="NotAllowed" />
    [Pure]
    public virtual Int32 BinarySearch([AllowNull] TElement? item,
                                      [AllowNull] IComparer<TElement?>? comparer) =>
        this.BinarySearch(startIndex: 0,
                          endIndex: this._size,
                          item: item,
                          comparer: comparer);
    /// <inheritdoc/>
    /// <exception cref="ArgumentException" />
    /// <exception cref="ArgumentNullException" />
    /// <exception cref="ArgumentOutOfRangeException" />
    /// <exception cref="NotAllowed" />
    [Pure]
    public virtual Int32 BinarySearch(in Int32 startIndex,
                                      in Int32 endIndex,
                                      [AllowNull] TElement? item,
                                      [AllowNull] IComparer<TElement?>? comparer)
    {
        if (startIndex < 0)
        {
            ArgumentOutOfRangeException ex = new(paramName: nameof(startIndex),
                                                 message: START_INDEX_IS_NEGATIVE);
            ex.Data.Add(key: "Index",
                        value: startIndex);
            ex.Data.Add(key: "Count",
                        value: endIndex);
            ex.Data.Add(key: "Collection Size",
                        value: this._size);
            throw ex;
        }
        if (endIndex < 0 ||
            endIndex < startIndex)
        {
            ArgumentOutOfRangeException ex = new(paramName: nameof(startIndex),
                                                 message: COUNT_IS_NEGATIVE);
            ex.Data.Add(key: "Count",
                        value: endIndex);
            throw ex;
        }
        if (this._size < endIndex)
        {
            ArgumentException ex = new(message: COUNT_IS_GREATER_THAN_ITEMS);
            ex.Data.Add(key: "Index",
                        value: startIndex);
            ex.Data.Add(key: "Count",
                        value: endIndex);
            ex.Data.Add(key: "Collection Size",
                        value: this._size);
            throw ex;
        }

        lock (this._syncRoot)
        {
            Int32 result = Array.BinarySearch(array: this._items,
                                              index: startIndex,
                                              length: endIndex,
                                              value: item,
                                              comparer: comparer);
            if (result >= this._size)
            {
                return -1;
            }
            return result;
        }
    }
}

// IContentCopyable<T, U>
partial class ReadOnlyListBase<TElement> : IContentCopyable<Int32, TElement?[]>
{
    /// <summary>
    /// Copies the entire <see cref="ReadOnlyListBase{T}"/> to the specified one-dimensional array.
    /// </summary>
    /// <param name="array">An array with a fitting size to copy the items into.</param>
    /// <exception cref="ArgumentException" />
    /// <exception cref="ArgumentNullException" />
    /// <exception cref="ArgumentOutOfRangeException" />
    [Pure]
    public virtual void CopyTo([DisallowNull] TElement?[] array) =>
        this.CopyTo(array,
                    0);
}

// IContentSegmentable<T, U>
partial class ReadOnlyListBase<TElement> : IContentSegmentable<Int32, TElement?>
{
    /// <inheritdoc/>
    /// <exception cref="ArgumentException" />
    /// <exception cref="ArgumentOutOfRangeException" />
    [Pure]
    [return: NotNull]
    public virtual ICollection<TElement?> GetRange(in Int32 index,
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
            Int32 v = this._version;
            Collection<TElement?> result = new();
            Int32 end = index + count;

            for (Int32 i = index; i < end; i++)
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
                result.Add(item: this._items[i]);
            }
            return result;
        }
    }
}

// IIndexFinder<T, U>
partial class ReadOnlyListBase<TElement> : IIndexFinder<Int32, TElement?>
{
    /// <inheritdoc/>
    /// <exception cref="ArgumentNullException" />
    /// <exception cref="ArgumentOutOfRangeException" />
    [Pure]
    public virtual Int32 FindIndex([DisallowNull] Func<TElement?, Boolean> predicate) =>
        this.FindIndex(startIndex: 0,
                       count: this._size,
                       predicate: predicate);
    /// <inheritdoc/>
    /// <exception cref="ArgumentNullException" />
    /// <exception cref="ArgumentOutOfRangeException" />
    [Pure]
    public virtual Int32 FindIndex(in Int32 startIndex,
                                   in Int32 count,
                                   [DisallowNull] Func<TElement?, Boolean> predicate)
    {
        ExceptionHelpers.ThrowIfArgumentNull(predicate);
        if ((UInt32)startIndex > (UInt32)this._size)
        {
            ArgumentOutOfRangeException ex = new(paramName: nameof(startIndex),
                                                 message: INDEX_GREATER_THAN_COUNT);
            ex.Data.Add(key: "Index",
                        value: startIndex);
            ex.Data.Add(key: "Collection Size",
                        value: this._size);
            throw ex;
        }
        if (count < 0 ||
            startIndex > this.Count - count)
        {
            ArgumentException ex = new(message: COUNT_IS_GREATER_THAN_ITEMS);
            ex.Data.Add(key: "Index",
                        value: startIndex);
            ex.Data.Add(key: "Count",
                        value: count);
            ex.Data.Add(key: "Collection Size",
                        value: this._size);
            throw ex;
        }
        lock (this._syncRoot)
        {
            Int32 v = this._version;
            Int32 end = startIndex + count;
            for (Int32 i = startIndex; i < end; i++)
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
                if (predicate.Invoke(arg: this._items[i]))
                {
                    return i;
                }
            }
            return -1;
        }
    }

    /// <inheritdoc/>
    /// <exception cref="ArgumentNullException" />
    /// <exception cref="ArgumentOutOfRangeException" />
    [Pure]
    public virtual Int32 FindLastIndex([DisallowNull] Func<TElement?, Boolean> predicate) =>
        this.FindLastIndex(startIndex: 0,
                           count: this._size,
                           predicate: predicate);
    /// <inheritdoc/>
    /// <exception cref="ArgumentNullException" />
    /// <exception cref="ArgumentOutOfRangeException" />
    [Pure]
    public virtual Int32 FindLastIndex(in Int32 startIndex,
                                       in Int32 count,
                                       [DisallowNull] Func<TElement?, Boolean> predicate)
    {
        ExceptionHelpers.ThrowIfArgumentNull(predicate);
        if ((UInt32)startIndex > (UInt32)this._size)
        {
            ArgumentOutOfRangeException ex = new(paramName: nameof(startIndex),
                                                 message: INDEX_GREATER_THAN_COUNT);
            ex.Data.Add(key: "Index",
                        value: startIndex);
            ex.Data.Add(key: "Collection Size",
                        value: this._size);
            throw ex;
        }
        if (count < 0 ||
            startIndex > this.Count - count)
        {
            ArgumentException ex = new(message: COUNT_IS_GREATER_THAN_ITEMS);
            ex.Data.Add(key: "Index",
                        value: startIndex);
            ex.Data.Add(key: "Count",
                        value: count);
            ex.Data.Add(key: "Collection Size",
                        value: this._size);
            throw ex;
        }

        if (this.Count == 0)
        {
            return -1;
        }

        lock (this._syncRoot)
        {
            Int32 v = this._version;
            Int32 end = startIndex - count;
            for (Int32 i = startIndex; i > end; i--)
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
                if (predicate.Invoke(arg: this._items[i]))
                {
                    return i;
                }
            }
            return -1;
        }
    }
}

// IIndexedReadOnlyCollection<T>
partial class ReadOnlyListBase<TElement> : IIndexedReadOnlyCollection<Int32>
{
    /// <inheritdoc />
    /// <exception cref="ArgumentNullException" />
    /// <exception cref="ArgumentOutOfRangeException" />
    [Pure]
    Int32 IIndexedReadOnlyCollection<Int32>.IndexOf([AllowNull] Object? item)
    {
        if (item is not TElement element)
        {
            return -1;
        }

        lock (this._syncRoot)
        {
            Int32 result = Array.IndexOf(array: this._items,
                                         value: element,
                                         startIndex: 0,
                                         count: this._size);
            if (result >= this._size)
            {
                return -1;
            }
            return result;
        }
    }

    /// <inheritdoc />
    /// <exception cref="ArgumentNullException" />
    /// <exception cref="ArgumentOutOfRangeException" />
    [Pure]
    Int32 IIndexedReadOnlyCollection<Int32>.LastIndexOf([AllowNull] Object? item)
    {
        ExceptionHelpers.ThrowIfArgumentNull(item);

        if (item is not TElement element)
        {
            return -1;
        }

        lock (this._syncRoot)
        {
            Int32 result = Array.LastIndexOf(array: this._items,
                                             value: element,
                                             startIndex: 0,
                                             count: this._size);
            if (result >= this._size)
            {
                return -1;
            }
            return result;
        }
    }

    /// <inheritdoc />
    /// <exception cref="IndexOutOfRangeException" />
    /// <exception cref="NotAllowed" />
    [MaybeNull]
    Object? IIndexedReadOnlyCollection<Int32>.this[Int32 index] => this[index];
}

// IIndexedReadOnlyCollection<T, U>
partial class ReadOnlyListBase<TElement> : IIndexedReadOnlyCollection<Int32, TElement?>
{
    /// <inheritdoc />
    /// <exception cref="ArgumentNullException" />
    /// <exception cref="ArgumentOutOfRangeException" />
    [Pure]
    public virtual Int32 IndexOf([AllowNull] TElement? item)
    {
        lock (this._syncRoot)
        {
            Int32 result = Array.IndexOf(array: this._items,
                                         value: item,
                                         startIndex: 0,
                                         count: this._size);
            if (result >= this._size)
            {
                return -1;
            }
            return result;
        }
    }

    /// <inheritdoc />
    /// <exception cref="ArgumentNullException" />
    /// <exception cref="ArgumentOutOfRangeException" />
    [Pure]
    public virtual Int32 LastIndexOf([AllowNull] TElement? item)
    {
        lock (this._syncRoot)
        {
            Int32 result = Array.LastIndexOf(array: this._items,
                                             value: item,
                                             startIndex: 0,
                                             count: this._size);
            if (result >= this._size)
            {
                return -1;
            }
            return result;
        }
    }
}

// IReadOnlyList<T>
partial class ReadOnlyListBase<TElement> : IReadOnlyList<TElement?>
{
    /// <inheritdoc />
    /// <exception cref="IndexOutOfRangeException" />
    /// <exception cref="NotSupportedException" />
    [MaybeNull]
    public virtual TElement? this[Int32 index]
    {
        get
        {
            lock (this._syncRoot)
            {
                return (UInt32)index >= (UInt32)this._size
                            ? throw new IndexOutOfRangeException()
                            : this._items[index];
            }
        }
        set => throw new NotSupportedException(message: LIST_IS_READONLY);
    }
}