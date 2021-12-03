namespace Narumikazuchi.Collections.Abstract;

internal readonly partial struct __ListWrapper<TList, TElement>
    where TList : ICollection, ICollection<TElement?>, IList, IList<TElement?>, IReadOnlyCollection<TElement?>, IReadOnlyList<TElement?>
{
    public __ListWrapper(TList source) =>
        this._source = source;

    public static explicit operator TList(__ListWrapper<TList, TElement> source) =>
        source._source;
}

// Non-Public
partial struct __ListWrapper<TList, TElement>
{
    private readonly TList _source;
}

// IEnumerable<T>
partial struct __ListWrapper<TList, TElement> : IEnumerable<TElement?>
{
    IEnumerator<TElement?> IEnumerable<TElement?>.GetEnumerator() =>
        this._source.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() =>
        this._source.GetEnumerator();
}

// IReadOnlyCollection
partial struct __ListWrapper<TList, TElement> : IElementContainer
{
    Boolean IElementContainer.Contains(Object? item) => 
        this._source.Contains(item);

    void ICollection.CopyTo(Array array,
                            Int32 index) =>
        this._source.CopyTo(array: array,
                            index: index);

    Int32 ICollection.Count =>
        ((ICollection)this._source).Count;

    Boolean ICollection.IsSynchronized =>
        this._source.IsSynchronized;

    Object ICollection.SyncRoot =>
        this._source.SyncRoot;

    Boolean IElementContainer.IsFixedSize =>
        this._source.IsFixedSize;
}

// IReadOnlyCollection<T>
partial struct __ListWrapper<TList, TElement> : IReadOnlyCollection<TElement?>
{
    Int32 IReadOnlyCollection<TElement?>.Count =>
        ((IReadOnlyCollection<TElement?>)this._source).Count;
}

// IReadOnlyCollection2<T>
partial struct __ListWrapper<TList, TElement> : IElementContainer<TElement?>
{
    Boolean IElementContainer<TElement?>.Contains(TElement? item) =>
        this._source.Contains(item: item);
}

// IReadOnlyList<T>
partial struct __ListWrapper<TList, TElement> : IReadOnlyList<TElement?>
{
    TElement? IReadOnlyList<TElement?>.this[Int32 index] => 
        ((IReadOnlyList<TElement?>)this._source)[index];
}

// IReadOnlyList2<T>
partial struct __ListWrapper<TList, TElement> : IIndexedReadOnlyCollection<Int32>
{
    Int32 IIndexedReadOnlyCollection<Int32>.IndexOf(Object? item)
    {
        if (item is TElement element)
        {
            return this._source.IndexOf(item: element);
        }
        return -1;
    }

    Int32 IIndexedReadOnlyCollection<Int32>.LastIndexOf(Object? item)
    {
        if (item is TElement element)
        {
            TElement?[] array = this._source.ToArray();
            return Array.LastIndexOf(array: array,
                                     value: element);
        }
        return -1;
    }

    Object? IIndexedReadOnlyCollection<Int32>.this[Int32 index] =>
        ((IIndexedReadOnlyCollection<Int32>)this._source)[index];
}

// IReadOnlyList2<T, U>
partial struct __ListWrapper<TList, TElement> : IIndexedReadOnlyCollection<Int32, TElement?>
{
    Int32 IIndexedReadOnlyCollection<Int32, TElement?>.IndexOf(TElement? item) =>
        this._source.IndexOf(item: item);
    Int32 IIndexedReadOnlyCollection<Int32, TElement?>.LastIndexOf(TElement? item) =>
        ((IIndexedReadOnlyCollection<Int32>)this).LastIndexOf(item: item);

    TElement? IIndexedReadOnlyCollection<Int32, TElement?>.this[Int32 index] =>
        ((IIndexedReadOnlyCollection<Int32, TElement?>)this._source)[index];
}