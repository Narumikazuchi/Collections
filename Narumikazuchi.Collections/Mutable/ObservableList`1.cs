﻿namespace Narumikazuchi.Collections;

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
    public static new ObservableList<TElement> CreateFrom<TEnumerable>(
#if NETCOREAPP3_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        [DisallowNull]
#endif
        TEnumerable items)
            where TEnumerable : IEnumerable<TElement>
    {
#if NET6_0_OR_GREATER
        ArgumentNullException.ThrowIfNull(items);
#else
        if (items == null)
        {
            throw new ArgumentNullException(nameof(items));
        }
#endif

        return new(new List<TElement>(items));
    }
    /// <summary>
    /// Initializes a new instance of the <see cref="ObservableList{TElement}"/> class.
    /// </summary>
    /// <param name="items">The items that the resulting collection shall hold.</param>
    /// <exception cref="ArgumentNullException" />
    public static new ObservableList<TElement> CreateFrom<TEnumerable, TEnumerator>(
#if NETCOREAPP3_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        [DisallowNull]
#endif
        TEnumerable items)
            where TEnumerator : struct, IStrongEnumerator<TElement>
            where TEnumerable : IStrongEnumerable<TElement, TEnumerator>
    {
#if NET6_0_OR_GREATER
        ArgumentNullException.ThrowIfNull(items);
#else
        if (items == null)
        {
            throw new ArgumentNullException(nameof(items));
        }
#endif

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
#if NETCOREAPP3_1_OR_GREATER
    /// <inheritdoc />
    public TElement this[Index index]
    {
        get => m_Items[index];
        set => m_Items[index] = value;
    }
    /// <inheritdoc />
    public ImmutableArray<TElement> this[Range range]
    {
        get
        {
            if ((range.Start.IsFromEnd &&
                range.Start.Value > m_Items.Count - 1) ||
                (!range.Start.IsFromEnd &&
                range.Start.Value < 0))
            {
                throw new ArgumentOutOfRangeException(paramName: nameof(range));
            }
            if ((range.End.IsFromEnd &&
                range.End.Value > m_Items.Count - 1) ||
                (!range.End.IsFromEnd &&
                range.End.Value < 0))
            {
                throw new ArgumentOutOfRangeException(paramName: nameof(range));
            }

            (Int32 offset, Int32 length) = range.GetOffsetAndLength(m_Items.Count);
            return m_Items.GetRange(index: offset,
                                    count: length)
                          .ToImmutableArray();
        }
    }
#endif
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
    public void InsertRange<TEnumerable, TEnumerator>(Int32 index,
#if NETCOREAPP3_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        [DisallowNull]
#endif
        TEnumerable enumerable)
            where TEnumerator : struct, IStrongEnumerator<TElement>
            where TEnumerable : IStrongEnumerable<TElement, TEnumerator>
    {
#if NET6_0_OR_GREATER
        ArgumentNullException.ThrowIfNull(enumerable);
#else
        if (enumerable == null)
        {
            throw new ArgumentNullException(nameof(enumerable));
        }
#endif

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