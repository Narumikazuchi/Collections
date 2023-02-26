namespace Narumikazuchi.Collections;

public partial class SortedList<TElement, TComparer> : IModifyableCollection<TElement, CommonListEnumerator<TElement>>
{
    /// <inheritdoc/>
    public Boolean Add(
#if NETCOREAPP3_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        [DisallowNull]
#endif
        NotNull<TElement> item)
    {
        TComparer comparer = this.Comparer;
        Boolean shouldInsert = false;
        for (Int32 index = 0;
             index < this.Count;
             index++)
        {
            if (comparer.Compare(x: item,
                                 y: m_Items[index]) < 0)
            {
                m_Items.Insert(index: index,
                               item: item);
                return true;
            }
            else if (comparer.Compare(x: item,
                                      y: m_Items[index]) == 0)
            {
                shouldInsert = true;
            }
            else if (comparer.Compare(x: item,
                                      y: m_Items[index]) > 0)
            {
                if (shouldInsert)
                {
                    m_Items.Insert(index: index - 1,
                                   item: item);
                    return true;
                }
            }
        }

        m_Items.Add(item);
        return true;
    }

    /// <inheritdoc/>
    public void AddRange<TEnumerable>(
#if NETCOREAPP3_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        [DisallowNull]
#endif
        NotNull<TEnumerable> collection)
            where TEnumerable : IEnumerable<TElement>
    {
        TEnumerable source = collection;
        foreach (TElement item in source)
        {
            this.Add(item!);
        }
    }

    /// <inheritdoc/>
    public void Clear()
    {
        m_Items.Clear();
    }

    /// <inheritdoc/>
    public Boolean Remove(
#if NETCOREAPP3_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        [DisallowNull]
#endif
        NotNull<TElement> item)
    {
        return m_Items.Remove(item);
    }
}