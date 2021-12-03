namespace Narumikazuchi.Collections.Abstract;

/// <summary>
/// Represents a strongly typed collection of objects. 
/// </summary>
public abstract partial class ReadOnlyCollectionBase<TElement>
{
    /// <summary>
    /// Gets whether this collection can be edited.
    /// </summary>
    [Pure]
    public virtual Boolean IsReadOnly { get; } = true;
}

// Non-Public
partial class ReadOnlyCollectionBase<TElement> : ArrayBasedCollection<TElement?>
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
    /// <exception cref="ArgumentNullException" />
    protected ReadOnlyCollectionBase([DisallowNull] IEnumerable<TElement?> collection,
                                     Boolean exactCapacity)
    {
        ExceptionHelpers.ThrowIfArgumentNull(collection);

        if (collection is ICollection c)
        {
            if (c.Count == 0)
            {
                this._items = _emptyArray;
            }
            else
            {
                this._items = new TElement[c.Count];
                c.CopyTo(array: this._items, 
                         index: 0);
                this._size = c.Count;
            }
        }
        else
        {
            this._size = 0;
            this._items = _emptyArray;

            using IEnumerator<TElement?> enumerator = collection.GetEnumerator();
            while (enumerator.MoveNext())
            {
                this.EnsureCapacity(capacity: this._size + 1);
                this._items[this._size++] = enumerator.Current;
            }
            // Resize to an exact fit
            if (exactCapacity &&
                this._items.Length != this._size)
            {
                TElement?[] array = new TElement[this._size];
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
    protected const String MULTI_DIMENSIONAL_ARRAYS = "Multidimensional arrays are not supported.";
    /// <summary>
    /// Error message, when trying to copy this collection to a multidimensional array.
    /// </summary>
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    protected const String ARRAY_TYPE_MISMATCH = "The elements of this collection can't be copied to the specified array, since the types are incompatible.";
    /// <summary>
    /// Error message, when trying to copy this collection to a multidimensional array.
    /// </summary>
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    protected const String INDEX_LESS_THAN_ZERO = "Can't start copying at a negative start index in the destination array.";
#pragma warning restore
}

// ICollection
partial class ReadOnlyCollectionBase<TElement> : ICollection
{
    /// <inheritdoc/>
    /// <exception cref="ArrayTypeMismatchException" />
    /// <exception cref="ArgumentOutOfRangeException" />
    [Pure]
    public override void CopyTo([DisallowNull] Array array,
                                Int32 index)
    {
        if (array is not TElement[]
                  and not Object[])
        {
            ArrayTypeMismatchException ex = new(message: ARRAY_TYPE_MISMATCH);
            ex.Data.Add(key: "Required Type",
                        value: typeof(TElement).FullName);
            ex.Data.Add(key: "Array Type",
                        value: array.GetType()
                                    .GetElementType()?.FullName);
            throw ex;
        }
        if (index < 0)
        {
            ArgumentOutOfRangeException ex = new(paramName: nameof(index),
                                                 message: INDEX_LESS_THAN_ZERO);
            ex.Data.Add(key: "Index",
                        value: index);
            throw ex;
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

// IContentConvertable<T>
partial class ReadOnlyCollectionBase<TElement> : IContentConvertable<TElement?>
{
    /// <inheritdoc />
    /// <exception cref="ArgumentNullException" />
    /// <exception cref="NotAllowed" />
    [Pure]
    [return: NotNull]
    public virtual ICollection<TOutput> ConvertAll<TOutput>([DisallowNull] Converter<TElement?, TOutput> converter)
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
}

// IContentCopyable<T, U>
partial class ReadOnlyCollectionBase<TElement> : IContentCopyable<Int32, TElement?[]>
{
    /// <inheritdoc />
    /// <exception cref="ArgumentException" />
    /// <exception cref="ArgumentNullException" />
    [Pure]
    public virtual void CopyTo([DisallowNull] TElement?[] array,
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

// IContentForEach<T>
partial class ReadOnlyCollectionBase<TElement> : IContentForEach<TElement?>
{
    /// <summary>
    /// Performs the specified action for every element of this <see cref="ICollection{T}"/>.
    /// </summary>
    /// <param name="action">The action to perform on each item.</param>
    /// <exception cref="ArgumentNullException" />
    /// <exception cref="NotAllowed" />
    [Pure]
    public virtual void ForEach([DisallowNull] Action<TElement?> action)
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
}

// IElementContainer<T>
partial class ReadOnlyCollectionBase<TElement> : IElementContainer<TElement?>
{
    /// <inheritdoc />
    /// <exception cref="ArgumentNullException" />
    /// <exception cref="NotAllowed" />
    [Pure]
    public override Boolean Contains([AllowNull] TElement item)
    {
        ExceptionHelpers.ThrowIfArgumentNull(item);

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
}

// IElementFinder<T>
partial class ReadOnlyCollectionBase<TElement> : IElementFinder<TElement?>
{
    /// <inheritdoc/>
    /// <exception cref="ArgumentNullException" />
    /// <exception cref="NotAllowed" />
    [Pure]
    public virtual Boolean Exists([DisallowNull] Func<TElement?, Boolean> predicate)
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
    /// <exception cref="NotAllowed" />
    [Pure]
    [return: MaybeNull]
    public virtual TElement? Find([DisallowNull] Func<TElement?, Boolean> predicate)
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
    /// <exception cref="NotAllowed" />
    [Pure]
    [return: NotNull]
    public virtual IElementContainer<TElement?> FindAll([DisallowNull] Func<TElement?, Boolean> predicate)
    {
        ExceptionHelpers.ThrowIfArgumentNull(predicate);

        Collection<TElement?> result = new();
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
        return result.AsIReadOnlyCollection2<Collection<TElement?>, TElement?>();
    }

    /// <inheritdoc/>
    /// <exception cref="ArgumentNullException" />
    /// <exception cref="NotAllowed" />
    [Pure]
    [return: NotNull]
    public virtual IElementContainer<TElement?> FindExcept([DisallowNull] Func<TElement?, Boolean> predicate)
    {
        ExceptionHelpers.ThrowIfArgumentNull(predicate);

        Collection<TElement?> result = new();
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
        return result.AsIReadOnlyCollection2<Collection<TElement?>, TElement?>();
    }

    /// <inheritdoc/>
    /// <exception cref="ArgumentNullException" />
    /// <exception cref="NotAllowed" />
    [Pure]
    [return: MaybeNull]
    public virtual TElement? FindLast([DisallowNull] Func<TElement?, Boolean> predicate)
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