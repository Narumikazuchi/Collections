namespace Narumikazuchi.Collections.Abstract;

/// <summary>
/// Represents a strongly typed collection of objects. 
/// </summary>
public abstract partial class CollectionBase<TIndex, TElement>
    where TIndex : ISignedNumber<TIndex>
{
    /// <inheritdoc/>
    [Pure]
    public override Boolean IsReadOnly { get; } = false;
}

// Non-Public
partial class CollectionBase<TIndex, TElement> : ReadOnlyCollectionBase<TIndex, TElement>
{
    /// <summary>
    /// Initializes a new empty instance of the <see cref="CollectionBase{TIndex, TElement}"/> class.
    /// </summary>
    protected CollectionBase() :
        base()
    { }
    /// <summary>
    /// Initializes a new instance of the <see cref="CollectionBase{TIndex, TElement}"/> class having the specified collection of items.
    /// </summary>
    /// <param name="collection">The initial collection of items in this list.</param>
    /// <exception cref="ArgumentException" />
    /// <exception cref="ArgumentNullException" />
    /// <exception cref="NotAllowed" />
    protected CollectionBase([DisallowNull] IEnumerable<KeyValuePair<TIndex, TElement?>> collection) :
        base(collection: collection)
    { }
    /// <summary>
    /// Initializes a new instance of the <see cref="CollectionBase{TIndex, TElement}"/> class having the specified collection of items.
    /// </summary>
    /// <param name="collection">The initial collection of items in this list.</param>
    /// <exception cref="ArgumentException" />
    /// <exception cref="ArgumentNullException" />
    /// <exception cref="NotAllowed" />
    protected CollectionBase([DisallowNull] IEnumerable<(TIndex, TElement?)> collection) :
        base(collection: collection)
    { }
    /// <summary>
    /// Initializes a new instance of the <see cref="CollectionBase{TIndex, TElement}"/> class having the specified collection of items.
    /// </summary>
    /// <param name="collection">The initial collection of items in this list.</param>
    /// <exception cref="ArgumentException" />
    /// <exception cref="ArgumentNullException" />
    /// <exception cref="NotAllowed" />
    protected CollectionBase([DisallowNull] IEnumerable<Tuple<TIndex, TElement?>> collection) :
        base(collection: collection)
    { }

#pragma warning disable
    /// <summary>
    /// Error message, when trying to write to a readonly list.
    /// </summary>
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    protected const String COLLECTION_IS_READONLY = "This collection is readonly and can't be written to.";
#pragma warning restore
}

// ICollection<T>
partial class CollectionBase<TIndex, TElement> : ICollection<TElement?>
{
    void ICollection<TElement?>.Add([AllowNull] TElement? item) =>
        this.AppendInternal(item);

    void ICollection<TElement?>.CopyTo(TElement?[] array, 
                                       Int32 arrayIndex) =>
        this.CopyTo(array: array,
                    index: arrayIndex);
}

// IContentClearable
partial class CollectionBase<TIndex, TElement> : IContentClearable
{
    /// <inheritdoc />
    /// <exception cref="NotAllowed" />
    public virtual void Clear() =>
        this.ClearInternal();
}

// IContentRemovable
partial class CollectionBase<TIndex, TElement> : IContentRemovable
{
    Boolean IContentRemovable.Remove(Object item) =>
        item is TElement element &&
        this.Remove(element);
}

// IContentRemovable<T>
partial class CollectionBase<TIndex, TElement> : IContentRemovable<TElement?>
{
    /// <inheritdoc />
    /// <exception cref="ArgumentNullException" />
    /// <exception cref="NotAllowed" />
    public virtual Boolean Remove([AllowNull] TElement? item) =>
        this.RemoveInternal(item);

    /// <inheritdoc />
    /// <exception cref="ArgumentNullException" />
    /// <exception cref="IndexOutOfRangeException" />
    /// <exception cref="NotAllowed" />
    public virtual Int32 RemoveAll([DisallowNull] Func<TElement?, Boolean> predicate)
    {
        ExceptionHelpers.ThrowIfArgumentNull(predicate);

        Collection<TIndex> remove = new();
        foreach (KeyValuePair<TIndex, TElement?> kv in this.GetKeyValuePairsFirstToLast())
        {
            if (predicate.Invoke(kv.Value))
            {
                remove.Add(kv.Key);
            }
        }

        Int32 skipped = 0;
        foreach(TIndex key in remove)
        {
            if (!this.RemoveAtInternal(key))
            {
                skipped++;
            }
        }
        return remove.Count - skipped;
    }
}

// IContentInsertable<T>
partial class CollectionBase<TIndex, TElement> : IContentInsertable<TIndex>
{
    void IContentInsertable<TIndex>.Insert(in TIndex index, 
                                           Object item)
    {
        if (item is TElement element)
        {
            this.InsertInternal(index: index, 
                                item: element);
            return;
        }
        throw new InvalidCastException();
    }
}

// IContentInsertable<T, U>
partial class CollectionBase<TIndex, TElement> : IContentInsertable<TIndex, TElement?>
{
    /// <inheritdoc/>
    public virtual void Insert([DisallowNull] in TIndex index, 
                               TElement? item) =>
            this.InsertInternal(index: index,
                                item: item);

    /// <inheritdoc/>
    public virtual void InsertRange([DisallowNull] in TIndex index, 
                                    [DisallowNull] IEnumerable<TElement?> collection)
    {
        ExceptionHelpers.ThrowIfArgumentNull(collection);

        TIndex current = index;
        foreach (TElement? element in collection)
        {
            this.InsertInternal(index: current,
                                item: element);
            current += TIndex.One;
        }
    }
}

// IContentIndexRemovable<T>
partial class CollectionBase<TIndex, TElement> : IContentIndexRemovable<TIndex>
{
    /// <inheritdoc/>
    public virtual void RemoveAt([DisallowNull] in TIndex index) =>
        this.RemoveAtInternal(index);
}