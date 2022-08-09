namespace Narumikazuchi.Collections;

/// <summary>
/// An enumerator that iterates through a <see cref="List{T}"/>.
/// </summary>
public struct CommonHashSetEnumerator<TElement> :
    IStrongEnumerator<TElement>,
    IEnumerator<TElement>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CommonHashSetEnumerator{TElement}"/> struct.
    /// </summary>
    /// <param name="items">The <see cref="HashSet{T}"/> containing the items to iterate through.</param>
    /// <exception cref="ArgumentNullException"/>
    public CommonHashSetEnumerator([DisallowNull] HashSet<TElement> items)
    {
        ArgumentNullException.ThrowIfNull(items);

        m_Enumerator = items.GetEnumerator();
    }

    /// <inheritdoc/>
    public Boolean MoveNext() =>
        m_Enumerator.MoveNext();

    void IEnumerator.Reset()
    { }

    void IDisposable.Dispose()
    { }

    /// <inheritdoc/>
    public TElement Current =>
        m_Enumerator.Current;

    Object? IEnumerator.Current =>
        this.Current;

    internal HashSet<TElement>.Enumerator m_Enumerator;
}