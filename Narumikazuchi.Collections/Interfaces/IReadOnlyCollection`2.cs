namespace Narumikazuchi.Collections;

/// <summary>
/// Represents a strongly-typed read-only collection of elements.
/// </summary>
public interface IReadOnlyCollection<TElement, TEnumerator> : 
    ICollectionWithCount<TElement, TEnumerator>
        where TEnumerator : struct, IStrongEnumerator<TElement>
{
    /// <summary>
    /// Determines whether the <see cref="IReadOnlyCollection{TElement, TEnumerator}"/> contains the specifiec element.
    /// </summary>
    /// <param name="element">The element to locate in the <see cref="IReadOnlyCollection{TElement, TEnumerator}"/>.</param>
    /// <returns><see langword="true"/> if the element is found in the <see cref="IReadOnlyCollection{TElement, TEnumerator}"/>; otherwise, <see langword="false"/>.</returns>
    public Boolean Contains(TElement element);

    /// <summary>
    /// Copies the entire <see cref="IReadOnlyCollection{TElement, TEnumerator}"/> to a compatible one-dimensional array, starting at the beginning of the target array.
    /// </summary>
    /// <param name="array">The one-dimensional <see cref="Array"/> that is the destination of the elements copied from <see cref="IReadOnlyCollection{TElement, TEnumerator}"/>.
    /// The <see cref="Array"/> must have zero-based indexing.</param>
    /// <exception cref="ArgumentException" />
    /// <exception cref="ArgumentNullException" />
    public void CopyTo(
#if NETCOREAPP3_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        [DisallowNull]
#endif
        TElement[] array);
    /// <summary>
    /// Copies the entire <see cref="IReadOnlyCollection{TElement, TEnumerator}"/> to a compatible one-dimensional array, starting at the specified index of the target array.
    /// </summary>
    /// <param name="array">The one-dimensional <see cref="Array"/> that is the destination of the elements copied from <see cref="IReadOnlyCollection{TElement, TEnumerator}"/>.
    /// The <see cref="Array"/> must have zero-based indexing.</param>
    /// <param name="destinationIndex">The zero-based index in array at which copying begins.</param>
    /// <exception cref="ArgumentException" />
    /// <exception cref="ArgumentNullException" />
    /// <exception cref="ArgumentOutOfRangeException" />
    public void CopyTo(
#if NETCOREAPP3_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        [DisallowNull]
#endif
        TElement[] array,
        Int32 destinationIndex);
}