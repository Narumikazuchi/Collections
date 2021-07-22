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
    /// Represents a strongly typed collection of objects, which reports changes, can be searched and sorted. 
    /// </summary>
    public class ObservableCollection<T> : SearchableCollectionBase<T>, INotifyCollectionChanged, INotifyPropertyChanging, INotifyPropertyChanged, IAutoSortable<T>
    {
        #region Constructor

        /// <summary>
        /// Initializes a new empty instance of the <see cref="ObservableCollection{T}"/> class.
        /// </summary>
        public ObservableCollection() : base() { }
        /// <summary>
        /// Initializes a new instance of the <see cref="ObservableCollection{T}"/> class having the specified collection of items and the specified capacity.
        /// </summary>
        /// <param name="collection">The initial collection of items in this list.</param>
        /// <exception cref="ArgumentException" />
        /// <exception cref="ArgumentNullException" />
        /// <exception cref="InvalidOperationException" />
        public ObservableCollection([DisallowNull] IEnumerable<T> collection) : base(collection) { }

        #endregion

        #region Collection Management

        /// <summary>
        /// Removes all items from the <see cref="ObservableCollection{T}"/> that match the specified condition.
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
        public event EventHandler<ObservableCollection<T>, ItemChangedEventArgs<T>>? ItemChanged;

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
