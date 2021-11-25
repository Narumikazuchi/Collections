namespace Narumikazuchi.Collections.Abstract;

/// <summary>
/// Represents a strongly-typed, index-accessible collection that can be searched through.
/// </summary>
public interface ISearchableList<TIndex, TElement> : IReadOnlyList2<TIndex, TElement>, ISearchableCollection<TElement>
{
    /// <summary>
    /// Searches the sorted <see cref="ISearchableList{TIndex, TElement}"/> for the specified item in the entire <see cref="ISearchableList{TIndex, TElement}"/>.
    /// </summary>
    /// <remarks>
    /// If the <see cref="ISearchableList{TIndex, TElement}"/> is not previously sorted, this method will sort it before searching.
    /// </remarks>
    /// <param name="item">The item to search.</param>
    /// <returns>The zero-based index of the item or -1 if the list doesn't contain the item</returns>
    [Pure]
    public TIndex BinarySearch([DisallowNull] TElement item);
    /// <summary>
    /// Searches the sorted <see cref="ISearchableList{TIndex, TElement}"/> for the specified item using the specified <see cref="IComparer{T}"/>.
    /// </summary>
    /// <param name="item">The item to search.</param>
    /// <param name="comparer">The comparer to determine a match.</param>
    /// <returns>The zero-based index of the item or -1 if the list doesn't contain the item</returns>
    [Pure]
    public TIndex BinarySearch([DisallowNull] TElement item, 
                               [AllowNull] IComparer<TElement>? comparer);
    /// <summary>
    /// Searches the sorted <see cref="ISearchableList{TIndex, TElement}"/> for the specified item in the specified range using the
    /// specified <see cref="IComparer{T}"/>.
    /// </summary>
    /// <remarks>
    /// If the <see cref="ISearchableList{TIndex, TElement}"/> is not previously sorted, this method will sort it before searching.
    /// </remarks>
    /// <param name="index">The index of the first item in the range.</param>
    /// <param name="count">The length of the range.</param>
    /// <param name="item">The item to search.</param>
    /// <param name="comparer">The comparer to determine a match.</param>
    /// <returns>The zero-based index of the item or -1 if the list doesn't contain the item</returns>
    [Pure]
    public TIndex BinarySearch(in TIndex index, 
                               in Int32 count, 
                               [DisallowNull] TElement item, 
                               [AllowNull] IComparer<TElement>? comparer);

    /// <summary>
    /// Searches the entire <see cref="ISearchableList{TIndex, TElement}"/> for an item matching the specified condition and
    /// returns the index of the first occurence of a matching item.
    /// </summary>
    /// <param name="predicate">The condition to check items against.</param>
    /// <returns>The index of the first item matching the condition or -1 if no item in this list matches the condition</returns>
    [Pure]
    public TIndex FindIndex([DisallowNull] Func<TElement, Boolean> predicate);
    /// <summary>
    /// Searches the specified range in the <see cref="ISearchableList{TIndex, TElement}"/> for an item matching the specified condition and
    /// returns the index of the first occurence of a matching item.
    /// </summary>
    /// <param name="startIndex">The index of the first item in the range.</param>
    /// <param name="count">The length of the range to search.</param>
    /// <param name="predicate">The condition to check items against.</param>
    /// <returns>The index of the first item matching the condition or -1 if no item in this list matches the condition</returns>
    [Pure]
    public TIndex FindIndex(in TIndex startIndex, 
                            in Int32 count, 
                            [DisallowNull] Func<TElement, Boolean> predicate);
    /// <summary>
    /// Searches the entire <see cref="ISearchableList{TIndex, TElement}"/> for an item matching the specified condition and
    /// returns the index of the last occurence of a matching item.
    /// </summary>
    /// <param name="predicate">The condition to check items against.</param>
    /// <returns>The index of the last item matching the condition or -1 if no item in this list matches the condition</returns>
    [Pure]
    public TIndex FindLastIndex([DisallowNull] Func<TElement, Boolean> predicate);
    /// <summary>
    /// Searches the specified range in the <see cref="ISearchableList{TIndex, TElement}"/> for an item matching the specified condition and
    /// returns the index of the last occurence of a matching item.
    /// </summary>
    /// <param name="startIndex">The index of the first item in the range.</param>
    /// <param name="count">The length of the range to search.</param>
    /// <param name="predicate">The condition to check items against.</param>
    /// <returns>The index of the last item matching the condition or -1 if no item in this list matches the condition</returns>
    [Pure]
    public TIndex FindLastIndex(in TIndex startIndex, 
                                in Int32 count, 
                                [DisallowNull] Func<TElement, Boolean> predicate);

    /// <summary>
    /// Searches for the specified item and returns the index of the last occurence in the <see cref="ISearchableList{TIndex, TElement}"/>.
    /// </summary>
    /// <param name="item">The item to search for.</param>
    /// <returns>The index of the last occurence of the specified item or -1 if the item does not exist in the list</returns>
    [Pure]
    public TIndex LastIndexOf([DisallowNull] in TElement item);
}


/// <summary>
/// Represents a strongly-typed, index-accessible collection that can be searched through.
/// </summary>
public interface ISearchableList<TElement> : ISearchableList<Int32, TElement>, IReadOnlyList<TElement>
{ }