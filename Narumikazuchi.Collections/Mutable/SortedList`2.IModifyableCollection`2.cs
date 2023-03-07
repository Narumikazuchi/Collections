namespace Narumikazuchi.Collections;

public partial class SortedList<TElement, TComparer> : IModifyableCollection<TElement, CommonListEnumerator<TElement>>
{
    /// <inheritdoc/>
    public Boolean Add([DisallowNull] TElement item)
    {
#if NET6_0_OR_GREATER
        ArgumentNullException.ThrowIfNull(item);
#else
        if (item is null)
        {
            throw new ArgumentNullException(nameof(item));
        }
#endif

        Boolean shouldInsert = false;
        for (Int32 index = 0;
             index < this.Count;
             index++)
        {
            if (this.Comparer.Compare(x: item,
                                      y: m_Items[index]) < 0)
            {
                m_Items.Insert(index: index,
                               item: item);
                return true;
            }
            else if (this.Comparer.Compare(x: item,
                                           y: m_Items[index]) == 0)
            {
                shouldInsert = true;
            }
            else if (this.Comparer.Compare(x: item,
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
    public void AddRange<TEnumerable>([DisallowNull] TEnumerable collection)
        where TEnumerable : IEnumerable<TElement>
    {
#if NET6_0_OR_GREATER
        ArgumentNullException.ThrowIfNull(collection);
#else
        if (collection is null)
        {
            throw new ArgumentNullException(nameof(collection));
        }
#endif

        foreach (TElement item in collection)
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
    public Boolean Remove([DisallowNull] TElement item)
    {
#if NET6_0_OR_GREATER
        ArgumentNullException.ThrowIfNull(item);
#else
        if (item is null)
        {
            throw new ArgumentNullException(nameof(item));
        }
#endif

        return m_Items.Remove(item);
    }
}