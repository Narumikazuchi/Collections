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
    public CommonArrayEnumerator(TElement[] items)
    {
        ArgumentNullException.ThrowIfNull(items);

        m_Elements = items;
        m_Index = -1;
    }
    /// <summary>
    /// Initializes a new instance of the <see cref="CommonArrayEnumerator{TElement}"/> struct.
    /// </summary>
    /// <param name="items">The array containing the items to iterate through.</param>
    /// <exception cref="ArgumentNullException"/>
    public CommonArrayEnumerator(ImmutableArray<TElement> items)
    {
        ArgumentNullException.ThrowIfNull(items);

        m_Elements = items.ToArray();
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
            return ++m_Index < m_Elements.Length;
        }
    }

    void IEnumerator.Reset()
    { }

    void IDisposable.Dispose()
    { }

    /// <inheritdoc/>
    public TElement Current =>
        m_Elements[m_Index];

    Object? IEnumerator.Current =>
        this.Current;

    internal readonly TElement[] m_Elements;
    private Int32 m_Index;
}