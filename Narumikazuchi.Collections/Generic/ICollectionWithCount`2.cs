namespace Narumikazuchi.Collections;

/// <summary>
/// Represents a strongly-typed collection that exposes it's element count.
/// </summary>
public interface ICollectionWithCount<out TElement, TEnumerator> : 
    IStrongEnumerable<TElement, TEnumerator>
        where TEnumerator : struct, IStrongEnumerator<TElement>
{
    /// <summary>
    /// Gets the number of elements in the collection.
    /// </summary>
    [Pure]
    [NotNull]
    public Int32 Count { get; }
}