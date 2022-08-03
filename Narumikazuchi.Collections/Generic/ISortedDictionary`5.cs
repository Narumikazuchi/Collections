namespace Narumikazuchi.Collections;

/// <summary>
/// Represents a strongly-typed read-only dictionary of elements.
/// </summary>
public interface ISortedDictionary<TKey, TValue, TEnumerator, TEqualityComparer, TComparer> :
    IReadOnlyLookup<TKey, TValue, TEnumerator, TEqualityComparer>
        where TKey : notnull
        where TEnumerator : struct, IStrongEnumerator<KeyValuePair<TKey, TValue>>
        where TComparer : IComparer<TKey>
        where TEqualityComparer : IEqualityComparer<TKey>
{
    /// <summary>
    /// Gets the <see cref="IComparer{T}"/> which is used by the <see cref="ISortedCollection{TElement, TEnumerator, TComparer}"/>.
    /// </summary>
    public TComparer Comparer { get; }
}