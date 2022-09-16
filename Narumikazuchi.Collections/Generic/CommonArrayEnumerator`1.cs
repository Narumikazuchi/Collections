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
        TElement[] items)
    {
#if NET6_0_OR_GREATER
        ArgumentNullException.ThrowIfNull(items);
#else
        if (items == null)
        {
            throw new ArgumentNullException(nameof(items));
        }
#endif

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
#if NET6_0_OR_GREATER
        ArgumentNullException.ThrowIfNull(items);
#else
        if (items == null)
        {
            throw new ArgumentNullException(nameof(items));
        }
#endif

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