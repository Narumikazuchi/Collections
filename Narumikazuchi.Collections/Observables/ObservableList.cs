using Narumikazuchi.Collections.Abstract;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using System.Linq;

namespace Narumikazuchi.Collections
{
    /// <summary>
    /// Represents a strongly typed list of objects, which reports changes, can be accessed by index, searched and sorted. 
    /// </summary>
    public partial class ObservableList<TElement> : SearchableListBase<TElement>
    {
        /// <summary>
        /// Initializes a new empty instance of the <see cref="ObservableList{T}"/> class.
        /// </summary>
        public ObservableList() : 
            base() 
        { }
        /// <summary>
        /// Initializes a new empty instance of the <see cref="ObservableList{T}"/> class having the specified capacity.
        /// </summary>
        /// <param name="capacity">The capacity of this list.</param>
        /// <exception cref="ArgumentOutOfRangeException" />
        public ObservableList(in Int32 capacity) : 
            base(capacity) 
        { }
        /// <summary>
        /// Initializes a new instance of the <see cref="ObservableList{T}"/> class having the specified collection of items and the specified capacity.
        /// </summary>
        /// <param name="collection">The initial collection of items in this list.</param>
        /// <exception cref="ArgumentException" />
        /// <exception cref="ArgumentNullException" />
        /// <exception cref="InvalidOperationException" />
        public ObservableList([DisallowNull] IEnumerable<TElement> collection) : 
            base(collection) 
        { }

        /// <summary>
        /// Inserts the items from the specified collection into this <see cref="ObservableList{T}"/> starting at the specified index.
        /// </summary>
        /// <param name="index">The index where to start inserting the new items.</param>
        /// <param name="collection">The collection of items to insert.</param>
        /// <exception cref="ArgumentNullException" />
        /// <exception cref="ArgumentOutOfRangeException" />
        public override void InsertRange(Int32 index, 
                                         [DisallowNull] IEnumerable<TElement> collection)
        {
            if (collection is null)
            {
                throw new ArgumentNullException(nameof(collection));
            }
            if (this.IsReadOnly)
            {
                throw new InvalidOperationException(LIST_IS_READONLY);
            }
            if ((UInt32)index > (UInt32)this.Count)
            {
                throw new ArgumentOutOfRangeException(nameof(index),
                                                      INDEX_GREATER_THAN_COUNT);
            }

            this.OnPropertyChanging(nameof(this.Count));
            IList? list = new List<TElement>();
            using IEnumerator<TElement> enumerator = collection.GetEnumerator();
            while (enumerator.MoveNext())
            {
                if (enumerator.Current is null)
                {
                    continue;
                }
                if (this.Contains(enumerator.Current))
                {
                    continue;
                }
                list.Add(enumerator.Current);
                this.Insert(index++, 
                            enumerator.Current);
            }
            lock (this._syncRoot)
            {
                this._version++;
            }
            this.OnPropertyChanged(nameof(this.Count));
            this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, 
                                                                          list));
        }

        /// <summary>
        /// Removes all items from the <see cref="ObservableList{T}"/> that match the specified condition.
        /// </summary>
        /// <param name="predicate">The condition to determine if an item should be removed.</param>
        /// <returns>The number of items removed from the list</returns>
        /// <exception cref="ArgumentNullException" />
        /// <exception cref="IndexOutOfRangeException" />
        public override Int32 RemoveAll([DisallowNull] Func<TElement, Boolean> predicate)
        {
            if (predicate is null)
            {
                throw new ArgumentNullException(nameof(predicate));
            }
            if (this.IsReadOnly)
            {
                throw new InvalidOperationException(LIST_IS_READONLY);
            }

            lock (this._syncRoot)
            {
                this.OnPropertyChanging(nameof(this.Count));
                Int32 v = this._version;
                Int32 free = 0;
                while (free < this._size &&
                       !predicate.Invoke(this._items[free]))
                {
                    if (this._version != v)
                    {
                        throw new InvalidOperationException(COLLECTION_CHANGED);
                    }
                    free++;
                }
                if (free >= this._size)
                {
                    return 0;
                }

                Int32 current = free + 1;
                List<TElement> list = new();
                while (current < this._size)
                {
                    if (this._version != v)
                    {
                        throw new InvalidOperationException(COLLECTION_CHANGED);
                    }
                    while (current < this._size &&
                           predicate.Invoke(this._items[current]))
                    {
                        list.Add(this._items[current++]);
                    }

                    if (current < this._size)
                    {
                        this._items[free++] = this._items[current++];
                    }
                }

                Array.Clear(this._items, 
                            free, 
                            this._size - free);
                Int32 result = this._size - free;
                this._size = free;
                this._version++;
                this.OnPropertyChanged(nameof(this.Count));
                this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, 
                                                                              list));
                return result;
            }
        }

        /// <summary>
        /// Removes a range of items from the <see cref="ObservableList{T}"/>.
        /// </summary>
        /// <param name="index">The index of the first item to remove.</param>
        /// <param name="count">The amount of items to remove.</param>
        /// <exception cref="ArgumentException" />
        /// <exception cref="ArgumentOutOfRangeException" />
        /// <exception cref="IndexOutOfRangeException" />
        public override void RemoveRange(in Int32 index, 
                                         in Int32 count)
        {
            if (index < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(index),
                                                      START_INDEX_IS_NEGATIVE);
            }
            if (count < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(count),
                                                      COUNT_IS_NEGATIVE);
            }
            if (this.IsReadOnly)
            {
                throw new InvalidOperationException(LIST_IS_READONLY);
            }
            if (this.Count - index < count)
            {
                throw new ArgumentException(COUNT_IS_GREATER_THAN_ITEMS);
            }

            IList<TElement> list = this.GetRange(index, 
                                                 count);
            lock (this._syncRoot)
            {
                this.OnPropertyChanging(nameof(this.Count));
                if (count > 0)
                {
                    Int32 i = this._size;
                    this._size -= count;
                    if (index < this._size)
                    {
                        Array.Copy(this._items, 
                                   index + count, 
                                   this._items, 
                                   index, 
                                   this._size - index);
                    }
                    Array.Clear(this._items, 
                                this._size, 
                                count);
                }
                this._version++;
            }
            this.OnPropertyChanged(nameof(this.Count));
            this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, 
                                                                          (IList)list));
        }

        /// <summary>
        /// Reverses the order of items in the specified range in the <see cref="ObservableList{T}"/>.
        /// </summary>
        /// <param name="index">The index of the first item in the range.</param>
        /// <param name="count">The length of the range to reverse.</param>
        /// <exception cref="ArgumentException" />
        /// <exception cref="ArgumentOutOfRangeException" />
        /// <exception cref="InvalidOperationException" />
        public override void Reverse(in Int32 index, 
                                     in Int32 count)
        {
            if (index < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(index),
                                                      START_INDEX_IS_NEGATIVE);
            }
            if (count < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(count),
                                                      COUNT_IS_NEGATIVE);
            }
            if (this.IsReadOnly)
            {
                throw new InvalidOperationException(LIST_IS_READONLY);
            }
            if (this.Count - index < count)
            {
                throw new ArgumentException(COUNT_IS_GREATER_THAN_ITEMS);
            }

            lock (this._syncRoot)
            {
                List<TElement> temp = new(this.GetRange(index, 
                                                        count));
                IList list = temp;

                Array.Reverse(this._items, 
                              index, 
                              count);
                this._version++;
                this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Move, 
                                                                              list, 
                                                                              index, 
                                                                              index));
            }
        }

        /// <summary>
        /// Searches the sorted <see cref="ObservableList{T}"/> for the specified item in the specified range using the
        /// specified <see cref="IComparer{T}"/>.
        /// </summary>
        /// <remarks>
        /// If the <see cref="ObservableList{T}"/> is not previously sorted, this method will sort it before searching.
        /// </remarks>
        /// <param name="index">The index of the first item in the range.</param>
        /// <param name="count">The length of the range.</param>
        /// <param name="item">The item to search.</param>
        /// <param name="comparer">The comparer to determine a match.</param>
        /// <returns>The zero-based index of the item or -1 if the list doesn't contain the item</returns>
        /// <exception cref="ArgumentException" />
        /// <exception cref="ArgumentOutOfRangeException" />
        /// <exception cref="InvalidOperationException" />
        [Pure]
        public override Int32 BinarySearch(in Int32 index, 
                                           in Int32 count, 
                                           [DisallowNull] TElement item, 
                                           [AllowNull] IComparer<TElement>? comparer)
        {
            if (!this.IsSorted)
            {
                this.Sort(SortDirection.Ascending);
            }
            return base.BinarySearch(index, 
                                     count, 
                                     item, 
                                     comparer);
        }
    }

    // Non-Public
    partial class ObservableList<TElement>
    {
        /// <summary>
        /// Internally contains the current sort direction, if the <see cref="ObservableList{T}"/> is sorted.
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        protected SortDirection _sortDirection = SortDirection.NotSorted;
        /// <summary>
        /// Internally contains the automatic sort direction for the <see cref="AutoSort"/> feature.
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        protected SortDirection _autoSortDirection = SortDirection.NotSorted;
        /// <summary>
        /// Internally contains the automatic sort comparer for the <see cref="AutoSort"/> feature.
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        protected IComparer<TElement>? _autoSortComparer = null;
    }

    // IAutoSortable
    partial class ObservableList<TElement> : IAutoSortable<TElement>
    {
        /// <inheritdoc />
        /// <exception cref="ArgumentException" />
        /// <exception cref="ArgumentNullException" />
        /// <exception cref="InvalidOperationException" />
        public virtual void Sort(in SortDirection direction) => 
            this.Sort(direction, 
                      (IComparer<TElement>?)null);
        /// <inheritdoc />
        /// <exception cref="ArgumentException" />
        /// <exception cref="ArgumentNullException" />
        /// <exception cref="InvalidOperationException" />
        public virtual void Sort(in SortDirection direction, 
                                 [DisallowNull] Comparison<TElement> comparison)
        {
            if (comparison is null)
            {
                throw new ArgumentNullException(nameof(comparison));
            }
            this.Sort(direction, 
                      new __FuncComparer<TElement>(comparison));
        }
        /// <inheritdoc />
        /// <exception cref="ArgumentException" />
        /// <exception cref="ArgumentNullException" />
        /// <exception cref="InvalidOperationException" />
        public virtual void Sort(in SortDirection direction, 
                                 [AllowNull] IComparer<TElement>? comparer)
        {
            if (this.Count < 2 ||
                direction == SortDirection.NotSorted)
            {
                return;
            }

            lock (this._syncRoot)
            {
                Array.Sort(this._items, 
                           0, 
                           this._size, 
                           comparer);
                if (direction == SortDirection.Descending)
                {
                    Array.Reverse(this._items, 
                                  0, 
                                  this._size);
                }
                this._sortDirection = direction;
            }
            IList list = new List<TElement>(this._items.Take(this._size));
            this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Move, 
                                                                          list, 
                                                                          0, 
                                                                          0));
        }

        /// <inheritdoc />
        /// <exception cref="ArgumentException" />
        public virtual void EnableAutoSort(in SortDirection direction) => 
            this.EnableAutoSort(direction, 
                                (IComparer<TElement>?)null);
        /// <inheritdoc />
        /// <exception cref="ArgumentException" />
        /// <exception cref="ArgumentNullException" />
        public virtual void EnableAutoSort(in SortDirection direction, 
                                           [DisallowNull] Comparison<TElement> comparison)
        {
            if (comparison is null)
            {
                throw new ArgumentNullException(nameof(comparison));
            }
            if (direction is SortDirection.NotSorted)
            {
                this._autoSortDirection = SortDirection.NotSorted;
                return;
            }

            this._autoSortDirection = direction;
            this._autoSortComparer = new __FuncComparer<TElement>(comparison);
            this.Sort(this._autoSortDirection, 
                      this._autoSortComparer);
        }
        /// <inheritdoc />
        /// <exception cref="ArgumentException" />
        public virtual void EnableAutoSort(in SortDirection direction, 
                                           [AllowNull] IComparer<TElement>? comparer)
        {
            if (direction is SortDirection.NotSorted)
            {
                this._autoSortDirection = SortDirection.NotSorted;
                return;
            }

            this._autoSortDirection = direction;
            this._autoSortComparer = comparer;
            this.Sort(this._autoSortDirection, 
                      this._autoSortComparer);
        }

        /// <inheritdoc />
        public virtual void DisableAutoSort() => 
            this._autoSortDirection = SortDirection.NotSorted;

        /// <inheritdoc />
        [DebuggerBrowsable(DebuggerBrowsableState.Collapsed)]
        public Boolean IsSorted => this._sortDirection is not SortDirection.NotSorted;
        /// <inheritdoc />
        [DebuggerBrowsable(DebuggerBrowsableState.Collapsed)]
        public SortDirection SortDirection => this._sortDirection;
        /// <inheritdoc />
        [DebuggerBrowsable(DebuggerBrowsableState.Collapsed)]
        public Boolean AutoSort => this._autoSortDirection is not SortDirection.NotSorted;
    }

    // ICollection
    partial class ObservableList<TElement> : ICollection<TElement>
    {
        /// <inheritdoc />
        public override void Add([DisallowNull] TElement item)
        {
            this.OnPropertyChanging(nameof(this.Count));
            base.Add(item);
            if (this.AutoSort)
            {
                this.Sort(this._autoSortDirection, 
                          this._autoSortComparer);
            }
            else
            {
                this._sortDirection = SortDirection.NotSorted;
            }
            this.OnPropertyChanged(nameof(this.Count));
            this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, 
                                                                          item));
        }

        /// <inheritdoc />
        public override void Clear()
        {
            this.OnPropertyChanging(nameof(this.Count));
            base.Clear();
            this.OnPropertyChanged(nameof(this.Count));
            this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        /// <inheritdoc />
        public override Boolean Remove([DisallowNull] TElement item)
        {
            if (item is null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            lock (this._syncRoot)
            {
                Int32 index = Array.IndexOf(this._items,
                                            item);
                if (index >= 0)
                {
                    this._size--;
                    if (index < this._size)
                    {
                        Array.Copy(this._items,
                                   index + 1,
                                   this._items,
                                   index,
                                   this._size - index);
                    }
                    this._items[this._size] = default;
                    this._version++;
                    this.OnPropertyChanged(nameof(this.Count));
                    this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove,
                                                                                  item));
                    return true;
                }
                return false;
            }
        }
    }

    // IList
    partial class ObservableList<TElement> : IList<TElement>
    {
        /// <inheritdoc />
        public override void Insert(Int32 index, 
                                    [DisallowNull] TElement item)
        {
            this.OnPropertyChanging(nameof(this.Count));
            base.Insert(index, item);
            if (this.AutoSort)
            {
                this.Sort(this._autoSortDirection, 
                          this._autoSortComparer);
            }
            else
            {
                this._sortDirection = SortDirection.NotSorted;
            }
            this.OnPropertyChanged(nameof(this.Count));
            this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, 
                                                                          item));
        }

        /// <inheritdoc />
        public override void RemoveAt(Int32 index)
        {
            if (this.IsReadOnly)
            {
                throw new InvalidOperationException(LIST_IS_READONLY);
            }
            if ((UInt32)index > (UInt32)this.Count)
            {
                throw new ArgumentOutOfRangeException(nameof(index),
                                                      INDEX_GREATER_THAN_COUNT);
            }

            lock (this._syncRoot)
            {
                this.OnPropertyChanging(nameof(this.Count));
                this._size--;
                TElement item = this._items[index];
                if (index < this._size)
                {
                    Array.Copy(this._items, 
                               index + 1, 
                               this._items, 
                               index, 
                               this._size - index);
                }
                this._items[this._size] = default;
                this._version++;
                this.OnPropertyChanged(nameof(this.Count));
                this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, 
                                                                              item));
            }
        }
    }

    // IReadOnlyList
    partial class ObservableList<TElement> : IReadOnlyList<TElement>
    {
        /// <inheritdoc />
        public override TElement this[Int32 index]
        {
            get => base[index];
            set
            {
                if ((UInt32)index >= (UInt32)this.Count)
                {
                    throw new IndexOutOfRangeException();
                }
                lock (this._syncRoot)
                {
                    TElement old = this._items[index];
                    this._items[index] = value;
                    this._version++;
                    this._sortDirection = SortDirection.NotSorted;
                    this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, 
                                                                                  old, 
                                                                                  value));
                }
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
            this.CollectionChanged?.Invoke(this, 
                                           e);
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
            this.PropertyChanging?.Invoke(this, 
                                          new PropertyChangingEventArgs(propertyName));
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
            this.PropertyChanged?.Invoke(this, 
                                         new PropertyChangedEventArgs(propertyName));
    }
}