namespace Narumikazuchi.Collections;

/// <summary>
/// An enumerator that iterates through the <see cref="ReadOnlySortedDictionary{TKey, TValue, TComparer, TEqualityComparer}"/>.
/// </summary>
public struct CommonDictionaryEnumerator<TKey, TValue> :
    IStrongEnumerator<KeyValuePair<TKey, TValue>>,
    IEnumerator<KeyValuePair<TKey, TValue>>
        where TKey : notnull
{
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