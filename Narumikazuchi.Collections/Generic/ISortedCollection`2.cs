namespace Narumikazuchi.Collections;

/// <summary>
/// Represents a strongly-typed read-only collection of elements.
/// </summary>
public interface ISortedCollection<TElement, TEnumerator> : 
    IReadOnlyCollection<TElement, TEnumerator>
        where TEnumerator : struct, IStrongEnumerator<TElement>
{
    /// <summary>
    /// Gets the <see cref="IComparer{T}"/> which is used by the <see cref="ISortedCollection{TElement, TEnumerator}"/>.
    /// </summary>
    public IComparer<TElement> Comparer { get; }
}