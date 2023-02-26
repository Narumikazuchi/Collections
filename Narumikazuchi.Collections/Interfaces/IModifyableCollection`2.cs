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
    public Boolean Add(
#if NETCOREAPP3_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        [DisallowNull]
#endif
        NotNull<TElement> element);

    /// <summary>
    /// Adds the elements of the specified <typeparamref name="TEnumerable"/> to the <see cref="IModifyableCollection{TElement, TEnumerator}"/>.
    /// </summary>
    /// <param name="enumerable">The collection of items to add.</param>
    /// <exception cref="ArgumentNullException"/>
    public void AddRange<TEnumerable>(
#if NETCOREAPP3_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        [DisallowNull]
#endif
        NotNull<TEnumerable> enumerable)
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
    public Boolean Remove(
#if NETCOREAPP3_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        [DisallowNull]
#endif
        NotNull<TElement> element);
}