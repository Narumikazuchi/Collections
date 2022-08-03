namespace Narumikazuchi.Collections;

/// <summary>
/// Represents a strongly-typed collection of key-value pairs.
/// </summary>
public interface ILookup<TKey, TValue, TEnumerator, TEqualityComparer> :
    IReadOnlyLookup<TKey, TValue, TEnumerator, TEqualityComparer>
        where TEnumerator : struct, IStrongEnumerator<KeyValuePair<TKey, TValue>>
        where TKey : notnull
        where TEqualityComparer : IEqualityComparer<TKey>
{
    /// <summary>
    /// Adds the specified key and value to the <see cref="ILookup{TKey, TValue, TEnumerator, TEqualityComparer}"/>.
    /// </summary>
    /// <param name="key">The key to add to the collection.</param>
    /// <param name="value">The corresponding value to the key added to the collection.</param>
    /// <exception cref="ArgumentNullException"/>
    public void Add([DisallowNull] TKey key,
                    TValue value);

    /// <summary>
    /// Adds the specified key-value pairs to the <see cref="ILookup{TKey, TValue, TEnumerator, TEqualityComparer}"/>.
    /// </summary>
    /// <param name="enumerable">The enumerable that contains the key-value pairs to add to the collection.</param>
    /// <exception cref="ArgumentNullException"/>
    public void AddRange<TEnumerable, TOtherEnumerator>([DisallowNull] TEnumerable enumerable)
        where TOtherEnumerator : struct, IStrongEnumerator<KeyValuePair<TKey, TValue>>
        where TEnumerable : IStrongEnumerable<KeyValuePair<TKey, TValue>, TOtherEnumerator>;

    /// <summary>
    /// Removes the key-value pair with the specified key from the collection.
    /// </summary>
    /// <param name="key">The key of the key-value pair to remove.</param>
    /// <returns><see langword="true"/> if the key-value pair has been removed from the collection; otherwise, <see langword="false"/>.</returns>
    /// <exception cref="ArgumentNullException"/>
    public Boolean Remove([DisallowNull] TKey key);

    /// <summary>
    /// Removes all elements from this collection.
    /// </summary>
    public void Clear();
}