namespace Narumikazuchi.Collections.Abstract;

/// <summary>
/// Represents a strongly typed collection of objects. 
/// </summary>
public abstract partial class ReadOnlyCollectionBase<TIndex, TElement>
    where TIndex : ISignedNumber<TIndex>
{
    /// <summary>
    /// Gets whether this collection can be edited.
    /// </summary>
    [Pure]
    public virtual Boolean IsReadOnly { get; } = true;
}

// Non-Public
partial class ReadOnlyCollectionBase<TIndex, TElement> : FastCollectionBase<TIndex, TElement?>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ReadOnlyCollectionBase{TIndex, TElement}"/> class.
    /// </summary>
    protected ReadOnlyCollectionBase() :
        base()
    { }
    /// <summary>
    /// Initializes a new instance of the <see cref="ReadOnlyCollectionBase{TIndex, TElement}"/> class containing the specified collection of items.
    /// </summary>
    /// <param name="collection">The collection of items in this list.</param>
    /// <exception cref="ArgumentNullException" />
    protected ReadOnlyCollectionBase([DisallowNull] IEnumerable<KeyValuePair<TIndex, TElement?>> collection) :
        base()
    {
        ExceptionHelpers.ThrowIfArgumentNull(collection);

        using IEnumerator<KeyValuePair<TIndex, TElement?>> enumerator = collection.GetEnumerator();
        while (enumerator.MoveNext())
        {
            this.InsertInternal(index: enumerator.Current.Key,
                                item: enumerator.Current.Value);
        }
    }
    /// <summary>
    /// Initializes a new instance of the <see cref="ReadOnlyCollectionBase{TIndex, TElement}"/> class containing the specified collection of items.
    /// </summary>
    /// <param name="collection">The collection of items in this list.</param>
    /// <exception cref="ArgumentNullException" />
    protected ReadOnlyCollectionBase([DisallowNull] IEnumerable<(TIndex, TElement?)> collection) :
        base()
    {
        ExceptionHelpers.ThrowIfArgumentNull(collection);

        using IEnumerator<(TIndex, TElement?)> enumerator = collection.GetEnumerator();
        while (enumerator.MoveNext())
        {
            this.InsertInternal(index: enumerator.Current.Item1,
                                item: enumerator.Current.Item2);
        }
    }
    /// <summary>
    /// Initializes a new instance of the <see cref="ReadOnlyCollectionBase{TIndex, TElement}"/> class containing the specified collection of items.
    /// </summary>
    /// <param name="collection">The collection of items in this list.</param>
    /// <exception cref="ArgumentNullException" />
    protected ReadOnlyCollectionBase([DisallowNull] IEnumerable<Tuple<TIndex, TElement?>> collection) :
        base()
    {
        ExceptionHelpers.ThrowIfArgumentNull(collection);

        using IEnumerator<Tuple<TIndex, TElement?>> enumerator = collection.GetEnumerator();
        while (enumerator.MoveNext())
        {
            this.InsertInternal(index: enumerator.Current.Item1,
                                item: enumerator.Current.Item2);
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
partial class ReadOnlyCollectionBase<TIndex, TElement> : ICollection
{
    void ICollection.CopyTo([DisallowNull] Array array,
                            Int32 index)
    {
        if (array is not KeyValuePair<TIndex, TElement?>[]
                  and not Object[])
        {
            ArrayTypeMismatchException exception = new(message: ARRAY_TYPE_MISMATCH);
            exception.Data
                     .Add(key: "Required Type",
                          value: typeof(TElement).FullName);
            exception.Data
                     .Add(key: "Array Type",
                          value: array.GetType()
                                      .GetElementType()?
                                      .FullName);
            throw exception;
        }
        if (index < 0)
        {
            ArgumentOutOfRangeException exception = new(paramName: nameof(index),
                                                 message: INDEX_LESS_THAN_ZERO);
            exception.Data
                     .Add(key: "Index",
                          value: index);
            throw exception;
        }

        Int32 i = 0;
        foreach (KeyValuePair<TIndex, TElement?> kv in this.GetKeyValuePairsFirstToLast())
        {
            array.SetValue(index: index + i++,
                           value: kv);
        }
    }
}

// IContentConvertable<T>
partial class ReadOnlyCollectionBase<TIndex, TElement> : IContentConvertable<TElement?>
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
        Int32 v = this.Version;

        foreach (TElement? element in this.GetValuesFirstToLast())
        {
            if (this.Version != v)
            {
                NotAllowed exception = new(auxMessage: COLLECTION_CHANGED);
                exception.Data
                         .Add(key: "Fixed Version",
                              value: v);
                exception.Data
                         .Add(key: "Altered Version",
                              value: this.Version);
                throw exception;
            }
            result.Add(converter.Invoke(input: element));
        }
        return result;
    }
}

// IContentCopyable<T, U>
partial class ReadOnlyCollectionBase<TIndex, TElement> : IContentCopyable<Int32, TElement?[]>
{
    /// <inheritdoc />
    /// <exception cref="ArgumentException" />
    /// <exception cref="ArgumentNullException" />
    [Pure]
    public virtual void CopyTo([DisallowNull] TElement?[] array,
                               in Int32 index)
    {
        ExceptionHelpers.ThrowIfArgumentNull(array);
        if (array.Rank != 1)
        {
            throw new ArgumentException(message: MULTI_DIMENSIONAL_ARRAYS);
        }

        Int32 i = 0;
        foreach (TElement? element in this.GetValuesFirstToLast())
        {
            array.SetValue(index: index + i++,
                           value: element);
        }
    }
}

// IContentForEach<T>
partial class ReadOnlyCollectionBase<TIndex, TElement> : IContentForEach<TElement?>
{
    /// <inheritdoc />
    /// <exception cref="ArgumentNullException" />
    /// <exception cref="NotAllowed" />
    [Pure]
    public virtual void ForEach([DisallowNull] Action<TElement?> action)
    {
        ExceptionHelpers.ThrowIfArgumentNull(action);

        foreach (TElement? element in this.GetValuesFirstToLast())
        {
            action.Invoke(element);
        }
    }
}

// IContentSegmentable<T, U>
partial class ReadOnlyCollectionBase<TIndex, TElement> : IContentSegmentable<TIndex, TElement?>
{
    /// <inheritdoc/>
    /// <exception cref="ArgumentException" />
    /// <exception cref="ArgumentOutOfRangeException" />
    [Pure]
    [return: NotNull]
    public virtual ICollection<TElement?> GetRange([DisallowNull] in TIndex startIndex,
                                                   [DisallowNull] in TIndex endIndex)
    {
        if (endIndex.CompareTo(other: startIndex) <= 0)
        {
            // No or negative count
            throw new ArgumentException(message: "The endIndex parameter needs to be larger than the startIndex parameter.",
                                        paramName: nameof(endIndex));
        }
        Collection<TElement?> result = new();
        foreach (KeyValuePair<TIndex, TElement?> kv in this.GetKeyValuePairsFirstToLast())
        {
            if (kv.Key
                  .CompareTo(other: startIndex) >= 0 &&
                kv.Key
                  .CompareTo(other: endIndex) <= 0)
            {
                result.Add(kv.Value);
            }
        }
        return result;
    }
}

// IElementFinder<T, U>
partial class ReadOnlyCollectionBase<TIndex, TElement> : IElementFinder<TElement?, TElement?>
{
    /// <inheritdoc/>
    /// <exception cref="ArgumentNullException" />
    /// <exception cref="NotAllowed" />
    [Pure]
    public virtual Boolean Exists([DisallowNull] Func<TElement?, Boolean> predicate)
    {
        ExceptionHelpers.ThrowIfArgumentNull(predicate);

        foreach (TElement? element in this.GetValuesFirstToLast())
        {
            if (predicate.Invoke(element))
            {
                return true;
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

        foreach (TElement? element in this.GetValuesFirstToLast())
        {
            if (predicate.Invoke(element))
            {
                return element;
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

        __Collection<TElement?> result = new();
        foreach (TElement? element in this.GetValuesFirstToLast())
        {
            if (predicate.Invoke(element))
            {
                result.Insert(index: result.Count,
                              item: element);
            }
        }
        return result;
    }

    /// <inheritdoc/>
    /// <exception cref="ArgumentNullException" />
    /// <exception cref="NotAllowed" />
    [Pure]
    [return: NotNull]
    public virtual IElementContainer<TElement?> FindExcept([DisallowNull] Func<TElement?, Boolean> predicate)
    {
        ExceptionHelpers.ThrowIfArgumentNull(predicate);

        __Collection<TElement?> result = new();
        foreach (TElement? element in this.GetValuesFirstToLast())
        {
            if (predicate.Invoke(element))
            {
                continue;
            }
            result.Insert(index: result.Count,
                          item: element);
        }
        return result;
    }

    /// <inheritdoc/>
    /// <exception cref="ArgumentNullException" />
    /// <exception cref="NotAllowed" />
    [Pure]
    [return: MaybeNull]
    public virtual TElement? FindLast([DisallowNull] Func<TElement?, Boolean> predicate)
    {
        ExceptionHelpers.ThrowIfArgumentNull(predicate);

        foreach (TElement? element in this.GetValuesLastToFirst())
        {
            if (predicate.Invoke(element))
            {
                return element;
            }
        }
        return default;
    }
}

// IIndexFinder<T, U>
partial class ReadOnlyCollectionBase<TIndex, TElement> : IIndexFinder<TIndex, TElement?>
{
    /// <inheritdoc/>
    /// <exception cref="ArgumentNullException" />
    /// <exception cref="ArgumentOutOfRangeException" />
    [Pure]
    public virtual TIndex FindIndex([DisallowNull] Func<TElement?, Boolean> predicate)
    {
        ExceptionHelpers.ThrowIfArgumentNull(predicate);

        foreach (KeyValuePair<TIndex, TElement?> kv in this.GetKeyValuePairsFirstToLast())
        {
            if (predicate.Invoke(kv.Value))
            {
                return kv.Key;
            }
        }
        return TIndex.NegativeOne;
    }
    /// <inheritdoc/>
    /// <exception cref="ArgumentNullException" />
    /// <exception cref="ArgumentOutOfRangeException" />
    [Pure]
    public virtual TIndex FindIndex([DisallowNull] in TIndex startIndex,
                                    [DisallowNull] in TIndex endIndex,
                                    [DisallowNull] Func<TElement?, Boolean> predicate)
    {
        ExceptionHelpers.ThrowIfArgumentNull(predicate);

        foreach (KeyValuePair<TIndex, TElement?> kv in this.GetKeyValuePairsFirstToLast())
        {
            if (kv.Key
                  .CompareTo(other: startIndex) < 0 ||
                kv.Key
                  .CompareTo(other: endIndex) > 0)
            {
                continue;
            }
            if (predicate.Invoke(kv.Value))
            {
                return kv.Key;
            }
        }
        return TIndex.NegativeOne;
    }

    /// <inheritdoc/>
    /// <exception cref="ArgumentNullException" />
    /// <exception cref="ArgumentOutOfRangeException" />
    [Pure]
    public virtual TIndex FindLastIndex([DisallowNull] Func<TElement?, Boolean> predicate)
    {
        ExceptionHelpers.ThrowIfArgumentNull(predicate);

        foreach (KeyValuePair<TIndex, TElement?> kv in this.GetKeyValuePairsLastToFirst())
        {
            if (predicate.Invoke(kv.Value))
            {
                return kv.Key;
            }
        }
        return TIndex.NegativeOne;
    }
    /// <inheritdoc/>
    /// <exception cref="ArgumentNullException" />
    /// <exception cref="ArgumentOutOfRangeException" />
    [Pure]
    public virtual TIndex FindLastIndex([DisallowNull] in TIndex startIndex,
                                        [DisallowNull] in TIndex endIndex,
                                        [DisallowNull] Func<TElement?, Boolean> predicate)
    {
        ExceptionHelpers.ThrowIfArgumentNull(predicate);

        foreach (KeyValuePair<TIndex, TElement?> kv in this.GetKeyValuePairsLastToFirst())
        {
            if (kv.Key
                  .CompareTo(other: startIndex) < 0 ||
                kv.Key
                  .CompareTo(other: endIndex) > 0)
            {
                continue;
            }
            if (predicate.Invoke(kv.Value))
            {
                return kv.Key;
            }
        }
        return TIndex.NegativeOne;
    }
}

// IReadOnlyList<T>
partial class ReadOnlyCollectionBase<TIndex, TElement> : IReadOnlyList<TElement?>
{
    [MaybeNull]
    TElement? IReadOnlyList<TElement?>.this[Int32 index]
    {
        get
        {
            Int32 i = 0;
            IEnumerator<TElement?> enumerator = this.GetValuesFirstToLast()
                                                    .GetEnumerator();
            while(enumerator.MoveNext() &&
                  i < index)
            {
                ++i;
            }

            if (i != index)
            {
                throw new IndexOutOfRangeException();
            }

            return enumerator.Current;
        }
    }
}