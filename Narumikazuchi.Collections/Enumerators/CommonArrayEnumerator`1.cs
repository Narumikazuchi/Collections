namespace Narumikazuchi.Collections;

/// <summary>
/// An enumerator that iterates through an array of type <typeparamref name="TElement"/>.
/// </summary>
public struct CommonArrayEnumerator<TElement> :
    IStrongEnumerator<TElement>,
    IEnumerator<TElement>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CommonArrayEnumerator{TElement}"/> struct.
    /// </summary>
    /// <param name="items">The array containing the items to iterate through.</param>
    /// <exception cref="ArgumentNullException"/>
    public CommonArrayEnumerator(
#if NETCOREAPP3_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        [DisallowNull]
#endif
        NotNull<TElement[]> items)
    {
        m_Elements = items;
        m_Index = -1;
    }
#if NETCOREAPP3_1_OR_GREATER
    /// <summary>
    /// Initializes a new instance of the <see cref="CommonArrayEnumerator{TElement}"/> struct.
    /// </summary>
    /// <param name="items">The array containing the items to iterate through.</param>
    /// <exception cref="ArgumentNullException"/>
    public CommonArrayEnumerator(ImmutableArray<TElement> items)
    {
        m_Elements = items.ToArray();
        m_Index = -1;
    }
#endif

    /// <inheritdoc/>
    public Boolean MoveNext()
    {
        if (m_Elements is null)
        {
            return false;
        }
        else
        {
            return ++m_Index < m_Elements.Length;
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

#if !NETCOREAPP3_1_OR_GREATER
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

    internal readonly TElement[] m_Elements;
    private Int32 m_Index;
}