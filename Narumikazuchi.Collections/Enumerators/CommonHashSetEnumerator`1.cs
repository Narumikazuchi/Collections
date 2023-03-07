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
#if NET6_0_OR_GREATER
        ArgumentNullException.ThrowIfNull(items);
#else
        if (items is null)
        {
            throw new ArgumentNullException(nameof(items));
        }
#endif

        HashSet<TElement> elements = items;
        m_Enumerator = elements.GetEnumerator();
    }

    /// <inheritdoc/>
    public Boolean MoveNext()
    {
        return m_Enumerator.MoveNext();
    }

    /// <inheritdoc/>
    public TElement Current
    {
        get
        {
            return m_Enumerator.Current;
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

    internal HashSet<TElement>.Enumerator m_Enumerator;
}