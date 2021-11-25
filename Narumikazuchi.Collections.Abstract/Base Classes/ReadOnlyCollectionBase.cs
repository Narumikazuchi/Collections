namespace Narumikazuchi.Collections.Abstract;

/// <summary>
/// Represents a strongly typed collection of objects. 
/// </summary>
public abstract partial class ReadOnlyCollectionBase<TElement> : ArrayBasedCollection<TElement>
{
    /// <summary>
    /// Converts all elements in the <see cref="ReadOnlyCollectionBase{T}"/> into another type and returns an <see cref="IList{T}"/>
    /// containing the converted objects.
    /// </summary>
    /// <param name="converter">A delegate which converts every item into the new type.</param>
    /// <returns>An <see cref="IList{T}"/> which contains the converted objects</returns>
    /// <exception cref="ArgumentNullException" />
    [Pure]
    [return: NotNull]
    public virtual ICollection<TOutput> ConvertAll<TOutput>([DisallowNull] Converter<TElement, TOutput> converter)
    {
        ExceptionHelpers.ThrowIfArgumentNull(converter);

        List<TOutput> result = new();
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
                result.Add(item: converter.Invoke(input: this._items[i]));
            }
        }
        return result;
    }

    /// <summary>
    /// Performs the specified action for every element of this <see cref="ReadOnlyCollectionBase{T}"/>.
    /// </summary>
    /// <param name="action">The action to perform on each item.</param>
    /// <exception cref="ArgumentNullException" />
    /// <exception cref="NotAllowed" />
    [Pure]
    public virtual void ForEach([DisallowNull] Action<TElement> action)
    {
        ExceptionHelpers.ThrowIfArgumentNull(action);

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
                action.Invoke(obj: this._items[i]);
            }
        }
    }

    /// <summary>
    /// Gets a value indicating whether the <see cref="ReadOnlyCollectionBase{T}"/> is read-only.
    /// </summary>
    [DebuggerBrowsable(DebuggerBrowsableState.Collapsed)]
    public virtual Boolean IsReadOnly { get; } = true;
}

// Non-Public
partial class ReadOnlyCollectionBase<TElement>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ReadOnlyCollectionBase{T}"/> class.
    /// </summary>
    protected ReadOnlyCollectionBase() => 
        this._items = _emptyArray;
    /// <summary>
    /// Initializes a new instance of the <see cref="ReadOnlyCollectionBase{T}"/> class containing the specified collection of items.
    /// </summary>
    /// <param name="collection">The collection of items in this list.</param>
    /// <param name="exactCapacity">Whether to resize the internal array to the exact size of the passed in collection.</param>
    /// <exception cref="ArgumentException" />
    /// <exception cref="ArgumentNullException" />
    /// <exception cref="NotAllowed" />
    protected ReadOnlyCollectionBase([DisallowNull] IEnumerable<TElement> collection,
                                     Boolean exactCapacity)
    {
        ExceptionHelpers.ThrowIfArgumentNull(collection);

        if (collection is ICollection<TElement> c)
        {
            if (c.Count == 0)
            {
                this._items = _emptyArray;
            }
            else
            {
                this._items = new TElement[c.Count];
                c.CopyTo(array: this._items, 
                         arrayIndex: 0);
                this._size = c.Count;
            }
        }
        else
        {
            this._size = 0;
            this._items = _emptyArray;

            using IEnumerator<TElement> enumerator = collection.GetEnumerator();
            while (enumerator.MoveNext())
            {
                this.EnsureCapacity(capacity: this._size + 1);
                this._items[this._size++] = enumerator.Current;
            }
            // Resize to an exact fit
            if (exactCapacity &&
                this._items.Length != this._size)
            {
                TElement[] array = new TElement[this._size];
                Array.Copy(sourceArray: this._items, 
                           sourceIndex: 0, 
                           destinationArray: array, 
                           destinationIndex: 0, 
                           length: this._size);
                this._items = array;
            }
        }
    }

#pragma warning disable
    /// <summary>
    /// Error message, when trying to copy this collection to a multidimensional array.
    /// </summary>
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    protected const String MULTI_DIMENSIONAL_ARRAYS = "Multidimensional array are not supported.";
#pragma warning restore
}

// IReadOnlyCollection2
partial class ReadOnlyCollectionBase<TElement> : IReadOnlyCollection2<TElement>
{
    /// <inheritdoc />
    /// <exception cref="ArgumentNullException" />
    /// <exception cref="ArgumentOutOfRangeException" />
    [Pure]
    public virtual Boolean Contains([AllowNull] TElement item)
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
                if ((item is null && 
                    this._items[i] is null) ||
                    (item is not null &&
                    item.Equals(this._items[i])))
                {
                    return true;
                }
            }
            return false;
        }
    }

    /// <inheritdoc />
    /// <exception cref="ArgumentException" />
    /// <exception cref="ArgumentNullException" />
    /// <exception cref="ArgumentOutOfRangeException" />
    [Pure]
    public virtual void CopyTo([DisallowNull] TElement[] array, 
                               Int32 index)
    {
        ExceptionHelpers.ThrowIfArgumentNull(array);
        if (array.Rank != 1)
        {
            throw new ArgumentException(message: MULTI_DIMENSIONAL_ARRAYS);
        }

        lock (this._syncRoot)
        {
            Array.Copy(sourceArray: this._items, 
                       sourceIndex: 0, 
                       destinationArray: array, 
                       destinationIndex: index, 
                       length: this._size);
        }
    }
}