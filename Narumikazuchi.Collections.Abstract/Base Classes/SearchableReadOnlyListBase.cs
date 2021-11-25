namespace Narumikazuchi.Collections.Abstract;

/// <summary>
/// Represents a strongly typed list of objects, which can be accessed by index and searched. 
/// </summary>
// Non-Public
public abstract partial class SearchableReadOnlyListBase<TElement> : ReadOnlyListBase<TElement>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SearchableReadOnlyListBase{T}"/> class.
    /// </summary>
    protected SearchableReadOnlyListBase() : 
        base() 
    { }
    /// <summary>
    /// Initializes a new instance of the <see cref="SearchableReadOnlyListBase{T}"/> class containing the specified collection of items.
    /// </summary>
    /// <param name="collection">The collection of items in this list.</param>
    /// <param name="exactCapacity">Whether to resize the internal array to the exact size of the passed in collection.</param>
    /// <exception cref="ArgumentException" />
    /// <exception cref="ArgumentNullException" />
    /// <exception cref="NotAllowed" />
    protected SearchableReadOnlyListBase([DisallowNull] IEnumerable<TElement> collection,
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
#pragma warning restore
}

// ISearchableList
partial class SearchableReadOnlyListBase<TElement> : ISearchableCollection<TElement>
{
    /// <inheritdoc/>
    /// <exception cref="ArgumentNullException" />
    /// <exception cref="ArgumentOutOfRangeException" />
    [Pure]
    public virtual Boolean Exists([DisallowNull] Func<TElement, Boolean> predicate)
    {
        ExceptionHelpers.ThrowIfArgumentNull(predicate);

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
                if (predicate.Invoke(arg: this._items[i]))
                {
                    return true;
                }
            }
        }
        return false;
    }

    /// <inheritdoc/>
    /// <exception cref="ArgumentNullException" />
    [Pure]
    [return: MaybeNull]
    public virtual TElement Find([DisallowNull] Func<TElement, Boolean> predicate)
    {
        ExceptionHelpers.ThrowIfArgumentNull(predicate);

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
                if (predicate.Invoke(arg: this._items[i]))
                {
                    return this._items[i];
                }
            }
        }
        return default;
    }

    /// <inheritdoc/>
    /// <exception cref="ArgumentNullException" />
    [Pure]
    [return: NotNull]
    public virtual IReadOnlyList2<TElement> FindAll([DisallowNull] Func<TElement, Boolean> predicate)
    {
        ExceptionHelpers.ThrowIfArgumentNull(predicate);

        List<TElement> result = new();
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
                if (predicate.Invoke(arg: this._items[i]))
                {
                    result.Add(item: this._items[i]);
                }
            }
        }
        return result.AsIReadOnlyList2();
    }

    /// <inheritdoc/>
    /// <exception cref="ArgumentNullException" />
    [Pure]
    [return: NotNull]
    public virtual IReadOnlyList2<TElement> FindExcept([DisallowNull] Func<TElement, Boolean> predicate)
    {
        ExceptionHelpers.ThrowIfArgumentNull(predicate);

        List<TElement> result = new();
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
                if (!predicate.Invoke(arg: this._items[i]))
                {
                    result.Add(item: this._items[i]);
                }
            }
        }
        return result.AsIReadOnlyList2();
    }

    /// <inheritdoc/>
    /// <exception cref="ArgumentNullException" />
    [Pure]
    [return: MaybeNull]
    public virtual TElement FindLast([DisallowNull] Func<TElement, Boolean> predicate)
    {
        ExceptionHelpers.ThrowIfArgumentNull(predicate);

        lock (this._syncRoot)
        {
            Int32 v = this._version;
            for (Int32 i = this._size - 1; i >= 0; i--)
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
                 
                    return this._items[i];
                }
            }
        }
        return default;
    }
}

// ISearchableList
partial class SearchableReadOnlyListBase<TElement> : ISearchableList<TElement>
{
    /// <inheritdoc/>
    /// <exception cref="ArgumentException" />
    /// <exception cref="ArgumentNullException" />
    /// <exception cref="ArgumentOutOfRangeException" />
    /// <exception cref="NotAllowed" />
    [Pure]
    public virtual Int32 BinarySearch([DisallowNull] TElement item) => 
        this.BinarySearch(index: 0, 
                          count: this._size, 
                          item: item, 
                          comparer: null);
    /// <inheritdoc/>
    /// <exception cref="ArgumentException" />
    /// <exception cref="ArgumentNullException" />
    /// <exception cref="ArgumentOutOfRangeException" />
    /// <exception cref="NotAllowed" />
    [Pure]
    public virtual Int32 BinarySearch([DisallowNull] TElement item, 
                                      [AllowNull] IComparer<TElement>? comparer) => 
        this.BinarySearch(index: 0,
                          count: this._size,
                          item: item,
                          comparer: comparer);
    /// <inheritdoc/>
    /// <exception cref="ArgumentException" />
    /// <exception cref="ArgumentNullException" />
    /// <exception cref="ArgumentOutOfRangeException" />
    /// <exception cref="NotAllowed" />
    [Pure]
    public virtual Int32 BinarySearch(in Int32 index, 
                                      in Int32 count, 
                                      [DisallowNull] TElement item, 
                                      [AllowNull] IComparer<TElement>? comparer)
    {
        ExceptionHelpers.ThrowIfArgumentNull(item);
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
            ArgumentOutOfRangeException ex = new(nameof(index),
                                                 COUNT_IS_NEGATIVE);
            ex.Data.Add(key: "Count",
                        value: count);
            throw ex;
        }
        if (this._size - index < count)
        {
            ArgumentException ex = new(COUNT_IS_GREATER_THAN_ITEMS);
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
            return Array.BinarySearch(array: this._items, 
                                      index: index, 
                                      length: count, 
                                      value: item, 
                                      comparer: comparer);
        }
    }

    /// <inheritdoc/>
    /// <exception cref="ArgumentNullException" />
    /// <exception cref="ArgumentOutOfRangeException" />
    [Pure]
    public virtual Int32 FindIndex([DisallowNull] Func<TElement, Boolean> predicate) => 
        this.FindIndex(startIndex: 0, 
                       count: this._size, 
                       predicate: predicate);
    /// <inheritdoc/>
    /// <exception cref="ArgumentNullException" />
    /// <exception cref="ArgumentOutOfRangeException" />
    [Pure]
    public virtual Int32 FindIndex(in Int32 startIndex, 
                                   in Int32 count, 
                                   [DisallowNull] Func<TElement, Boolean> predicate)
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
    public virtual Int32 FindLastIndex([DisallowNull] Func<TElement, Boolean> predicate) => 
        this.FindLastIndex(startIndex: 0, 
                           count: this._size, 
                           predicate: predicate);
    /// <inheritdoc/>
    /// <exception cref="ArgumentNullException" />
    /// <exception cref="ArgumentOutOfRangeException" />
    [Pure]
    public virtual Int32 FindLastIndex(in Int32 startIndex, 
                                       in Int32 count, 
                                       [DisallowNull] Func<TElement, Boolean> predicate)
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

    /// <inheritdoc/>
    /// <exception cref="ArgumentNullException" />
    /// <exception cref="ArgumentOutOfRangeException" />
    [Pure]
    public virtual Int32 LastIndexOf([DisallowNull] in TElement item)
    {
        ExceptionHelpers.ThrowIfArgumentNull(item);

        if (this._size == 0)
        {
            return -1;
        }
        return Array.LastIndexOf(array: this._items,
                                 value: item,
                                 startIndex: 0,
                                 count: this._size);
    }
}