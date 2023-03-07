namespace Narumikazuchi.Collections;

/// <summary>
/// Represents a strongly-typed collection that can be modified and accessed by index.
/// </summary>
public interface IModifyableCollectionWithIndex<TElement, TEnumerator>
    : IReadOnlyCollection<TElement, TEnumerator>,
      ICollectionWithReadWriteIndexer<TElement, TEnumerator>
        where TEnumerator : struct, IStrongEnumerator<TElement>
{
    /// <summary>
    /// Inserts the specified element at the specified index in the <see cref="IModifyableCollectionWithIndex{TElement, TEnumerator}"/>.
    /// </summary>
    /// <param name="index">The zer-based index of the location where to insert the element.</param>
    /// <param name="element">The element to insert.</param>
    /// <exception cref="ArgumentOutOfRangeException"/>
    public void Insert(Int32 index,
                       [DisallowNull] TElement element);

    /// <summary>
    /// Removes the element at the specified index.
    /// </summary>
    /// <param name="index">The zero-based index of the element to remove.</param>
    /// <returns><see langword="true"/> if the element was removed; otherwise, <see langword="false"/>.</returns>
    public Boolean RemoveAt(Int32 index);
}