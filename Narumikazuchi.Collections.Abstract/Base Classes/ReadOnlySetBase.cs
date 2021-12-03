namespace Narumikazuchi.Collections.Abstract;

/// <summary>
/// Represents a strongly typed immutable collection where every object is only contained once. The procedure to check whether the object is already in the <see cref="ReadOnlySetBase{TElement}"/> can be specified
/// with a corresponding <see cref="IEqualityComparer{T}"/> object or a <see cref="EqualityComparison{T}"/> delegate.
/// </summary>
public abstract partial class ReadOnlySetBase<TElement>
{
    /// <inheritdoc/>
    [Pure]
    public override Boolean Contains([AllowNull] TElement? item)
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
                if (item is null &&
                    this._items[i] is null)
                {
                    return true;
                }
                if (this.Comparer is not null)
                {
                    if (this.Comparer.Equals(x: item,
                                             y: this._items[i]))
                    {
                        return true;
                    }
                    continue;
                }
                if (item is IEquatable<TElement> eq)
                {
                    if (eq.Equals(this._items[i]))
                    {
                        return true;
                    }
                    continue;
                }
            }
            return false;
        }
    }

    /// <summary>
    /// Gets or sets the <see cref="IEqualityComparer{T}"/> that the <see cref="ReadOnlySetBase{TElement}"/> uses for duplicate-checks.
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
partial class ReadOnlySetBase<TElement> : ReadOnlyCollectionBase<TElement>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ReadOnlySetBase{TElement}"/> class.
    /// </summary>
    protected ReadOnlySetBase() : 
        base() =>
            this.Comparer = null;
    /// <summary>
    /// Initializes a new instance of the <see cref="ReadOnlySetBase{TElement}"/> class containing the specified collection of items.
    /// </summary>
    /// <param name="collection">The collection of items in this list.</param>
    /// <param name="exactCapacity">Whether to resize the internal array to the exact size of the passed in collection.</param>
    /// <exception cref="ArgumentException" />
    /// <exception cref="ArgumentNullException" />
    /// <exception cref="NotAllowed" />
    protected ReadOnlySetBase([DisallowNull] IEnumerable<TElement?> collection,
                              Boolean exactCapacity) :
        this(collection: collection,
             comparer: null,
             exactCapacity: exactCapacity)
    { }
    /// <summary>
    /// Initializes a new instance of the <see cref="ReadOnlySetBase{TElement}"/> class containing the specified collection of items.
    /// </summary>
    /// <param name="collection">The collection of items in this list.</param>
    /// <param name="comparison">The delegate that is used to compare two instances of <typeparamref name="TElement"/>.</param>
    /// <param name="exactCapacity">Whether to resize the internal array to the exact size of the passed in collection.</param>
    /// <exception cref="ArgumentException" />
    /// <exception cref="ArgumentNullException" />
    /// <exception cref="NotAllowed" />
    protected ReadOnlySetBase([DisallowNull] IEnumerable<TElement?> collection,
                              [DisallowNull] EqualityComparison<TElement?> comparison,
                              Boolean exactCapacity) :
         this(collection: collection,
              comparer: new __FuncEqualityComparer<TElement?>(comparison),
              exactCapacity: exactCapacity)
    { }
    /// <summary>
    /// Initializes a new instance of the <see cref="ReadOnlySetBase{TElement}"/> class containing the specified collection of items.
    /// </summary>
    /// <param name="collection">The collection of items in this list.</param>
    /// <param name="comparer">The comparer that is used to compare two instances of <typeparamref name="TElement"/>.</param>
    /// <param name="exactCapacity">Whether to resize the internal array to the exact size of the passed in collection.</param>
    /// <exception cref="ArgumentException" />
    /// <exception cref="ArgumentNullException" />
    /// <exception cref="NotAllowed" />
    protected ReadOnlySetBase([DisallowNull] IEnumerable<TElement?> collection,
                              [AllowNull] IEqualityComparer<TElement?>? comparer,
                              Boolean exactCapacity)
    {
        ExceptionHelpers.ThrowIfArgumentNull(collection);

        this._size = 0;
        this._items = _emptyArray;
        this.Comparer = comparer;

        using IEnumerator<TElement?> enumerator = collection.GetEnumerator();
        while (enumerator.MoveNext())
        {
            if (enumerator.Current is null ||
                this.Contains(item: enumerator.Current))
            {
                continue;
            }
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

    /// <summary>
    /// Attempts to find all of the items in this collection in the specified other enumerable.
    /// </summary>
    /// <param name="other">The enumerable to search through.</param>
    /// <returns><see langword="true"/> if all of the items of this collection are also present in the enumerable; otherwise, <see langword="false"/></returns>
    [Pure]
    protected Boolean FindInOther(IEnumerable<TElement?> other)
    {
        BitArray arr = new(length: this._size);

        foreach (TElement? item in other)
        {
            Int32 index = Array.IndexOf(array: this._items,
                                        value: item);
            if (index >= 0)
            {
                if (!arr.Get(index: index))
                {
                    arr.Set(index: index,
                            value: true);
                }
            }
        }

        for (Int32 i = 0; i < arr.Count; i++)
        {
            if (!arr.Get(index: i))
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
partial class ReadOnlySetBase<TElement> : IReadOnlySet<TElement?>
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
            if (this._size == 0)
            {
                return collection.Count > 0;
            }

            if (this._size >= collection.Count)
            {
                return false;
            }
        }

        return this.FindInOther(other: other);
    }

    /// <inheritdoc/>
    /// <exception cref="ArgumentNullException"/>
    [Pure]
    public virtual Boolean IsProperSupersetOf([DisallowNull] IEnumerable<TElement?> other)
    {
        ExceptionHelpers.ThrowIfArgumentNull(other);

        if (other == this ||
            this._size == 0)
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

        if (this._size == 0 ||
            other == this)
        {
            return true;
        }

        if (other is ICollection<TElement?> collection &&
            this._size > collection.Count)
        {
            return false;
        }

        return this.FindInOther(other: other);
    }

    /// <inheritdoc/>
    /// <exception cref="ArgumentNullException"/>
    [Pure]
    public virtual Boolean IsSupersetOf([DisallowNull] IEnumerable<TElement?> other)
    {
        ExceptionHelpers.ThrowIfArgumentNull(other);

        if ((other is ICollection<TElement?> collection &&
            collection.Count == 0) ||
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

        if (this._size == 0)
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
            if (collection.Count != this._size)
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