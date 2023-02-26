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
    public CommonHashSetEnumerator(
#if NETCOREAPP3_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        [DisallowNull]
#endif
        NotNull<HashSet<TElement>> items)
    {
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

    internal HashSet<TElement>.Enumerator m_Enumerator;
}