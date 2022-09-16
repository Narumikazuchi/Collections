namespace Narumikazuchi.Collections;

/// <summary>
/// Represents a strongly-typed collection that can be modified and accessed by index.
/// </summary>
public interface IModifyableCollectionWithIndex<TElement, TEnumerator> :
    IReadOnlyCollection<TElement, TEnumerator>,
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
                       TElement element);

    /// <summary>
    /// Inserts the specified collection of elements into the <see cref="IModifyableCollectionWithIndex{TElement, TEnumerator}"/>
    /// starting at the specified index.
    /// </summary>
    /// <param name="index">The zero-based index of the location where to start inserting the elements.</param>
    /// <param name="enumerable">The elements to insert.</param>
    /// <exception cref="ArgumentOutOfRangeException"/>
    public void InsertRange<TEnumerable, TOtherEnumerator>(Int32 index,
#if NETCOREAPP3_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        [DisallowNull]
#endif
        TEnumerable enumerable)
            where TOtherEnumerator : struct, IStrongEnumerator<TElement>
            where TEnumerable : IStrongEnumerable<TElement, TOtherEnumerator>;

    /// <summary>
    /// Removes the element at the specified index.
    /// </summary>
    /// <param name="index">The zero-based index of the element to remove.</param>
    /// <returns><see langword="true"/> if the element was removed; otherwise, <see langword="false"/>.</returns>
    public Boolean RemoveAt(Int32 index);
}