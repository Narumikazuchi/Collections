namespace Narumikazuchi.Collections.Abstract;

/// <summary>
/// Represents a strongly typed list of objects, which can be accessed by index. 
/// </summary>
public abstract partial class ReadOnlyListBase<TElement> : ReadOnlyCollectionBase<TElement>
{
    /// <summary>
    /// Copies the entire <see cref="ReadOnlyListBase{T}"/> to the specified one-dimensional array.
    /// </summary>
    /// <param name="array">An array with a fitting size to copy the items into.</param>
    /// <exception cref="ArgumentException" />
    /// <exception cref="ArgumentNullException" />
    /// <exception cref="ArgumentOutOfRangeException" />
    [Pure]
    public virtual void CopyTo([DisallowNull] TElement[] array) => 
        this.CopyTo(array, 
                    0);
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

    /// <summary>
    /// Creates a shallow copy of the elements of this <see cref="ReadOnlyListBase{T}"/> from the specified range.
    /// </summary>
    /// <param name="index">The index of the first item in the resulting range.</param>
    /// <param name="count">The amount of items in the range.</param>
    /// <returns>An <see cref="IList{T}"/> containing the items from the specified range</returns>
    /// <exception cref="ArgumentException" />
    /// <exception cref="ArgumentOutOfRangeException" />
    [Pure]
    public virtual IList<TElement> GetRange(in Int32 index, 
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
            List<TElement> result = new();
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

// Non-Public
partial class ReadOnlyListBase<TElement>
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
    protected ReadOnlyListBase([DisallowNull] IEnumerable<TElement> collection,
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
#pragma warning restore
}

// IReadOnlyList2
partial class ReadOnlyListBase<TElement> : IReadOnlyList2<TElement>
{
    /// <inheritdoc />
    /// <exception cref="ArgumentNullException" />
    /// <exception cref="ArgumentOutOfRangeException" />
    [Pure]
    public virtual Int32 IndexOf([DisallowNull] TElement item)
    {
        ExceptionHelpers.ThrowIfArgumentNull(item);

        lock (this._syncRoot)
        {
            return Array.IndexOf(array: this._items, 
                                 value: item, 
                                 startIndex: 0, 
                                 count: this._size);
        }
    }

    /// <inheritdoc />
    /// <exception cref="IndexOutOfRangeException" />
    /// <exception cref="NotAllowed" />
    public virtual TElement this[Int32 index]
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