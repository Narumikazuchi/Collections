namespace Narumikazuchi.Collections;

/// <summary>
/// Represents a strongly typed collection of objects of type <typeparamref name="TElement"/>, which reports changes.
/// </summary>
public partial class ObservableCollection<TElement>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ObservableCollection{TElement}"/> class.
    /// </summary>
    public static ObservableCollection<TElement> Create() => 
        new();
    /// <summary>
    /// Initializes a new instance of the <see cref="ObservableCollection{TElement}"/> class.
    /// </summary>
    /// <param name="items">The items that the resulting collection shall hold.</param>
    /// <exception cref="ArgumentNullException" />
    public static ObservableCollection<TElement> CreateFrom([DisallowNull] IEnumerable<TElement> items)
    {
        ArgumentNullException.ThrowIfNull(items);
        return new(new List<TElement>(items));
    }
    /// <summary>
    /// Initializes a new instance of the <see cref="ObservableCollection{TElement}"/> class.
    /// </summary>
    /// <param name="items">The items that the resulting collection shall hold.</param>
    /// <exception cref="ArgumentNullException" />
    public static ObservableCollection<TElement> CreateFrom<TEnumerator>([DisallowNull] IStrongEnumerable<TElement, TEnumerator> items)
        where TEnumerator : struct, IStrongEnumerator<TElement>
    {
        ArgumentNullException.ThrowIfNull(items);

        ObservableCollection<TElement> result = new();
        foreach (TElement item in items)
        {
            result.Add(item);
        }
        return result;
    }
}

// Non-Public
partial class ObservableCollection<TElement>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ObservableCollection{TElement}"/> class.
    /// </summary>
    protected ObservableCollection()
    {
        m_Items = new();
    }
    /// <summary>
    /// Initializes a new instance of the <see cref="ObservableCollection{TElement}"/> class.
    /// </summary>
    /// <param name="items">The items that the resulting collection shall hold.</param>
    /// <exception cref="ArgumentNullException" />
    protected ObservableCollection(List<TElement> items)
    {
        ArgumentNullException.ThrowIfNull(items);

        m_Items = items;
    }

    internal readonly List<TElement> m_Items;
}

// ICollectionWithCount<T, U>
partial class ObservableCollection<TElement> : ICollectionWithCount<TElement, CommonListEnumerator<TElement>>
{
    /// <inheritdoc/>
    public Int32 Count => 
        m_Items.Count;
}

// IEnumerable
partial class ObservableCollection<TElement> : IEnumerable
{
    IEnumerator IEnumerable.GetEnumerator() =>
        this.GetEnumerator();
}

// IEnumerable<T>
partial class ObservableCollection<TElement> : IEnumerable<TElement>
{
    IEnumerator<TElement> IEnumerable<TElement>.GetEnumerator() =>
        this.GetEnumerator();
}

// IModifyableCollection<T, U>
partial class ObservableCollection<TElement> : IModifyableCollection<TElement, CommonListEnumerator<TElement>>
{
    /// <inheritdoc/>
    public Boolean Add(TElement item)
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
    public void AddRange<TEnumerator>(IStrongEnumerable<TElement, TEnumerator> collection)
        where TEnumerator : struct, IStrongEnumerator<TElement>
    {
        ((INotifyPropertyChangingHelper)this).OnPropertyChanging(nameof(this.Count));
        if (collection is ICollectionWithCount<TElement, TEnumerator> counted)
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
    public Boolean Remove(TElement item)
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

// INotifyCollectionChanged
partial class ObservableCollection<TElement> : INotifyCollectionChanged
{
    /// <inheritdoc/>
    public event NotifyCollectionChangedEventHandler? CollectionChanged;
}

// INotifyCollectionChangedHelper
partial class ObservableCollection<TElement> : INotifyCollectionChangedHelper
{
    void INotifyCollectionChangedHelper.OnCollectionChanged(NotifyCollectionChangedEventArgs eventArgs)
    {
        ArgumentNullException.ThrowIfNull(eventArgs);

        this.CollectionChanged?.Invoke(sender: this,
                                       e: eventArgs);
    }
}

// INotifyPropertyChanging
partial class ObservableCollection<TElement> : INotifyPropertyChanging
{
    /// <inheritdoc/>
    public event PropertyChangingEventHandler? PropertyChanging;
}

// INotifyPropertyChangingHelper
partial class ObservableCollection<TElement> : INotifyPropertyChangingHelper
{
    void INotifyPropertyChangingHelper.OnPropertyChanging(String propertyName)
    {
        ArgumentNullException.ThrowIfNull(propertyName);

        this.PropertyChanging?.Invoke(sender: this,
                                      e: new(propertyName));
    }
}

// INotifyPropertyChanged
partial class ObservableCollection<TElement> : INotifyPropertyChanged
{
    /// <inheritdoc/>
    public event PropertyChangedEventHandler? PropertyChanged;
}

// INotifyPropertyChangedHelper
partial class ObservableCollection<TElement> : INotifyPropertyChangedHelper
{
    void INotifyPropertyChangedHelper.OnPropertyChanged(String propertyName)
    {
        ArgumentNullException.ThrowIfNull(propertyName);

        this.PropertyChanged?.Invoke(sender: this,
                                     e: new(propertyName));
    }
}

// IStrongEnumerable<T, U>
partial class ObservableCollection<TElement> : IStrongEnumerable<TElement, CommonListEnumerator<TElement>>
{
    /// <inheritdoc/>
    public CommonListEnumerator<TElement> GetEnumerator() =>
        new(m_Items);
}

// IReadOnlyCollection<T, U>
partial class ObservableCollection<TElement> : IReadOnlyCollection<TElement, CommonListEnumerator<TElement>>
{

    /// <inheritdoc/>
    public Boolean Contains(TElement item) =>
        m_Items.Contains(item);

    /// <inheritdoc/>
    public void CopyTo(TElement[] array) =>
        m_Items.CopyTo(array: array);
    /// <inheritdoc/>
    public void CopyTo(TElement[] array,
                       Int32 destinationIndex) =>
        m_Items.CopyTo(array: array,
                       arrayIndex: destinationIndex);
}