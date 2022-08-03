namespace Narumikazuchi.Collections;

/// <summary>
/// Represents a strongly-typed collection that can be modified.
/// </summary>
public interface IModifyableCollection<TElement, TEnumerator> : 
    IReadOnlyCollection<TElement, TEnumerator>
        where TEnumerator : struct, IStrongEnumerator<TElement>
{
    /// <summary>
    /// Adds an element to the <see cref="IModifyableCollection{TElement, TEnumerator}"/>.
    /// </summary>
    /// <param name="element">The element to add to the <see cref="IModifyableCollection{TElement, TEnumerator}"/>.</param>
    /// <returns><see langword="true"/> if the element was added to the <see cref="IModifyableCollection{TElement, TEnumerator}"/>; otherwise, <see langword="false"/>.</returns>
    public Boolean Add(TElement element);

    /// <summary>
    /// Adds all elements of an enumerable to the <see cref="IModifyableCollection{TElement, TEnumerator}"/>.
    /// </summary>
    /// <param name="enumerable">The elements to add to the <see cref="IModifyableCollection{TElement, TEnumerator}"/>.</param>
    /// <exception cref="ArgumentNullException" />
    public void AddRange<TEnumerable, TOtherEnumerator>([DisallowNull] TEnumerable enumerable)
        where TOtherEnumerator : struct, IStrongEnumerator<TElement>
        where TEnumerable : IStrongEnumerable<TElement, TOtherEnumerator>;

    /// <summary>
    /// Removes all elements from the <see cref="IModifyableCollection{TElement, TEnumerator}"/>.
    /// </summary>
    public void Clear();

    /// <summary>
    /// Removes the first occurrence of an element from the <see cref="IModifyableCollection{TElement, TEnumerator}"/>.
    /// </summary>
    /// <param name="element">Tehe element to remove from the <see cref="IModifyableCollection{TElement, TEnumerator}"/>.</param>
    /// <returns><see langword="true"/> if the element was removed from the <see cref="IModifyableCollection{TElement, TEnumerator}"/>; otherwise, <see langword="false"/>.</returns>
    public Boolean Remove(TElement element);
}