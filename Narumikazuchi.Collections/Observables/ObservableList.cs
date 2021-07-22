using Narumikazuchi.Collections.Abstract;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using System.Reflection;

namespace Narumikazuchi.Collections
{
    /// <summary>
    /// Represents a strongly typed list of objects, which reports changes, can be accessed by index, searched and sorted. 
    /// </summary>
    public class ObservableList<T> : SearchableListBase<T>, INotifyCollectionChanged, INotifyPropertyChanging, INotifyPropertyChanged, IAutoSortable<T>
    {
        #region Constructor

        /// <summary>
        /// Initializes a new empty instance of the <see cref="ObservableList{T}"/> class.
        /// </summary>
        public ObservableList() : base() { }
        /// <summary>
        /// Initializes a new empty instance of the <see cref="ObservableList{T}"/> class having the specified capacity.
        /// </summary>
        /// <param name="capacity">The capacity of this list.</param>
        /// <exception cref="ArgumentOutOfRangeException" />
        public ObservableList(in Int32 capacity) : base(capacity) { }
        /// <summary>
        /// Initializes a new instance of the <see cref="ObservableList{T}"/> class having the specified collection of items and the specified capacity.
        /// </summary>
        /// <param name="collection">The initial collection of items in this list.</param>
        /// <exception cref="ArgumentException" />
        /// <exception cref="ArgumentNullException" />
        /// <exception cref="InvalidOperationException" />
        public ObservableList([DisallowNull] IEnumerable<T> collection) : base(collection) { }

        #endregion

        #region Collection Management

        /// <summary>
        /// Inserts the items from the specified collection into this <see cref="ObservableList{T}"/> starting at the specified index.
        /// </summary>
        /// <param name="index">The index where to start inserting the new items.</param>
        /// <param name="collection">The collection of items to insert.</param>
        /// <exception cref="ArgumentNullException" />
        /// <exception cref="ArgumentOutOfRangeException" />
        public override void InsertRange(Int32 index, [DisallowNull] IEnumerable<T> collection)
        {
            if (collection is null)
            {
                throw new ArgumentNullException(nameof(collection));
            }
            if (this.IsReadOnly)
            {
                throw new InvalidOperationException("The list is read-only.");
            }
            if ((UInt32)index > (UInt32)this.Count)
            {
                throw new ArgumentOutOfRangeException(nameof(index), "The specified index exceeds the item count.");
            }

            this.OnPropertyChanging(nameof(this.Count));
            IList? list = new List<T>();
            using IEnumerator<T> enumerator = collection.GetEnumerator();
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
                this.Insert(index++, enumerator.Current);
            }
            lock (this._syncRoot)
            {
                this._version++;
            }
            this.OnPropertyChanged(nameof(this.Count));
            this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, list));
        }

        /// <summary>
        /// Moves the item at the given index the given amount of positions in the specified direction in the <see cref="ObservableList{T}"/>.
        /// </summary>
        public override Boolean MoveItem(Int32 index, ItemMoveDirection direction, Int32 positions)
        {
#pragma warning disable
            T tmp;
            Register<T> list = new((a, b) => ReferenceEquals(a, b));
            if (direction == ItemMoveDirection.ToLowerIndex)
            {
                while (positions-- > 0)
                {
                    if (index > 0)
                    {
                        lock (this._syncRoot)
                        {
                            tmp = this[index];
                            this[index] = this[index - 1];
                            this[index - 1] = tmp;
                            list.Add(this[index]);
                            list.Add(this[index - 1]);
                        }
                    }
                    else
                    {
                        this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Move, list, index - 1, index - 1 + positions));
                        return false;
                    }
                }
                this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Move, list, index - 1, index - 1 + positions));
            }
            else
            {
                while (positions-- > 0)
                {
                    if (index > -1 &&
                        index < this._size - 1)
                    {
                        lock (this._syncRoot)
                        {
                            tmp = this[index];
                            this[index] = this[index + 1];
                            this[index + 1] = tmp;
                            list.Add(this[index]);
                            list.Add(this[index + 1]);
                        }
                    }
                    else
                    {
                        this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Move, list, index + 1, index + 1 + positions));
                        return false;
                    }
                }
                this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Move, list, index + 1, index + 1 + positions));
            }
            return true;
#pragma warning restore
        }

        /// <summary>
        /// Moves the item at the given index the given amount of positions in the specified direction in the <see cref="ObservableList{T}"/>.
        /// </summary>
        public override Boolean MoveItem([DisallowNull] in T item, ItemMoveDirection direction, Int32 positions)
        {
#pragma warning disable
            Int32 index = this.IndexOf(item);
            Register<T> list = new((a, b) => ReferenceEquals(a, b));
            if (direction == ItemMoveDirection.ToLowerIndex)
            {
                while (positions-- > 0)
                {
                    if (index > 0)
                    {
                        lock (this._syncRoot)
                        {
                            this[index] = this[index - 1];
                            this[index - 1] = item;
                            index--;
                            list.Add(this[index]);
                            list.Add(this[index - 1]);
                        }
                    }
                    else
                    {
                        this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Move, list, index - 1, index - 1 + positions));
                        return false;
                    }
                }
                this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Move, list, index - 1, index - 1 + positions));
            }
            else
            {
                while (positions-- > 0)
                {
                    if (index > -1 &&
                        index < this._size - 1)
                    {
                        lock (this._syncRoot)
                        {
                            this[index] = this[index + 1];
                            this[index + 1] = item;
                            index++;
                            list.Add(this[index]);
                            list.Add(this[index + 1]);
                        }
                    }
                    else
                    {
                        this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Move, list, index + 1, index + 1 + positions));
                        return false;
                    }
                }
                this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Move, list, index + 1, index + 1 + positions));
            }
            return true;
#pragma warning restore
        }

        /// <summary>
        /// Removes all items from the <see cref="ObservableList{T}"/> that match the specified condition.
        /// </summary>
        /// <param name="predicate">The condition to determine if an item should be removed.</param>
        /// <returns>The number of items removed from the list</returns>
        /// <exception cref="ArgumentNullException" />
        /// <exception cref="IndexOutOfRangeException" />
        public override Int32 RemoveAll([DisallowNull] Func<T, Boolean> predicate)
        {
            if (predicate is null)
            {
                throw new ArgumentNullException(nameof(predicate));
            }
            if (this.IsReadOnly)
            {
                throw new InvalidOperationException("The list is read-only.");
            }

            lock (this._syncRoot)
            {
                this.OnPropertyChanging(nameof(this.Count));
#pragma warning disable
                Int32 v = this._version;
                Int32 free = 0;
                while (free < this._size &&
                       !predicate.Invoke(this._items[free]))
                {
                    if (this._version != v)
                    {
                        throw new InvalidOperationException("The collection changed during enumeration.");
                    }
                    free++;
                }
                if (free >= this._size)
                {
                    return 0;
                }

                Int32 current = free + 1;
                List<T> list = new();
                while (current < this._size)
                {
                    if (this._version != v)
                    {
                        throw new InvalidOperationException("The collection changed during enumeration.");
                    }
                    while (current < this._size &&
                           predicate(this._items[current]))
                    {
                        list.Add(this._items[current]);
                        if (this._items[current] is INotifyPropertyChanging changing
                                                 and INotifyPropertyChanged changed)
                        {
                            changing.PropertyChanging -= this.CacheItemChangingState;
                            changed.PropertyChanged -= this.RaiseItemChangedEvent;
                        }
                        current++;
                    }

                    if (current < this._size)
                    {
                        this._items[free++] = this._items[current++];
                    }
                }
#pragma warning restore

                Array.Clear(this._items, free, this._size - free);
                Int32 result = this._size - free;
                this._size = free;
                this._version++;
                this.OnPropertyChanged(nameof(this.Count));
                this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, list));
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
        public override void RemoveRange(in Int32 index, in Int32 count)
        {
            if (index < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(index), "Start index cannot be negative.");
            }
            if (count < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(count), "Count cannot be negative.");
            }
            if (this.IsReadOnly)
            {
                throw new InvalidOperationException("The list is read-only.");
            }
            if (this.Count - index < count)
            {
                throw new ArgumentException("The specified count is greater than the available number of items.");
            }

            IList<T> list = this.GetRange(index, count);
            lock (this._syncRoot)
            {
                this.OnPropertyChanging(nameof(this.Count));
                if (count > 0)
                {
                    Int32 i = this._size;
                    this._size -= count;
                    if (index < this._size)
                    {
                        Array.Copy(this._items, index + count, this._items, index, this._size - index);
                    }
                    Array.Clear(this._items, this._size, count);
                }
                this._version++;
            }
            for (Int32 i = 0; i < list.Count; i++)
            {
                if (list[i] is INotifyPropertyChanging changing
                            and INotifyPropertyChanged changed)
                {
                    changing.PropertyChanging += this.CacheItemChangingState;
                    changed.PropertyChanged += this.RaiseItemChangedEvent;
                }
            }
            this.OnPropertyChanged(nameof(this.Count));
            this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, (IList)list));
        }

        /// <summary>
        /// Reverses the order of items in the specified range in the <see cref="ObservableList{T}"/>.
        /// </summary>
        /// <param name="index">The index of the first item in the range.</param>
        /// <param name="count">The length of the range to reverse.</param>
        /// <exception cref="ArgumentException" />
        /// <exception cref="ArgumentOutOfRangeException" />
        /// <exception cref="InvalidOperationException" />
        public override void Reverse(in Int32 index, in Int32 count)
        {
            if (index < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(index), "Start index cannot be negative.");
            }
            if (count < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(count), "Count cannot be negative.");
            }
            if (this.IsReadOnly)
            {
                throw new InvalidOperationException("The list is read-only.");
            }
            if (this.Count - index < count)
            {
                throw new ArgumentException("The specified count is greater than the available number of items.");
            }

            lock (this._syncRoot)
            {
                List<T> temp = new(this.GetRange(index, count));
                IList list = temp;

                Array.Reverse(this._items, index, count);
                this._version++;
                this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Move, list, index, index));
            }
        }

        #endregion

        #region Search

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
        public override Int32 BinarySearch(in Int32 index, in Int32 count, [DisallowNull] in T item, IComparer<T>? comparer)
        {
            if (!this.IsSorted)
            {
                this.Sort(SortDirection.Ascending);
            }
            return base.BinarySearch(index, count, item, comparer);
        }

        #endregion

        #region ICollection

        /// <inheritdoc />
        public override void Add([DisallowNull] T item)
        {
            this.OnPropertyChanging(nameof(this.Count));
            base.Add(item);
            if (this.AutoSort)
            {
                this.Sort(this._autoSortDirection, this._autoSortComparer);
            }
            else
            {
                this._sortDirection = SortDirection.NotSorted;
            }
            if (item is INotifyPropertyChanging changing
                     and INotifyPropertyChanged changed)
            {
                changing.PropertyChanging += this.CacheItemChangingState;
                changed.PropertyChanged += this.RaiseItemChangedEvent;
            }
            this.OnPropertyChanged(nameof(this.Count));
            this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item));
        }

        /// <inheritdoc />
        public override void Clear()
        {
            this.OnPropertyChanging(nameof(this.Count));
            for (Int32 i = 0; i < this.Count; i++)
            {
                if (this._items[i] is INotifyPropertyChanging changing
                                   and INotifyPropertyChanged changed)
                {
                    changing.PropertyChanging -= this.CacheItemChangingState;
                    changed.PropertyChanged -= this.RaiseItemChangedEvent;
                }
            }
            base.Clear();
            this.OnPropertyChanged(nameof(this.Count));
            this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        /// <inheritdoc />
        public override Boolean Remove([DisallowNull] T item)
        {
            Boolean result = base.Remove(item);
            return result;
        }

        #endregion

        #region IList

        /// <inheritdoc />
        public override void Insert(Int32 index, [DisallowNull] T item)
        {
            this.OnPropertyChanging(nameof(this.Count));
            base.Insert(index, item);
            if (this.AutoSort)
            {
                this.Sort(this._autoSortDirection, this._autoSortComparer);
            }
            else
            {
                this._sortDirection = SortDirection.NotSorted;
            }
            if (item is INotifyPropertyChanging changing
                     and INotifyPropertyChanged changed)
            {
                changing.PropertyChanging += this.CacheItemChangingState;
                changed.PropertyChanged += this.RaiseItemChangedEvent;
            }
            this.OnPropertyChanged(nameof(this.Count));
            this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item));
        }

        /// <inheritdoc />
        public override void RemoveAt(Int32 index)
        {
            if (this.IsReadOnly)
            {
                throw new InvalidOperationException("The list is read-only.");
            }
            if ((UInt32)index > (UInt32)this.Count)
            {
                throw new ArgumentOutOfRangeException(nameof(index), "The specified index exceeds the item count.");
            }

            lock (this._syncRoot)
            {
#pragma warning disable
                this.OnPropertyChanging(nameof(this.Count));
                this._size--;
                T item = this._items[index];
                if (index < this._size)
                {
                    Array.Copy(this._items, index + 1, this._items, index, this._size - index);
                }
                this._items[this._size] = default;
                this._version++;
                if (item is INotifyPropertyChanging changing
                         and INotifyPropertyChanged changed)
                {
                    changing.PropertyChanging -= this.CacheItemChangingState;
                    changed.PropertyChanged -= this.RaiseItemChangedEvent;
                }
                this.OnPropertyChanged(nameof(this.Count));
                this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, item));
#pragma warning restore
            }
        }

        #endregion

        #region IReadOnlyList

        /// <inheritdoc />
        public override T this[Int32 index]
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
#pragma warning disable
                    T old = this._items[index];
                    if (old is INotifyPropertyChanging changingOld
                            and INotifyPropertyChanged changedOld)
                    {
                        changingOld.PropertyChanging -= this.CacheItemChangingState;
                        changedOld.PropertyChanged -= this.RaiseItemChangedEvent;
                    }
                    if (value is INotifyPropertyChanging changingNew
                             and INotifyPropertyChanged changedNew)
                    {
                        changingNew.PropertyChanging += this.CacheItemChangingState;
                        changedNew.PropertyChanged += this.RaiseItemChangedEvent;
                    }
                    this._items[index] = value;
                    this._version++;
                    this._sortDirection = SortDirection.NotSorted;
                    this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, old, value));
#pragma warning restore
                }
            }
        }

        #endregion

        #region INotifyCollectionChanged

        /// <inheritdoc />
        public event NotifyCollectionChangedEventHandler? CollectionChanged;

        /// <summary>
        /// Raises the <see cref="CollectionChanged"/> event with the specified event args.
        /// </summary>
        protected void OnCollectionChanged(NotifyCollectionChangedEventArgs e) => 
            this.CollectionChanged?.Invoke(this, e);

        #endregion

        #region INotifyPropertyChanging

        /// <inheritdoc />
        public event PropertyChangingEventHandler? PropertyChanging;

        /// <summary>
        /// Raises the <see cref="PropertyChanging"/> event with the specified event args.
        /// </summary>
        protected void OnPropertyChanging(String propertyName) =>
            this.PropertyChanging?.Invoke(this, new PropertyChangingEventArgs(propertyName));

        #endregion

        #region INotifyPropertyChanged

        /// <inheritdoc />
        public event PropertyChangedEventHandler? PropertyChanged;

        /// <summary>
        /// Raises the <see cref="PropertyChanged"/> event with the specified event args.
        /// </summary>
        protected void OnPropertyChanged(String propertyName) => 
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        #endregion

        #region IAutoSortable

        /// <inheritdoc />
        /// <exception cref="ArgumentException" />
        /// <exception cref="ArgumentNullException" />
        /// <exception cref="InvalidOperationException" />
        public virtual void Sort(SortDirection direction) => this.Sort(direction, (IComparer<T>?)null);
        /// <inheritdoc />
        /// <exception cref="ArgumentException" />
        /// <exception cref="ArgumentNullException" />
        /// <exception cref="InvalidOperationException" />
        public virtual void Sort(SortDirection direction, [DisallowNull] Comparison<T> comparison)
        {
            if (comparison is null)
            {
                throw new ArgumentNullException(nameof(comparison));
            }
#pragma warning disable
            this.Sort(direction, new __FuncComparer<T>(comparison));
#pragma warning restore
        }
        /// <inheritdoc />
        /// <exception cref="ArgumentException" />
        /// <exception cref="ArgumentNullException" />
        /// <exception cref="InvalidOperationException" />
        public virtual void Sort(SortDirection direction, IComparer<T>? comparer)
        {
            if (this.Count < 2 ||
                direction == SortDirection.NotSorted)
            {
                return;
            }

            lock (this._syncRoot)
            {
#pragma warning disable
                Array.Sort(this._items, 0, this.Count, comparer);
#pragma warning restore
                if (direction == SortDirection.Descending)
                {
                    Array.Reverse(this._items, 0, this.Count);
                }
                this._sortDirection = direction;
            }
#pragma warning disable
            IList list = new List<T>(this._items);
#pragma warning restore
            this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Move, list, 0, 0));
        }

        /// <inheritdoc />
        /// <exception cref="ArgumentException" />
        public virtual void EnableAutoSort(SortDirection direction) => this.EnableAutoSort(direction, (IComparer<T>?)null);
        /// <inheritdoc />
        /// <exception cref="ArgumentException" />
        /// <exception cref="ArgumentNullException" />
        public virtual void EnableAutoSort(SortDirection direction, [DisallowNull] Comparison<T> comparison)
        {
            if (direction is SortDirection.NotSorted)
            {
                throw new ArgumentException("Can't enable auto sort with no sort direction.", nameof(direction));
            }
            if (comparison is null)
            {
                throw new ArgumentNullException(nameof(comparison));
            }

            this._autoSortDirection = direction;
#pragma warning disable
            this._autoSortComparer = new __FuncComparer<T>(comparison);
#pragma warning restore
            this.Sort(this._autoSortDirection, this._autoSortComparer);
        }
        /// <inheritdoc />
        /// <exception cref="ArgumentException" />
        public virtual void EnableAutoSort(SortDirection direction, IComparer<T>? comparer)
        {
            if (direction is SortDirection.NotSorted)
            {
                throw new ArgumentException("Can't enable auto sort with no sort direction.", nameof(direction));
            }

            this._autoSortDirection = direction;
            this._autoSortComparer = comparer;
            this.Sort(this._autoSortDirection, this._autoSortComparer);
        }

        /// <inheritdoc />
        public virtual void DisableAutoSort() => this._autoSortDirection = SortDirection.NotSorted;

        #region Properties

        /// <inheritdoc />
        [DebuggerBrowsable(DebuggerBrowsableState.Collapsed)]
        public Boolean IsSorted => this._sortDirection is not SortDirection.NotSorted;
        /// <inheritdoc />
        [DebuggerBrowsable(DebuggerBrowsableState.Collapsed)]
        public SortDirection SortDirection => this._sortDirection;
        /// <inheritdoc />
        [DebuggerBrowsable(DebuggerBrowsableState.Collapsed)]
        public Boolean AutoSort => this._autoSortDirection is not SortDirection.NotSorted;

        #endregion

        #endregion

        #region Events

        /// <summary>
        /// Occurs when an item in the list changed.
        /// </summary>
        public event EventHandler<ObservableList<T>, ItemChangedEventArgs<T>>? ItemChanged;

        /// <summary>
        /// Caches the "previous" value of the changing property of the sender.
        /// </summary>
        protected void CacheItemChangingState(Object? sender, PropertyChangingEventArgs e)
        {
            this._cache = null;
            if (sender is null ||
                sender is not T obj ||
                String.IsNullOrWhiteSpace(e.PropertyName))
            {
                return;
            }

            PropertyInfo? property = typeof(T).GetProperty(e.PropertyName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            if (property is null)
            {
                return;
            }
            this._cache = property.GetValue(obj)?.DeepCopy();
        }

        /// <summary>
        /// Raises the <see cref="ItemChanged"/> event after an item in the list changed.
        /// </summary>
        protected void RaiseItemChangedEvent(Object? sender, PropertyChangedEventArgs e)
        {
            if (sender is null ||
                sender is not T obj ||
                String.IsNullOrWhiteSpace(e.PropertyName))
            {
                return;
            }

            this.ItemChanged?.Invoke(this, new ItemChangedEventArgs<T>(obj, this._cache, e.PropertyName));
            this._cache = null;
        }

        #endregion

        #region Fields

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
        protected IComparer<T>? _autoSortComparer = null;

        [DebuggerBrowsable(DebuggerBrowsableState.Collapsed)]
        private Object? _cache = null;

        #endregion
    }
}