namespace Narumikazuchi.Collections;

/// <summary>
/// Represents a strongly typed collection of objects of type <typeparamref name="TElement"/>, which reports changes.
/// </summary>
public partial class ObservableCollection<TElement>
{
    /// <summary>
    /// Copies the entire <see cref="ObservableCollection{TElement}"/> to a compatible one-dimensional array, starting at the beginning of the target array.
    /// </summary>
    /// <param name="destination">The one-dimensional <see cref="Array"/> that is the destination of the elements copied from <see cref="ObservableCollection{TElement}"/>.
    /// The <see cref="Array"/> must have zero-based indexing.</param>
    /// <exception cref="ArgumentException" />
    /// <exception cref="ArgumentNullException" />
    public void CopyTo([DisallowNull] TElement[] destination)
    {
        ArgumentNullException.ThrowIfNull(destination);

        m_Items.CopyTo(destination);
    }
    /// <summary>
    /// Copies the entire <see cref="ObservableCollection{TElement}"/> to a compatible one-dimensional array, starting at the specified index of the target array.
    /// </summary>
    /// <param name="destination">The one-dimensional <see cref="Array"/> that is the destination of the elements copied from <see cref="ObservableCollection{TElement}"/>.
    /// The <see cref="Array"/> must have zero-based indexing.</param>
    /// <param name="destinationIndex">The zero-based index in array at which copying begins.</param>
    /// <exception cref="ArgumentException" />
    /// <exception cref="ArgumentNullException" />
    /// <exception cref="ArgumentOutOfRangeException" />
    public void CopyTo([DisallowNull] TElement[] destination,
                       in Int32 destinationIndex)
    {
        ArgumentNullException.ThrowIfNull(destination);

        m_Items.CopyTo(array: destination,
                       arrayIndex: destinationIndex);
    }

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
        TEnumerator enumerator = items.GetEnumerator();
        while (enumerator.MoveNext())
        {
            result.Add(enumerator.Current);
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

// ICollection
partial class ObservableCollection<TElement> : ICollection
{
    Boolean ICollection.IsSynchronized =>
        ((ICollection)m_Items).IsSynchronized;

    Object ICollection.SyncRoot =>
        ((ICollection)m_Items).SyncRoot;

    void ICollection.CopyTo(Array array,
                            Int32 index) =>
        ((ICollection)m_Items).CopyTo(array: array,
                                      index: index);
}

// ICollection
partial class ObservableCollection<TElement> : ICollection<TElement>
{
    /// <inheritdoc/>
    public void Add(TElement item)
    {
        ((INotifyPropertyChangingHelper)this).OnPropertyChanging(nameof(this.Count));
        m_Items.Add(item);
        NotifyCollectionChangedEventArgs eventArgs = new(action: NotifyCollectionChangedAction.Add,
                                                         changedItem: item);
        ((INotifyCollectionChangedHelper)this).OnCollectionChanged(eventArgs);
        ((INotifyPropertyChangedHelper)this).OnPropertyChanged(nameof(this.Count));
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
    public Boolean Contains(TElement item) =>
        m_Items.Contains(item);

    /// <inheritdoc/>
    public void CopyTo(TElement[] array,
                       Int32 arrayIndex) =>
        m_Items.CopyTo(array: array,
                       arrayIndex: arrayIndex);

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

    /// <inheritdoc/>
    public Boolean IsReadOnly { get; } = false;
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

        this.CollectionChanged?
            .Invoke(sender: this,
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

        this.PropertyChanging?
            .Invoke(sender: this,
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

        this.PropertyChanged?
            .Invoke(sender: this,
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

// IReadOnlyCollection<T>
partial class ObservableCollection<TElement> : IReadOnlyCollection<TElement>
{
    /// <inheritdoc/>
    public Int32 Count =>
        m_Items.Count;
}