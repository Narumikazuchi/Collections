namespace Narumikazuchi.Collections;

/// <summary>
/// Represents a strongly-typed read-only collection of elements.
/// </summary>
public interface ISortedCollection<TElement, TEnumerator, TComparer> : 
    IReadOnlyCollection<TElement, TEnumerator>
        where TEnumerator : struct, IStrongEnumerator<TElement>
        where TComparer : IComparer<TElement>
{
    /// <summary>
    /// Gets the <see cref="IComparer{T}"/> which is used by the <see cref="ISortedCollection{TElement, TEnumerator, TComparer}"/>.
    /// </summary>
    public TComparer Comparer { get; }
}