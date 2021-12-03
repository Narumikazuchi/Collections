namespace Narumikazuchi.Collections.Abstract;

/// <summary>
/// Represents a read-only collection of elements that can be accessed by index.
/// </summary>
public interface IIndexedReadOnlyCollection<TIndex> : 
    IElementContainer
        where TIndex : IComparable<TIndex>
{
    /// <summary>
    /// Determines the index of the first occurrence of a specific item in the <see cref="IIndexedReadOnlyCollection{TIndex}"/>.
    /// </summary>
    /// <param name="item">The object to locate in the <see cref="IIndexedReadOnlyCollection{TIndex}"/>.</param>
    /// <returns>The index of <paramref name="item"/> if found in the <see cref="IIndexedReadOnlyCollection{TIndex}"/></returns>
    [Pure]
    [return: NotNull]
    public TIndex IndexOf([AllowNull] Object? item);

    /// <summary>
    /// Determines the index of the last occurrence of a specific item in the <see cref="IIndexedReadOnlyCollection{TIndex}"/>.
    /// </summary>
    /// <param name="item">The object to locate in the <see cref="IIndexedReadOnlyCollection{TIndex}"/>.</param>
    /// <returns>The last index of <paramref name="item"/> if found in the <see cref="IIndexedReadOnlyCollection{TIndex}"/></returns>
    [Pure]
    [return: NotNull]
    public TIndex LastIndexOf([AllowNull] Object? item);

    /// <summary>
    /// Gets the element at the specified index.
    /// </summary>
    /// <param name="index">The index of the lement to get.</param>
    /// <returns>The element at the specified index</returns>
    [MaybeNull]
    public Object? this[[DisallowNull] TIndex index] { get; }
}

/// <summary>
/// Represents a read-only collection of elements that can be accessed by index.
/// </summary>
public interface IIndexedReadOnlyCollection<TIndex, TElement> :
    IElementContainer<TElement>,
    IIndexedReadOnlyCollection<TIndex>
        where TIndex : IComparable<TIndex>
{
    /// <summary>
    /// Determines the index of the first occurrence a specific tem in the <see cref="IIndexedReadOnlyCollection{TIndex, TElement}"/>.
    /// </summary>
    /// <param name="item">The object to locate in the <see cref="IIndexedReadOnlyCollection{TIndex, TElement}"/>.</param>
    /// <returns>The index of <paramref name="item"/> if found in the <see cref="IIndexedReadOnlyCollection{TIndex, TElement}"/></returns>
    [Pure]
    [return: NotNull]
    public TIndex IndexOf(TElement item);

    /// <summary>
    /// Determines the index of the last occurrence of a specific item in the <see cref="IIndexedReadOnlyCollection{TIndex}"/>.
    /// </summary>
    /// <param name="item">The object to locate in the <see cref="IIndexedReadOnlyCollection{TIndex}"/>.</param>
    /// <returns>The last index of <paramref name="item"/> if found in the <see cref="IIndexedReadOnlyCollection{TIndex}"/></returns>
    [Pure]
    [return: NotNull]
    public TIndex LastIndexOf(TElement item);

    /// <summary>
    /// Gets the element at the specified index.
    /// </summary>
    /// <param name="index">The index of the lement to get.</param>
    /// <returns>The element at the specified index</returns>
    public new TElement this[[DisallowNull] TIndex index] { get; }
}