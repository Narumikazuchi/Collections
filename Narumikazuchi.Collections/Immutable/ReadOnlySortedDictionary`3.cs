namespace Narumikazuchi.Collections;

/// <summary>
/// Represents a strongly-typed collection of key-value pairs with a significantly faster enumerator to
/// improve iteration times for <see langword="foreach"/>-loops. The key-value pairs are sorted by using
/// the provided <see cref="IComparer{T}"/>.
/// </summary>
/// <remarks>
/// The faster enumeration only works if the collection is used as-is or by using the
/// <see cref="IStrongEnumerable{TElement, TEnumerator}"/> interface. If you you plan on using 
/// the <see cref="IEnumerable{T}"/> or any other derivative interface in your code then the 
/// efficiency of the enumerator will be lost due to call virtualization in the compiler generated IL.
/// </remarks>
public sealed partial class ReadOnlySortedDictionary<TKey, TValue, TEqualityComparer> : StrongEnumerable<KeyValuePair<TKey, TValue>, CommonDictionaryEnumerator<TKey, TValue>>
    where TKey : notnull
    where TEqualityComparer : IEqualityComparer<TKey>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ReadOnlySortedDictionary{TKey, TValue, TEqualityComparer}"/> struct.
    /// </summary>
    /// <param name="items">The items that the resulting collection shall hold.</param>
    /// <param name="equalityComparer">The comparer that will be used to compare two instances of type <typeparamref name="TKey"/> for equality.</param>
    /// <param name="comparer">The comparer that will be used to compare two instances of type <typeparamref name="TKey"/>.</param>
    /// <exception cref="ArgumentNullException" />
    public static ReadOnlySortedDictionary<TKey, TValue, TEqualityComparer> CreateFrom<TComparer, TEnumerable>([DisallowNull] TEnumerable items,
                                                                                                               [DisallowNull] TEqualityComparer equalityComparer,
                                                                                                               [DisallowNull] TComparer comparer)
        where TComparer : IComparer<TKey>
        where TEnumerable : IEnumerable<KeyValuePair<TKey, TValue>>
    {
#if NET6_0_OR_GREATER
        ArgumentNullException.ThrowIfNull(items);
        ArgumentNullException.ThrowIfNull(equalityComparer);
        ArgumentNullException.ThrowIfNull(comparer);
#else
        if (items is null)
        {
            throw new ArgumentNullException(nameof(items));
        }
        
        if (equalityComparer is null)
        {
            throw new ArgumentNullException(nameof(equalityComparer));
        }
        
        if (comparer is null)
        {
            throw new ArgumentNullException(nameof(comparer));
        }
#endif

        if (items is IReadOnlyCollection<KeyValuePair<TKey, TValue>> roc)
        {
            return new(items: roc.OrderBy(kv => kv.Key, (TComparer)comparer)
                                 .ToArray(),
                       equalityComparer: equalityComparer);
        }
        else if (items is ICollection<KeyValuePair<TKey, TValue>> c)
        {
            return new(items: c.OrderBy(kv => kv.Key, (TComparer)comparer)
                               .ToArray(),
                       equalityComparer: equalityComparer);
        }
        else
        {
            return new(items: items.OrderBy(kv => kv.Key, (TComparer)comparer)
                                   .ToArray(),
                       equalityComparer: equalityComparer);
        }
    }

    /// <inheritdoc/>
    public sealed override CommonDictionaryEnumerator<TKey, TValue> GetEnumerator()
    {
        return new(elements: m_Entries,
                   count: this.Count);
    }

    /// <inheritdoc/>
    public Boolean ContainsKey([DisallowNull] TKey key)
    {
#if NET6_0_OR_GREATER
        ArgumentNullException.ThrowIfNull(key);
#else
        if (key is null)
        {
            throw new ArgumentNullException(nameof(key));
        }
#endif

        return this.FindEntry(key) > -1;
    }

    /// <inheritdoc/>
    public Boolean ContainsValue([DisallowNull] TValue value)
    {
#if NET6_0_OR_GREATER
        ArgumentNullException.ThrowIfNull(value);
#else
        if (value is null)
        {
            throw new ArgumentNullException(nameof(value));
        }
#endif

        return this.Values.Contains(value);
    }

    /// <inheritdoc/>
    public Boolean TryGetValue([DisallowNull] TKey key,
                               [NotNullWhen(true)] out TValue? value)
    {
#if NET6_0_OR_GREATER
        ArgumentNullException.ThrowIfNull(key);
#else
        if (key is null)
        {
            throw new ArgumentNullException(nameof(key));
        }
#endif

        Int32 index = this.FindEntry(key);
        if (index > -1)
        {
            value = m_Entries[index].Value!;
            return true;
        }
        else
        {
            value = default;
            return false;
        }
    }

    /// <inheritdoc/>
    public TValue this[TKey key]
    {
        get
        {
#if NET6_0_OR_GREATER
            ArgumentNullException.ThrowIfNull(key);
#else
            if (key is null)
            {
                throw new ArgumentNullException(nameof(key));
            }
#endif

            Int32 index = this.FindEntry(key);
            if (index > -1)
            {
                return m_Entries[index].Value!;
            }
            else
            {
                throw new KeyNotFoundException();
            }
        }
    }

    /// <inheritdoc/>
    public ReadOnlyList<TKey> Keys { get; }

    /// <inheritdoc/>
    public ReadOnlyList<TValue> Values { get; }

    /// <inheritdoc/>
    public TEqualityComparer EqualityComparer { get; }
}