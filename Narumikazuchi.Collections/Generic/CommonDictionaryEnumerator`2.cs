namespace Narumikazuchi.Collections;

/// <summary>
/// An enumerator that iterates through the <see cref="ReadOnlySortedDictionary{TKey, TValue, TComparer, TEqualityComparer}"/>.
/// </summary>
public struct CommonDictionaryEnumerator<TKey, TValue> :
    IStrongEnumerator<KeyValuePair<TKey, TValue>>,
    IEnumerator<KeyValuePair<TKey, TValue>>
        where TKey : notnull
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CommonArrayEnumerator{TElement}"/> struct.
    /// </summary>
    /// <param name="items">The <see cref="Dictionary{TKey, TValue}"/> containing the key-value pairs to iterate through.</param>
    /// <exception cref="ArgumentNullException"></exception>
    public CommonDictionaryEnumerator(
#if NETCOREAPP3_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        [DisallowNull]
#endif
        Dictionary<TKey, TValue> items)
    {
#if NET6_0_OR_GREATER
        ArgumentNullException.ThrowIfNull(items);
#else
        if (items == null)
        {
            throw new ArgumentNullException(nameof(items));
        }
#endif

        m_Elements = new __DictionaryEntry<TKey, TValue>[items.Count];
        m_Count = items.Count;
        Int32 index = 0;
        foreach (KeyValuePair<TKey, TValue> element in items)
        {
            m_Elements[index++] = new()
            {
                Key = element.Key,
                Value = element.Value
            };
        }
        m_Index = -1;
    }
#if NETCOREAPP3_1_OR_GREATER
    /// <summary>
    /// Initializes a new instance of the <see cref="CommonArrayEnumerator{TElement}"/> struct.
    /// </summary>
    /// <param name="items">The <see cref="ImmutableDictionary{TKey, TValue}"/> containing the key-value pairs to iterate through.</param>
    /// <exception cref="ArgumentNullException"></exception>
    public CommonDictionaryEnumerator(
#if NETCOREAPP3_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        [DisallowNull]
#endif
        ImmutableDictionary<TKey, TValue> items)
    {
#if NET6_0_OR_GREATER
        ArgumentNullException.ThrowIfNull(items);
#else
        if (items == null)
        {
            throw new ArgumentNullException(nameof(items));
        }
#endif

        m_Elements = new __DictionaryEntry<TKey, TValue>[items.Count];
        m_Count = items.Count;
        Int32 index = 0;
        foreach (KeyValuePair<TKey, TValue> element in items)
        {
            m_Elements[index++] = new()
            {
                Key = element.Key,
                Value = element.Value
            };
        }
        m_Index = -1;
    }
#endif
    internal CommonDictionaryEnumerator(__DictionaryEntry<TKey, TValue>[] elements,
                                        Int32 count)
    {
        m_Elements = elements;
        m_Count = count;
        m_Index = -1;
    }

    /// <inheritdoc/>
    public Boolean MoveNext()
    {
        if (m_Elements is null)
        {
            return false;
        }
        else
        {
            return ++m_Index < m_Count;
        }
    }

    /// <inheritdoc/>
    public KeyValuePair<TKey, TValue> Current =>
        new(key: m_Elements[m_Index].Key,
            value: m_Elements[m_Index].Value);

#if !NETCOREAPP3_1_OR_GREATER
    void IEnumerator.Reset()
    { }

    void IDisposable.Dispose()
    { }

    Object? IEnumerator.Current =>
        this.Current;
#endif

    private readonly __DictionaryEntry<TKey, TValue>[] m_Elements;
    private readonly Int32 m_Count;
    private Int32 m_Index;
}