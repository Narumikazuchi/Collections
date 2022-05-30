namespace Narumikazuchi.Collections;

/// <summary>
/// Represents a strongly-typed read-only collection of key-value pairs.
/// </summary>
public interface IReadOnlyLookup<TKey, TValue, TEnumerator> : 
    ICollectionWithCount<KeyValuePair<TKey, TValue>, TEnumerator>
        where TEnumerator : struct, IStrongEnumerator<KeyValuePair<TKey, TValue>>
        where TKey : notnull
{
    /// <summary>
    /// Returns whether the <see cref="IReadOnlyLookup{TKey, TValue, TEnumerator}"/> contains the specified key.
    /// </summary>
    /// <param name="key">The key to search in the lookup.</param>
    /// <returns><see langword="true"/> if the key was found in the <see cref="IReadOnlyLookup{TKey, TValue, TEnumerator}"/>; otherwise, <see langword="false"/>.</returns>
    public Boolean ContainsKey([DisallowNull] TKey key);

    /// <summary>
    /// Returns whether the <see cref="IReadOnlyLookup{TKey, TValue, TEnumerator}"/> contains the specified value.
    /// </summary>
    /// <param name="value">The value to search in the lookup.</param>
    /// <returns><see langword="true"/> if the value was found in the <see cref="IReadOnlyLookup{TKey, TValue, TEnumerator}"/>; otherwise, <see langword="false"/>.</returns>
    public Boolean ContainsValue(TValue value);

    /// <summary>
    /// Returns whether the <see cref="IReadOnlyLookup{TKey, TValue, TEnumerator}"/> could retrieve the value for the specified key.
    /// </summary>
    /// <param name="key">The key to search in the <see cref="IReadOnlyLookup{TKey, TValue, TEnumerator}"/>.</param>
    /// <param name="value">The value that corresponds the the key.</param>
    /// <returns><see langword="true"/> if the key was found in the <see cref="IReadOnlyLookup{TKey, TValue, TEnumerator}"/>; otherwise, <see langword="false"/>.</returns>
    public Boolean TryGetValue([DisallowNull] TKey key,
                               [NotNullWhen(true)] out TValue? value);

    /// <summary>
    /// Returns the value that is identified by the specified key.
    /// </summary>
    /// <param name="key">The key to search in the <see cref="IReadOnlyLookup{TKey, TValue, TEnumerator}"/>.</param>
    /// <returns>The value that is identified by the specified key.</returns>
    public TValue this[[DisallowNull] TKey key] { get; }

    /// <summary>
    /// Gets a collection that contains all the keys that are present in the <see cref="IReadOnlyLookup{TKey, TValue, TEnumerator}"/>.
    /// </summary>
    public ReadOnlyCollection<TKey> Keys { get; }

    /// <summary>
    /// Gets a collection that contains all the values that are present in the <see cref="IReadOnlyLookup{TKey, TValue, TEnumerator}"/>.
    /// </summary>
    public ReadOnlyCollection<TValue> Values { get; }
}