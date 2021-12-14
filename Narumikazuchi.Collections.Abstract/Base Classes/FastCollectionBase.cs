namespace Narumikazuchi.Collections.Abstract;

/// <summary>
/// Represents a collection which occupies more memory in favor of speed.
/// </summary>
public abstract partial class FastCollectionBase<TIndex, TElement>
    where TIndex : ISignedNumber<TIndex>
{ }

// Non-Public
partial class FastCollectionBase<TIndex, TElement>
{
    private protected FastCollectionBase()
    {
        this._keys = new __CollectionBucket[3];
        this._values = new __CollectionBucket[3];
        this._entries = new __CollectionEntry<TIndex, TElement?>[DEFAULTCAPACITY];
        Reset(keys: this._keys,
              values: this._values,
              entries: this._entries);
    }

    private static void Reset(__CollectionBucket[] keys,
                              __CollectionBucket[] values,
                              __CollectionEntry<TIndex, TElement?>[] entries)
    {
        for (Int32 i = 0; i < entries.Length; i++)
        {
            if (i < keys.Length)
            {
                keys[i].First = -1;
                keys[i].Last = -1;
                values[i].First = -1;
                values[i].Last = -1;
            }
            entries[i].Key = default;
            entries[i].KeyHashcode = -1;
            entries[i].ValueHashcode = -1;
            entries[i].NextKey = -1;
            entries[i].PreviousKey = -1;
            entries[i].NextValue = -1;
            entries[i].PreviousValue = -1;
            entries[i].Value = default;
        }
    }

    /// <summary>
    /// Returns an iterator for this collection, which iterates from the first key to the last.
    /// </summary>
    [Pure]
    [return: NotNull]
    protected IEnumerable<TIndex> GetKeysFirstToLast()
    {
        Int32 v = this._version;
        lock (this._syncRoot)
        {
            for (Int32 i = 0; i < this._count; i++)
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
                yield return this._entries[i].Key;
            }
        }
        yield break;
    }

    /// <summary>
    /// Returns an iterator for this collection, which iterates from the last key to the first.
    /// </summary>
    [Pure]
    [return: NotNull]
    protected IEnumerable<TIndex> GetKeysLastToFirst()
    {
        Int32 v = this._version;
        lock (this._syncRoot)
        {
            for (Int32 i = this._count - 1; i > 0; i--)
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
                yield return this._entries[i].Key;
            }
        }
        yield break;
    }

    /// <summary>
    /// Returns an iterator for this collection, which iterates from the first value to the last.
    /// </summary>
    [Pure]
    [return: NotNull]
    protected IEnumerable<TElement?> GetValuesFirstToLast()
    {
        Int32 v = this._version;
        lock (this._syncRoot)
        {
            for (Int32 i = 0; i < this._count; i++)
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
                yield return this._entries[i].Value;
            }
        }
        yield break;
    }

    /// <summary>
    /// Returns an iterator for this collection, which iterates from the last value to the first.
    /// </summary>
    [Pure]
    [return: NotNull]
    protected IEnumerable<TElement?> GetValuesLastToFirst()
    {
        Int32 v = this._version;
        lock (this._syncRoot)
        {
            for (Int32 i = this._count - 1; i > 0; i--)
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
                yield return this._entries[i].Value;
            }
        }
        yield break;
    }

    /// <summary>
    /// Returns an iterator for this collection, which iterates from the first key-value-pair to the last.
    /// </summary>
    [Pure]
    [return: NotNull]
    protected IEnumerable<KeyValuePair<TIndex, TElement?>> GetKeyValuePairsFirstToLast()
    {
        Int32 v = this._version;
        lock (this._syncRoot)
        {
            for (Int32 i = 0; i < this._count; i++)
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
                yield return new KeyValuePair<TIndex, TElement?>(key: this._entries[i].Key,
                                                                 value: this._entries[i].Value);
            }
        }
        yield break;
    }

    /// <summary>
    /// Returns an iterator for this collection, which iterates from the last key-value-pair to the first.
    /// </summary>
    [Pure]
    [return: NotNull]
    protected IEnumerable<KeyValuePair<TIndex, TElement?>> GetKeyValuePairsLastToFirst()
    {
        Int32 v = this._version;
        lock (this._syncRoot)
        {
            for (Int32 i = this._count - 1; i > 0; i--)
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
                yield return new KeyValuePair<TIndex, TElement?>(key: this._entries[i].Key,
                                                                 value: this._entries[i].Value);
            }
        }
        yield break;
    }

    /// <summary>
    /// Appends the specified item at the end of the collection.
    /// </summary>
    /// <param name="item">The item to append.</param>
    protected Boolean AppendInternal(TElement item)
    {
        TIndex key = this._lastKey + TIndex.One;
        return this.InsertInternal(index: key, 
                                   item: item);
    }

    /// <summary>
    /// Clears the entire collection of all items.
    /// </summary>
    protected void ClearInternal()
    {
        Reset(keys: this._keys,
              values: this._values,
              entries: this._entries);
        this._count = 0;
    }

    /// <summary>
    /// Adds the specified item at the specified index.
    /// </summary>
    /// <param name="index">The index at which to insert the item.</param>
    /// <param name="item">The item to insert.</param>
    /// <exception cref="ArgumentNullException" />
    protected Boolean InsertInternal([DisallowNull] in TIndex index,
                                     TElement item)
    {
        ExceptionHelpers.ThrowIfArgumentNull(index);

        Int32 hashcode = index.GetHashCode() & MAXARRAYSIZE;
        Int32 target = hashcode % this._keys.Length;

        for (Int32 i = this._keys[target].First; i >= 0; i = this._entries[i].NextKey)
        {
            if (this._entries[i].KeyHashcode == hashcode &&
                ((item is not null &&
                item.Equals(this._entries[i].Value)) ||
                (item is null &&
                this._entries[i].Value is null)))
            {
                this._entries[i].Value = item;
                this._entries[i].ValueHashcode = item is null
                                                    ? Int32.MaxValue
                                                    : item.GetHashCode() & MAXARRAYSIZE;
                return true;
            }
        }

        if (this._count == this._entries.Length)
        {
            this.EnsureCapacity(capacity: this._count + 1);
            target = hashcode % this._keys.Length;
        }

        Int32 insert = this._count++;

        this._entries[insert].KeyHashcode = hashcode;
        this._entries[insert].Key = index;
        this._entries[insert].Value = item;

        if (this._keys[target].IsEmpty)
        {
            this._entries[insert].NextKey = -1;
            this._entries[insert].PreviousKey = -1;
            this._keys[target].First = insert;
            this._keys[target].Last = insert;
        }
        else
        {
            this._entries[this._keys[target].Last].NextKey = insert;
            this._entries[insert].PreviousKey = this._keys[target].Last;
            this._keys[target].Last = insert;
        }

        hashcode = item is null
                        ? Int32.MaxValue
                        : item.GetHashCode() & MAXARRAYSIZE;
        target = hashcode % this._values.Length;

        this._entries[insert].ValueHashcode = hashcode;

        if (this._values[target].IsEmpty)
        {
            this._entries[insert].NextValue = -1;
            this._entries[insert].PreviousValue = -1;
            this._values[target].First = insert;
            this._values[target].Last = insert;
        }
        else
        {
            this._entries[this._values[target].Last].NextValue = insert;
            this._entries[insert].PreviousValue = this._values[target].Last;
            this._values[target].Last = insert;
        }

        this._lastKey = index;
        return true;
    }

    /// <summary>
    /// Removes the specified item from the collection.
    /// </summary>
    /// <param name="item">The item to remove.</param>
    /// <returns><see langword="true"/> if the specified item was removed successfully; otherwise, <see langword="false"/></returns>
    protected Boolean RemoveInternal(TElement item)
    {
        Int32 hashcode = item is null
                            ? Int32.MaxValue
                            : item.GetHashCode() & MAXARRAYSIZE;
        Int32 target = hashcode % this._values.Length;

        for (Int32 i = this._values[target].First; i >= 0; i = this._entries[i].NextKey)
        {
            if (this._entries[i].ValueHashcode == hashcode)
            {
                __CollectionEntry<TIndex, TElement?> currentEntry = this._entries[i];
                ref __CollectionBucket currentValue = ref this._values[target];
                if (currentValue.First == currentValue.Last)
                {
                    currentValue.First = -1;
                    currentValue.Last = -1;
                }
                else if (currentValue.First == i)
                {
                    currentValue.First = currentEntry.NextKey;
                }
                else if (currentValue.Last == i)
                {
                    currentValue.Last = currentEntry.PreviousKey;
                }
                else
                {
                    this._entries[currentEntry.PreviousKey].NextKey = currentEntry.NextKey;
                    this._entries[currentEntry.NextKey].PreviousKey = currentEntry.PreviousKey;
                    this._entries[currentEntry.PreviousValue].NextValue = currentEntry.NextValue;
                    this._entries[currentEntry.NextValue].PreviousValue = currentEntry.PreviousValue;
                }

                Array.Copy(sourceArray: this._entries,
                           sourceIndex: i + 1,
                           destinationArray: this._entries,
                           destinationIndex: i,
                           length: this._count - i - 1);

                this._entries[this._count].Key = default;
                this._entries[this._count].Value = default;
                this._entries[this._count].KeyHashcode = -1;
                this._entries[this._count].ValueHashcode = -1;
                this._entries[this._count].NextKey = -1;
                this._entries[this._count].PreviousKey = -1;
                this._entries[this._count].NextValue = -1;
                this._entries[this._count].PreviousValue = -1;

                this._count--;

                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// Removes the item with the specified index from the collection.
    /// </summary>
    /// <param name="index">The index of the item to remove.</param>
    /// <returns><see langword="true"/> if the specified item was removed successfully; otherwise, <see langword="false"/></returns>
    protected Boolean RemoveAtInternal([DisallowNull] in TIndex index)
    {
        ExceptionHelpers.ThrowIfArgumentNull(index);

        Int32 hashcode = index.GetHashCode() & MAXARRAYSIZE;
        Int32 target = hashcode % this._keys.Length;

        for (Int32 i = this._keys[target].First; i >= 0; i = this._entries[i].NextKey)
        {
            if (this._entries[i].KeyHashcode == hashcode)
            {
                __CollectionEntry<TIndex, TElement?> currentEntry = this._entries[i];
                ref __CollectionBucket currentKey = ref this._keys[target];
                if (currentKey.First == currentKey.Last)
                {
                    currentKey.First = -1;
                    currentKey.Last = -1;
                }
                else if (currentKey.First == i)
                {
                    currentKey.First = currentEntry.NextKey;
                }
                else if (currentKey.Last == i)
                {
                    currentKey.Last = currentEntry.PreviousKey;
                }
                else
                {
                    this._entries[currentEntry.PreviousKey].NextKey = currentEntry.NextKey;
                    this._entries[currentEntry.NextKey].PreviousKey = currentEntry.PreviousKey;
                    this._entries[currentEntry.PreviousValue].NextValue = currentEntry.NextValue;
                    this._entries[currentEntry.NextValue].PreviousValue = currentEntry.PreviousValue;
                }

                Array.Copy(sourceArray: this._entries,
                           sourceIndex: i + 1,
                           destinationArray: this._entries,
                           destinationIndex: i,
                           length: this._count - i - 1);

                this._entries[this._count].Key = default;
                this._entries[this._count].Value = default;
                this._entries[this._count].KeyHashcode = -1;
                this._entries[this._count].ValueHashcode = -1;
                this._entries[this._count].NextKey = -1;
                this._entries[this._count].PreviousKey = -1;
                this._entries[this._count].NextValue = -1;
                this._entries[this._count].PreviousValue = -1;

                this._count--;

                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// Ensures that the collection can hold the specified amount of items.
    /// </summary>
    /// <param name="capacity">The amount of items that need space.</param>
    protected void EnsureCapacity(in Int32 capacity)
    {
        Int32 size = Primes.GetPrevious(origin: (capacity / 2).Clamp(5, Int32.MaxValue));
        __CollectionBucket[] newKeys = new __CollectionBucket[size];
        __CollectionBucket[] newValues = new __CollectionBucket[size];
        __CollectionEntry<TIndex, TElement?>[] newEntries = new __CollectionEntry<TIndex, TElement?>[Primes.GetNext(origin: capacity)];
        Reset(keys: newKeys,
              values: newValues,
              entries: newEntries);

        Array.Copy(sourceArray: this._entries,
                   sourceIndex: 0,
                   destinationArray: newEntries,
                   destinationIndex: 0,
                   length: this._count);

        for (Int32 i = 0; i < this._count; i++)
        {
            if (newEntries[i].KeyHashcode >= 0)
            {
                newEntries[i].NextKey = -1;
                newEntries[i].PreviousKey = -1;
                newEntries[i].NextValue = -1;
                newEntries[i].PreviousValue = -1;

                Int32 target = newEntries[i].KeyHashcode % size;
                if (newKeys[target].IsEmpty)
                {
                    newKeys[target].First = i;
                    newKeys[target].Last = i;
                }
                else
                {
                    newEntries[newKeys[target].Last].NextKey = i;
                    newEntries[i].PreviousKey = newKeys[target].Last;
                    newKeys[target].Last = i;
                }

                target = newEntries[i].ValueHashcode % size;
                if (newValues[target].IsEmpty)
                {
                    newValues[target].First = i;
                    newValues[target].Last = i;
                }
                else
                {
                    newEntries[newValues[target].Last].NextValue = i;
                    newEntries[i].PreviousValue = newValues[target].Last;
                    newValues[target].Last = i;
                }
            }
        }

        this._keys = newKeys;
        this._values = newValues;
        this._entries = newEntries;
    }

    /// <summary>
    /// Gets or sets the current version of the collection.
    /// </summary>
    [Pure]
    protected Int32 Version
    {
        get => this._version;
        set
        {
            if (value < this._version)
            {
                throw new ArgumentException(message: "The version of the collection can't be decremented.",
                                            paramName: nameof(value));
            }
            this._version = value;
        }
    }

    /// <summary>
    /// Represents the amount of items currently present in the collection.
    /// </summary>
    protected Int32 _count = 0;
    [NotNull]
    private protected __CollectionEntry<TIndex, TElement?>[] _entries;
    [NotNull]
    private __CollectionBucket[] _keys;
    [NotNull]
    private __CollectionBucket[] _values;
    [NotNull]
    private readonly Object _syncRoot = new();
    private TIndex _lastKey = TIndex.NegativeOne;
    private Int32 _version = 0;

#pragma warning disable
    /// <summary>
    /// Represents the default capacity for a new <see cref="ArrayBasedCollection{T}"/>.
    /// </summary>
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    protected const Int32 DEFAULTCAPACITY = 4;
    /// <summary>
    /// Represents the max. size to which the internal array can expand to.
    /// </summary>
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    protected const Int32 MAXARRAYSIZE = 0x7FEFFFFF;
    /// <summary>
    /// Error message, when trying to change _item size while <see cref="IsFixedSize"/> is <see langword="true"/>.
    /// </summary>
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    protected const String SIZE_IS_FIXED = "The capacity for the collection can't be changed, since its size is fixed.";
    /// <summary>
    /// Error message, when the amount of items in the collection have changed during enumeration.
    /// </summary>
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    protected const String COLLECTION_CHANGED = "The collection changed during enumeration.";
#pragma warning restore
}

// ICollection
partial class FastCollectionBase<TIndex, TElement> : ICollection
{
    [Pure]
    void ICollection.CopyTo(Array array,
                            Int32 index)
    {
        ExceptionHelpers.ThrowIfArgumentNull(array);
        if (array.Rank != 1)
        {
            throw new ArgumentException(message: "Multidimensional arrays are not suported for a 'CopyTo' action.",
                                        paramName: nameof(array));
        }

        if (array is not TElement[]
                  and not Object[])
        {
            throw new ArrayTypeMismatchException();
        }

        for (Int32 i = 0; i < this.Count; i++)
        {
            array.SetValue(index: index + i,
                           value: this._entries[i].Value);
        }
    }

    /// <inheritdoc/>
    [Pure]
    public Int32 Count => 
        this._count;
}

// ICollectionImmutability
partial class FastCollectionBase<TIndex, TElement> : ICollectionImmutability
{
    /// <inheritdoc/>
    [Pure]
    public virtual Boolean IsFixedSize { get; } = false;
}

// IConvertToArray<T>
partial class FastCollectionBase<TIndex, TElement> : IConvertToArray<TElement?[]>
{
    /// <inheritdoc/>
    [Pure]
    [return: NotNull]
    public virtual TElement?[] ToArray()
    {
        lock (this._syncRoot)
        {
            if (this._count == 0)
            {
                return Array.Empty<TElement?>();
            }

            TElement?[] array = new TElement?[this._count];
            for (Int32 i = 0; i < this._count; i++)
            {
                array[i] = this._entries[i].Value;
            }
            return array;
        }
    }
}

// IElementContainer
partial class FastCollectionBase<TIndex, TElement> : IElementContainer
{
    [Pure]
    Boolean IElementContainer.Contains(Object? item) =>
        item is TElement element &&
        this.Contains(element);
}

// IElementContainer<T>
partial class FastCollectionBase<TIndex, TElement> : IElementContainer<TElement?>
{
    /// <inheritdoc/>
    [Pure]
    public Boolean Contains([AllowNull] TElement? item)
    {
        Int32 hashcode = item is null
                            ? Int32.MaxValue
                            : item.GetHashCode() & MAXARRAYSIZE;
        for (Int32 i = this._values[hashcode % this._values.Length].First; i >= 0; i = this._entries[i].NextValue)
        {
            if (this._entries[i].ValueHashcode == hashcode)
            {
                return true;
            }
        }
        return false;
    }
}

// IEnumerable
partial class FastCollectionBase<TIndex, TElement> : IEnumerable
{
    IEnumerator IEnumerable.GetEnumerator() =>
        this.GetKeyValuePairsFirstToLast()
            .GetEnumerator();
}

// IEnumerable<T>
partial class FastCollectionBase<TIndex, TElement> : IEnumerable<TElement?>
{
    IEnumerator<TElement?> IEnumerable<TElement?>.GetEnumerator() =>
        this.GetValuesFirstToLast()
            .GetEnumerator();
}

// IIndexedReadOnlyCollection<T>
partial class FastCollectionBase<TIndex, TElement> : IIndexedReadOnlyCollection<TIndex>
{
    TIndex IIndexedReadOnlyCollection<TIndex>.IndexOf(Object? item) =>
        item is TElement element
            ? this.IndexOf(element)
            : TIndex.NegativeOne;
    TIndex IIndexedReadOnlyCollection<TIndex>.LastIndexOf(Object? item) =>
        item is TElement element
            ? this.LastIndexOf(element)
            : TIndex.NegativeOne;

    Object? IIndexedReadOnlyCollection<TIndex>.this[TIndex index] =>
        this[index];
}

// IIndexedReadOnlyCollection<T, U>
partial class FastCollectionBase<TIndex, TElement> : IIndexedReadOnlyCollection<TIndex, TElement?>
{
    /// <inheritdoc/>
    [return: NotNull]
    public TIndex IndexOf(TElement? item)
    {
        Int32 hashcode = item is null
                            ? Int32.MaxValue
                            : item.GetHashCode() & MAXARRAYSIZE;
        Int32 target = hashcode % this._values.Length;

        TIndex? indexDefault = default;

        for (Int32 i = this._values[target].First; i >= 0; i = this._entries[i].NextValue)
        {
            if (hashcode == this._entries[i].ValueHashcode)
            {
                if (indexDefault is not null &&
                    indexDefault.CompareTo(this._entries[i].Key) == 0)
                {
                    return TIndex.NegativeOne;
                }
                if (indexDefault is null &&
                    this._entries[i].Key is null)
                {
                    return TIndex.NegativeOne;
                }
                return this._entries[i].Key;
            }
        }
        return TIndex.NegativeOne;
    }

    /// <inheritdoc/>
    [return: NotNull]
    public TIndex LastIndexOf(TElement? item)
    {
        Int32 hashcode = item is null
                            ? Int32.MaxValue
                            : item.GetHashCode() & MAXARRAYSIZE;
        Int32 target = hashcode % this._values.Length;

        TIndex? indexDefault = default;

        for (Int32 i = this._values[target].Last; i >= 0; i = this._entries[i].PreviousValue)
        {
            if (hashcode == this._entries[i].ValueHashcode)
            {
                if (indexDefault is not null &&
                    indexDefault.CompareTo(this._entries[i].Key) == 0)
                {
                    return TIndex.NegativeOne;
                }
                if (indexDefault is null &&
                    this._entries[i].Key is null)
                {
                    return TIndex.NegativeOne;
                }
                return this._entries[i].Key;
            }
        }
        return TIndex.NegativeOne;
    }

    /// <inheritdoc/>
    public TElement? this[[DisallowNull] TIndex index]
    {
        get
        {
            ExceptionHelpers.ThrowIfArgumentNull(index);

            Int32 hashcode = index.GetHashCode() & MAXARRAYSIZE;
            Int32 target = hashcode % this._keys.Length;

            for (Int32 i = this._keys[target].First; i >= 0; i = this._entries[i].NextKey)
            {
                if (this._entries[i].KeyHashcode == hashcode)
                {
                    return this._entries[i].Value;
                }
            }
            throw new KeyNotFoundException();
        }
    }
}

// ISynchronized
partial class FastCollectionBase<TIndex, TElement> : ISynchronized
{
    /// <inheritdoc/>
    [Pure]
    public virtual Boolean IsSynchronized { get; } = true;

    /// <inheritdoc/>
    [Pure]
    public Object SyncRoot => 
        this._syncRoot;
}