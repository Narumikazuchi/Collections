namespace Narumikazuchi.Collections;

/// <summary>
/// An enumerator that iterates through a <see cref="List{T}"/>.
/// </summary>
public struct CommonListEnumerator<TElement> :
    IStrongEnumerator<TElement>,
    IEnumerator<TElement>
{
    /// <summary>
    /// The default constructor for the <see cref="CommonListEnumerator{TElement}"/> is not allowed.
    /// </summary>
    /// <exception cref="NotAllowed"></exception>
    public CommonListEnumerator()
    {
        throw new NotAllowed();
    }
    /// <summary>
    /// Initializes a new instance of the <see cref="CommonListEnumerator{TElement}"/> struct.
    /// </summary>
    /// <param name="items">The <see cref="List{T}"/> containing the items to iterate through.</param>
    /// <exception cref="ArgumentNullException"/>
    public CommonListEnumerator([DisallowNull] List<TElement> items)
    {
        ArgumentNullException.ThrowIfNull(items);

        m_Elements = items;
        m_Index = -1;
    }

    /// <inheritdoc/>
    public Boolean MoveNext() =>
        ++m_Index < m_Elements.Count;

    void IEnumerator.Reset()
    { }

    void IDisposable.Dispose()
    { }

    /// <inheritdoc/>
    public TElement Current =>
        m_Elements[m_Index];

    Object? IEnumerator.Current =>
        this.Current;

    internal readonly List<TElement> m_Elements;
    private Int32 m_Index;
}