namespace Narumikazuchi.Collections.Abstract;

/// <summary>
/// Extends the <see cref="IEnumerable{T}"/> interface.
/// </summary>
public static class Enumerable
{
    /// <summary>
    /// Represents this object as the <see cref="IElementContainer{TElement}"/> interface.
    /// </summary>
    [return: NotNull]
    public static IContentAddable<TElement> AsContentAddable<TCollection, TElement>(this TCollection source)
        where TCollection : ICollection, ICollection<TElement?>, IReadOnlyCollection<TElement?>
    {
        if (source is IContentAddable<TElement> original)
        {
            return original;
        }
        return new __CollectionWrapper<TCollection, TElement>(source: source);
    }

    /// <summary>
    /// Represents this object as the <see cref="IElementContainer{TElement}"/> interface.
    /// </summary>
    [return: NotNull]
    public static IContentClearable AsContentClearable<TCollection, TElement>(this TCollection source)
        where TCollection : ICollection, ICollection<TElement?>, IReadOnlyCollection<TElement?>
    {
        if (source is IContentClearable original)
        {
            return original;
        }
        return new __CollectionWrapper<TCollection, TElement>(source: source);
    }

    /// <summary>
    /// Represents this object as the <see cref="IElementContainer{TElement}"/> interface.
    /// </summary>
    [return: NotNull]
    public static IContentConvertable<TElement> AsContentConvertable<TCollection, TElement>(this TCollection source)
        where TCollection : ICollection, ICollection<TElement?>, IReadOnlyCollection<TElement?>
    {
        if (source is IContentConvertable<TElement> original)
        {
            return original;
        }
        return new __CollectionWrapper<TCollection, TElement>(source: source);
    }

    /// <summary>
    /// Represents this object as the <see cref="IElementContainer{TElement}"/> interface.
    /// </summary>
    [return: NotNull]
    public static IContentCopyable<Int32, TElement[]> AsContentCopyable<TCollection, TElement>(this TCollection source)
        where TCollection : ICollection, ICollection<TElement?>, IReadOnlyCollection<TElement?>
    {
        if (source is IContentCopyable<Int32, TElement[]> original)
        {
            return original;
        }
        return new __CollectionWrapper<TCollection, TElement>(source: source);
    }

    /// <summary>
    /// Represents this object as the <see cref="IElementContainer{TElement}"/> interface.
    /// </summary>
    [return: NotNull]
    public static IContentForEach<TElement> AsContentForEach<TCollection, TElement>(this TCollection source)
        where TCollection : ICollection, ICollection<TElement?>, IReadOnlyCollection<TElement?>
    {
        if (source is IContentForEach<TElement> original)
        {
            return original;
        }
        return new __CollectionWrapper<TCollection, TElement>(source: source);
    }

    /// <summary>
    /// Represents this object as the <see cref="IElementContainer{TElement}"/> interface.
    /// </summary>
    [return: NotNull]
    public static IContentRemovable AsContentRemovable<TCollection, TElement>(this TCollection source)
        where TCollection : ICollection, ICollection<TElement?>, IReadOnlyCollection<TElement?>
    {
        if (source is IContentRemovable original)
        {
            return original;
        }
        return new __CollectionWrapper<TCollection, TElement>(source: source);
    }

    /// <summary>
    /// Represents this object as the <see cref="IElementContainer{TElement}"/> interface.
    /// </summary>
    [return: NotNull]
    public static IConvertToArray<TElement[]> AsConvertToArray<TCollection, TElement>(this TCollection source)
        where TCollection : ICollection, ICollection<TElement?>, IReadOnlyCollection<TElement?>
    {
        if (source is IConvertToArray<TElement[]> original)
        {
            return original;
        }
        return new __CollectionWrapper<TCollection, TElement>(source: source);
    }

    /// <summary>
    /// Represents this object as the <see cref="IElementContainer{TElement}"/> interface.
    /// </summary>
    [return: NotNull]
    public static IElementContainer AsElementContainer<TCollection, TElement>(this TCollection source)
        where TCollection : ICollection, ICollection<TElement?>, IReadOnlyCollection<TElement?>
    {
        if (source is IElementContainer original)
        {
            return original;
        }
        return new __CollectionWrapper<TCollection, TElement>(source: source);
    }

    /// <summary>
    /// Represents this object as the <see cref="IElementContainer{TElement}"/> interface.
    /// </summary>
    [return: NotNull]
    public static IElementContainer<TElement> AsGenericElementContainer<TCollection, TElement>(this TCollection source)
        where TCollection : ICollection, ICollection<TElement?>, IReadOnlyCollection<TElement?>
    {
        if (source is IElementContainer<TElement> original)
        {
            return original;
        }
        return new __CollectionWrapper<TCollection, TElement>(source: source);
    }

    /// <summary>
    /// Represents this object as the <see cref="IElementContainer{TElement}"/> interface.
    /// </summary>
    [return: NotNull]
    public static IElementFinder<TElement, TElement> AsElementFinder<TCollection, TElement>(this TCollection source)
        where TCollection : ICollection, ICollection<TElement?>, IReadOnlyCollection<TElement?>
    {
        if (source is IElementFinder<TElement, TElement> original)
        {
            return original;
        }
        return new __CollectionWrapper<TCollection, TElement>(source: source);
    }

    /// <summary>
    /// Represents this object as the <see cref="IElementContainer{TElement}"/> interface.
    /// </summary>
    [return: NotNull]
    public static IContentIndexRemovable<Int32> AsContentIndexRemovable<TList, TElement>(this TList source)
        where TList : ICollection, ICollection<TElement?>, IList, IList<TElement?>, IReadOnlyCollection<TElement?>, IReadOnlyList<TElement?>
    {
        if (source is IContentIndexRemovable<Int32> original)
        {
            return original;
        }
        return new __ListWrapper<TList, TElement>(source: source);
    }

    /// <summary>
    /// Represents this object as the <see cref="IElementContainer{TElement}"/> interface.
    /// </summary>
    [return: NotNull]
    public static IContentInsertable<Int32> AsContentInsertable<TList, TElement>(this TList source)
        where TList : ICollection, ICollection<TElement?>, IList, IList<TElement?>, IReadOnlyCollection<TElement?>, IReadOnlyList<TElement?>
    {
        if (source is IContentInsertable<Int32> original)
        {
            return original;
        }
        return new __ListWrapper<TList, TElement>(source: source);
    }

    /// <summary>
    /// Represents this object as the <see cref="IElementContainer{TElement}"/> interface.
    /// </summary>
    [return: NotNull]
    public static IContentInsertable<Int32, TElement> AsGenericContentInsertable<TList, TElement>(this TList source)
        where TList : ICollection, ICollection<TElement?>, IList, IList<TElement?>, IReadOnlyCollection<TElement?>, IReadOnlyList<TElement?>
    {
        if (source is IContentInsertable<Int32, TElement> original)
        {
            return original;
        }
        return new __ListWrapper<TList, TElement>(source: source);
    }

    /// <summary>
    /// Represents this object as the <see cref="IElementContainer{TElement}"/> interface.
    /// </summary>
    [return: NotNull]
    public static IContentSegmentable<Int32, TElement> AsContentSegmentable<TList, TElement>(this TList source)
        where TList : ICollection, ICollection<TElement?>, IList, IList<TElement?>, IReadOnlyCollection<TElement?>, IReadOnlyList<TElement?>
    {
        if (source is IContentSegmentable<Int32, TElement> original)
        {
            return original;
        }
        return new __ListWrapper<TList, TElement>(source: source);
    }

    /// <summary>
    /// Represents this object as the <see cref="IIndexedReadOnlyCollection{TElement}"/> interface.
    /// </summary>
    [return: NotNull]
    public static IIndexedReadOnlyCollection<Int32> AsIndexedReadOnlyCollection<TList, TElement>(this TList source)
        where TList : ICollection, ICollection<TElement?>, IList, IList<TElement?>, IReadOnlyCollection<TElement?>, IReadOnlyList<TElement?>
    {
        if (source is IIndexedReadOnlyCollection<Int32> original)
        {
            return original;
        }
        return new __ListWrapper<TList, TElement>(source: source);
    }

    /// <summary>
    /// Represents this object as the <see cref="IIndexedReadOnlyCollection{TElement}"/> interface.
    /// </summary>
    [return: NotNull]
    public static IIndexedReadOnlyCollection<Int32, TElement> AsGenericIndexedReadOnlyCollection<TList, TElement>(this TList source)
        where TList : ICollection, ICollection<TElement?>, IList, IList<TElement?>, IReadOnlyCollection<TElement?>, IReadOnlyList<TElement?>
    {
        if (source is IIndexedReadOnlyCollection<Int32, TElement> original)
        {
            return original;
        }
        return new __ListWrapper<TList, TElement>(source: source);
    }

    /// <summary>
    /// Represents this object as the <see cref="IElementContainer{TElement}"/> interface.
    /// </summary>
    [return: NotNull]
    public static IIndexFinder<Int32, TElement> AsIndexFinder<TList, TElement>(this TList source)
        where TList : ICollection, ICollection<TElement?>, IList, IList<TElement?>, IReadOnlyCollection<TElement?>, IReadOnlyList<TElement?>
    {
        if (source is IIndexFinder<Int32, TElement> original)
        {
            return original;
        }
        return new __ListWrapper<TList, TElement>(source: source);
    }
}