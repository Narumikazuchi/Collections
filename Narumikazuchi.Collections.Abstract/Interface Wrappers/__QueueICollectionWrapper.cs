namespace Narumikazuchi.Collections.Abstract;

internal readonly partial struct __QueueICollectionWrapper<TElement>
{
    public __QueueICollectionWrapper(Queue<TElement> source) =>
        this._source = source;

    public static explicit operator Queue<TElement>(__QueueICollectionWrapper<TElement> source) =>
        source._source;

}

// Non-Public
partial struct __QueueICollectionWrapper<TElement>
{
    private readonly Queue<TElement> _source;
}

// IEnumerable
partial struct __QueueICollectionWrapper<TElement> : IEnumerable<TElement>
{
    public IEnumerator<TElement> GetEnumerator() =>
        this._source.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() =>
        this._source.GetEnumerator();
}

// IReadOnlyCollection
partial struct __QueueICollectionWrapper<TElement> : IReadOnlyCollection<TElement>
{
    public Int32 Count =>
        this._source.Count;
}

// IReadOnlyCollection2
partial struct __QueueICollectionWrapper<TElement> : IReadOnlyCollection2<TElement>
{
    public Boolean Contains(TElement? item) =>
        this._source.Contains(item);

    public void CopyTo(TElement[] array, 
                       Int32 index) =>
        this._source.CopyTo(array, 
                            index);
}