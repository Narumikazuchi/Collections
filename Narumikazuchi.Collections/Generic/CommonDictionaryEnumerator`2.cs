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
    public CommonDictionaryEnumerator([DisallowNull] Dictionary<TKey, TValue> items)
    {
        ArgumentNullException.ThrowIfNull(items);

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
    /// <summary>
    /// Initializes a new instance of the <see cref="CommonArrayEnumerator{TElement}"/> struct.
    /// </summary>
    /// <param name="items">The <see cref="ImmutableDictionary{TKey, TValue}"/> containing the key-value pairs to iterate through.</param>
    /// <exception cref="ArgumentNullException"></exception>
    public CommonDictionaryEnumerator([DisallowNull] ImmutableDictionary<TKey, TValue> items)
    {
        ArgumentNullException.ThrowIfNull(items);

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

    void IEnumerator.Reset()
    { }

    void IDisposable.Dispose()
    { }

    /// <inheritdoc/>
    public KeyValuePair<TKey, TValue> Current =>
        new(key: m_Elements[m_Index].Key,
            value: m_Elements[m_Index].Value);

    Object? IEnumerator.Current =>
        this.Current;

    private readonly __DictionaryEntry<TKey, TValue>[] m_Elements;
    private readonly Int32 m_Count;
    private Int32 m_Index;
}