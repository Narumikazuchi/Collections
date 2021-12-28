namespace Narumikazuchi.Collections;

/// <summary>
/// Represents a strongly typed list of objects, which reports changes, can be accessed by index and searched. 
/// </summary>
public partial class ObservableList<TElement> : CollectionBase<Int32, TElement>, IObservableList<TElement?>
{
    /// <summary>
    /// Initializes a new empty instance of the <see cref="ObservableList{T}"/> class.
    /// </summary>
    public ObservableList() : 
        base() 
    { }
    /// <summary>
    /// Initializes a new instance of the <see cref="ObservableList{T}"/> class having the specified collection of items and the specified capacity.
    /// </summary>
    /// <param name="collection">The initial collection of items in this list.</param>
    /// <exception cref="ArgumentException" />
    /// <exception cref="ArgumentNullException" />
    /// <exception cref="InvalidOperationException" />
    public ObservableList([DisallowNull] IEnumerable<TElement?> collection) : 
        base() 
    { 
        foreach (TElement? item in collection)
        {
            this.Insert(index: this.Count,
                        item: item);
        }
    }
}

// IContentClearable
partial class ObservableList<TElement> : IContentClearable
{
    /// <inheritdoc />
    public override void Clear()
    {
        this.OnPropertyChanging(nameof(this.Count));
        base.Clear();
        this.OnPropertyChanged(nameof(this.Count));
        this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(action: NotifyCollectionChangedAction.Reset));
    }
}

// IContentInsertable<T, U>
partial class ObservableList<TElement> : IContentInsertable<Int32, TElement?>
{
    /// <inheritdoc />
    public override void Insert(in Int32 index,
                                TElement? item)
    {
        this.OnPropertyChanging(nameof(this.Count));
        base.Insert(index: index, 
                    item: item);
        this.OnPropertyChanged(nameof(this.Count));
        this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(action: NotifyCollectionChangedAction.Add,
                                                                      changedItem: item));
    }
}

// IContentIndexRemovable<T>
partial class ObservableList<TElement> : IContentIndexRemovable<Int32>
{
    /// <inheritdoc />
    public override void RemoveAt(in Int32 index)
    {
        if (this.IsReadOnly)
        {
            throw new NotAllowed(auxMessage: COLLECTION_IS_READONLY);
        }

        this.OnPropertyChanging(nameof(this.Count));
        TElement? item = this[index];
        base.RemoveAt(index);
        this.OnPropertyChanged(nameof(this.Count));
        this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(action: NotifyCollectionChangedAction.Remove,
                                                                      changedItem: item));
    }
}

// IReadOnlyList<T>
partial class ObservableList<TElement> : IReadOnlyList<TElement?>
{
    /// <inheritdoc />
    [MaybeNull]
    public override TElement? this[Int32 index]
    {
        get => base[index];
        set
        {
            if ((UInt32)index >= (UInt32)this.Count)
            {
                throw new IndexOutOfRangeException();
            }
            this.Insert(index: index,
                        item: value);
        }
    }
}

// INotifyCollectionChanged
partial class ObservableList<TElement> : INotifyCollectionChanged
{
    /// <inheritdoc />
    public event NotifyCollectionChangedEventHandler? CollectionChanged;

    /// <summary>
    /// Raises the <see cref="CollectionChanged"/> event with the specified event args.
    /// </summary>
    protected void OnCollectionChanged(NotifyCollectionChangedEventArgs e) =>
        this.CollectionChanged?.Invoke(sender: this, 
                                       e: e);
}

// INotifyPropertyChanging
partial class ObservableList<TElement> : INotifyPropertyChanging
{
    /// <inheritdoc />
    public event PropertyChangingEventHandler? PropertyChanging;

    /// <summary>
    /// Raises the <see cref="PropertyChanging"/> event with the specified event args.
    /// </summary>
    protected void OnPropertyChanging(String propertyName) =>
        this.PropertyChanging?.Invoke(sender: this, 
                                      e: new PropertyChangingEventArgs(propertyName));
}

// INotifyPropertyChanged
partial class ObservableList<TElement> : INotifyPropertyChanged
{
    /// <inheritdoc />
    public event PropertyChangedEventHandler? PropertyChanged;

    /// <summary>
    /// Raises the <see cref="PropertyChanged"/> event with the specified event args.
    /// </summary>
    protected void OnPropertyChanged(String propertyName) =>
        this.PropertyChanged?.Invoke(sender: this, 
                                     e: new PropertyChangedEventArgs(propertyName));
}