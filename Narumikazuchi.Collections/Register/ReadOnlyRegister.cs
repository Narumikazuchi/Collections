using Narumikazuchi.Collections.Abstract;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using System.Linq;

namespace Narumikazuchi.Collections
{
    /// <summary>
    /// Represents a collection where every object is only contained once. The procedure to check whether the object is already in the <see cref="ReadOnlyRegister{TElement}"/> can be specified
    /// with a corresponding <see cref="EqualityComparer{T}"/> or <see cref="EqualityComparison{T}"/> delegate.
    /// </summary>
    public partial class ReadOnlyRegister<TElement> : SearchableReadOnlyListBase<TElement>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ReadOnlyRegister{TElement}"/> class with the specified collection as items.
        /// </summary>
        public ReadOnlyRegister([DisallowNull] IEnumerable<TElement> collection)
        {
            if (collection is null)
            {
                throw new ArgumentNullException(nameof(collection));
            }

            this.Comparer = null;

            this._size = 0;
            this._items = _emptyArray;
            this.Fill(collection);
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="ReadOnlyRegister{TElement}"/> class with the specified capacity using the specified function to check items for equality.
        /// </summary>
        public ReadOnlyRegister([AllowNull] EqualityComparison<TElement> comparison)
        {
            if (comparison is null)
            {
                this.Comparer = null;
                return;
            }
            this.Comparer = new __FuncEqualityComparer<TElement>(comparison);
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="ReadOnlyRegister{TElement}"/> class with the specified collection as items using the specified function to check items for equality.
        /// </summary>
        public ReadOnlyRegister([AllowNull] EqualityComparison<TElement> comparison, 
                                [DisallowNull] IEnumerable<TElement> collection)
        {
            if (comparison is null)
            {
                this.Comparer = null;
                return;
            }
            this.Comparer = new __FuncEqualityComparer<TElement>(comparison);
            this.Fill(collection);
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="ReadOnlyRegister{TElement}"/> class using the specified <see cref="IEqualityComparer{T}"/> to check items for equality.
        /// </summary>
        public ReadOnlyRegister([AllowNull] IEqualityComparer<TElement>? comparer) => 
            this.Comparer = comparer;
        /// <summary>
        /// Initializes a new instance of the <see cref="ReadOnlyRegister{TElement}"/> class with the specified collection as items using the specified <see cref="IEqualityComparer{T}"/> to check items for equality.
        /// </summary>
        public ReadOnlyRegister([AllowNull] IEqualityComparer<TElement>? comparer, 
                                [DisallowNull] IEnumerable<TElement> collection)
        {
            this.Comparer = comparer;
            this.Fill(collection);
        }
    }

    // Non-Public
    partial class ReadOnlyRegister<TElement>
    {
        /// <summary>
        /// Adds the specified item to the <see cref="ReadOnlyRegister{TElement}"/>.
        /// </summary>
        protected Boolean AddInternal([DisallowNull] TElement item)
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
            this.AppendToInternalRegister(item.GetHashCode());
            lock (this._syncRoot)
            {
                this._items[this._size++] = item;
                this._version++;
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
            return true;
        }

        /// <summary>
        /// Inserts the specified item at the specified index into the <see cref="ReadOnlyRegister{TElement}"/>.
        /// </summary>
        protected Boolean InsertInternal(Int32 index, [DisallowNull] TElement item)
        {
            if (item is null)
            {
                throw new ArgumentNullException(nameof(item));
            }
            if ((UInt32)index > (UInt32)this.Count)
            {
                throw new ArgumentOutOfRangeException(nameof(index),
                                                      INDEX_GREATER_THAN_COUNT);
            }

            if (this.Contains(item))
            {
                return false;
            }

            this.EnsureCapacity(this.Count + 1);
            lock (this._syncRoot)
            {
                if (index < this._size)
                {
                    Array.Copy(this._items,
                               index,
                               this._items,
                               index + 1,
                               this._size - index);
                }
            }
            this.AppendToInternalRegister(item.GetHashCode());
            lock (this._syncRoot)
            {
                this._items[index] = item;
                this._size++;
                this._version++;
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
            return true;
        }

        /// <summary>
        /// Removes the specified item from the <see cref="ReadOnlyRegister{TElement}"/>.
        /// </summary>
        protected Boolean RemoveInternal([DisallowNull] TElement item)
        {
            if (item is null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            Int32 index = this.IndexOf(item);
            if (index > -1)
            {
                lock (this._syncRoot)
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
                }
                this.RemoveFromInternalRegister(item.GetHashCode());
                lock (this._syncRoot)
                {
                    this._version++;
                }
                return true;
            }
            return false;
        }

        /// <summary>
        /// Appends the hashcode to the internal register for quick <see cref="Contains(TElement)"/> calls.
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
            lock (this._syncRoot)
            {
                this._hashTree.Insert(hashcode);
            }
        }

        /// <summary>
        /// Removes the hashcode from the internal register.
        /// </summary>
        protected void RemoveFromInternalRegister(in Int32 hashcode)
        {
            lock (this._syncRoot)
            {
                for (Int32 i = 0; i < this._size; i++)
                {
                    if (this._items[i].GetHashCode() == hashcode)
                    {
                        return;
                    }
                }
            }
            if (_hashTree.RootNode.Value == hashcode)
            {
                this._hashTree = null;
                return;
            }
            lock (this._syncRoot)
            {
                this._hashTree.Remove(hashcode);
            }
        }

        private void Fill(IEnumerable<TElement> collection)
        {
            if (collection is null)
            {
                throw new ArgumentNullException(nameof(collection));
            }

            using IEnumerator<TElement> enumerator = collection.GetEnumerator();
            while (enumerator.MoveNext())
            {
                this.EnsureCapacity(this.Count + 1);
                this.AddInternal(enumerator.Current);
            }
            lock (this._syncRoot)
            {
                if (this._items.Length != this._size)
                {
                    TElement[] array = new TElement[this._size];
                    Array.Copy(this._items, 
                               0, 
                               array, 
                               0, 
                               this._size);
                    this._items = array;
                }
            }
        }

        /// <summary>
        /// Internally contains the current sort direction, if the <see cref="ReadOnlyRegister{TElement}"/> is sorted.
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
        protected IComparer<TElement>? _autoSortComparer = null;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private IEqualityComparer<TElement>? _comparer = null;
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private BinaryTree<Int32>? _hashTree = null;

#pragma warning disable
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        protected const String COMPARER_IS_NULL_WITHOUT_IEQUATABLE = "The Comparer property cannot be null if the type parameter '{0}' does not inherit from the IEquatable<T> interface.";
#pragma warning restore
    }

    // IAutoSortable
    partial class ReadOnlyRegister<TElement> : IAutoSortable<TElement>
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

    // IReadOnlyCollection
    partial class ReadOnlyRegister<TElement> : IReadOnlyCollection<TElement>
    {
        /// <inheritdoc/>
        public override Boolean Contains([DisallowNull] TElement item) => item is null
                                                                            ? throw new ArgumentNullException(nameof(item))
                                                                            : this._hashTree is not null &&
                                                                              this._hashTree.Find(item.GetHashCode()) is not null &&
                                                                              this.IndexOf(item) > -1;
    }

    // IReadOnlyList2
    partial class ReadOnlyRegister<TElement> : IReadOnlyList2<TElement>
    {
        /// <inheritdoc/>
        [Pure]
        public override Int32 IndexOf([DisallowNull] TElement item)
        {
            if (item is null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            lock (this._syncRoot)
            {
                if (this.Comparer is null &&
                    item is IEquatable<TElement> eq)
                {
                    for (Int32 i = 0; i < this._size; i++)
                    {
                        if (eq.Equals(this._items[i]))
                        {
                            return i;
                        }
                    }
                }
                else if (this.Comparer is not null)
                {
                    for (Int32 i = 0; i < this._size; i++)
                    {
                        if (this.Comparer.Equals(this._items[i], 
                                                 item))
                        {
                            return i;
                        }
                    }
                }
            }
            return -1;
        }
    }

    // IReadOnlyRegister
    partial class ReadOnlyRegister<TElement> : IReadOnlyRegister<TElement>
    {
        /// <inheritdoc/>
        [MaybeNull]
        public IEqualityComparer<TElement>? Comparer
        {
            get => this._comparer;
            set
            {
                if (value is null &&
                    !typeof(IEquatable<TElement>).IsAssignableFrom(typeof(TElement)))
                {
                    throw new ArgumentException(String.Format(COMPARER_IS_NULL_WITHOUT_IEQUATABLE,
                                                              typeof(TElement).FullName));
                }
                this._comparer = value;
            }
        }
    }

    // IReadOnlySet
    partial class ReadOnlyRegister<TElement> : IReadOnlySet<TElement>
    {
        /// <inheritdoc/>
        /// <exception cref="ArgumentNullException"/>
        public virtual Boolean IsProperSubsetOf(IEnumerable<TElement> other)
        {
#pragma warning disable
            if (other is null)
            {
                throw new ArgumentNullException(nameof(other));
            }

            if (other == this)
            {
                return false;
            }

            if (other is ICollection<TElement> collection)
            {
                if (this._size == 0)
                {
                    return collection.Count > 0;
                }

                if (this._size >= collection.Count)
                {
                    return false;
                }
            }

            if (other is IReadOnlyRegister<TElement> register)
            {
                if ((this.Comparer is null &&
                    register.Comparer is null) ||
                    (this.Comparer is not null &&
                    this.Comparer.Equals(register.Comparer)))
                {
                    for (Int32 i = 0; i < this._size; i++)
                    {
                        if (!(register as IReadOnlyCollection2<TElement>).Contains(this._items[i]) &&
                            !(register as IReadOnlySet<TElement>).Contains(this._items[i]))
                        {
                            return false;
                        }
                    }
                    return true;
                }
            }

            return this.FindInOther(other);
#pragma warning restore
        }

        /// <inheritdoc/>
        /// <exception cref="ArgumentNullException"/>
        public virtual Boolean IsProperSupersetOf(IEnumerable<TElement> other)
        {
#pragma warning disable
            if (other is null)
            {
                throw new ArgumentNullException(nameof(other));
            }

            if (other == this ||
                this._size == 0)
            {
                return false;
            }

            if (other is ICollection<TElement> collection &&
                collection.Count == 0)
            {
                return true;
            }

            if (other is IReadOnlyRegister<TElement> register)
            {
                if (register.Count >= this._size)
                {
                    return false;
                }
                if ((this.Comparer is null &&
                    register.Comparer is null) ||
                    (this.Comparer is not null &&
                    this.Comparer.Equals(register.Comparer)))
                {
                    for (Int32 i = 0; i < register.Count; i++)
                    {
                        if (!this.Contains(register[i]))
                        {
                            return false;
                        }
                    }
                    return true;
                }
            }

            if (!other.Any())
            {
                return true;
            }

            foreach (TElement item in other)
            {
                if (!this.Contains(item))
                {
                    return false;
                }
            }
            return true;
#pragma warning restore
        }

        /// <inheritdoc/>
        /// <exception cref="ArgumentNullException"/>
        public virtual Boolean IsSubsetOf(IEnumerable<TElement> other)
        {
#pragma warning disable
            if (other is null)
            {
                throw new ArgumentNullException(nameof(other));
            }

            if (this._size == 0 ||
                other == this)
            {
                return true;
            }

            if (other is ICollection<TElement> collection &&
                this._size > collection.Count)
            {
                return false;
            }

            if (other is IReadOnlyRegister<TElement> register)
            {
                if ((this.Comparer is null &&
                    register.Comparer is null) ||
                    (this.Comparer is not null &&
                    this.Comparer.Equals(register.Comparer)))
                {
                    for (Int32 i = 0; i < this._size; i++)
                    {
                        if (!(register as IReadOnlyCollection2<TElement>).Contains(this._items[i]) &&
                            !(register as IReadOnlySet<TElement>).Contains(this._items[i]))
                        {
                            return false;
                        }
                    }
                    return true;
                }
            }

            return this.FindInOther(other);
#pragma warning restore
        }

        /// <inheritdoc/>
        /// <exception cref="ArgumentNullException"/>
        public virtual Boolean IsSupersetOf(IEnumerable<TElement> other)
        {
#pragma warning disable
            if (other is null)
            {
                throw new ArgumentNullException(nameof(other));
            }

            if ((other is ICollection<TElement> collection &&
                collection.Count == 0) ||
                other == this)
            {
                return true;
            }

            if (other is IReadOnlyRegister<TElement> register)
            {
                if (register.Count > this._size)
                {
                    return false;
                }
                if ((this.Comparer is null &&
                    register.Comparer is null) ||
                    (this.Comparer is not null &&
                    this.Comparer.Equals(register.Comparer)))
                {
                    for (Int32 i = 0; i < register.Count; i++)
                    {
                        if (!this.Contains(register[i]))
                        {
                            return false;
                        }
                    }
                    return true;
                }
            }

            if (!other.Any())
            {
                return true;
            }

            foreach (TElement item in other)
            {
                if (!this.Contains(item))
                {
                    return false;
                }
            }
            return true;
#pragma warning restore
        }

        /// <inheritdoc/>
        /// <exception cref="ArgumentNullException"/>
        public virtual Boolean Overlaps(IEnumerable<TElement> other)
        {
            if (other is null)
            {
                throw new ArgumentNullException(nameof(other));
            }

            if (this._size == 0)
            {
                return false;
            }

            foreach (TElement item in other)
            {
#pragma warning disable
                if (this.Contains(item))
#pragma warning restore
                {
                    return true;
                }
            }
            return false;
        }

        /// <inheritdoc/>
        /// <exception cref="ArgumentNullException"/>
        public virtual Boolean SetEquals(IEnumerable<TElement> other)
        {
#pragma warning disable
            if (other is null)
            {
                throw new ArgumentNullException(nameof(other));
            }

            if (other == this)
            {
                return true;
            }

            if (other is ICollection<TElement> collection)
            {
                if (collection.Count != this._size)
                {
                    return false;
                }
            }

            if (other is IReadOnlyRegister<TElement> register)
            {
                if ((this.Comparer is null &&
                    register.Comparer is null) ||
                    (this.Comparer is not null &&
                    this.Comparer.Equals(register.Comparer)))
                {
                    for (Int32 i = 0; i < register.Count; i++)
                    {
                        if (!this.Contains(register[i]))
                        {
                            return false;
                        }
                    }
                    return true;
                }
            }

            foreach (TElement item in other)
            {
                if (!this.Contains(item))
                {
                    return false;
                }
            }
            return true;
#pragma warning restore
        }

        /// <summary>
        /// Attempts to find all of the items in this collection in the specified other enumerable.
        /// </summary>
        /// <param name="other">The enumerable to search through.</param>
        /// <returns><see langword="true"/> if all of the items of this collection are also present in the enumerable; otherwise, <see langword="false"/></returns>
        protected Boolean FindInOther(IEnumerable<TElement> other)
        {
            BitArray arr = new(this._size);

            foreach (TElement item in other)
            {
#pragma warning disable
                Int32 index = this.IndexOf(item);
#pragma warning restore
                if (index >= 0)
                {
                    if (!arr.Get(index))
                    {
                        arr.Set(index, true);
                    }
                }
            }

            for (Int32 i = 0; i < arr.Count; i++)
            {
                if (!arr.Get(i))
                {
                    return false;
                }
            }
            return true;
        }
    }
}
