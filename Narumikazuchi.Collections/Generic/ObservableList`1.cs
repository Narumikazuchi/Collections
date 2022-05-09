namespace Narumikazuchi.Collections;

/// <summary>
/// Represents a strongly typed list of objects of type <typeparamref name="TElement"/>, which reports changes and can be accessed by index. 
/// </summary>
public partial class ObservableList<TElement> : ObservableCollection<TElement>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ObservableList{TElement}"/> class.
    /// </summary>
    public static new ObservableList<TElement> Create() =>
        new(new List<TElement>());
    /// <summary>
    /// Initializes a new instance of the <see cref="ObservableList{TElement}"/> class.
    /// </summary>
    /// <param name="items">The items that the resulting collection shall hold.</param>
    /// <exception cref="ArgumentNullException" />
    public static new ObservableList<TElement> CreateFrom([DisallowNull] IEnumerable<TElement> items)
    {
        ArgumentNullException.ThrowIfNull(items);
        return new(new List<TElement>(items));
    }
    /// <summary>
    /// Initializes a new instance of the <see cref="ObservableList{TElement}"/> class.
    /// </summary>
    /// <param name="items">The items that the resulting collection shall hold.</param>
    /// <exception cref="ArgumentNullException" />
    public static new ObservableList<TElement> CreateFrom<TEnumerator>([DisallowNull] IStrongEnumerable<TElement, TEnumerator> items)
        where TEnumerator : IEnumerator<TElement>
    {
        ArgumentNullException.ThrowIfNull(items);
        ObservableList<TElement> result = new();
        TEnumerator enumerator = items.GetEnumerator();
        while (enumerator.MoveNext())
        {
            result.Add(enumerator.Current);
        }
        return result;
    }
}

// Non-Public
partial class ObservableList<TElement>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ObservableList{TElement}"/> class.
    /// </summary>
    protected ObservableList() :
        base()
    { }
    /// <summary>
    /// Initializes a new instance of the <see cref="ObservableList{TElement}"/> class.
    /// </summary>
    /// <param name="items">The items that the resulting collection shall hold.</param>
    /// <exception cref="ArgumentNullException" />
    protected ObservableList(List<TElement> items) :
        base(items)
    { }
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
        }
    }

    Boolean IList.IsFixedSize { get; } = false;
}

// IList<T>
partial class ObservableList<TElement> : IList<TElement>
{
    /// <inheritdoc />
    public Int32 IndexOf(TElement item) =>
        m_Items.IndexOf(item);

    /// <inheritdoc />
    public void Insert(Int32 index,
                       TElement item)
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

// IReadOnlyList<T>
partial class ObservableList<TElement> : IReadOnlyList<TElement>
{
    /// <inheritdoc />
    public TElement this[Int32 index]
    {
        get => m_Items[index]!;
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