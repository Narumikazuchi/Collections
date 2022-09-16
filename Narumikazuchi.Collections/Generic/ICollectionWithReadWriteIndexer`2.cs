namespace Narumikazuchi.Collections;

/// <summary>
/// Represents a strongly-typed collection which allows read-write access to it's elements via an indexer.
/// </summary>
public interface ICollectionWithReadWriteIndexer<TElement, TEnumerator> : 
    ICollectionWithReadIndexer<TElement, TEnumerator>,
    ICollectionWithWriteIndexer<TElement, TEnumerator>
        where TEnumerator : struct, IStrongEnumerator<TElement>
{
    /// <summary>
    /// Gets or sets the <typeparamref name="TElement"/> at the specified index.
    /// </summary>
    /// <param name="index">The zero-based index of the <typeparamref name="TElement"/> to get or set.</param>
    /// <returns>The element at the specified index.</returns>
    /// <exception cref="IndexOutOfRangeException"/>
    public new TElement this[Int32 index]
    {
        get;
        set;
    }
#if NETCOREAPP3_1_OR_GREATER
    /// <summary>
    /// Gets or sets the <typeparamref name="TElement"/> at the specified index.
    /// </summary>
    /// <param name="index">The zero-based index of the <typeparamref name="TElement"/> to get or set.</param>
    /// <returns>The element at the specified index.</returns>
    /// <exception cref="IndexOutOfRangeException"/>
    public new TElement this[Index index]
    {
        get;
        set;
    }
#endif
}