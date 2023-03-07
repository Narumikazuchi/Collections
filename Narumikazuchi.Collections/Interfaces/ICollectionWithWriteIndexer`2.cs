namespace Narumikazuchi.Collections;

/// <summary>
/// Represents a strongly-typed collection which allows write access to it's elements via an indexer.
/// </summary>
public interface ICollectionWithWriteIndexer<TElement, TEnumerator>
    : IStrongEnumerable<TElement, TEnumerator>
        where TEnumerator : struct, IStrongEnumerator<TElement>
{
    /// <summary>
    /// Sets the <typeparamref name="TElement"/> at the specified index.
    /// </summary>
    /// <param name="index">The zero-based index of the <typeparamref name="TElement"/> to set.</param>
    /// <exception cref="IndexOutOfRangeException"/>
    [NotNull]
    public TElement this[Int32 index] { set; }
#if NET6_0_OR_GREATER
    /// <summary>
    /// Sets the <typeparamref name="TElement"/> at the specified index.
    /// </summary>
    /// <param name="index">The zero-based index of the <typeparamref name="TElement"/> to set.</param>
    /// <exception cref="IndexOutOfRangeException"/>
    [NotNull]
    public TElement this[Index index] { set; }
#endif
}