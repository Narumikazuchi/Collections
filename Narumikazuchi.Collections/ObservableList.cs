namespace Narumikazuchi.Collections;

#pragma warning disable CS8767
/// <summary>
/// Represents a strongly typed list of objects, which reports changes, can be accessed by index and searched. 
/// </summary>
public partial class ObservableList<TElement>
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
    /// <exception cref="ArgumentNullException" />
    public ObservableList([DisallowNull] IEnumerable<TElement> collection) : 
        base() 
    {
        ArgumentNullException.ThrowIfNull(collection);

        m_Items.AddRange(collection.Where(x => x is not null));
    }

    /// <summary>
    /// Copies the entire <see cref="ObservableList{TElement}"/> to a compatible one-dimensional array, starting at the beginning of the target array.
    /// </summary>
    /// <param name="destination">The one-dimensional <see cref="Array"/> that is the destination of the elements copied from <see cref="ObservableList{TElement}"/>.
    /// The <see cref="Array"/> must have zero-based indexing.</param>
    /// <exception cref="ArgumentException" />
    /// <exception cref="ArgumentNullException" />
    public void CopyTo([DisallowNull] TElement[] destination)
    {
        ArgumentNullException.ThrowIfNull(destination);

        m_Items.CopyTo(destination);
    }
    /// <summary>
    /// Copies the entire <see cref="ObservableList{TElement}"/> to a compatible one-dimensional array, starting at the specified index of the target array.
    /// </summary>
    /// <param name="destination">The one-dimensional <see cref="Array"/> that is the destination of the elements copied from <see cref="ObservableList{TElement}"/>.
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
    /// Copies the entire <see cref="ObservableList{TElement}"/> to a compatible one-dimensional array, starting at the specified index of the target array.
    /// </summary>
    /// <param name="startIndex">The zero-based index in the source <see cref="ObservableList{TElement}"/> at which copying begins.</param>
    /// <param name="destination">The one-dimensional <see cref="Array"/> that is the destination of the elements copied from <see cref="ObservableList{TElement}"/>.
    /// The <see cref="Array"/> must have zero-based indexing.</param>
    /// <param name="destinationIndex">The zero-based index in array at which copying begins.</param>
    /// <param name="count">The number of elements to copy.</param>
    /// <exception cref="ArgumentException" />
    /// <exception cref="ArgumentNullException" />
    /// <exception cref="ArgumentOutOfRangeException" />
    public void CopyTo(in Int32 startIndex,
                       [DisallowNull] TElement[] destination,
                       in Int32 destinationIndex,
                       in Int32 count)
    {
        ArgumentNullException.ThrowIfNull(destination);

        m_Items.CopyTo(index: startIndex,
                       array: destination,
                       arrayIndex: destinationIndex,
                       count: count);
    }
}

// Non-Public
partial class ObservableList<TElement>
{
    private readonly List<TElement> m_Items = new();
}

// ICollection
partial class ObservableList<TElement> : ICollection
{
    /// <inheritdoc />
    public Boolean IsSynchronized => 
        ((ICollection)m_Items).IsSynchronized;

    /// <inheritdoc />
    public Object SyncRoot => 
        ((ICollection)m_Items).SyncRoot;

    /// <inheritdoc />
    public void CopyTo([DisallowNull] Array array,
                       Int32 index)
    {
        ArgumentNullException.ThrowIfNull(array);

        ((ICollection)m_Items).CopyTo(array: array,
                                      index: index);
    }
}

// ICollection<T>
partial class ObservableList<TElement> : ICollection<TElement>
{
    /// <inheritdoc />
    public void Add([DisallowNull] TElement item)
    {
        ArgumentNullException.ThrowIfNull(item);

        ((INotifyPropertyChangingHelper)this).OnPropertyChanging(nameof(this.Count));
        m_Items.Add(item);
        ((INotifyPropertyChangedHelper)this).OnPropertyChanged(nameof(this.Count));
        ((INotifyCollectionChangedHelper)this).OnCollectionChanged(new(action: NotifyCollectionChangedAction.Add,
                                                                       changedItem: item));
    }

    /// <inheritdoc />
    public void Clear()
    {
        ((INotifyPropertyChangingHelper)this).OnPropertyChanging(nameof(this.Count));
        m_Items.Clear();
        ((INotifyPropertyChangedHelper)this).OnPropertyChanged(nameof(this.Count));
        ((INotifyCollectionChangedHelper)this).OnCollectionChanged(new(action: NotifyCollectionChangedAction.Reset));
    }

    /// <inheritdoc />
    public Boolean Contains([DisallowNull] TElement item)
    {
        ArgumentNullException.ThrowIfNull(item);

        return m_Items.Contains(item);
    }

    /// <inheritdoc />
    public void CopyTo([DisallowNull] TElement[] array,
                       Int32 arrayIndex)
    {
        ArgumentNullException.ThrowIfNull(array);

        m_Items.CopyTo(array: array,
                       arrayIndex: arrayIndex);
    }

    /// <inheritdoc />
    public Boolean Remove([DisallowNull] TElement item)
    {
        ArgumentNullException.ThrowIfNull(item);

        ((INotifyPropertyChangingHelper)this).OnPropertyChanging(nameof(this.Count));
        if (!m_Items.Remove(item))
        {
            return false;
        }
        ((INotifyPropertyChangedHelper)this).OnPropertyChanged(nameof(this.Count));
        ((INotifyCollectionChangedHelper)this).OnCollectionChanged(new(action: NotifyCollectionChangedAction.Remove,
                                                                       changedItem: item));
        return true;
    }

    /// <inheritdoc />
    public Boolean IsReadOnly =>
        false;
}

// IEnumerable
partial class ObservableList<TElement> : IEnumerable
{
    IEnumerator IEnumerable.GetEnumerator() =>
        m_Items.GetEnumerator();
}

// IEnumerable<T>
partial class ObservableList<TElement> : IEnumerable<TElement>
{
    /// <inheritdoc />
    public IEnumerator<TElement> GetEnumerator() =>
        m_Items.GetEnumerator();
}

// IList
partial class ObservableList<TElement> : IList
{
    Int32 IList.Add(Object? value)
    {
        if (value is TElement element)
        {
            Int32 index = this.Count;
            this.Add(element);
            return index;
        }
        return -1;
    }

    void IList.Clear() =>
        this.Clear();

    Boolean IList.Contains(Object? value) =>
        value is TElement element &&
        this.Contains(element);

    void ICollection.CopyTo(Array array,
                            Int32 index) =>
        this.CopyTo(array: array,
                    index: index);

    Int32 IList.IndexOf(Object? value)
    {
        if (value is TElement element)
        {
            return this.IndexOf(element);
        }
        return -1;
    }

    void IList.Insert(Int32 index,
                      Object? value)
    {
        if (value is TElement element)
        {
            this.Insert(index: index,
                        item: element);
        }
    }

    void IList.Remove(Object? value)
    {
        if (value is TElement element)
        {
            this.Remove(element);
        }
    }

    void IList.RemoveAt(Int32 index) =>
        this.RemoveAt(index);

    Object? IList.this[Int32 index]
    {
        get => this[index];
        set
        {
            if (value is TElement element)
            {
                this[index] = element;
            }
            throw new InvalidCastException();
        }
    }

    Boolean IList.IsFixedSize =>
        false;
}

// IList<T>
partial class ObservableList<TElement> : IList<TElement>
{
    /// <inheritdoc />
    public Int32 IndexOf([DisallowNull] TElement item)
    {
        ArgumentNullException.ThrowIfNull(item);

        return m_Items.IndexOf(item);
    }

    /// <inheritdoc />
    public void Insert(Int32 index,
                       [DisallowNull] TElement item)
    {
        ((INotifyPropertyChangingHelper)this).OnPropertyChanging(nameof(this.Count));
        m_Items.Insert(index: index,
                       item: item);
        ((INotifyPropertyChangedHelper)this).OnPropertyChanged(nameof(this.Count));
        ((INotifyCollectionChangedHelper)this).OnCollectionChanged(new(action: NotifyCollectionChangedAction.Add,
                                                                       changedItem: item));
    }

    /// <inheritdoc />
    public void RemoveAt(Int32 index)
    {
        ((INotifyPropertyChangingHelper)this).OnPropertyChanging(nameof(this.Count));
        TElement item = this[index];
        m_Items.RemoveAt(index);
        ((INotifyPropertyChangedHelper)this).OnPropertyChanged(nameof(this.Count));
        ((INotifyCollectionChangedHelper)this).OnCollectionChanged(new(action: NotifyCollectionChangedAction.Remove,
                                                                       changedItem: item));
    }
}

// IReadOnlyCollection<T>
partial class ObservableList<TElement> : IReadOnlyCollection<TElement>
{
    /// <inheritdoc />
    public Int32 Count =>
        m_Items.Count;
}

// IReadOnlyList<T>
partial class ObservableList<TElement> : IReadOnlyList<TElement>
{
    /// <inheritdoc />
    [NotNull]
    public TElement this[Int32 index]
    {
        get => m_Items[index]!;
        set
        {
            ArgumentNullException.ThrowIfNull(value);
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
}

// INotifyCollectionChangedHelper
partial class ObservableList<TElement> : INotifyCollectionChangedHelper
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
partial class ObservableList<TElement> : INotifyPropertyChanging
{
    /// <inheritdoc />
    public event PropertyChangingEventHandler? PropertyChanging;
}

// INotifyPropertyChangingHelper
partial class ObservableList<TElement> : INotifyPropertyChangingHelper
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
partial class ObservableList<TElement> : INotifyPropertyChanged
{
    /// <inheritdoc />
    public event PropertyChangedEventHandler? PropertyChanged;
}

// INotifyPropertyChangedHelper
partial class ObservableList<TElement> : INotifyPropertyChangedHelper
{
    void INotifyPropertyChangedHelper.OnPropertyChanged(String propertyName!!)
    {
        ArgumentNullException.ThrowIfNull(propertyName);

        this.PropertyChanged?
            .Invoke(sender: this,
                    e: new(propertyName));
    }
}