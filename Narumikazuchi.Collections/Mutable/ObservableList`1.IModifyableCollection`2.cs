namespace Narumikazuchi.Collections;

public partial class ObservableList<TElement> : IModifyableCollection<TElement, CommonListEnumerator<TElement>>
{
    /// <inheritdoc/>
    public Boolean Add(
#if NETCOREAPP3_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        [DisallowNull]
#endif
        NotNull<TElement> item)
    {
        ((INotifyPropertyChangingHelper)this).OnPropertyChanging(nameof(this.Count));
        m_Items.Add(item);
        NotifyCollectionChangedEventArgs eventArgs = new(action: NotifyCollectionChangedAction.Add,
                                                         changedItem: item);
        ((INotifyCollectionChangedHelper)this).OnCollectionChanged(eventArgs);
        ((INotifyPropertyChangedHelper)this).OnPropertyChanged(nameof(this.Count));
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
        ((INotifyPropertyChangingHelper)this).OnPropertyChanging(nameof(this.Count));
        if (source is IHasCount counted)
        {
            TElement[] changed = new TElement[counted.Count];
            Int32 index = 0;
            foreach (TElement item in source)
            {
                changed[index++] = item;
                m_Items.Add(item);
            }

            NotifyCollectionChangedEventArgs eventArgs = new(action: NotifyCollectionChangedAction.Add,
                                                             changedItems: changed);
            ((INotifyCollectionChangedHelper)this).OnCollectionChanged(eventArgs);
            ((INotifyPropertyChangedHelper)this).OnPropertyChanged(nameof(this.Count));
        }
        else
        {
            List<TElement> changed = new();
            foreach (TElement item in source)
            {
                changed.Add(item);
                m_Items.Add(item);
            }

            NotifyCollectionChangedEventArgs eventArgs = new(action: NotifyCollectionChangedAction.Add,
                                                             changedItems: changed);
            ((INotifyCollectionChangedHelper)this).OnCollectionChanged(eventArgs);
            ((INotifyPropertyChangedHelper)this).OnPropertyChanged(nameof(this.Count));
        }
    }

    /// <inheritdoc/>
    public void Clear()
    {
        ((INotifyPropertyChangingHelper)this).OnPropertyChanging(nameof(this.Count));
        m_Items.Clear();
        NotifyCollectionChangedEventArgs eventArgs = new(action: NotifyCollectionChangedAction.Reset);
        ((INotifyCollectionChangedHelper)this).OnCollectionChanged(eventArgs);
        ((INotifyPropertyChangedHelper)this).OnPropertyChanged(nameof(this.Count));
    }

    /// <inheritdoc/>
    public Boolean Remove(
#if NETCOREAPP3_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        [DisallowNull]
#endif
        NotNull<TElement> item)
    {
        ((INotifyPropertyChangingHelper)this).OnPropertyChanging(nameof(this.Count));
        if (!m_Items.Remove(item))
        {
            return false;
        }

        NotifyCollectionChangedEventArgs eventArgs = new(action: NotifyCollectionChangedAction.Remove,
                                                         changedItem: item);
        ((INotifyCollectionChangedHelper)this).OnCollectionChanged(eventArgs);
        ((INotifyPropertyChangedHelper)this).OnPropertyChanged(nameof(this.Count));
        return true;
    }
}