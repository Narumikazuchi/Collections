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

// ICollection
partial struct __ListWrapper<TList, TElement> : ICollection
{
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
}

// IContentAddable<T>
partial struct __ListWrapper<TList, TElement> : IContentAddable<TElement>
{
    Boolean IContentAddable<TElement>.Add(TElement item)
    {
        this._source.Add(item: item);
        return true;
    }

    void IContentAddable<TElement>.AddRange(IEnumerable<TElement> collection)
    {
        foreach (TElement item in collection)
        {
            this._source.Add(item: item);
        }
    }
}

// IContentClearable
partial struct __ListWrapper<TList, TElement> : IContentClearable
{
    void IContentClearable.Clear() =>
        ((IList)this._source).Clear();
}

// IContentConvertable<T>
partial struct __ListWrapper<TList, TElement> : IContentConvertable<TElement>
{
    ICollection<TOutput> IContentConvertable<TElement>.ConvertAll<TOutput>(Converter<TElement, TOutput> converter)
    {
        if (this._source is List<TElement> list)
        {
            return list.ConvertAll(converter: converter);
        }
        if (this._source is TElement[] array)
        {
            return Array.ConvertAll(array: array,
                                    converter: converter);
        }
        if (this._source is ImmutableList<TElement> immutable)
        {
            return immutable.ConvertAll(converter: input => converter.Invoke(input));
        }

        ExceptionHelpers.ThrowIfArgumentNull(converter);

        Collection<TOutput> result = new();
        foreach (TElement? item in this._source)
        {
            result.Add(item: converter.Invoke(input: item!));
        }
        return result;
    }
}

// IContentCopyable<T, U>
partial struct __ListWrapper<TList, TElement> : IContentCopyable<Int32, TElement[]>
{
    void IContentCopyable<Int32, TElement[]>.CopyTo(TElement[] array,
                                                    in Int32 index) =>
        this._source.CopyTo(array: array,
                            index: index);
}

// IContentForEach<T>
partial struct __ListWrapper<TList, TElement> : IContentForEach<TElement>
{
    void IContentForEach<TElement>.ForEach(Action<TElement> action)
    {
        if (this._source is List<TElement> list)
        {
            list.ForEach(action: action);
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
            immutable.ForEach(action: action);
            return;
        }

        ExceptionHelpers.ThrowIfArgumentNull(action);
        foreach (TElement? item in this._source)
        {
            action.Invoke(obj: item!);
        }
    }
}

// IContentIndexRemovable<T>
partial struct __ListWrapper<TList, TElement> : IContentIndexRemovable<Int32>
{
    void IContentIndexRemovable<Int32>.RemoveAt(in Int32 index) =>
        ((IList)this._source).RemoveAt(index: index);
}

// IContentInsertable<T>
partial struct __ListWrapper<TList, TElement> : IContentInsertable<Int32>
{
    void IContentInsertable<Int32>.Insert(in Int32 index, 
                                          Object item) =>
        this._source.Insert(index: index,
                            value: item);
}

// IContentInsertable<T, U>
partial struct __ListWrapper<TList, TElement> : IContentInsertable<Int32, TElement>
{
    void IContentInsertable<Int32, TElement>.Insert(in Int32 index, 
                                                    TElement item) =>
        this._source.Insert(index: index,
                            item: item);

    void IContentInsertable<Int32, TElement>.InsertRange(in Int32 index, 
                                                         IEnumerable<TElement?> collection)
    {
        if (this._source is List<TElement> list)
        {
            list.InsertRange(index: index,
                             collection: collection!);
            return;
        }

        ExceptionHelpers.ThrowIfArgumentNull(collection);

        Int32 i = index;
        foreach (TElement? item in collection)
        {
            this._source.Insert(index: i++,
                                item: item);
        }
    }
}

// IContentRemovable
partial struct __ListWrapper<TList, TElement> : IContentRemovable
{
    Boolean IContentRemovable.Remove(Object item) =>
        item is TElement element &&
        this._source.Remove(element);
}

// IContentRemovable<T>
partial struct __ListWrapper<TList, TElement> : IContentRemovable<TElement>
{
    Boolean IContentRemovable<TElement>.Remove(TElement item) =>
        this._source.Remove(item);

    Int32 IContentRemovable<TElement>.RemoveAll(Func<TElement, Boolean> predicate)
    {
        if (this._source is List<TElement> list)
        {
            return list.RemoveAll(match: input => predicate.Invoke(input));
        }

        ExceptionHelpers.ThrowIfArgumentNull(predicate);

        Collection<TElement> remove = new();
        foreach (TElement? item in this._source)
        {
            if (predicate.Invoke(arg: item!))
            {
                remove.Add(item: item!);
            }
        }

        Int32 skipped = 0;
        foreach (TElement item in remove)
        {
            if (!this._source.Remove(item))
            {
                skipped++;
            }
        }
        return remove.Count - skipped;
    }
}

// IContentSegmentable<T, U>
partial struct __ListWrapper<TList, TElement> : IContentSegmentable<Int32, TElement>
{
    ICollection<TElement> IContentSegmentable<Int32, TElement>.GetRange(in Int32 startIndex, 
                                                                        in Int32 endIndex)
    {
        if (this._source is List<TElement> list)
        {
            return list.GetRange(index: startIndex,
                                 count: endIndex - startIndex);
        }

        Collection<TElement> range = new();
        for (Int32 i = startIndex; i < endIndex; i++)
        {
            range.Add(item: ((IReadOnlyList<TElement>)this._source)[i]);
        }
        return range;
    }
}

// IConvertToArray<T>
partial struct __ListWrapper<TList, TElement> : IConvertToArray<TElement[]>
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
        this._source.CopyTo(array: result,
                            arrayIndex: 0);
        return result;
    }
}

// IElementContainer
partial struct __ListWrapper<TList, TElement> : IElementContainer
{
    Boolean IElementContainer.Contains(Object? item) =>
        this._source.Contains(item);
}

// IElementContainer<T>
partial struct __ListWrapper<TList, TElement> : IElementContainer<TElement?>
{
    Boolean IElementContainer<TElement?>.Contains(TElement? item) =>
        this._source.Contains(item: item);
}

// IElementFinder<T, U>
partial struct __ListWrapper<TList, TElement> : IElementFinder<TElement, TElement>
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
            return list.Exists(match: input => predicate.Invoke(input));
        }

        ExceptionHelpers.ThrowIfArgumentNull(predicate);

        foreach (TElement? item in this._source)
        {
            if (predicate.Invoke(arg: item!))
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
            return list.Find(match: input => predicate.Invoke(input));
        }

        ExceptionHelpers.ThrowIfArgumentNull(predicate);

        foreach (TElement? item in this._source)
        {
            if (predicate.Invoke(arg: item!))
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
            return new __Collection<TElement>(Array.FindAll(array: array,
                                                            match: input => predicate.Invoke(input)))!;
        }
        if (this._source is List<TElement> list)
        {
            return new __Collection<TElement>(list.FindAll(match: input => predicate.Invoke(input)))!;
        }

        ExceptionHelpers.ThrowIfArgumentNull(predicate);

        __Collection<TElement> result = new();
        foreach (TElement? item in this._source)
        {
            if (predicate.Invoke(arg: item!))
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
            return new __Collection<TElement>(array.Except(Array.FindAll(array: array,
                                                                         match: input => predicate.Invoke(input))))!;
        }
        if (this._source is List<TElement> list)
        {
            return new __Collection<TElement>(list.Except(list.FindAll(match: input => predicate.Invoke(input))))!;
        }

        ExceptionHelpers.ThrowIfArgumentNull(predicate);

        __Collection<TElement> result = new();
        foreach (TElement? item in this._source)
        {
            if (predicate.Invoke(arg: item!))
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
            return list.FindLast(match: input => predicate.Invoke(input));
        }

        ExceptionHelpers.ThrowIfArgumentNull(predicate);

        TElement? result = default;
        foreach (TElement? item in this._source)
        {
            if (predicate.Invoke(arg: item!))
            {
                result = item;
            }
        }
        return result;
    }
}

// IEnumerable
partial struct __ListWrapper<TList, TElement> : IEnumerable
{
    IEnumerator IEnumerable.GetEnumerator() =>
        this._source.GetEnumerator();
}

// IEnumerable<T>
partial struct __ListWrapper<TList, TElement> : IEnumerable<TElement?>
{
    IEnumerator<TElement?> IEnumerable<TElement?>.GetEnumerator() =>
        this._source.GetEnumerator();
}

// IIndexedReadOnlyCollection<T>
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

// IIndexedReadOnlyCollection<T, U>
partial struct __ListWrapper<TList, TElement> : IIndexedReadOnlyCollection<Int32, TElement?>
{
    Int32 IIndexedReadOnlyCollection<Int32, TElement?>.IndexOf(TElement? item) =>
        this._source.IndexOf(item: item);
    Int32 IIndexedReadOnlyCollection<Int32, TElement?>.LastIndexOf(TElement? item) =>
        ((IIndexedReadOnlyCollection<Int32>)this).LastIndexOf(item: item);

    TElement? IIndexedReadOnlyCollection<Int32, TElement?>.this[Int32 index] =>
        ((IIndexedReadOnlyCollection<Int32, TElement?>)this._source)[index];
}

// IIndexFinder<T, U>
partial struct __ListWrapper<TList, TElement> : IIndexFinder<Int32, TElement>
{
    Int32 IIndexFinder<Int32, TElement>.FindIndex(Func<TElement, Boolean> predicate)
    {
        if (this._source is TElement[] array)
        {
            return Array.FindIndex(array: array,
                                   match: input => predicate.Invoke(input));
        }
        if (this._source is List<TElement> list)
        {
            return list.FindIndex(match: input => predicate.Invoke(input));
        }
        if (this._source is ImmutableList<TElement> immutable)
        {
            return immutable.FindIndex(match: input => predicate.Invoke(input));
        }

        for (Int32 i = 0; i < ((ICollection)this._source).Count; i++)
        {
            if (predicate.Invoke(arg: ((IReadOnlyList<TElement>)this._source)[i]))
            {
                return i;
            }
        }
        return -1;
    }
    Int32 IIndexFinder<Int32, TElement>.FindIndex(in Int32 startIndex, 
                                                  in Int32 endIndex, 
                                                  Func<TElement, Boolean> predicate)
    {
        if (this._source is TElement[] array)
        {
            return Array.FindIndex(array: array,
                                   startIndex: startIndex,
                                   count: endIndex - startIndex,
                                   match: input => predicate.Invoke(input));
        }
        if (this._source is List<TElement> list)
        {
            return list.FindIndex(startIndex: startIndex,
                                  count: endIndex - startIndex, 
                                  match: input => predicate.Invoke(input));
        }
        if (this._source is ImmutableList<TElement> immutable)
        {
            return immutable.FindIndex(startIndex: startIndex,
                                       count: endIndex - startIndex,
                                       match: input => predicate.Invoke(input));
        }

        for (Int32 i = startIndex; i < endIndex; i++)
        {
            if (predicate.Invoke(arg: ((IReadOnlyList<TElement>)this._source)[i]))
            {
                return i;
            }
        }
        return -1;
    }

    Int32 IIndexFinder<Int32, TElement>.FindLastIndex(Func<TElement, Boolean> predicate)
    {
        if (this._source is TElement[] array)
        {
            return Array.FindLastIndex(array: array,
                                       match: input => predicate.Invoke(input));
        }
        if (this._source is List<TElement> list)
        {
            return list.FindLastIndex(match: input => predicate.Invoke(input));
        }
        if (this._source is ImmutableList<TElement> immutable)
        {
            return immutable.FindLastIndex(match: input => predicate.Invoke(input));
        }

        for (Int32 i = ((ICollection)this._source).Count - 1; i > 0; i--)
        {
            if (predicate.Invoke(arg: ((IReadOnlyList<TElement>)this._source)[i]))
            {
                return i;
            }
        }
        return -1;
    }
    Int32 IIndexFinder<Int32, TElement>.FindLastIndex(in Int32 startIndex, 
                                                      in Int32 endIndex, 
                                                      Func<TElement, Boolean> predicate)
    {
        if (this._source is TElement[] array)
        {
            return Array.FindLastIndex(array: array,
                                       startIndex: startIndex,
                                       count: endIndex - startIndex,
                                       match: input => predicate.Invoke(input));
        }
        if (this._source is List<TElement> list)
        {
            return list.FindLastIndex(startIndex: startIndex,
                                      count: endIndex - startIndex,
                                      match: input => predicate.Invoke(input));
        }
        if (this._source is ImmutableList<TElement> immutable)
        {
            return immutable.FindLastIndex(startIndex: startIndex,
                                           count: endIndex - startIndex,
                                           match: input => predicate.Invoke(input));
        }

        for (Int32 i = endIndex; i > startIndex; i--)
        {
            if (predicate.Invoke(arg: ((IReadOnlyList<TElement>)this._source)[i]))
            {
                return i;
            }
        }
        return -1;
    }
}

// IReadOnlyCollection<T>
partial struct __ListWrapper<TList, TElement> : IReadOnlyCollection<TElement?>
{
    Int32 IReadOnlyCollection<TElement?>.Count =>
        ((IReadOnlyCollection<TElement?>)this._source).Count;
}

// IReadOnlyList<T>
partial struct __ListWrapper<TList, TElement> : IReadOnlyList<TElement?>
{
    TElement? IReadOnlyList<TElement?>.this[Int32 index] => 
        ((IReadOnlyList<TElement?>)this._source)[index];
}

// ISynchronized
partial struct __ListWrapper<TList, TElement> : ISynchronized
{
    Boolean ISynchronized.IsSynchronized =>
        this._source.IsSynchronized;

    Object ISynchronized.SyncRoot =>
        this._source.SyncRoot;
}