using Narumikazuchi.Collections.Abstract;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;

namespace Narumikazuchi.Collections
{
    /// <summary>
    /// Represents a collection where every object is only contained once. The procedure to check whether the object is already in the <see cref="ReadOnlyRegister{T}"/> can be specified
    /// with a corresponding <see cref="EqualityComparer{T}"/> or <see cref="EqualityComparison{T}"/> delegate.
    /// </summary>
    public class ReadOnlyRegister<T> : SearchableReadOnlyListBase<T>, IRegister<T>, IAutoSortable<T>
    {
        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="ReadOnlyRegister{T}"/> class with the specified collection as items.
        /// </summary>
        public ReadOnlyRegister([DisallowNull] IEnumerable<T> collection)
        {
            if (collection is null)
            {
                throw new ArgumentNullException(nameof(collection));
            }

            this.Comparer = null;

            this._size = 0;
            this._items = _emptyArray;

            using IEnumerator<T> enumerator = collection.GetEnumerator();
            while (enumerator.MoveNext())
            {
                this.EnsureCapacity(this.Count + 1);
                lock (this._syncRoot)
                {
#pragma warning disable 
                    this.AddInternal(enumerator.Current);
#pragma warning restore
                }
            }
            lock (this._syncRoot)
            {
                if (this._items.Length != this._size)
                {
                    T[] array = new T[this._size];
                    Array.Copy(this._items, 0, array, 0, this._size);
                    this._items = array;
                }
            }
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="ReadOnlyRegister{T}"/> class with the specified capacity using the specified function to check items for equality.
        /// </summary>
        public ReadOnlyRegister(EqualityComparison<T> comparison)
        {
            if (comparison is null)
            {
                this.Comparer = null;
                return;
            }
#pragma warning disable
            this.Comparer = new __FuncEqualityComparer<T>(comparison);
#pragma warning restore
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="ReadOnlyRegister{T}"/> class with the specified collection as items using the specified function to check items for equality.
        /// </summary>
        public ReadOnlyRegister(EqualityComparison<T> comparison, [DisallowNull] IEnumerable<T> collection) : this(collection)
        {
            if (comparison is null)
            {
                this.Comparer = null;
                return;
            }
#pragma warning disable
            this.Comparer = new __FuncEqualityComparer<T>(comparison);
#pragma warning restore
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="ReadOnlyRegister{T}"/> class using the specified <see cref="IEqualityComparer{T}"/> to check items for equality.
        /// </summary>
        public ReadOnlyRegister(IEqualityComparer<T>? comparer) => this.Comparer = comparer;
        /// <summary>
        /// Initializes a new instance of the <see cref="ReadOnlyRegister{T}"/> class with the specified collection as items using the specified <see cref="IEqualityComparer{T}"/> to check items for equality.
        /// </summary>
        public ReadOnlyRegister(IEqualityComparer<T>? comparer, [DisallowNull] IEnumerable<T> collection) : this(collection) => this.Comparer = comparer;

        #endregion

        #region Collection Management

        /// <summary>
        /// Adds the specified item to the <see cref="ReadOnlyRegister{T}"/>.
        /// </summary>
        protected Boolean AddInternal([DisallowNull] T item)
        {
            if (item is null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            if (this.Contains(item))
            {
                return false;
            }

            this.EnsureCapacity(this.Count + 1);
            lock (this._syncRoot)
            {
                this.AppendToInternalRegister(item.GetHashCode());
                this._items[this._size++] = item;
                this._version++;
            }
            if (this.AutoSort)
            {
                this.Sort(this._autoSortDirection, this._autoSortComparer);
            }
            else
            {
                this._sortDirection = SortDirection.NotSorted;
            }
            return true;
        }

        /// <summary>
        /// Inserts the specified item at the specified index into the <see cref="ReadOnlyRegister{T}"/>.
        /// </summary>
        protected Boolean InsertInternal(Int32 index, [DisallowNull] T item)
        {
            if (item is null)
            {
                throw new ArgumentNullException(nameof(item));
            }
            if ((UInt32)index > (UInt32)this.Count)
            {
                throw new ArgumentOutOfRangeException(nameof(index), "The specified index exceeds the item count.");
            }

            if (this.Contains(item))
            {
                return false;
            }

            this.EnsureCapacity(this.Count + 1);
            lock (this._syncRoot)
            {
                if (index < this.Count)
                {
                    Array.Copy(this._items, index, this._items, index + 1, this.Count - index);
                }
                this.AppendToInternalRegister(item.GetHashCode());
                this._items[index] = item;
                this._size++;
                this._version++;
            }
            if (this.AutoSort)
            {
                this.Sort(this._autoSortDirection, this._autoSortComparer);
            }
            else
            {
                this._sortDirection = SortDirection.NotSorted;
            }
            return true;
        }

        /// <summary>
        /// Removes the specified item from the <see cref="ReadOnlyRegister{T}"/>.
        /// </summary>
        protected Boolean RemoveInternal([DisallowNull] T item)
        {
#pragma warning disable
            if (item is null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            lock (this._syncRoot)
            {
                Int32 index = this.IndexOf(item);
                if (index > -1)
                {
                    this._size--;
                    if (index < this.Count)
                    {
                        Array.Copy(this._items, index + 1, this._items, index, this.Count - index);
                    }
                    this._items[this.Count] = default;
                    this.RemoveFromInternalRegister(item.GetHashCode());
                    this._version++;
                    return true;
                }
                return false;
            }
#pragma warning restore
        }

        /// <summary>
        /// Appends the hashcode to the internal register for quick <see cref="Contains(T)"/> calls.
        /// </summary>
        protected void AppendToInternalRegister(in Int32 hashcode)
        {
            if (this._hashTree is null)
            {
                this._hashTree = new BinaryTree<Int32>(hashcode)
                {
                    ThrowExceptionOnDuplicate = false
                };
                return;
            }
            this._hashTree.Add(hashcode);
        }

        /// <summary>
        /// Removes the hashcode from the internal register.
        /// </summary>
        protected void RemoveFromInternalRegister(in Int32 hashcode)
        {
#pragma warning disable
            for (Int32 i = 0; i < this.Count; i++)
            {
                if (this._items[i].GetHashCode() == hashcode)
                {
                    return;
                }
            }
            if (_hashTree.RootNode.Value == hashcode)
            {
                this._hashTree = null;
                return;
            }
            this._hashTree.Remove(hashcode);
#pragma warning restore
        }

        #endregion

        #region IReadOnlyCollection

        /// <inheritdoc/>
#pragma warning disable
        public override Boolean Contains([DisallowNull] T item) => item is null
                                                                    ? throw new ArgumentNullException(nameof(item))
                                                                    : this._hashTree is not null &&
                                                                      this._hashTree.Find(item.GetHashCode()) is not null &&
                                                                      this.IndexOf(item) > -1;
#pragma warning restore

        #endregion

        #region IReadOnlyList

        /// <inheritdoc/>
#pragma warning disable
        [Pure]
        public override Int32 IndexOf([DisallowNull] T item)
        {
            if (item is null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            lock (this._syncRoot)
            {
                if (this.Comparer is null)
                {
                    for (Int32 i = 0; i < this.Count; i++)
                    {
                        if ((this._items[i] as IEquatable<T>).Equals(item))
                        {
                            return i;
                        }
                    }
                }
                else
                {
                    for (Int32 i = 0; i < this.Count; i++)
                    {
                        if (this.Comparer.Equals(this._items[i], item))
                        {
                            return i;
                        }
                    }
                }
            }
            return -1;
        }
#pragma warning restore

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

        #region Properties

        /// <inheritdoc/>
        [DebuggerBrowsable(DebuggerBrowsableState.Collapsed)]
        public IEqualityComparer<T>? Comparer
        {
            get => this._comparer;
            set
            {
                if (value is null &&
                    !typeof(IEquatable<T>).IsAssignableFrom(typeof(T)))
                {
                    throw new ArgumentException($"Comparer can not be null if the generic type T ({typeof(T).Name}) does not inherit from the IEquatable<T> interface.");
                }
                this._comparer = value;
            }
        }

        #endregion

        #region Fields

        /// <summary>
        /// Internally contains the current sort direction, if the <see cref="ReadOnlyRegister{T}"/> is sorted.
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
        protected IComparer<T>? _autoSortComparer = null;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private IEqualityComparer<T>? _comparer = null;
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private BinaryTree<Int32>? _hashTree = null;

        #endregion
    }
}
