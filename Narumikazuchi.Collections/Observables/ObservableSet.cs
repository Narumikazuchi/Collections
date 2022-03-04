namespace Narumikazuchi.Collections;

/// <summary>
/// Represents a collection which reports changes and contains every object is only once. The procedure to check whether the object is already in the <see cref="ObservableSet{TElement}"/> can be specified
/// with a corresponding <see cref="IEqualityComparer{T}"/> or <see cref="EqualityComparison{T}"/> delegate.
/// </summary>
/// <remarks>
/// If neither <see cref="IEqualityComparer{T}"/> nor <see cref="EqualityComparison{T}"/> are specified, the register will compare the references for classes or check each field/property for values types.
/// </remarks>
public partial class ObservableSet<TElement> : SetBase<Int32, TElement?>, IObservableSet<TElement?>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ObservableSet{TElement}"/> class.
    /// </summary>
    public ObservableSet() : 
        base() 
    { }
    /// <summary>
    /// Initializes a new instance of the <see cref="ObservableSet{TElement}"/> class using the specified function to check items for equality.
    /// </summary>
    public ObservableSet([DisallowNull] EqualityComparison<TElement?> comparison!!) : 
        base(collection: Array.Empty<(Int32, TElement?)>(), 
             comparison: comparison) 
    { }
    /// <summary>
    /// Initializes a new instance of the <see cref="ObservableSet{TElement}"/> class with the specified collection as items.
    /// </summary>
    public ObservableSet([DisallowNull] IEnumerable<TElement?> collection!!) : 
        base() 
    {
        this.Comparer = __FuncEqualityComparer<TElement?>.Default;
        foreach (TElement? item in collection)
        {
            this.Add(item);
        }
    }
    /// <summary>
    /// Initializes a new instance of the <see cref="ObservableSet{TElement}"/> class with the specified collection as items using the specified function to check items for equality.
    /// </summary>
    public ObservableSet([DisallowNull] EqualityComparison<TElement?> comparison!!, 
                         [DisallowNull] IEnumerable<TElement?> collection!!) : 
        this(comparer: new __FuncEqualityComparer<TElement?>(comparison: comparison),
             collection: collection)
    { }
    /// <summary>
    /// Initializes a new instance of the <see cref="ObservableSet{TElement}"/> class using the specified <see cref="IEqualityComparer{T}"/> to check items for equality.
    /// </summary>
    public ObservableSet([AllowNull] IEqualityComparer<TElement?>? comparer) :
        base()
    {
        this.Comparer = comparer;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ObservableSet{TElement}"/> class with the specified collection as items using the specified <see cref="IEqualityComparer{T}"/> to check items for equality.
    /// </summary>
    public ObservableSet([AllowNull] IEqualityComparer<TElement?>? comparer, 
                         [DisallowNull] IEnumerable<TElement?> collection!!) : 
        base()
    {
        this.Comparer = comparer;
        foreach (TElement? item in collection)
        {
            this.Add(item);
        }
    }
}

// IContentAddable<T>
partial class ObservableSet<TElement> : IContentAddable<TElement?>
{
    /// <inheritdoc/>
    public override Boolean Add(TElement? item)
    {
        this.OnPropertyChanging(nameof(this.Count));
        if (!base.Add(item))
        {
            return false;
        }

        this.OnPropertyChanged(nameof(this.Count));
        this.OnCollectionChanged(new(action: NotifyCollectionChangedAction.Add,
                                     changedItem: item));
        return true;
    }
}

// IContentClearable
partial class ObservableSet<TElement> : IContentClearable
{
    /// <inheritdoc />
    public override void Clear()
    {
        this.OnPropertyChanging(nameof(this.Count));
        base.Clear();
        this.OnPropertyChanged(nameof(this.Count));
        this.OnCollectionChanged(new(action: NotifyCollectionChangedAction.Reset));
    }
}

// INotifyCollectionChanged
partial class ObservableSet<TElement> : INotifyCollectionChanged
{
    /// <inheritdoc />
    public event NotifyCollectionChangedEventHandler? CollectionChanged;

    /// <summary>
    /// Raises the <see cref="CollectionChanged"/> event with the specified event args.
    /// </summary>
    protected void OnCollectionChanged(NotifyCollectionChangedEventArgs eventArgs!!) =>
        this.CollectionChanged?
            .Invoke(sender: this, 
                    e: eventArgs);
}

// INotifyPropertyChanging
partial class ObservableSet<TElement> : INotifyPropertyChanging
{
    /// <inheritdoc />
    public event PropertyChangingEventHandler? PropertyChanging;

    /// <summary>
    /// Raises the <see cref="PropertyChanging"/> event with the specified event args.
    /// </summary>
    protected void OnPropertyChanging(String propertyName!!) =>
        this.PropertyChanging?
            .Invoke(sender: this, 
                    e: new(propertyName));
}

// INotifyPropertyChanged
partial class ObservableSet<TElement> : INotifyPropertyChanged
{
    /// <inheritdoc />
    public event PropertyChangedEventHandler? PropertyChanged;

    /// <summary>
    /// Raises the <see cref="PropertyChanged"/> event with the specified event args.
    /// </summary>
    protected void OnPropertyChanged(String propertyName!!) =>
        this.PropertyChanged?
            .Invoke(sender: this, 
                    e: new(propertyName));
}