namespace Narumikazuchi.Collections.Abstract;

/// <summary>
/// Represents a strongly typed collection where every object is only contained once. The procedure to check whether the object is already in the <see cref="SetBase{TIndex, TElement}"/> can be specified
/// with a corresponding <see cref="IEqualityComparer{T}"/> object or a <see cref="EqualityComparison{T}"/> delegate.
/// </summary>
public abstract partial class SetBase<TIndex, TElement>
    where TIndex : ISignedNumber<TIndex>
{ }

// Non-Public
partial class SetBase<TIndex, TElement> : ReadOnlySetBase<TIndex, TElement>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SetBase{TIndex, TElement}"/> class.
    /// </summary>
    protected SetBase() :
        base()
    { }
    /// <summary>
    /// Initializes a new instance of the <see cref="SetBase{TIndex, TElement}"/> class containing the specified collection of items.
    /// </summary>
    /// <param name="collection">The collection of items in this list.</param>
    /// <exception cref="ArgumentException" />
    /// <exception cref="ArgumentNullException" />
    /// <exception cref="NotAllowed" />
    protected SetBase([DisallowNull] IEnumerable<KeyValuePair<TIndex, TElement?>> collection) :
        base(collection: collection,
             comparer: null)
    { }
    /// <summary>
    /// Initializes a new instance of the <see cref="SetBase{TIndex, TElement}"/> class containing the specified collection of items.
    /// </summary>
    /// <param name="collection">The collection of items in this list.</param>
    /// <param name="comparison">The delegate that is used to compare two instances of <typeparamref name="TElement"/>.</param>
    /// <exception cref="ArgumentException" />
    /// <exception cref="ArgumentNullException" />
    /// <exception cref="NotAllowed" />
    protected SetBase([DisallowNull] IEnumerable<KeyValuePair<TIndex, TElement?>> collection,
                      [DisallowNull] EqualityComparison<TElement?> comparison) :
         base(collection: collection,
              comparer: new __FuncEqualityComparer<TElement?>(comparison))
    { }
    /// <summary>
    /// Initializes a new instance of the <see cref="SetBase{TIndex, TElement}"/> class containing the specified collection of items.
    /// </summary>
    /// <param name="collection">The collection of items in this list.</param>
    /// <param name="comparer">The comparer that is used to compare two instances of <typeparamref name="TElement"/>.</param>
    /// <exception cref="ArgumentException" />
    /// <exception cref="ArgumentNullException" />
    /// <exception cref="NotAllowed" />
    protected SetBase([DisallowNull] IEnumerable<KeyValuePair<TIndex, TElement?>> collection,
                      [AllowNull] IEqualityComparer<TElement?>? comparer) :
        base(collection: collection,
             comparer: comparer)
    { }
    /// <summary>
    /// Initializes a new instance of the <see cref="SetBase{TIndex, TElement}"/> class containing the specified collection of items.
    /// </summary>
    /// <param name="collection">The collection of items in this list.</param>
    /// <exception cref="ArgumentException" />
    /// <exception cref="ArgumentNullException" />
    /// <exception cref="NotAllowed" />
    protected SetBase([DisallowNull] IEnumerable<(TIndex, TElement?)> collection) :
        base(collection: collection,
             comparer: null)
    { }
    /// <summary>
    /// Initializes a new instance of the <see cref="SetBase{TIndex, TElement}"/> class containing the specified collection of items.
    /// </summary>
    /// <param name="collection">The collection of items in this list.</param>
    /// <param name="comparison">The delegate that is used to compare two instances of <typeparamref name="TElement"/>.</param>
    /// <exception cref="ArgumentException" />
    /// <exception cref="ArgumentNullException" />
    /// <exception cref="NotAllowed" />
    protected SetBase([DisallowNull] IEnumerable<(TIndex, TElement?)> collection,
                      [DisallowNull] EqualityComparison<TElement?> comparison) :
         base(collection: collection,
              comparer: new __FuncEqualityComparer<TElement?>(comparison))
    { }
    /// <summary>
    /// Initializes a new instance of the <see cref="SetBase{TIndex, TElement}"/> class containing the specified collection of items.
    /// </summary>
    /// <param name="collection">The collection of items in this list.</param>
    /// <param name="comparer">The comparer that is used to compare two instances of <typeparamref name="TElement"/>.</param>
    /// <exception cref="ArgumentException" />
    /// <exception cref="ArgumentNullException" />
    /// <exception cref="NotAllowed" />
    protected SetBase([DisallowNull] IEnumerable<(TIndex, TElement?)> collection,
                      [AllowNull] IEqualityComparer<TElement?>? comparer) :
        base(collection: collection,
             comparer: comparer)
    { }
    /// <summary>
    /// Initializes a new instance of the <see cref="SetBase{TIndex, TElement}"/> class containing the specified collection of items.
    /// </summary>
    /// <param name="collection">The collection of items in this list.</param>
    /// <exception cref="ArgumentException" />
    /// <exception cref="ArgumentNullException" />
    /// <exception cref="NotAllowed" />
    protected SetBase([DisallowNull] IEnumerable<Tuple<TIndex, TElement?>> collection) :
        base(collection: collection,
             comparer: null)
    { }
    /// <summary>
    /// Initializes a new instance of the <see cref="ReadOnlySetBase{TIndex, TElement}"/> class containing the specified collection of items.
    /// </summary>
    /// <param name="collection">The collection of items in this list.</param>
    /// <param name="comparison">The delegate that is used to compare two instances of <typeparamref name="TElement"/>.</param>
    /// <exception cref="ArgumentException" />
    /// <exception cref="ArgumentNullException" />
    /// <exception cref="NotAllowed" />
    protected SetBase([DisallowNull] IEnumerable<Tuple<TIndex, TElement?>> collection,
                      [DisallowNull] EqualityComparison<TElement?> comparison) :
         base(collection: collection,
              comparer: new __FuncEqualityComparer<TElement?>(comparison))
    { }
    /// <summary>
    /// Initializes a new instance of the <see cref="SetBase{TIndex, TElement}"/> class containing the specified collection of items.
    /// </summary>
    /// <param name="collection">The collection of items in this list.</param>
    /// <param name="comparer">The comparer that is used to compare two instances of <typeparamref name="TElement"/>.</param>
    /// <exception cref="ArgumentException" />
    /// <exception cref="ArgumentNullException" />
    /// <exception cref="NotAllowed" />
    protected SetBase([DisallowNull] IEnumerable<Tuple<TIndex, TElement?>> collection,
                      [AllowNull] IEqualityComparer<TElement?>? comparer) :
        base(collection: collection,
             comparer: comparer)
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
partial class SetBase<TIndex, TElement> : IContentAddable<TElement?>
{
    /// <inheritdoc/>
    /// <exception cref="ArgumentNullException"/>
    /// <exception cref="NotAllowed"/>
    public virtual Boolean Add([AllowNull] TElement? item) =>
        !this.Contains(item: item) &&
        this.AppendInternal(item: item);

    /// <inheritdoc/>
    /// <exception cref="ArgumentNullException" />
    /// <exception cref="ArgumentOutOfRangeException" />
    /// <exception cref="NotAllowed" />
    public virtual void AddRange([DisallowNull] IEnumerable<TElement?> collection)
    {
        foreach (TElement? item in collection)
        {
            this.Add(item: item);
        }
    }
}

// ICollection<T>
partial class SetBase<TIndex, TElement> : ICollection<TElement?>
{
    void ICollection<TElement?>.Add([AllowNull] TElement? item) =>
        this.Add(item: item);

    void ICollection<TElement?>.CopyTo(TElement?[] array,
                                       Int32 arrayIndex) =>
        this.CopyTo(array: array,
                    index: arrayIndex);
}

// IContentRemovable
partial class SetBase<TIndex, TElement> : IContentRemovable
{
    Boolean IContentRemovable.Remove(Object item) =>
        item is TElement element &&
        this.Remove(item: element);
}

// IContentRemovable<T>
partial class SetBase<TIndex, TElement> : IContentRemovable<TElement?>
{
    /// <inheritdoc/>
    /// <exception cref="NotAllowed" />
    public virtual void Clear() =>
        this.ClearInternal();

    /// <inheritdoc/>
    /// <exception cref="ArgumentNullException" />
    /// <exception cref="NotAllowed" />
    public virtual Boolean Remove([AllowNull] TElement? item) =>
        this.RemoveInternal(item: item);

    /// <inheritdoc/>
    /// <exception cref="ArgumentNullException" />
    /// <exception cref="IndexOutOfRangeException" />
    /// <exception cref="NotAllowed" />
    public virtual Int32 RemoveAll([DisallowNull] Func<TElement?, Boolean> predicate)
    {
        Collection<TIndex> indecies = new();
        foreach (KeyValuePair<TIndex, TElement?> kv in this.GetKeyValuePairsFirstToLast())
        {
            if (predicate.Invoke(arg: kv.Value))
            {
                indecies.Add(item: kv.Key);
            }
        }

        for (Int32 i = 0; i < indecies.Count; i++)
        {
            this.RemoveAtInternal(index: indecies[i]);
        }

        return indecies.Count;
    }
}

// ISet<T>
partial class SetBase<TIndex, TElement> : ISet<TElement?>
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

        Collection<TIndex> indecies = new();
        foreach (KeyValuePair<TIndex, TElement?> kv in this.GetKeyValuePairsFirstToLast())
        {
            foreach (TElement? item in other)
            {
                if (item is null && 
                    kv.Value is null)
                {
                    continue;
                }
                if (item is not null &&
                    item.Equals(kv.Value))
                {
                    continue;
                }
                indecies.Add(item: kv.Key);
            }
        }

        for (Int32 i = 0; i < indecies.Count; i++)
        {
            this.RemoveAtInternal(index: indecies[i]);
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

        Collection<TIndex> indecies = new();
        foreach (KeyValuePair<TIndex, TElement?> kv in this.GetKeyValuePairsFirstToLast())
        {
            foreach (TElement? item in other)
            {
                if (item is null &&
                    kv.Value is null)
                {
                    indecies.Add(item: kv.Key);
                }
                if (item is not null &&
                    item.Equals(kv.Value))
                {
                    indecies.Add(item: kv.Key);
                }
            }
        }

        for (Int32 i = 0; i < indecies.Count; i++)
        {
            this.RemoveAtInternal(index: indecies[i]);
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