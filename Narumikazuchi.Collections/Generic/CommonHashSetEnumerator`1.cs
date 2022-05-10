namespace Narumikazuchi.Collections;

/// <summary>
/// An enumerator that iterates through a <see cref="List{T}"/>.
/// </summary>
public struct CommonHashSetEnumerator<TElement> :
    IStrongEnumerator<TElement>
{
    /// <summary>
    /// The default constructor for the <see cref="CommonHashSetEnumerator{TElement}"/> is not allowed.
    /// </summary>
    /// <exception cref="NotAllowed"></exception>
    public CommonHashSetEnumerator()
    {
        throw new NotAllowed();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="CommonHashSetEnumerator{TElement}"/> struct.
    /// </summary>
    /// <param name="items">The array containing the items to iterate through.</param>
    public CommonHashSetEnumerator([DisallowNull] HashSet<TElement> items)
    {
        ArgumentNullException.ThrowIfNull(items);

        m_Elements = items;
        m_Enumerator = items.GetEnumerator();
    }

    /// <inheritdoc/>
    public Boolean MoveNext() =>
        m_Enumerator.MoveNext();

    /// <inheritdoc/>
    public TElement Current =>
        m_Enumerator.Current;

    internal readonly HashSet<TElement> m_Elements;
    internal readonly HashSet<TElement>.Enumerator m_Enumerator;
}