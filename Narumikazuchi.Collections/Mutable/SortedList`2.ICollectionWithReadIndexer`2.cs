namespace Narumikazuchi.Collections;

public partial class SortedList<TElement, TComparer> : ICollectionWithReadIndexer<TElement, CommonListEnumerator<TElement>>
{
    /// <inheritdoc/>
    public Int32 IndexOf(
#if NETCOREAPP3_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        [DisallowNull]
#endif
        NotNull<TElement> element)
    {
        return m_Items.BinarySearch(item: element,
                                    comparer: (TComparer)this.Comparer);
    }
    /// <inheritdoc/>
    public Int32 IndexOf<TEqualityComparer>(
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
        TEqualityComparer comparer = equalityComparer;

        Int32 hashcode = comparer.GetHashCode(element);
        for (Int32 index = 0;
             index < m_Items.Count;
             index++)
        {
            TElement current = m_Items[index];
            if (comparer.GetHashCode(current!) == hashcode &&
                comparer.Equals(x: element,
                                y: current))
            {
                return index;
            }
        }

        return -1;
    }

    /// <inheritdoc />
#if NETCOREAPP3_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
    [NotNull]
#endif
    public NotNull<TElement> this[Int32 index]
    {
        get
        {
            return m_Items[index]!;
        }
    }
#if NETCOREAPP3_1_OR_GREATER
    /// <inheritdoc />
    [NotNull]
    public NotNull<TElement> this[Index index]
    {
        get
        {
            return m_Items[index]!;
        }
    }
    /// <inheritdoc />
    public ReadOnlyMemory<TElement> this[Range range]
    {
        get
        {
            if ((range.Start.IsFromEnd &&
                range.Start.Value > m_Items.Count - 1) ||
                (!range.Start.IsFromEnd &&
                range.Start.Value < 0))
            {
                throw new ArgumentOutOfRangeException(paramName: nameof(range));
            }
            else if ((range.End.IsFromEnd &&
                     range.End.Value > m_Items.Count - 1) ||
                     (!range.End.IsFromEnd &&
                     range.End.Value < 0))
            {
                throw new ArgumentOutOfRangeException(paramName: nameof(range));
            }
            else
            {
                (Int32 offset, Int32 length) = range.GetOffsetAndLength(m_Items.Count);
                return m_Items.GetRange(index: offset,
                                        count: length)
                              .ToArray();
            }
        }
    }
#endif
}