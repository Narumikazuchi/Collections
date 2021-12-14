namespace Narumikazuchi.Collections.Abstract;

/// <summary>
/// Represents a strongly-typed, index-accessible collection that can be searched through.
/// </summary>
public interface IIndexFinder<TIndex, TElement>
    where TIndex : IComparable<TIndex>
{
    /// <summary>
    /// Searches the entire <see cref="IIndexFinder{TIndex, TElement}"/> for an item matching the specified condition and
    /// returns the index of the first occurence of a matching item.
    /// </summary>
    /// <param name="predicate">The condition to check items against.</param>
    /// <returns>The index of the first item matching the condition or -1 if no item in this list matches the condition</returns>
    [Pure]
    [return: NotNull]
    public TIndex FindIndex([DisallowNull] Func<TElement, Boolean> predicate);
    /// <summary>
    /// Searches the specified range in the <see cref="IIndexFinder{TIndex, TElement}"/> for an item matching the specified condition and
    /// returns the index of the first occurence of a matching item.
    /// </summary>
    /// <param name="startIndex">The index of the first item in the range.</param>
    /// <param name="endIndex">The index of the last item in the range.</param>
    /// <param name="predicate">The condition to check items against.</param>
    /// <returns>The index of the first item matching the condition or -1 if no item in this list matches the condition</returns>
    [Pure]
    [return: NotNull]
    public TIndex FindIndex([DisallowNull] in TIndex startIndex,
                            [DisallowNull] in TIndex endIndex,
                            [DisallowNull] Func<TElement, Boolean> predicate);
    /// <summary>
    /// Searches the entire <see cref="IIndexFinder{TIndex, TElement}"/> for an item matching the specified condition and
    /// returns the index of the last occurence of a matching item.
    /// </summary>
    /// <param name="predicate">The condition to check items against.</param>
    /// <returns>The index of the last item matching the condition or -1 if no item in this list matches the condition</returns>
    [Pure]
    [return: NotNull]
    public TIndex FindLastIndex([DisallowNull] Func<TElement, Boolean> predicate);
    /// <summary>
    /// Searches the specified range in the <see cref="IIndexFinder{TIndex, TElement}"/> for an item matching the specified condition and
    /// returns the index of the last occurence of a matching item.
    /// </summary>
    /// <param name="startIndex">The index of the first item in the range.</param>
    /// <param name="endIndex">The index of the last item in the range.</param>
    /// <param name="predicate">The condition to check items against.</param>
    /// <returns>The index of the last item matching the condition or -1 if no item in this list matches the condition</returns>
    [Pure]
    [return: NotNull]
    public TIndex FindLastIndex([DisallowNull] in TIndex startIndex,
                                [DisallowNull] in TIndex endIndex,
                                [DisallowNull] Func<TElement, Boolean> predicate);
}