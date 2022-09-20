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
        HashSet<TElement> items)
    {
#if NET6_0_OR_GREATER
        ArgumentNullException.ThrowIfNull(items);
#else
        if (items == null)
        {
            throw new ArgumentNullException(nameof(items));
        }
#endif

        m_Enumerator = items.GetEnumerator();
    }

    /// <inheritdoc/>
    public Boolean MoveNext() =>
        m_Enumerator.MoveNext();

    /// <inheritdoc/>
    public TElement Current =>
        m_Enumerator.Current;

#if !NETCOREAPP3_1_OR_GREATER
    void IEnumerator.Reset()
    { }

    void IDisposable.Dispose()
    { }

    Object? IEnumerator.Current =>
        this.Current;
#endif

    internal HashSet<TElement>.Enumerator m_Enumerator;
}