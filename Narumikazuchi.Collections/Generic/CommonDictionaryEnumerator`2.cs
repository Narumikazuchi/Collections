namespace Narumikazuchi.Collections;

/// <summary>
/// An enumerator that iterates through the <see cref="ReadOnlySortedDictionary{TKey, TValue}"/>.
/// </summary>
public struct CommonDictionaryEnumerator<TKey, TValue> :
    IStrongEnumerator<KeyValuePair<TKey, TValue>>
        where TKey : notnull
{
    public CommonDictionaryEnumerator([DisallowNull] Dictionary<TKey, TValue> elements)
    {
        m_Elements = new __DictionaryEntry<TKey, TValue>[elements.Count];
        Int32 index = 0;
        foreach (KeyValuePair<TKey, TValue> element in elements)
        {
            m_Elements[index++] = new()
            {
                Key = element.Key,
                Value = element.Value
            };
        }
        m_Index = -1;
    }
    public CommonDictionaryEnumerator([DisallowNull] ImmutableDictionary<TKey, TValue> elements)
    {
        m_Elements = new __DictionaryEntry<TKey, TValue>[elements.Count];
        Int32 index = 0;
        foreach (KeyValuePair<TKey, TValue> element in elements)
        {
            m_Elements[index++] = new()
            {
                Key = element.Key,
                Value = element.Value
            };
        }
        m_Index = -1;
    }
    internal CommonDictionaryEnumerator(in __DictionaryEntry<TKey, TValue>[] elements)
    {
        m_Elements = elements;
        m_Index = -1;
    }

    /// <inheritdoc/>
    public Boolean MoveNext() =>
        ++m_Index < m_Elements.Length;

    /// <inheritdoc/>
    public KeyValuePair<TKey, TValue> Current =>
        new(key: m_Elements[m_Index].Key,
            value: m_Elements[m_Index].Value);

    private readonly __DictionaryEntry<TKey, TValue>[] m_Elements;
    private Int32 m_Index;
}