namespace Narumikazuchi.Collections;

public partial class BaseReadOnlyCollection<TElement, TCollection, TEnumerator> : IReadOnlyCollection<TElement, TEnumerator>
{
    /// <inheritdoc/>
    public Boolean Contains(
#if NETCOREAPP3_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        [DisallowNull]
#endif
        NotNull<TElement> element)
    {
        return this.IndexOf(element) > -1;
    }
    /// <inheritdoc/>
    public Boolean Contains<TEqualityComparer>(
#if NETCOREAPP3_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        [DisallowNull]
#endif
        NotNull<TElement> element,
#if NETCOREAPP3_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        [DisallowNull]
#endif
        NotNull<TEqualityComparer> equalityComparer)
            where TEqualityComparer : IEqualityComparer<TElement>
    {
        return this.IndexOf(element: element,
                            equalityComparer: equalityComparer) > -1;
    }

    /// <inheritdoc/>
    public void CopyTo(
#if NETCOREAPP3_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        [DisallowNull]
#endif
        NotNull<TElement[]> array)
    {
        this.CopyTo(array: array,
                    destinationIndex: 0);
    }
    /// <inheritdoc/>
    public void CopyTo(
#if NETCOREAPP3_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        [DisallowNull]
#endif
        NotNull<TElement[]> array,
        Int32 destinationIndex)
    {
#if NETCOREAPP3_1_OR_GREATER
        destinationIndex.ThrowIfOutOfRange(0, Int32.MaxValue);
#else
        if (destinationIndex is < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(destinationIndex));
        }
#endif

        if (m_Items is TElement[] sourceArray)
        {
            Array.Copy(sourceArray: sourceArray,
                       destinationArray: (TElement[])array,
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
                       destinationArray: (TElement[])array,
                       sourceIndex: m_SectionStart,
                       destinationIndex: destinationIndex,
                       length: this.Count);
        }
    }
}