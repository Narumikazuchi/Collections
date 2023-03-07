namespace Narumikazuchi.Collections;

public partial class BaseReadOnlyCollection<TElement, TCollection, TEnumerator> : IReadOnlyCollection<TElement, TEnumerator>
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

        if (m_Items is TElement[] sourceArray)
        {
            Array.Copy(sourceArray: sourceArray,
                       destinationArray: array,
                       sourceIndex: m_SectionStart,
                       destinationIndex: destinationIndex,
                       length: this.Count);
        }
        else if (m_Items is List<TElement> list)
        {
            list.GetRange(index: m_SectionStart,
                          count: this.Count)
                .CopyTo(array: array,
                        arrayIndex: destinationIndex);
        }
        else if (m_Items is ICollection<TElement> collectionT)
        {
            collectionT.Skip(m_SectionStart)
                       .Take(this.Count)
                       .ToList()
                       .CopyTo(array: array,
                               arrayIndex: destinationIndex);
        }
        else
        {
            Array.Copy(sourceArray: m_Items.Skip(m_SectionStart)
                                           .Take(this.Count)
                                           .ToArray(),
                       destinationArray: array,
                       sourceIndex: m_SectionStart,
                       destinationIndex: destinationIndex,
                       length: this.Count);
        }
    }
}