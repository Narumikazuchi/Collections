namespace Narumikazuchi.Collections;

public partial class ObservableList<TElement> : IModifyableCollection<TElement, CommonListEnumerator<TElement>>
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

        ((INotifyPropertyChangingHelper)this).OnPropertyChanging(nameof(this.Count));
        m_Items.Add(item);
        NotifyCollectionChangedEventArgs eventArgs = new(action: NotifyCollectionChangedAction.Add,
                                                         changedItem: item);
        ((INotifyCollectionChangedHelper)this).OnCollectionChanged(eventArgs);
        ((INotifyPropertyChangedHelper)this).OnPropertyChanged(nameof(this.Count));
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

        ((INotifyPropertyChangingHelper)this).OnPropertyChanging(nameof(this.Count));
        if (collection is IHasCount counted)
        {
            TElement[] changed = new TElement[counted.Count];
            Int32 index = 0;
            foreach (TElement item in collection)
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
            foreach (TElement item in collection)
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