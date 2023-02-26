namespace Narumikazuchi.Collections;

public partial class BaseReadOnlyCollection<TElement, TCollection, TEnumerator> : ICollectionWithReadIndexer<TElement, TEnumerator>
{
    /// <inheritdoc/>
    public Int32 IndexOf(
#if NETCOREAPP3_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        [DisallowNull]
#endif
        NotNull<TElement> element)
    {
        return this.IndexOf<EqualityComparer<TElement>>(element: element,
                                                        equalityComparer: EqualityComparer<TElement>.Default);
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
        for (Int32 index = m_SectionStart; index < m_SectionEnd; index++)
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

    /// <inheritdoc/>
    public NotNull<TElement> this[Int32 index]
    {
        get
        {
            Int32 adjustedIndex = m_SectionStart + index;
            if (m_Items is TElement[] array)
            {
                return array[adjustedIndex]!;
            }
            else if (m_Items is List<TElement> list)
            {
                return list[adjustedIndex]!;
            }
            else
            {
                return m_Items[adjustedIndex]!;
            }
        }
    }
#if NETCOREAPP3_1_OR_GREATER
    /// <inheritdoc/>
    public NotNull<TElement> this[Index index]
    {
        get
        {
            Int32 adjustedIndex = m_SectionStart + index.Value;
            if (m_Items is TElement[] array)
            {
                return array[adjustedIndex]!;
            }
            else if (m_Items is List<TElement> list)
            {
                return list[adjustedIndex]!;
            }
            else
            {
                return m_Items[adjustedIndex]!;
            }
        }
    }
    /// <inheritdoc/>
    public ReadOnlyMemory<TElement> this[Range range]
    {
        get
        {
            Range adjustedIndex = (m_SectionStart + range.Start.Value)..(m_SectionStart + range.End.Value);
            if (m_Items is TElement[] array)
            {
                return array[adjustedIndex]!;
            }
            else if (m_Items is List<TElement> list)
            {
                (Int32 offset, Int32 length) = adjustedIndex.GetOffsetAndLength(this.Count);
                return list.GetRange(index: offset,
                                     count: length)
                           .ToArray();
            }
            else
            {
                (Int32 offset, Int32 length) = adjustedIndex.GetOffsetAndLength(this.Count);
                TElement[] newArray = new TElement[length];
                Array.Copy(sourceArray: m_Items.ToArray(),
                           destinationArray: newArray,
                           sourceIndex: offset,
                           destinationIndex: 0,
                           length: length);
                return newArray;
            }
        }
    }
#endif
}