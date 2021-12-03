namespace Narumikazuchi.Collections.Abstract;

internal interface IBinarySearchable<TIndex, TElement> :
    IBackingContainer<TElement[]>
        where TIndex : IBinaryInteger<TIndex>, ISignedNumber<TIndex>
{
    /// <summary>
    /// Searches the sorted <see cref="IBinarySearchable{TIndex, TElement}"/> for the specified item in the entire <see cref="IBinarySearchable{TIndex, TElement}"/>.
    /// </summary>
    /// <remarks>
    /// If the <see cref="IBinarySearchable{TIndex, TElement}"/> is not previously sorted, this method will sort it before searching.
    /// </remarks>
    /// <param name="item">The item to search.</param>
    /// <returns>The zero-based index of the item or -1 if the list doesn't contain the item</returns>
    [Pure]
    [return: NotNull]
    public TIndex BinarySearch(TElement item);
    /// <summary>
    /// Searches the sorted <see cref="IBinarySearchable{TIndex, TElement}"/> for the specified item using the specified <see cref="IComparer{T}"/>.
    /// </summary>
    /// <param name="item">The item to search.</param>
    /// <param name="comparer">The comparer to determine a match.</param>
    /// <returns>The zero-based index of the item or -1 if the list doesn't contain the item</returns>
    [Pure]
    [return: NotNull]
    public TIndex BinarySearch(TElement item,
                               [AllowNull] IComparer<TElement>? comparer);
    /// <summary>
    /// Searches the sorted <see cref="IIndexFinder{TIndex, TElement}"/> for the specified item in the specified range using the
    /// specified <see cref="IComparer{T}"/>.
    /// </summary>
    /// <remarks>
    /// If the <see cref="IIndexFinder{TIndex, TElement}"/> is not previously sorted, this method will sort it before searching.
    /// </remarks>
    /// <param name="startIndex">The index of the first item in the range.</param>
    /// <param name="endIndex">The index of the last item in the range.</param>
    /// <param name="item">The item to search.</param>
    /// <param name="comparer">The comparer to determine a match.</param>
    /// <returns>The zero-based index of the item or -1 if the list doesn't contain the item</returns>
    [Pure]
    [return: NotNull]
    public TIndex BinarySearch([DisallowNull] in TIndex startIndex,
                               [DisallowNull] in TIndex endIndex,
                               TElement item,
                               [AllowNull] IComparer<TElement>? comparer);
}