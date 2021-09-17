using Narumikazuchi.Collections.Abstract;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Narumikazuchi.Collections
{
    /// <summary>
    /// Represents a strongly typed collection of objects, which reports changes, can be searched and sorted. 
    /// </summary>
    public partial class ObservableCollection<TElement> : SearchableCollectionBase<TElement>
    {
        /// <summary>
        /// Initializes a new empty instance of the <see cref="ObservableCollection{TElement}"/> class.
        /// </summary>
        public ObservableCollection() : 
            base() 
        { }
        /// <summary>
        /// Initializes a new instance of the <see cref="ObservableCollection{TElement}"/> class having the specified collection of items and the specified capacity.
        /// </summary>
        /// <param name="collection">The initial collection of items in this list.</param>
        /// <exception cref="ArgumentException" />
        /// <exception cref="ArgumentNullException" />
        /// <exception cref="InvalidOperationException" />
        public ObservableCollection([DisallowNull] IEnumerable<TElement> collection) : 
            base(collection) 
        { }

        /// <summary>
        /// Removes all items from the <see cref="ObservableCollection{TElement}"/> that match the specified condition.
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
                throw new InvalidOperationException(COLLECTION_IS_READONLY);
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
    }

    // Non-Public
    partial class ObservableCollection<TElement>
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
        [MaybeNull]
        protected IComparer<TElement>? _autoSortComparer = null;
    }

    // IAutoSortable
    partial class ObservableCollection<TElement> : IAutoSortable<TElement>
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
    partial class ObservableCollection<TElement> : ICollection<TElement>
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

    // INotifyCollectionChanged
    partial class ObservableCollection<TElement> : INotifyCollectionChanged
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
    partial class ObservableCollection<TElement> : INotifyPropertyChanging
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
    partial class ObservableCollection<TElement> : INotifyPropertyChanged
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
