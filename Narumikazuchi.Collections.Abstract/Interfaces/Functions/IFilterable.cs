namespace Narumikazuchi.Collections.Abstract;

/// <summary>
/// Represents a collection that can be filtered.
/// </summary>
public interface IFilterable<out TElement> : 
    IEnumerable<TElement>
{
    /// <summary>
    /// Filters the <see cref="IFilterable{TElement}"/> through the specified predicate.
    /// </summary>
    /// <param name="predicate">The predicate to filter the collection by.</param>
    /// <returns><see langword="true"/> if the filtering succeeded, else <see langword="false"/></returns>
    public Boolean Filter([DisallowNull] Func<TElement, Boolean> predicate);
    /// <summary>
    /// Resets the filtering for the <see cref="IFilterable{T}"/>.
    /// </summary>
    public void ResetFilter();

    /// <summary>
    /// Gets if the <see cref="IFilterable{TElement}"/> is currently filtered.
    /// </summary>
    [Pure]
    public Boolean IsFiltered { get; }
}