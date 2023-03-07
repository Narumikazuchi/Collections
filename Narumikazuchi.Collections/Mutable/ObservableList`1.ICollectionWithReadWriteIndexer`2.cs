using System.Collections.Generic;
using System.Xml.Linq;

namespace Narumikazuchi.Collections;

public partial class ObservableList<TElement> : ICollectionWithReadWriteIndexer<TElement, CommonListEnumerator<TElement>>
{
    /// <inheritdoc/>
    public Int32 IndexOf([DisallowNull] TElement element)
    {
#if NET6_0_OR_GREATER
        ArgumentNullException.ThrowIfNull(element);
#else
        if (element is null)
        {
            throw new ArgumentNullException(nameof(element));
        }
#endif

        return this.IndexOf(element: element,
                            equalityComparer: EqualityComparer<TElement>.Default);
    }
    /// <inheritdoc/>
    public Int32 IndexOf<TEqualityComparer>([DisallowNull] TElement element,
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

        Int32 hashcode = equalityComparer.GetHashCode(element);
        for (Int32 index = 0;
             index < m_Items.Count;
             index++)
        {
            TElement current = m_Items[index];
            if (equalityComparer.GetHashCode(current!) == hashcode &&
                equalityComparer.Equals(x: element,
                                        y: current))
            {
                return index;
            }
        }

        return -1;
    }

    /// <inheritdoc />
    [NotNull]
    public TElement this[Int32 index]
    {
        get
        {
            return m_Items[index]!;
        }
        set
        {
#if NET6_0_OR_GREATER
            ArgumentNullException.ThrowIfNull(value);
#else
            if (value is null)
            {
                throw new ArgumentNullException(nameof(value));
            }
#endif

            if ((UInt32)index >= (UInt32)this.Count)
            {
                throw new IndexOutOfRangeException();
            }
            else
            {
                this.Insert(index: index,
                            item: value!);
            }
        }
    }
#if NET6_0_OR_GREATER
    /// <inheritdoc />
    [NotNull]
    public TElement this[Index index]
    {
        get
        {
            return m_Items[index]!;
        }
        set
        {
            m_Items[index] = value;
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