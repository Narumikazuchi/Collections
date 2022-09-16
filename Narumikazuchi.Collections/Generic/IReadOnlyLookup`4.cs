namespace Narumikazuchi.Collections;

/// <summary>
/// Represents a strongly-typed read-only collection of key-value pairs.
/// </summary>
public interface IReadOnlyLookup<TKey, TValue, TEnumerator, TEqualityComparer> : 
    ICollectionWithCount<KeyValuePair<TKey, TValue>, TEnumerator>
        where TEnumerator : struct, IStrongEnumerator<KeyValuePair<TKey, TValue>>
        where TKey : notnull
        where TEqualityComparer : IEqualityComparer<TKey>
{
    /// <summary>
    /// Returns whether the <see cref="IReadOnlyLookup{TKey, TValue, TEnumerator, TEqualityComparer}"/> contains the specified key.
    /// </summary>
    /// <param name="key">The key to search in the lookup.</param>
    /// <returns><see langword="true"/> if the key was found in the <see cref="IReadOnlyLookup{TKey, TValue, TEnumerator, TEqualityComparer}"/>; otherwise, <see langword="false"/>.</returns>
    public Boolean ContainsKey(
#if NETCOREAPP3_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        [DisallowNull]
#endif
        TKey key);

    /// <summary>
    /// Returns whether the <see cref="IReadOnlyLookup{TKey, TValue, TEnumerator, TEqualityComparer}"/> contains the specified value.
    /// </summary>
    /// <param name="value">The value to search in the lookup.</param>
    /// <returns><see langword="true"/> if the value was found in the <see cref="IReadOnlyLookup{TKey, TValue, TEnumerator, TEqualityComparer}"/>; otherwise, <see langword="false"/>.</returns>
    public Boolean ContainsValue(TValue value);

    /// <summary>
    /// Returns whether the <see cref="IReadOnlyLookup{TKey, TValue, TEnumerator, TEqualityComparer}"/> could retrieve the value for the specified key.
    /// </summary>
    /// <param name="key">The key to search in the <see cref="IReadOnlyLookup{TKey, TValue, TEnumerator, TEqualityComparer}"/>.</param>
    /// <param name="value">The value that corresponds the the key.</param>
    /// <returns><see langword="true"/> if the key was found in the <see cref="IReadOnlyLookup{TKey, TValue, TEnumerator, TEqualityComparer}"/>; otherwise, <see langword="false"/>.</returns>
    public Boolean TryGetValue(
#if NETCOREAPP3_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        [DisallowNull]
#endif
        TKey key,
#if NETCOREAPP3_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        [NotNullWhen(true)]
#endif
        out TValue? value);

    /// <summary>
    /// Returns the value that is identified by the specified key.
    /// </summary>
    /// <param name="key">The key to search in the <see cref="IReadOnlyLookup{TKey, TValue, TEnumerator, TEqualityComparer}"/>.</param>
    /// <returns>The value that is identified by the specified key.</returns>
    public TValue this[TKey key] { get; }

    /// <summary>
    /// Gets a collection that contains all the keys that are present in the <see cref="IReadOnlyLookup{TKey, TValue, TEnumerator, TEqualityComparer}"/>.
    /// </summary>
    public ReadOnlyCollection<TKey> Keys { get; }

    /// <summary>
    /// Gets a collection that contains all the values that are present in the <see cref="IReadOnlyLookup{TKey, TValue, TEnumerator, TEqualityComparer}"/>.
    /// </summary>
    public ReadOnlyCollection<TValue> Values { get; }

    /// <summary>
    /// Gets the <see cref="IEqualityComparer{T}"/> for the <see cref="IReadOnlyLookup{TKey, TValue, TEnumerator, TEqualityComparer}"/>.
    /// </summary>
    public TEqualityComparer EqualityComparer { get; }
}