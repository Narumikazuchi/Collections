namespace Narumikazuchi.Collections.Abstract;

/// <summary>
/// Represents a strongly-typed collection that can be searched through.
/// </summary>
public interface IElementFinder<out TPattern, TResult>
{
    /// <summary>
    /// Determines if the <see cref="IElementFinder{TPattern, TResult}"/> contains item matching the specified condition.
    /// </summary>
    /// <param name="predicate">The condition to check items against.</param>
    /// <returns><see langword="true"/> if at least one item matching the condition exists in this list; else <see langword="false"/></returns>
    [Pure]
    public Boolean Exists([DisallowNull] Func<TPattern, Boolean> predicate);

    /// <summary>
    /// Searches the <see cref="IElementFinder{TPattern, TResult}"/> for an item matching the specified condition and
    /// returns the first occurence of a matching item.
    /// </summary>
    /// <param name="predicate">The condition to check items against.</param>
    /// <returns>The first item matching the condition or <see langword="default"/> if no item in this list matches the condition</returns>
    [Pure]
    [return: MaybeNull]
    public TResult? Find([DisallowNull] Func<TPattern, Boolean> predicate);

    /// <summary>
    /// Retrieves all items in the <see cref="IElementFinder{TPattern, TResult}"/> that match the specified condition.
    /// </summary>
    /// <param name="predicate">The condition to check items against.</param>
    /// <returns>An <see cref="IIndexedReadOnlyCollection{TElement}"/> containing all items matching the condition</returns>
    [Pure]
    [return: NotNull]
    public IElementContainer<TResult> FindAll([DisallowNull] Func<TPattern, Boolean> predicate);

    /// <summary>
    /// Retrieves all items in the <see cref="IElementFinder{TPattern, TResult}"/> that do not match the specified condition.
    /// </summary>
    /// <param name="predicate">The condition to check items against.</param>
    /// <returns>An <see cref="IIndexedReadOnlyCollection{TElement}"/> containing all items not matching the condition</returns>
    [Pure]
    [return: NotNull]
    public IElementContainer<TResult> FindExcept([DisallowNull] Func<TPattern, Boolean> predicate);

    /// <summary>
    /// Searches the <see cref="IElementFinder{TPattern, TResult}"/> for an item matching the specified condition and
    /// returns the last occurence of a matching item.
    /// </summary>
    /// <param name="predicate">The condition to check items against.</param>
    /// <returns>The last item matching the condition or <see langword="default"/> if no item in this list matches the condition</returns>
    [Pure]
    [return: MaybeNull]
    public TResult? FindLast([DisallowNull] Func<TPattern, Boolean> predicate);
}