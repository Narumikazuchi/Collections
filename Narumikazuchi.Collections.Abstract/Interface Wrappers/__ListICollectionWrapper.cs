namespace Narumikazuchi.Collections.Abstract;

internal readonly partial struct __ListICollectionWrapper<TElement>
{
    public __ListICollectionWrapper(List<TElement> source) =>
        this._source = source;

    public static explicit operator List<TElement>(__ListICollectionWrapper<TElement> source) =>
        source._source;

}

// Non-Public
partial struct __ListICollectionWrapper<TElement>
{
    private readonly List<TElement> _source;
}

// IEnumerable
partial struct __ListICollectionWrapper<TElement> : IEnumerable<TElement>
{
    public IEnumerator<TElement> GetEnumerator() =>
        this._source.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() =>
        this._source.GetEnumerator();
}

// IReadOnlyCollection
partial struct __ListICollectionWrapper<TElement> : IReadOnlyCollection<TElement>
{
    public Int32 Count =>
        this._source.Count;
}

// IReadOnlyCollection2
partial struct __ListICollectionWrapper<TElement> : IReadOnlyCollection2<TElement>
{
    public Boolean Contains(TElement? item) =>
        this._source.Contains(item);

    public void CopyTo(TElement[] array, 
                       Int32 index) =>
        this._source.CopyTo(array, 
                            index);
}