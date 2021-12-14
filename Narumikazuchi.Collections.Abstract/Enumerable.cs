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
    public static IElementContainer AsIReadOnlyCollection<TCollection, TElement>(this TCollection source)
        where TCollection : ICollection, ICollection<TElement?>, IReadOnlyCollection<TElement?> =>
            new __CollectionWrapper<TCollection, TElement>(source: source);

    /// <summary>
    /// Represents this object as the <see cref="IElementContainer{TElement}"/> interface.
    /// </summary>
    [return: NotNull]
    public static IElementContainer<TElement> AsIReadOnlyCollection2<TCollection, TElement>(this TCollection source)
        where TCollection : ICollection, ICollection<TElement?>, IReadOnlyCollection<TElement?> =>
            new __CollectionWrapper<TCollection, TElement>(source: source);

    /// <summary>
    /// Represents this object as the <see cref="IIndexedReadOnlyCollection{TElement}"/> interface.
    /// </summary>
    [return: NotNull]
    public static IIndexedReadOnlyCollection<Int32, TElement> AsIReadOnlyList2<TList, TElement>(this TList source)
        where TList : ICollection, ICollection<TElement?>, IList, IList<TElement?>, IReadOnlyCollection<TElement?>, IReadOnlyList<TElement?> =>
            new __ListWrapper<TList, TElement>(source: source);
}