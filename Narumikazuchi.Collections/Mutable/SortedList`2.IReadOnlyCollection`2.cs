namespace Narumikazuchi.Collections;

public partial class SortedList<TElement, TComparer> : IReadOnlyCollection<TElement, CommonListEnumerator<TElement>>
{
    /// <inheritdoc/>
    public Boolean Contains(
#if NETCOREAPP3_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        [DisallowNull]
#endif
        NotNull<TElement> element)
    {
        return m_Items.BinarySearch(item: element,
                                    comparer: (TComparer)this.Comparer) > -1;
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

        m_Items.CopyTo(array: array,
                       arrayIndex: destinationIndex);
    }
}