namespace Narumikazuchi.Collections;

public partial class ObservableList<TElement> : IReadOnlyCollection<TElement, CommonListEnumerator<TElement>>
{
    /// <inheritdoc/>
    public Boolean Contains([DisallowNull] TElement element)
    {
#if NET6_0_OR_GREATER
        ArgumentNullException.ThrowIfNull(element);
#else
        if (element is null)
        {
            throw new ArgumentNullException(nameof(element));
        }
#endif

        return this.IndexOf(element) > -1;
    }
    /// <inheritdoc/>
    public Boolean Contains<TEqualityComparer>([DisallowNull] TElement element,
                                               [DisallowNull] TEqualityComparer equalityComparer)
        where TEqualityComparer : IEqualityComparer<TElement>
    {
#if NET6_0_OR_GREATER
        ArgumentNullException.ThrowIfNull(element);
        ArgumentNullException.ThrowIfNull(equalityComparer);
#else
        if (element is null)
        {
            throw new ArgumentNullException(nameof(element));
        }
        
        if (equalityComparer is null)
        {
            throw new ArgumentNullException(nameof(equalityComparer));
        }
#endif

        return this.IndexOf(element: element,
                            equalityComparer: equalityComparer) > -1;
    }

    /// <inheritdoc/>
    public void CopyTo([DisallowNull] TElement[] array)
    {
#if NET6_0_OR_GREATER
        ArgumentNullException.ThrowIfNull(array);
#else
        if (array is null)
        {
            throw new ArgumentNullException(nameof(array));
        }
#endif

        this.CopyTo(array: array,
                    destinationIndex: 0);
    }
    /// <inheritdoc/>
    public void CopyTo([DisallowNull] TElement[] array,
                       Int32 destinationIndex)
    {
#if NET6_0_OR_GREATER
        ArgumentNullException.ThrowIfNull(array);
        destinationIndex.ThrowIfOutOfRange(0, Int32.MaxValue);
#else
        if (array is null)
        {
            throw new ArgumentNullException(nameof(array));
        }

        if (destinationIndex is < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(destinationIndex));
        }
#endif

        m_Items.CopyTo(array: array,
                       arrayIndex: destinationIndex);
    }
}