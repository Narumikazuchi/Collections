﻿namespace Narumikazuchi.Collections;

/// <summary>
/// Represents a strongly-typed collection which allows read access to it's elements via an indexer.
/// </summary>
public interface ICollectionWithReadIndexer<TElement, TEnumerator>
     : IStrongEnumerable<TElement, TEnumerator>
        where TEnumerator : struct, IStrongEnumerator<TElement>
{
    /// <summary>
    /// Searches for the specified <typeparamref name="TElement"/> and returns the zero-based index of the first
    /// occurrence within the entire <see cref="ICollectionWithReadIndexer{TElement, TEnumerator}"/>.
    /// </summary>
    /// <param name="element">The <typeparamref name="TElement"/> to locate.</param>
    /// <returns>
    /// The zero-based index of the first occurrence of item within the entire 
    /// <see cref="ICollectionWithReadIndexer{TElement, TEnumerator}"/>, if found; otherwise, -1.
    /// </returns>
    public Int32 IndexOf([DisallowNull] TElement element);
    /// <summary>
    /// Searches for the specified <typeparamref name="TElement"/> and returns the zero-based index of the first
    /// occurrence within the entire <see cref="ICollectionWithReadIndexer{TElement, TEnumerator}"/>.
    /// </summary>
    /// <param name="element">The <typeparamref name="TElement"/> to locate.</param>
    /// <param name="equalityComparer">The equality comparer to use to check for equality.</param>
    /// <returns>
    /// The zero-based index of the first occurrence of item within the entire 
    /// <see cref="ICollectionWithReadIndexer{TElement, TEnumerator}"/>, if found; otherwise, -1.
    /// </returns>
    public Int32 IndexOf<TEqualityComparer>([DisallowNull] TElement element,
                                            [DisallowNull] TEqualityComparer equalityComparer)
        where TEqualityComparer : IEqualityComparer<TElement>;

    /// <summary>
    /// Gets the <typeparamref name="TElement"/> at the specified index.
    /// </summary>
    /// <param name="index">The zero-based index of the <typeparamref name="TElement"/> to get.</param>
    /// <returns>The element at the specified index.</returns>
    /// <exception cref="IndexOutOfRangeException"/>
    [NotNull]
    public TElement this[Int32 index] { get; }
#if NET6_0_OR_GREATER
    /// <summary>
    /// Gets the <typeparamref name="TElement"/> at the specified index.
    /// </summary>
    /// <param name="index">The zero-based index of the <typeparamref name="TElement"/> to get.</param>
    /// <returns>The element at the specified index.</returns>
    /// <exception cref="IndexOutOfRangeException"/>
    public TElement this[Index index] { get; }
    /// <summary>
    /// Gets a slice specified by a range.
    /// </summary>
    /// <param name="range">The zero-based range of the slice to get.</param>
    /// <returns>The slice specified by the range.</returns>
    /// <exception cref="ArgumentOutOfRangeException"/>
    public ReadOnlyMemory<TElement> this[Range range] { get; }
#endif
}