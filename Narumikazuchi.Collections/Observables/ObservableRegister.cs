using Narumikazuchi.Collections.Abstract;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace Narumikazuchi.Collections
{
    /// <summary>
    /// Represents a collection which reports changes and contains every object is only once. The procedure to check whether the object is already in the <see cref="ObservableRegister{T}"/> can be specified
    /// with a corresponding <see cref="EqualityComparer{T}"/> or <see cref="EqualityComparison{T}"/> delegate.
    /// </summary>
    /// <remarks>
    /// If neither <see cref="EqualityComparer{T}"/> nor <see cref="EqualityComparison{T}"/> are specified, the register will compare the references for classes or check each field/property for values types.
    /// </remarks>
    public class ObservableRegister<T> : Register<T>, INotifyCollectionChanged, INotifyPropertyChanging, INotifyPropertyChanged
    {
        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="ObservableRegister{T}"/> class.
        /// </summary>
        public ObservableRegister() : base(4, __FuncEqualityComparer<T>.Default) { }
        /// <summary>
        /// Initializes a new instance of the <see cref="ObservableRegister{T}"/> class with the specified capacity.
        /// </summary>
        public ObservableRegister(in Int32 capacity) : base(capacity, __FuncEqualityComparer<T>.Default) { }
        /// <summary>
        /// Initializes a new instance of the <see cref="ObservableRegister{T}"/> class using the specified function to check items for equality.
        /// </summary>
        public ObservableRegister([DisallowNull] EqualityComparison<T> comparison) : base(4, comparison) { }
        /// <summary>
        /// Initializes a new instance of the <see cref="ObservableRegister{T}"/> class with the specified collection as items.
        /// </summary>
        public ObservableRegister([DisallowNull] IEnumerable<T> collection) : base(__FuncEqualityComparer<T>.Default, collection) { }
        /// <summary>
        /// Initializes a new instance of the <see cref="ObservableRegister{T}"/> class with the specified capacity using the specified function to check items for equality.
        /// </summary>
        public ObservableRegister(in Int32 capacity, [DisallowNull] EqualityComparison<T> comparison) : base(capacity, comparison) { }
        /// <summary>
        /// Initializes a new instance of the <see cref="ObservableRegister{T}"/> class with the specified collection as items using the specified function to check items for equality.
        /// </summary>
        public ObservableRegister([DisallowNull] EqualityComparison<T> comparison, [DisallowNull] IEnumerable<T> collection) : base(comparison, collection) { }
        /// <summary>
        /// Initializes a new instance of the <see cref="ObservableRegister{T}"/> class using the specified <see cref="IEqualityComparer{T}"/> to check items for equality.
        /// </summary>
        public ObservableRegister(IEqualityComparer<T>? comparer) : base(4, comparer) { }
        /// <summary>
        /// Initializes a new instance of the <see cref="ObservableRegister{T}"/> class with the specified capacity using the specified <see cref="IEqualityComparer{T}"/> to check items for equality.
        /// </summary>
        public ObservableRegister(in Int32 capacity, IEqualityComparer<T>? comparer) : base(capacity, comparer) { }
        /// <summary>
        /// Initializes a new instance of the <see cref="ObservableRegister{T}"/> class with the specified collection as items using the specified <see cref="IEqualityComparer{T}"/> to check items for equality.
        /// </summary>
        public ObservableRegister(IEqualityComparer<T>? comparer, [DisallowNull] IEnumerable<T> collection) : base(comparer, collection) { }

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

        /// <inheritdoc/>
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

        /// <inheritdoc/>
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

        #endregion

        #region ICollection

        /// <inheritdoc/>
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

        #endregion

        #region IList

        /// <inheritdoc/>
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

        /// <inheritdoc/>
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

        #region ISortable

        /// <inheritdoc />
        /// <exception cref="ArgumentException" />
        /// <exception cref="ArgumentNullException" />
        /// <exception cref="InvalidOperationException" />
        public override void Sort(SortDirection direction, IComparer<T>? comparer)
        {
            base.Sort(direction, comparer);
#pragma warning disable
            IList list = new List<T>(this._items);
#pragma warning restore
            this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Move, list, 0, 0));
        }

        #endregion

        #region Events

        /// <summary>
        /// Occurs when an item in the list changed.
        /// </summary>
        public event EventHandler<ObservableRegister<T>, ItemChangedEventArgs<T>>? ItemChanged;

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

        [DebuggerBrowsable(DebuggerBrowsableState.Collapsed)]
        private Object? _cache = null;

        #endregion
    }
}
