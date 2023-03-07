namespace Narumikazuchi.Collections;

public partial class ObservableList<TElement> : IModifyableCollectionWithIndex<TElement, CommonListEnumerator<TElement>>
{
    /// <inheritdoc />
    public void Insert(Int32 index,
                       [DisallowNull] TElement item)
    {
#if NET6_0_OR_GREATER
        ArgumentNullException.ThrowIfNull(item);
#else
        if (item is null)
        {
            throw new ArgumentNullException(nameof(item));
        }
#endif

        ((INotifyPropertyChangingHelper)this).OnPropertyChanging(nameof(this.Count));
        m_Items.Insert(index: index,
                       item: item);
        ((INotifyPropertyChangedHelper)this).OnPropertyChanged(nameof(this.Count));
        ((INotifyCollectionChangedHelper)this).OnCollectionChanged(new(action: NotifyCollectionChangedAction.Add,
                                                                       changedItem: item));
    }

    /// <inheritdoc />
    public void InsertRange<TEnumerable>(Int32 index, 
                                         [DisallowNull] TEnumerable enumerable)
        where TEnumerable : IEnumerable<TElement>
    {
#if NET6_0_OR_GREATER
        ArgumentNullException.ThrowIfNull(enumerable);
#else
        if (enumerable is null)
        {
            throw new ArgumentNullException(nameof(enumerable));
        }
#endif

        ((INotifyPropertyChangingHelper)this).OnPropertyChanging(nameof(this.Count));
        if (enumerable is IHasCount collection)
        {
            TElement[] changed = new TElement[collection.Count];
            Int32 addIndex = 0;
            foreach (TElement item in enumerable)
            {
                m_Items.Insert(index: index + addIndex,
                               item: item);
                changed[addIndex++] = item;
            }

            ((INotifyPropertyChangedHelper)this).OnPropertyChanged(nameof(this.Count));
            ((INotifyCollectionChangedHelper)this).OnCollectionChanged(new(action: NotifyCollectionChangedAction.Add,
                                                                           changedItems: changed));
        }
        else
        {
            List<TElement> changed = new();
            Int32 i = 0;
            foreach (TElement item in enumerable)
            {
                m_Items.Insert(index: index + i,
                               item: item);
                changed.Add(item);
                i++;
            }
            ((INotifyPropertyChangedHelper)this).OnPropertyChanged(nameof(this.Count));
            ((INotifyCollectionChangedHelper)this).OnCollectionChanged(new(action: NotifyCollectionChangedAction.Add,
                                                                           changedItems: changed));
        }
    }

    /// <inheritdoc />
    public Boolean RemoveAt(Int32 index)
    {
        if (index < 0 ||
            index >= this.Count)
        {
            return false;
        }

        ((INotifyPropertyChangingHelper)this).OnPropertyChanging(nameof(this.Count));
        TElement item = this[index];
        m_Items.RemoveAt(index);
        ((INotifyPropertyChangedHelper)this).OnPropertyChanged(nameof(this.Count));
        ((INotifyCollectionChangedHelper)this).OnCollectionChanged(new(action: NotifyCollectionChangedAction.Remove,
                                                                       changedItem: item));
        return true;
    }
}