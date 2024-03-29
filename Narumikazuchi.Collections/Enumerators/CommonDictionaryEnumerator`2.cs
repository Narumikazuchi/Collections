﻿namespace Narumikazuchi.Collections;

/// <summary>
/// An enumerator that iterates through the <see cref="ReadOnlyDictionary{TKey, TValue, TEqualityComparer}"/>.
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
    public KeyValuePair<TKey, TValue> Current
    {
        get
        {
            return new(key: m_Elements[m_Index].Key,
                       value: m_Elements[m_Index].Value);
        }
    }

#if !NET6_0_OR_GREATER
    void IEnumerator.Reset()
    { }

    void IDisposable.Dispose()
    { }

    Object? IEnumerator.Current
    {
        get
        {
            return this.Current;
        }
    }
#endif

    private readonly __DictionaryEntry<TKey, TValue>[] m_Elements;
    private readonly Int32 m_Count;
    private Int32 m_Index;
}