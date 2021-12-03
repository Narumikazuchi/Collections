namespace Narumikazuchi.Collections.Abstract;

//public abstract partial class EntryBasedCollection<TIndex, TElement>
//    where TIndex : ISignedNumber<TIndex>
//{
//    public void Add([DisallowNull] TIndex index,
//                    [DisallowNull] TElement item)
//    {
//        ExceptionHelpers.ThrowIfArgumentNull(index);
//        ExceptionHelpers.ThrowIfArgumentNull(item);

//        Int32 hashcode = index.GetHashCode() & 0x7FFFFFFF;
//        Int32 target = hashcode % this._keys.Length;

//        for (Int32 i = this._keys[target].First; i >= 0; i = this._entries[i].NextKey)
//        {
//            if (this._entries[i].KeyHashcode == hashcode &&
//                this._entries[i].Value.Equals(item))
//            {
//                this._entries[i].Value = item;
//                return;
//            }
//        }

//        if (this._count == this._entries.Length)
//        {
//            this.EnsureCapacity();
//        }

//        Int32 insert = this._count++;

//        this._entries[insert].KeyHashcode = hashcode;
//        this._entries[insert].Key = index;
//        this._entries[insert].Value = item;

//        if (this._keys[target].IsEmpty)
//        {
//            this._entries[insert].NextKey = -1;
//            this._entries[insert].PreviousKey = -1;
//            this._keys[target].First = insert;
//            this._keys[target].Last = insert;
//        }
//        else
//        {
//            this._entries[this._keys[target].Last].NextKey = insert;
//            this._entries[insert].PreviousKey = this._keys[target].Last;
//            this._keys[target].Last = insert;
//        }

//        hashcode = item.GetHashCode() & 0x7FFFFFFF;
//        target = hashcode % this._values.Length;

//        if (this._values[target].IsEmpty)
//        {
//            this._entries[insert].NextValue = -1;
//            this._entries[insert].PreviousValue = -1;
//            this._values[target].First = insert;
//            this._values[target].Last = insert;
//        }
//        else
//        {
//            this._entries[this._keys[target].Last].NextValue = insert;
//            this._entries[insert].PreviousValue = this._values[target].Last;
//            this._values[target].Last = insert;
//        }
//    }

//    public Boolean Contains([DisallowNull] TElement item)
//    {
//        ExceptionHelpers.ThrowIfArgumentNull(item);

//        Int32 hashcode = item.GetHashCode() & 0x7FFFFFFF;
//        for (Int32 i = this._values[hashcode % this._values.Length].First; i >= 0; i = this._entries[i].NextKey)
//        {
//            if (this._entries[i].KeyHashcode == hashcode &&
//                this._entries[i].Value.Equals(item))
//            {
//                return true;
//            }
//        }
//        return false;
//    }

//    public IEnumerable<TIndex> Keys
//    {
//        get
//        {
//            for (Int32 i = 0; i < this._count; i++)
//            {
//                yield return this._entries[i].Key;
//            }
//            yield break;
//        }
//    }

//    public IEnumerable<TElement> Values
//    {
//        get
//        {
//            for (Int32 i = 0; i < this._count; i++)
//            {
//                yield return this._entries[i].Value;
//            }
//            yield break;
//        }
//    }
//}

//// Non-Public
//partial class EntryBasedCollection<TIndex, TElement>
//{
//    private protected EntryBasedCollection(in Int32 capacity)
//    {
//        Int32 size = Primes.GetNext(origin: capacity.Clamp(3, Int32.MaxValue));
//        this._keys = new __CollectionBucket[size];
//        this._values = new __CollectionBucket[size];
//        this._entries = new __CollectionEntry<TIndex, TElement>[size];
//    }

//    private protected __CollectionBucket[] _keys;
//    private protected __CollectionBucket[] _values;
//    private protected __CollectionEntry<TIndex, TElement>[] _entries;
//    private protected Int32 _count = 0;
//}

////
//partial class EntryBasedCollection<TIndex, TElement> : ICollection
//{
//    /// <inheritdoc/>
//    public Int32 Count => this._count;
//}

//// ICollectionExpandable<T>
//partial class EntryBasedCollection<TIndex, TElement> : ICollectionExpandable<TElement>
//{
//    void ICollectionExpandable<TElement>.EnsureCapacity(in Int32 capacity) =>
//        this.EnsureCapacity();

//    protected void EnsureCapacity()
//    {
//        Int32 size = Primes.GetNext(origin: this._keys.Length + 1);
//        __CollectionBucket[] newKeys = new __CollectionBucket[size];
//        __CollectionBucket[] newValues = new __CollectionBucket[size];
//        __CollectionEntry<TIndex, TElement>[] newEntries = new __CollectionEntry<TIndex, TElement>[size];

//        Array.Copy(sourceArray: this._entries,
//                   sourceIndex: 0,
//                   destinationArray: newEntries,
//                   destinationIndex: 0,
//                   this._count);

//        for (Int32 i = 0; i < this._count; i++)
//        {
//            if (newEntries[i].KeyHashcode >= 0)
//            {
//                Int32 target = newEntries[i].KeyHashcode % size;
//                if (newKeys[target].IsEmpty)
//                {
//                    newEntries[i].NextKey = newKeys[target].First;
//                    newKeys[target].First = i;
//                    newKeys[target].Last = i;
//                }
//                else
//                {
//                    newEntries[newKeys[target].Last].NextKey = i;
//                    newEntries[i].PreviousKey = newKeys[target].Last;
//                    newKeys[target].Last = i;
//                }
//                if (newValues[target].IsEmpty)
//                {
//                    newEntries[i].NextValue = newValues[target].First;
//                    newValues[target].First = i;
//                    newValues[target].Last = i;
//                }
//                else
//                {
//                    newEntries[newKeys[target].Last].NextValue = i;
//                    newEntries[i].PreviousValue = newValues[target].Last;
//                    newValues[target].Last = i;
//                }
//            }
//        }

//        this._keys = newKeys;
//        this._values = newValues;
//        this._entries = newEntries;
//    }
//}

//// IEnumerable<T>
//partial class EntryBasedCollection<TIndex, TElement> : IEnumerable<KeyValuePair<TIndex, TElement>>
//{
//    /// <inheritdoc/>
//    public IEnumerator<KeyValuePair<TIndex, TElement>> GetEnumerator()
//    {
//        for (Int32 i = 0; i < this._count; i++)
//        {
//            yield return new(key: this._entries[i].Key,
//                             value: this._entries[i].Value);
//        }
//    }

//    IEnumerator IEnumerable.GetEnumerator() =>
//        this.GetEnumerator();
//}

//// IEnumerable<T> - Element-Type
//partial class EntryBasedCollection<TIndex, TElement> : IEnumerable<TElement>
//{
//    IEnumerator<TElement> IEnumerable<TElement>.GetEnumerator()
//    {
//        for (Int32 i = 0; i < this._count; i++)
//        {
//            yield return this._entries[i].Value;
//        }
//    }
//}

////
//partial class EntryBasedCollection<TIndex, TElement> : IElementContainer
//{

//}

////
//partial class EntryBasedCollection<TIndex, TElement> : IIndexedReadOnlyCollection<TIndex>
//{

//}

//// IIndexedReadOnlyCollection<T, U>
//partial class EntryBasedCollection<TIndex, TElement> : IIndexedReadOnlyCollection<TIndex, TElement>
//{
//    /// <inheritdoc/>
//    public TIndex IndexOf([DisallowNull] TElement item)
//    {
//        ExceptionHelpers.ThrowIfArgumentNull(item);

//        Int32 hashcode = item.GetHashCode() & 0x7FFFFFFF;
//        for (Int32 i = this._values[hashcode % this._values.Length].First; i >= 0; i = this._entries[i].NextKey)
//        {
//            if (this._entries[i].KeyHashcode == hashcode &&
//                this._entries[i].Value.Equals(item))
//            {
//                return this._entries[i].Key;
//            }
//        }
//        return TIndex.NegativeOne;
//    }

//    /// <inheritdoc/>
//    public TIndex LastIndexOf([DisallowNull] TElement item)
//    {
//        ExceptionHelpers.ThrowIfArgumentNull(item);

//        Int32 hashcode = item.GetHashCode() & 0x7FFFFFFF;
//        for (Int32 i = this._values[hashcode % this._values.Length].Last; i >= 0; i = this._entries[i].PreviousKey)
//        {
//            if (this._entries[i].KeyHashcode == hashcode &&
//                this._entries[i].Value.Equals(item))
//            {
//                return this._entries[i].Key;
//            }
//        }
//        return TIndex.NegativeOne;
//    }

//    /// <inheritdoc/>
//    [NotNull]
//    public TElement this[[DisallowNull] TIndex index]
//    {
//        get
//        {
//            ExceptionHelpers.ThrowIfArgumentNull(index);

//            Int32 hashcode = index.GetHashCode() & 0x7FFFFFFF;
//            Int32 target = hashcode % this._keys.Length;
//            for (Int32 i = this._keys[target].First; i >= 0; i = this._entries[i].NextKey)
//            {
//                if (this._entries[i].KeyHashcode == hashcode &&
//                    this._entries[i].Key.Equals(index))
//                {
//                    return this._entries[i].Value;
//                }
//            }
//            throw new KeyNotFoundException();
//        }
//        set => throw new NotSupportedException();
//    }
//}