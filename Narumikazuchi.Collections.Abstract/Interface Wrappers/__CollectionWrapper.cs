namespace Narumikazuchi.Collections.Abstract;

internal readonly partial struct __CollectionWrapper<TCollection, TElement>
    where TCollection : ICollection, ICollection<TElement?>, IReadOnlyCollection<TElement?>
{
    public __CollectionWrapper(TCollection source) =>
        this._source = source;

    public static explicit operator TCollection(__CollectionWrapper<TCollection, TElement> source) =>
        source._source;
}

// Non-Public
partial struct __CollectionWrapper<TCollection, TElement>
{
    private readonly TCollection _source;
}

// IEnumerable<T>
partial struct __CollectionWrapper<TCollection, TElement> : IEnumerable<TElement?>
{
    IEnumerator<TElement?> IEnumerable<TElement?>.GetEnumerator() =>
        this._source.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() =>
        this._source.GetEnumerator();
}

// IReadOnlyCollection
partial struct __CollectionWrapper<TCollection, TElement> : IElementContainer
{
    Boolean IElementContainer.Contains(Object? item)
    {
        if (item is TElement element)
        {
            return this._source.Contains(item: element);
        }
        return false;
    }

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
        this._source.IsReadOnly;
}

// IReadOnlyCollection<T>
partial struct __CollectionWrapper<TCollection, TElement> : IReadOnlyCollection<TElement?>
{
    Int32 IReadOnlyCollection<TElement?>.Count =>
        ((IReadOnlyCollection<TElement?>)this._source).Count;
}

// IReadOnlyCollection2<T>
partial struct __CollectionWrapper<TCollection, TElement> : IElementContainer<TElement?>
{
    Boolean IElementContainer<TElement?>.Contains(TElement? item) =>
        this._source.Contains(item: item);
}