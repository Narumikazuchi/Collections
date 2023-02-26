namespace Narumikazuchi.Collections;

public partial class ObservableList<TElement> : IModifyableCollectionWithIndex<TElement, CommonListEnumerator<TElement>>
{
    /// <inheritdoc />
    public void Insert(
        Int32 index,
#if NETCOREAPP3_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        [DisallowNull]
#endif
        NotNull<TElement> item)
    {
        ((INotifyPropertyChangingHelper)this).OnPropertyChanging(nameof(this.Count));
        m_Items.Insert(index: index,
                       item: item);
        ((INotifyPropertyChangedHelper)this).OnPropertyChanged(nameof(this.Count));
        ((INotifyCollectionChangedHelper)this).OnCollectionChanged(new NotifyCollectionChangedEventArgs(action: NotifyCollectionChangedAction.Add,
                                                                                                        changedItem: item));
    }

    /// <inheritdoc />
    public void InsertRange<TEnumerable>(
        Int32 index,
#if NETCOREAPP3_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        [DisallowNull]
#endif
        NotNull<TEnumerable> enumerable)
            where TEnumerable : IEnumerable<TElement>
    {
        TEnumerable source = enumerable;
        ((INotifyPropertyChangingHelper)this).OnPropertyChanging(nameof(this.Count));
        if (source is IHasCount collection)
        {
            TElement[] changed = new TElement[collection.Count];
            Int32 addIndex = 0;
            foreach (TElement item in source)
            {
                m_Items.Insert(index: index + addIndex,
                               item: item);
                changed[addIndex++] = item;
            }

            ((INotifyPropertyChangedHelper)this).OnPropertyChanged(nameof(this.Count));
            ((INotifyCollectionChangedHelper)this).OnCollectionChanged(new NotifyCollectionChangedEventArgs(action: NotifyCollectionChangedAction.Add,
                                                                                                            changedItems: changed));
        }
        else
        {
            List<TElement> changed = new();
            Int32 i = 0;
            foreach (TElement item in source)
            {
                m_Items.Insert(index: index + i,
                               item: item);
                changed.Add(item);
                i++;
            }
            ((INotifyPropertyChangedHelper)this).OnPropertyChanged(nameof(this.Count));
            ((INotifyCollectionChangedHelper)this).OnCollectionChanged(new NotifyCollectionChangedEventArgs(action: NotifyCollectionChangedAction.Add,
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
        ((INotifyCollectionChangedHelper)this).OnCollectionChanged(new NotifyCollectionChangedEventArgs(action: NotifyCollectionChangedAction.Remove,
                                                                                                        changedItem: item));
        return true;
    }
}