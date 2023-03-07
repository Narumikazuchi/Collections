namespace Narumikazuchi.Collections;

/// <summary>
/// Represents a strongly-typed collection that can be modified.
/// </summary>
public interface IModifyableCollection<TElement, TEnumerator>
    : IReadOnlyCollection<TElement, TEnumerator>
        where TEnumerator : struct, IStrongEnumerator<TElement>
{
    /// <summary>
    /// Adds an element to the <see cref="IModifyableCollection{TElement, TEnumerator}"/>.
    /// </summary>
    /// <param name="element">The element to add to the <see cref="IModifyableCollection{TElement, TEnumerator}"/>.</param>
    /// <returns><see langword="true"/> if the element was added to the <see cref="IModifyableCollection{TElement, TEnumerator}"/>; otherwise, <see langword="false"/>.</returns>
    public Boolean Add([DisallowNull] TElement element);

    /// <summary>
    /// Adds the elements of the specified <typeparamref name="TEnumerable"/> to the <see cref="IModifyableCollection{TElement, TEnumerator}"/>.
    /// </summary>
    /// <param name="enumerable">The collection of items to add.</param>
    /// <exception cref="ArgumentNullException"/>
    public void AddRange<TEnumerable>([DisallowNull] TEnumerable enumerable)
        where TEnumerable : IEnumerable<TElement>;

    /// <summary>
    /// Removes all elements from the <see cref="IModifyableCollection{TElement, TEnumerator}"/>.
    /// </summary>
    public void Clear();

    /// <summary>
    /// Removes the first occurrence of an element from the <see cref="IModifyableCollection{TElement, TEnumerator}"/>.
    /// </summary>
    /// <param name="element">Tehe element to remove from the <see cref="IModifyableCollection{TElement, TEnumerator}"/>.</param>
    /// <returns><see langword="true"/> if the element was removed from the <see cref="IModifyableCollection{TElement, TEnumerator}"/>; otherwise, <see langword="false"/>.</returns>
    public Boolean Remove([DisallowNull] TElement element);
}