namespace Narumikazuchi.Collections.Abstract;

/// <summary>
/// Represents a strongly typed immutable collection where every object is only contained once. The procedure to check whether the object is already in the <see cref="ReadOnlySetBase{TIndex, TElement}"/> can be specified
/// with a corresponding <see cref="IEqualityComparer{T}"/> object or a <see cref="EqualityComparison{T}"/> delegate.
/// </summary>
public abstract partial class ReadOnlySetBase<TIndex, TElement>
    where TIndex : ISignedNumber<TIndex>
{
    /// <summary>
    /// Gets or sets the <see cref="IEqualityComparer{T}"/> that the <see cref="ReadOnlySetBase{TIndex, TElement}"/> uses for duplicate-checks.
    /// </summary>
    /// <exception cref="ArgumentException"/>
    [MaybeNull]
    public IEqualityComparer<TElement?>? Comparer
    {
        get => this._comparer;
        set
        {
            if (value is null &&
                !typeof(IEquatable<TElement>).IsAssignableFrom(typeof(TElement)))
            {
                throw new ArgumentException(message: String.Format(format: COMPARER_IS_NULL_WITHOUT_IEQUATABLE,
                                                                   arg0: typeof(TElement).FullName));
            }
            this._comparer = value;
        }
    }
}

// Non-Public
partial class ReadOnlySetBase<TIndex, TElement> : ReadOnlyCollectionBase<TIndex, TElement>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ReadOnlySetBase{TIndex, TElement}"/> class.
    /// </summary>
    protected ReadOnlySetBase() :
        base()
    {
        this.Comparer = null;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ReadOnlySetBase{TIndex, TElement}"/> class containing the specified collection of items.
    /// </summary>
    /// <param name="collection">The collection of items in this list.</param>
    /// <exception cref="ArgumentException" />
    /// <exception cref="ArgumentNullException" />
    /// <exception cref="NotAllowed" />
    protected ReadOnlySetBase([DisallowNull] IEnumerable<KeyValuePair<TIndex, TElement?>> collection) :
        this(collection: collection,
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
    protected ReadOnlySetBase([DisallowNull] IEnumerable<KeyValuePair<TIndex, TElement?>> collection,
                              [DisallowNull] EqualityComparison<TElement?> comparison) :
         this(collection: collection,
              comparer: new __FuncEqualityComparer<TElement?>(comparison: comparison))
    { }
    /// <summary>
    /// Initializes a new instance of the <see cref="ReadOnlySetBase{TIndex, TElement}"/> class containing the specified collection of items.
    /// </summary>
    /// <param name="collection">The collection of items in this list.</param>
    /// <param name="comparer">The comparer that is used to compare two instances of <typeparamref name="TElement"/>.</param>
    /// <exception cref="ArgumentException" />
    /// <exception cref="ArgumentNullException" />
    /// <exception cref="NotAllowed" />
    protected ReadOnlySetBase([DisallowNull] IEnumerable<KeyValuePair<TIndex, TElement?>> collection,
                              [AllowNull] IEqualityComparer<TElement?>? comparer)
    {
        ExceptionHelpers.ThrowIfArgumentNull(collection);

        this.Comparer = comparer;

        using IEnumerator<KeyValuePair<TIndex, TElement?>> enumerator = collection.GetEnumerator();
        while (enumerator.MoveNext())
        {
            if (this.Contains(item: enumerator.Current
                                              .Value))
            {
                continue;
            }
            this.InsertInternal(index: enumerator.Current
                                                 .Key,
                                item: enumerator.Current
                                                .Value);
        }
    }
    /// <summary>
    /// Initializes a new instance of the <see cref="ReadOnlySetBase{TIndex, TElement}"/> class containing the specified collection of items.
    /// </summary>
    /// <param name="collection">The collection of items in this list.</param>
    /// <exception cref="ArgumentException" />
    /// <exception cref="ArgumentNullException" />
    /// <exception cref="NotAllowed" />
    protected ReadOnlySetBase([DisallowNull] IEnumerable<(TIndex, TElement?)> collection) :
        this(collection: collection,
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
    protected ReadOnlySetBase([DisallowNull] IEnumerable<(TIndex, TElement?)> collection,
                              [DisallowNull] EqualityComparison<TElement?> comparison) :
         this(collection: collection,
              comparer: new __FuncEqualityComparer<TElement?>(comparison: comparison))
    { }
    /// <summary>
    /// Initializes a new instance of the <see cref="ReadOnlySetBase{TIndex, TElement}"/> class containing the specified collection of items.
    /// </summary>
    /// <param name="collection">The collection of items in this list.</param>
    /// <param name="comparer">The comparer that is used to compare two instances of <typeparamref name="TElement"/>.</param>
    /// <exception cref="ArgumentException" />
    /// <exception cref="ArgumentNullException" />
    /// <exception cref="NotAllowed" />
    protected ReadOnlySetBase([DisallowNull] IEnumerable<(TIndex, TElement?)> collection,
                              [AllowNull] IEqualityComparer<TElement?>? comparer)
    {
        ExceptionHelpers.ThrowIfArgumentNull(collection);

        this.Comparer = comparer;

        using IEnumerator<(TIndex, TElement?)> enumerator = collection.GetEnumerator();
        while (enumerator.MoveNext())
        {
            if (this.Contains(item: enumerator.Current
                                              .Item2))
            {
                continue;
            }
            this.InsertInternal(index: enumerator.Current
                                                 .Item1,
                                item: enumerator.Current
                                                .Item2);
        }
    }
    /// <summary>
    /// Initializes a new instance of the <see cref="ReadOnlySetBase{TIndex, TElement}"/> class containing the specified collection of items.
    /// </summary>
    /// <param name="collection">The collection of items in this list.</param>
    /// <exception cref="ArgumentException" />
    /// <exception cref="ArgumentNullException" />
    /// <exception cref="NotAllowed" />
    protected ReadOnlySetBase([DisallowNull] IEnumerable<Tuple<TIndex, TElement?>> collection) :
        this(collection: collection,
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
    protected ReadOnlySetBase([DisallowNull] IEnumerable<Tuple<TIndex, TElement?>> collection,
                              [DisallowNull] EqualityComparison<TElement?> comparison) :
         this(collection: collection,
              comparer: new __FuncEqualityComparer<TElement?>(comparison: comparison))
    { }
    /// <summary>
    /// Initializes a new instance of the <see cref="ReadOnlySetBase{TIndex, TElement}"/> class containing the specified collection of items.
    /// </summary>
    /// <param name="collection">The collection of items in this list.</param>
    /// <param name="comparer">The comparer that is used to compare two instances of <typeparamref name="TElement"/>.</param>
    /// <exception cref="ArgumentException" />
    /// <exception cref="ArgumentNullException" />
    /// <exception cref="NotAllowed" />
    protected ReadOnlySetBase([DisallowNull] IEnumerable<Tuple<TIndex, TElement?>> collection,
                              [AllowNull] IEqualityComparer<TElement?>? comparer)
    {
        ExceptionHelpers.ThrowIfArgumentNull(collection);

        this.Comparer = comparer;

        using IEnumerator<Tuple<TIndex, TElement?>> enumerator = collection.GetEnumerator();
        while (enumerator.MoveNext())
        {
            if (this.Contains(item: enumerator.Current
                                              .Item2))
            {
                continue;
            }
            this.InsertInternal(index: enumerator.Current
                                                 .Item1,
                                item: enumerator.Current
                                                .Item2);
        }
    }

    /// <summary>
    /// Attempts to find all of the items in this collection in the specified other enumerable.
    /// </summary>
    /// <param name="other">The enumerable to search through.</param>
    /// <returns><see langword="true"/> if all of the items of this collection are also present in the enumerable; otherwise, <see langword="false"/></returns>
    [Pure]
    protected Boolean FindInOther(IEnumerable<TElement?> other)
    {
        BitArray arr = new(length: this.Count);

        foreach (TElement? item in other)
        {
            Int32 index = -1;
            for (Int32 i = 0; 
                 i < this.Count; 
                 i++)
            {
                if (item is null &&
                    this._entries[i]
                        .Value is null)
                {
                    index = i;
                    break;
                }
                if (item is not null &&
                    item.Equals(this._entries[i]
                                    .Value))
                {
                    index = i;
                    break;
                }
            }
            if (index >= 0)
            {
                if (!arr.Get(index))
                {
                    arr.Set(index: index,
                            value: true);
                }
            }
        }

        for (Int32 i = 0; 
             i < arr.Count; 
             i++)
        {
            if (!arr.Get(i))
            {
                return false;
            }
        }
        return true;
    }

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private IEqualityComparer<TElement?>? _comparer = null;

#pragma warning disable
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    protected const String COMPARER_IS_NULL_WITHOUT_IEQUATABLE = "The Comparer property can't be null if the type parameter '{0}' does not inherit from the IEquatable<T> interface.";
#pragma warning restore
}

// IReadOnlySet<T>
partial class ReadOnlySetBase<TIndex, TElement> : IReadOnlySet<TElement?>
{
    /// <inheritdoc/>
    /// <exception cref="ArgumentNullException"/>
    [Pure]
    public virtual Boolean IsProperSubsetOf([DisallowNull] IEnumerable<TElement?> other)
    {
        ExceptionHelpers.ThrowIfArgumentNull(other);

        if (other == this)
        {
            return false;
        }

        if (other is ICollection<TElement?> collection)
        {
            if (this.Count == 0)
            {
                return collection.Count > 0;
            }

            if (this.Count >= collection.Count)
            {
                return false;
            }
        }

        return this.FindInOther(other);
    }

    /// <inheritdoc/>
    /// <exception cref="ArgumentNullException"/>
    [Pure]
    public virtual Boolean IsProperSupersetOf([DisallowNull] IEnumerable<TElement?> other)
    {
        ExceptionHelpers.ThrowIfArgumentNull(other);

        if (other == this ||
            this.Count == 0)
        {
            return false;
        }

        if (other is ICollection<TElement?> collection &&
            collection.Count == 0)
        {
            return true;
        }

        if (!other.Any())
        {
            return true;
        }

        foreach (TElement? item in other)
        {
            if (!this.Contains(item: item))
            {
                return false;
            }
        }
        return true;
    }

    /// <inheritdoc/>
    /// <exception cref="ArgumentNullException"/>
    [Pure]
    public virtual Boolean IsSubsetOf([DisallowNull] IEnumerable<TElement?> other)
    {
        ExceptionHelpers.ThrowIfArgumentNull(other);

        if (this.Count == 0 ||
            other == this)
        {
            return true;
        }

        if (other is ICollection<TElement?> collection &&
            this.Count > collection.Count)
        {
            return false;
        }

        return this.FindInOther(other);
    }

    /// <inheritdoc/>
    /// <exception cref="ArgumentNullException"/>
    [Pure]
    public virtual Boolean IsSupersetOf([DisallowNull] IEnumerable<TElement?> other)
    {
        ExceptionHelpers.ThrowIfArgumentNull(other);

        if (other is ICollection<TElement?> collection &&
            collection.Count == 0 ||
            other == this)
        {
            return true;
        }

        if (!other.Any())
        {
            return true;
        }

        foreach (TElement? item in other)
        {
            if (!this.Contains(item: item))
            {
                return false;
            }
        }
        return true;
    }

    /// <inheritdoc/>
    /// <exception cref="ArgumentNullException"/>
    [Pure]
    public virtual Boolean Overlaps([DisallowNull] IEnumerable<TElement?> other)
    {
        ExceptionHelpers.ThrowIfArgumentNull(other);

        if (this.Count == 0)
        {
            return false;
        }

        foreach (TElement? item in other)
        {
            if (this.Contains(item: item))
            {
                return true;
            }
        }
        return false;
    }

    /// <inheritdoc/>
    /// <exception cref="ArgumentNullException"/>
    [Pure]
    public virtual Boolean SetEquals([DisallowNull] IEnumerable<TElement?> other)
    {
        ExceptionHelpers.ThrowIfArgumentNull(other);

        if (other == this)
        {
            return true;
        }

        if (other is ICollection<TElement?> collection)
        {
            if (collection.Count != this.Count)
            {
                return false;
            }
        }

        foreach (TElement? item in other)
        {
            if (!this.Contains(item: item))
            {
                return false;
            }
        }
        return true;
    }
}