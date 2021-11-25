namespace Narumikazuchi.Collections.Abstract;

/// <summary>
/// Represents a strongly typed collection of objects, which can be searched. 
/// </summary>
// Non-Public
public abstract partial class SearchableReadOnlyCollectionBase<TElement> : ReadOnlyCollectionBase<TElement>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SearchableReadOnlyCollectionBase{T}"/> class.
    /// </summary>
    protected SearchableReadOnlyCollectionBase() : 
        base() 
    { }
    /// <summary>
    /// Initializes a new instance of the <see cref="SearchableReadOnlyCollectionBase{T}"/> class containing the specified collection of items.
    /// </summary>
    /// <param name="collection">The collection of items in this list.</param>
    /// <param name="exactCapacity">Whether to resize the internal array to the exact size of the passed in collection.</param>
    /// <exception cref="ArgumentException" />
    /// <exception cref="ArgumentNullException" />
    /// <exception cref="NotAllowed" />
    protected SearchableReadOnlyCollectionBase([DisallowNull] IEnumerable<TElement> collection,
                                               Boolean exactCapacity) :
        base(collection: collection,
             exactCapacity: exactCapacity) 
    { }
}

// ISearchableCollection
partial class SearchableReadOnlyCollectionBase<TElement> : ISearchableCollection<TElement>
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