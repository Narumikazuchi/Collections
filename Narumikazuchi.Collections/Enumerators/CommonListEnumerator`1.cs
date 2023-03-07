namespace Narumikazuchi.Collections;

/// <summary>
/// An enumerator that iterates through a <see cref="List{T}"/>.
/// </summary>
public struct CommonListEnumerator<TElement> :
    IStrongEnumerator<TElement>,
    IEnumerator<TElement>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CommonListEnumerator{TElement}"/> struct.
    /// </summary>
    /// <param name="items">The <see cref="List{T}"/> containing the items to iterate through.</param>
    /// <exception cref="ArgumentNullException"/>
    public CommonListEnumerator([DisallowNull] List<TElement> items)
    {
#if NET6_0_OR_GREATER
        ArgumentNullException.ThrowIfNull(items);
#else
        if (items is null)
        {
            throw new ArgumentNullException(nameof(items));
        }
#endif

        m_Elements = items;
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
            return ++m_Index < m_Elements.Count;
        }
    }

    /// <inheritdoc/>
    public TElement Current
    {
        get
        {
            return m_Elements[m_Index];
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

    internal readonly List<TElement> m_Elements;
    private Int32 m_Index;
}