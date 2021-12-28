namespace Narumikazuchi.Collections.Abstract;

internal readonly partial struct __CollectionWrapper<TCollection, TElement>
    where TCollection : ICollection, ICollection<TElement?>, IReadOnlyCollection<TElement?>
{
    public __CollectionWrapper(TCollection source)
    {
        this._source = source;
    }

    public static explicit operator TCollection(__CollectionWrapper<TCollection, TElement> source) =>
        source._source;
}

// Non-Public
partial struct __CollectionWrapper<TCollection, TElement>
{
    private readonly TCollection _source;
}

// Collection
partial struct __CollectionWrapper<TCollection, TElement> : ICollection
{
    void ICollection.CopyTo(Array array,
                            Int32 index) =>
        this._source
            .CopyTo(array: array,
                    index: index);

    Int32 ICollection.Count =>
        ((ICollection)this._source).Count;

    Boolean ICollection.IsSynchronized =>
        this._source
            .IsSynchronized;

    Object ICollection.SyncRoot =>
        this._source
            .SyncRoot;
}

// IContentAddable<T>
partial struct __CollectionWrapper<TCollection, TElement> : IContentAddable<TElement>
{
    Boolean IContentAddable<TElement>.Add(TElement item)
    {
        this._source
            .Add(item);
        return true;
    }

    void IContentAddable<TElement>.AddRange(IEnumerable<TElement> collection)
    {
        ExceptionHelpers.ThrowIfArgumentNull(collection);
        foreach (TElement item in collection)
        {
            this._source
                .Add(item);
        }
    }
}

// IContentClearable
partial struct __CollectionWrapper<TCollection, TElement> : IContentClearable
{
    void IContentClearable.Clear() => 
        this._source
            .Clear();
}

// IContentConvertable<T>
partial struct __CollectionWrapper<TCollection, TElement> : IContentConvertable<TElement>
{
    ICollection<TOutput> IContentConvertable<TElement>.ConvertAll<TOutput>(Converter<TElement, TOutput> converter)
    {
        if (this._source is List<TElement> list)
        {
            return list.ConvertAll(converter);
        }
        if (this._source is TElement[] array)
        {
            return Array.ConvertAll(array: array,
                                    converter: converter);
        }
        if (this._source is ImmutableList<TElement> immutable)
        {
            return immutable.ConvertAll(input => converter.Invoke(input));
        }

        ExceptionHelpers.ThrowIfArgumentNull(converter);

        Collection<TOutput> result = new();
        foreach (TElement? item in this._source)
        {
            result.Add(converter.Invoke(item!));
        }
        return result;
    }
}

// IContentCopyable<T, U>
partial struct __CollectionWrapper<TCollection, TElement> : IContentCopyable<Int32, TElement[]>
{
    void IContentCopyable<Int32, TElement[]>.CopyTo(TElement[] array, 
                                                    in Int32 index) => 
        this._source
            .CopyTo(array: array,
                    index: index);
}

// IContentForEach<T>
partial struct __CollectionWrapper<TCollection, TElement> : IContentForEach<TElement>
{
    void IContentForEach<TElement>.ForEach(Action<TElement> action)
    {
        if (this._source is List<TElement> list)
        {
            list.ForEach(action);
            return;
        }
        if (this._source is TElement[] array)
        {
            Array.ForEach(array: array,
                          action: action);
            return;
        }
        if (this._source is ImmutableList<TElement> immutable)
        {
            immutable.ForEach(action);
            return;
        }

        ExceptionHelpers.ThrowIfArgumentNull(action);
        foreach (TElement? item in this._source)
        {
            action.Invoke(item!);
        }
    }
}

// IContentRemovable
partial struct __CollectionWrapper<TCollection, TElement> : IContentRemovable
{
    Boolean IContentRemovable.Remove(Object item) =>
        item is TElement element &&
        this._source
            .Remove(element);
}

// IContentRemovable<T>
partial struct __CollectionWrapper<TCollection, TElement> : IContentRemovable<TElement>
{
    Boolean IContentRemovable<TElement>.Remove(TElement item) =>
        this._source
            .Remove(item);

    Int32 IContentRemovable<TElement>.RemoveAll(Func<TElement, Boolean> predicate)
    {
        if (this._source is List<TElement> list)
        {
            return list.RemoveAll(input => predicate.Invoke(input));
        }

        ExceptionHelpers.ThrowIfArgumentNull(predicate);

        Collection<TElement> remove = new();
        foreach (TElement? item in this._source)
        {
            if (predicate.Invoke(item!))
            {
                remove.Add(item!);
            }
        }

        Int32 skipped = 0;
        foreach (TElement item in remove)
        {
            if (!this._source
                     .Remove(item))
            {
                skipped++;
            }
        }
        return remove.Count - skipped;
    }
}

// IConvertToArray<T>
partial struct __CollectionWrapper<TCollection, TElement> : IConvertToArray<TElement[]>
{
    TElement[] IConvertToArray<TElement[]>.ToArray()
    {
        if (this._source is TElement[] array)
        {
            return array.ToArray();
        }
        if (this._source is ArraySegment<TElement> segment)
        {
            return segment.ToArray();
        }
        if (this._source is BlockingCollection<TElement> blocking)
        {
            return blocking.ToArray();
        }
        if (this._source is ConcurrentBag<TElement> cbag)
        {
            return cbag.ToArray();
        }
        if (this._source is ConcurrentQueue<TElement> cqueue)
        {
            return cqueue.ToArray();
        }
        if (this._source is ConcurrentStack<TElement> cstack)
        {
            return cstack.ToArray();
        }
        if (this._source is List<TElement> list)
        {
            return list.ToArray();
        }
        if (this._source is Queue<TElement> gqueue)
        {
            return gqueue.ToArray();
        }
        if (this._source is Stack<TElement> gstack)
        {
            return gstack.ToArray();
        }

        TElement[] result = new TElement[((ICollection)this._source).Count];
        this._source
            .CopyTo(array: result,
                    arrayIndex: 0);
        return result;
    }
}

// IElementContainer
partial struct __CollectionWrapper<TCollection, TElement> : IElementContainer
{
    Boolean IElementContainer.Contains(Object? item) =>
        item is TElement element &&
        this._source
            .Contains(item: element);
}

// IElementContainer<T>
partial struct __CollectionWrapper<TCollection, TElement> : IElementContainer<TElement?>
{
    Boolean IElementContainer<TElement?>.Contains(TElement? item) =>
        this._source
            .Contains(item);
}

// IElementFinder<T, U>
partial struct __CollectionWrapper<TCollection, TElement> : IElementFinder<TElement, TElement>
{
    Boolean IElementFinder<TElement, TElement>.Exists(Func<TElement, Boolean> predicate)
    {
        if (this._source is TElement[] array)
        {
            return Array.Exists(array: array,
                                match: input => predicate.Invoke(input));
        }
        if (this._source is List<TElement> list)
        {
            return list.Exists(input => predicate.Invoke(input));
        }

        ExceptionHelpers.ThrowIfArgumentNull(predicate);

        foreach (TElement? item in this._source)
        {
            if (predicate.Invoke(item!))
            {
                return true;
            }
        }
        return false;
    }

    TElement? IElementFinder<TElement, TElement>.Find(Func<TElement, Boolean> predicate)
    {
        if (this._source is TElement[] array)
        {
            return Array.Find(array: array,
                              match: input => predicate.Invoke(input));
        }
        if (this._source is List<TElement> list)
        {
            return list.Find(input => predicate.Invoke(input));
        }

        ExceptionHelpers.ThrowIfArgumentNull(predicate);

        foreach (TElement? item in this._source)
        {
            if (predicate.Invoke(item!))
            {
                return item;
            }
        }
        return default;
    }

    IElementContainer<TElement> IElementFinder<TElement, TElement>.FindAll(Func<TElement, Boolean> predicate)
    {
        if (this._source is TElement[] array)
        {
            return new __Collection<TElement>(source: Array.FindAll(array: array,
                                                                    match: input => predicate.Invoke(input)))!;
        }
        if (this._source is List<TElement> list)
        {
            return new __Collection<TElement>(source: list.FindAll(input => predicate.Invoke(input)))!;
        }

        ExceptionHelpers.ThrowIfArgumentNull(predicate);

        __Collection<TElement> result = new();
        foreach (TElement? item in this._source)
        {
            if (predicate.Invoke(item!))
            {
                result.Insert(index: result.Count,
                              item: item);
            }
        }
        return result!;
    }

    IElementContainer<TElement> IElementFinder<TElement, TElement>.FindExcept(Func<TElement, Boolean> predicate)
    {
        if (this._source is TElement[] array)
        {
            return new __Collection<TElement>(source: array.Except(Array.FindAll(array: array,
                                                                                 match: input => predicate.Invoke(input))))!;
        }
        if (this._source is List<TElement> list)
        {
            return new __Collection<TElement>(source: list.Except(list.FindAll(input => predicate.Invoke(input))))!;
        }

        ExceptionHelpers.ThrowIfArgumentNull(predicate);

        __Collection<TElement> result = new();
        foreach (TElement? item in this._source)
        {
            if (predicate.Invoke(item!))
            {
                continue;
            }
            result.Insert(index: result.Count,
                          item: item);
        }
        return result!;
    }

    TElement? IElementFinder<TElement, TElement>.FindLast(Func<TElement, Boolean> predicate)
    {
        if (this._source is TElement[] array)
        {
            return Array.FindLast(array: array,
                                  match: input => predicate.Invoke(input));
        }
        if (this._source is List<TElement> list)
        {
            return list.FindLast(input => predicate.Invoke(input));
        }

        ExceptionHelpers.ThrowIfArgumentNull(predicate);

        TElement? result = default;
        foreach (TElement? item in this._source)
        {
            if (predicate.Invoke(item!))
            {
                result = item;
            }
        }
        return result;
    }
}

// IEnumerable
partial struct __CollectionWrapper<TCollection, TElement> : IEnumerable
{
    IEnumerator IEnumerable.GetEnumerator() =>
        this._source
            .GetEnumerator();
}

// IEnumerable<T>
partial struct __CollectionWrapper<TCollection, TElement> : IEnumerable<TElement?>
{
    IEnumerator<TElement?> IEnumerable<TElement?>.GetEnumerator() =>
        this._source
            .GetEnumerator();
}

// IReadOnlyCollection<T>
partial struct __CollectionWrapper<TCollection, TElement> : IReadOnlyCollection<TElement?>
{
    Int32 IReadOnlyCollection<TElement?>.Count =>
        ((IReadOnlyCollection<TElement?>)this._source).Count;
}

// ISynchronized
partial struct __CollectionWrapper<TCollection, TElement> : ISynchronized
{
    Boolean ISynchronized.IsSynchronized =>
        this._source
            .IsSynchronized;

    Object ISynchronized.SyncRoot =>
        this._source
            .SyncRoot;
}