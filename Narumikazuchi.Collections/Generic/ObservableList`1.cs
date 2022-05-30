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
        new();
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
        where TEnumerator : struct, IStrongEnumerator<TElement>
    {
        ArgumentNullException.ThrowIfNull(items);

        ObservableList<TElement> result = new();
        foreach (TElement item in items)
        {
            result.Add(item);
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

// ICollectionWithReadWriteIndexer<T, U>
partial class ObservableList<TElement> : ICollectionWithReadWriteIndexer<TElement, CommonListEnumerator<TElement>>
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
    /// <inheritdoc />
    public TElement this[Index index]
    {
        get => m_Items[index];
        set => m_Items[index] = value;
    }
}

// IEnumerable
partial class ObservableList<TElement> : IEnumerable
{
    IEnumerator IEnumerable.GetEnumerator() =>
        this.GetEnumerator();
}

// IEnumerable<T>
partial class ObservableList<TElement> : IEnumerable<TElement>
{
    IEnumerator<TElement> IEnumerable<TElement>.GetEnumerator() =>
        this.GetEnumerator();
}

// IModifyableCollectionWithIndex<T, U>
partial class ObservableList<TElement> : IModifyableCollectionWithIndex<TElement, CommonListEnumerator<TElement>>
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
    public void InsertRange<TEnumerator>(Int32 index,
                                         [DisallowNull] IStrongEnumerable<TElement, TEnumerator> enumerable)
        where TEnumerator : struct, IStrongEnumerator<TElement>
    {
        ((INotifyPropertyChangingHelper)this).OnPropertyChanging(nameof(this.Count));
        if (enumerable is ICollectionWithCount<TElement, TEnumerator> collection)
        {
            TElement[] changed = new TElement[collection.Count];
            if (enumerable is ICollectionWithReadIndexer<TElement, TEnumerator> readIndex)
            {
                for (Int32 i = 0;
                     i < collection.Count;
                     i++)
                {
                    m_Items.Insert(index: index + i,
                                   item: readIndex[i]);
                    changed[i] = readIndex[i];
                }
            }
            else
            {
                Int32 i = 0;
                foreach (TElement item in enumerable)
                {
                    m_Items.Insert(index: index + i,
                                   item: item);
                    changed[i++] = item;
                }
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