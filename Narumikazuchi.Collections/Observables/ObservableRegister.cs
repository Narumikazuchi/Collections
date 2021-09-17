using Narumikazuchi.Collections.Abstract;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Narumikazuchi.Collections
{
    /// <summary>
    /// Represents a collection which reports changes and contains every object is only once. The procedure to check whether the object is already in the <see cref="ObservableRegister{TElement}"/> can be specified
    /// with a corresponding <see cref="EqualityComparer{T}"/> or <see cref="EqualityComparison{T}"/> delegate.
    /// </summary>
    /// <remarks>
    /// If neither <see cref="EqualityComparer{T}"/> nor <see cref="EqualityComparison{T}"/> are specified, the register will compare the references for classes or check each field/property for values types.
    /// </remarks>
    public partial class ObservableRegister<TElement> : Register<TElement>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ObservableRegister{TElement}"/> class.
        /// </summary>
        public ObservableRegister() : 
            base(4, 
                 __FuncEqualityComparer<TElement>.Default) 
        { }
        /// <summary>
        /// Initializes a new instance of the <see cref="ObservableRegister{TElement}"/> class with the specified capacity.
        /// </summary>
        public ObservableRegister(in Int32 capacity) : 
            base(capacity, 
                 __FuncEqualityComparer<TElement>.Default) 
        { }
        /// <summary>
        /// Initializes a new instance of the <see cref="ObservableRegister{TElement}"/> class using the specified function to check items for equality.
        /// </summary>
        public ObservableRegister([DisallowNull] EqualityComparison<TElement> comparison) : 
            base(4, 
                 comparison) 
        { }
        /// <summary>
        /// Initializes a new instance of the <see cref="ObservableRegister{TElement}"/> class with the specified collection as items.
        /// </summary>
        public ObservableRegister([DisallowNull] IEnumerable<TElement> collection) : 
            base(__FuncEqualityComparer<TElement>.Default, 
                 collection) 
        { }
        /// <summary>
        /// Initializes a new instance of the <see cref="ObservableRegister{TElement}"/> class with the specified capacity using the specified function to check items for equality.
        /// </summary>
        public ObservableRegister(in Int32 capacity, 
                                  [DisallowNull] EqualityComparison<TElement> comparison) : 
            base(capacity, 
                 comparison) 
        { }
        /// <summary>
        /// Initializes a new instance of the <see cref="ObservableRegister{TElement}"/> class with the specified collection as items using the specified function to check items for equality.
        /// </summary>
        public ObservableRegister([DisallowNull] EqualityComparison<TElement> comparison, 
                                  [DisallowNull] IEnumerable<TElement> collection) : 
            base(comparison, 
                 collection) 
        { }
        /// <summary>
        /// Initializes a new instance of the <see cref="ObservableRegister{TElement}"/> class using the specified <see cref="IEqualityComparer{T}"/> to check items for equality.
        /// </summary>
        public ObservableRegister([AllowNull] IEqualityComparer<TElement>? comparer) : 
            base(4, 
                 comparer) 
        { }
        /// <summary>
        /// Initializes a new instance of the <see cref="ObservableRegister{TElement}"/> class with the specified capacity using the specified <see cref="IEqualityComparer{T}"/> to check items for equality.
        /// </summary>
        public ObservableRegister(in Int32 capacity, 
                                  [AllowNull] IEqualityComparer<TElement>? comparer) : 
            base(capacity, 
                 comparer) 
        { }
        /// <summary>
        /// Initializes a new instance of the <see cref="ObservableRegister{TElement}"/> class with the specified collection as items using the specified <see cref="IEqualityComparer{T}"/> to check items for equality.
        /// </summary>
        public ObservableRegister([AllowNull] IEqualityComparer<TElement>? comparer, 
                                  [DisallowNull] IEnumerable<TElement> collection) : 
            base(comparer, 
                 collection) 
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

        /// <inheritdoc/>
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

        /// <inheritdoc/>
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
    }

    // ICollection
    partial class ObservableRegister<TElement> : ICollection<TElement>
    {
        /// <inheritdoc/>
        public override Boolean Add([DisallowNull] TElement item)
        {
            this.OnPropertyChanging(nameof(this.Count));
            if (!base.Add(item))
            {
                return false;
            }
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
            return true;
        }

        /// <inheritdoc />
        public override void Clear()
        {
            this.OnPropertyChanging(nameof(this.Count));
            base.Clear();
            this.OnPropertyChanged(nameof(this.Count));
            this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }
    }

    // IList
    partial class ObservableRegister<TElement> : IList<TElement>
    {
        /// <inheritdoc/>
        public override void Insert(Int32 index, 
                                    [DisallowNull] TElement item)
        {
            this.OnPropertyChanging(nameof(this.Count));
            base.Insert(index, 
                        item);
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

        /// <inheritdoc/>
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

    // INotifyCollectionChanged
    partial class ObservableRegister<TElement> : INotifyCollectionChanged
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
    partial class ObservableRegister<TElement> : INotifyPropertyChanging
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
    partial class ObservableRegister<TElement> : INotifyPropertyChanged
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

    // ISortable
    partial class ObservableRegister<TElement> : ISortable<TElement>
    {
        /// <inheritdoc />
        /// <exception cref="ArgumentException" />
        /// <exception cref="ArgumentNullException" />
        /// <exception cref="InvalidOperationException" />
        public override void Sort(in SortDirection direction, 
                                  [AllowNull] IComparer<TElement>? comparer)
        {
            base.Sort(direction, 
                      comparer);
            IList list = new List<TElement>(this._items.Take(this._size));
            this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Move, 
                                                                          list, 
                                                                          0, 
                                                                          0));
        }
    }
}
