using Narumikazuchi.Collections.Abstract;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;

namespace Narumikazuchi.Collections
{
    /// <summary>
    /// Represents a collection where every object is only contained once. The procedure to check whether the object is already in the <see cref="Register{T}"/> can be specified
    /// with a corresponding <see cref="EqualityComparer{T}"/> or <see cref="EqualityComparison{T}"/> delegate.
    /// </summary>
#pragma warning disable
    public class Register<T> : ReadOnlyRegister<T>, ICollection<T>, IList<T>
#pragma warning restore
    {
        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="Register{T}"/> class.
        /// </summary>
        public Register() : this(4, (IEqualityComparer<T>?)null) { }
        /// <summary>
        /// Initializes a new instance of the <see cref="Register{T}"/> class with the specified capacity.
        /// </summary>
        public Register(in Int32 capacity) : this(capacity, (IEqualityComparer<T>?)null) { }
        /// <summary>
        /// Initializes a new instance of the <see cref="Register{T}"/> class using the specified function to check items for equality.
        /// </summary>
        public Register(EqualityComparison<T> comparison) : this(4, comparison) { }
        /// <summary>
        /// Initializes a new instance of the <see cref="Register{T}"/> class with the specified collection as items.
        /// </summary>
        public Register([DisallowNull] IEnumerable<T> collection) : base(collection) { }
        /// <summary>
        /// Initializes a new instance of the <see cref="Register{T}"/> class with the specified capacity using the specified function to check items for equality.
        /// </summary>
        public Register(in Int32 capacity, EqualityComparison<T> comparison) : base(comparison)
        {
            if (capacity < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(capacity), "The capacity of the list cannot be negative.");
            }

            this._items = capacity == 0 ? _emptyArray : (new T[capacity]);
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="Register{T}"/> class with the specified collection as items using the specified function to check items for equality.
        /// </summary>
        public Register(EqualityComparison<T> comparison, [DisallowNull] IEnumerable<T> collection) : base(comparison, collection) { }
        /// <summary>
        /// Initializes a new instance of the <see cref="Register{T}"/> class using the specified <see cref="IEqualityComparer{T}"/> to check items for equality.
        /// </summary>
        public Register(IEqualityComparer<T>? comparer) : this(4, comparer) { }
        /// <summary>
        /// Initializes a new instance of the <see cref="Register{T}"/> class with the specified capacity using the specified <see cref="IEqualityComparer{T}"/> to check items for equality.
        /// </summary>
        public Register(in Int32 capacity, IEqualityComparer<T>? comparer) : base(comparer)
        {
            if (capacity < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(capacity), "The capacity of the list cannot be negative.");
            }

            this._items = capacity == 0 ? _emptyArray : (new T[capacity]);
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="Register{T}"/> class with the specified collection as items using the specified <see cref="IEqualityComparer{T}"/> to check items for equality.
        /// </summary>
        public Register(IEqualityComparer<T>? comparer, [DisallowNull] IEnumerable<T> collection) : base(comparer, collection) { }

        #endregion

        #region Collection Management

        /// <summary>
        /// Adds the elements of the specified collection to the end of the <see cref="Register{T}"/>.
        /// </summary>
        /// <param name="collection">The collection of items to add.</param>
        /// <exception cref="ArgumentNullException" />
        /// <exception cref="ArgumentOutOfRangeException" />
        /// <exception cref="InvalidOperationException" />
        public virtual void AddRange([DisallowNull] IEnumerable<T> collection) => this.InsertRange(this._size, collection);

        /// <summary>
        /// Inserts the items from the specified collection into this <see cref="Register{T}"/> starting at the specified index.
        /// </summary>
        /// <param name="index">The index where to start inserting the new items.</param>
        /// <param name="collection">The collection of items to insert.</param>
        /// <exception cref="ArgumentNullException" />
        /// <exception cref="ArgumentOutOfRangeException" />
        /// <exception cref="InvalidOperationException" />
        public virtual void InsertRange(Int32 index, [DisallowNull] IEnumerable<T> collection)
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
                this.Insert(index++, enumerator.Current);
            }
            lock (this._syncRoot)
            {
                this._version++;
            }
        }

        /// <summary>
        /// Moves the item at the given index one position in the specified direction in the <see cref="Register{T}"/>.
        /// </summary>
        public virtual Boolean MoveItem(Int32 index, ItemMoveDirection direction) => this.MoveItem(index, direction, 1);

        /// <summary>
        /// Moves the item at the given index the given amount of positions in the specified direction in the <see cref="Register{T}"/>.
        /// </summary>
        public virtual Boolean MoveItem(Int32 index, ItemMoveDirection direction, Int32 positions)
        {
            T tmp;
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
                        }
                    }
                    else
                    {
                        return false;
                    }
                }
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
                        }
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        /// <summary>
        /// Moves the item one position in the specified direction in the <see cref="Register{T}"/>.
        /// </summary>
        public virtual Boolean MoveItem([DisallowNull] in T item, ItemMoveDirection direction) => this.MoveItem(item, direction, 1);

        /// <summary>
        /// Moves the item at the given index the given amount of positions in the specified direction in the <see cref="Register{T}"/>.
        /// </summary>
        public virtual Boolean MoveItem([DisallowNull] in T item, ItemMoveDirection direction, Int32 positions)
        {
            if (item is null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            Int32 index = this.IndexOf(item);
            if (index == -1)
            {
                return false;
            }
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
                        }
                        index--;
                    }
                    else
                    {
                        return false;
                    }
                }
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
                        }
                        index++;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        /// <summary>
        /// Removes all items from the <see cref="Register{T}"/> that match the specified condition.
        /// </summary>
        /// <param name="predicate">The condition to determine if an item should be removed.</param>
        /// <returns>The number of items removed from the list</returns>
        /// <exception cref="ArgumentNullException" />
        /// <exception cref="IndexOutOfRangeException" />
        /// <exception cref="InvalidOperationException" />
        public virtual Int32 RemoveAll([DisallowNull] Func<T, Boolean> predicate)
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
                while (current < this._size)
                {
                    if (this._version != v)
                    {
                        throw new InvalidOperationException("The collection changed during enumeration.");
                    }
                    while (current < this._size &&
                           predicate(this._items[current]))
                    {
                        this.RemoveFromInternalRegister(this._items[current].GetHashCode());
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
                return result;
            }
        }

        /// <summary>
        /// Removes a range of items from the <see cref="Register{T}"/>.
        /// </summary>
        /// <param name="index">The index of the first item to remove.</param>
        /// <param name="count">The amount of items to remove.</param>
        /// <exception cref="ArgumentException" />
        /// <exception cref="ArgumentOutOfRangeException" />
        /// <exception cref="IndexOutOfRangeException" />
        /// <exception cref="InvalidOperationException" />
        public virtual void RemoveRange(in Int32 index, in Int32 count)
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
#pragma warning disable
                this.RemoveFromInternalRegister(list[i].GetHashCode());
#pragma warning restore
            }
        }

        /// <summary>
        /// Reverses the order of items in the entire <see cref="Register{T}"/>.
        /// </summary>
        /// <exception cref="ArgumentException" />
        /// <exception cref="ArgumentOutOfRangeException" />
        /// <exception cref="InvalidOperationException" />
        public virtual void Reverse() => this.Reverse(0, this._size);
        /// <summary>
        /// Reverses the order of items in the specified range in the <see cref="Register{T}"/>.
        /// </summary>
        /// <param name="index">The index of the first item in the range.</param>
        /// <param name="count">The length of the range to reverse.</param>
        /// <exception cref="ArgumentException" />
        /// <exception cref="ArgumentOutOfRangeException" />
        /// <exception cref="InvalidOperationException" />
        public virtual void Reverse(in Int32 index, in Int32 count)
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
                Array.Reverse(this._items, index, count);
                this._version++;
            }
        }

        #endregion

        #region ICollection

        /// <summary>
        /// Copies the entire <see cref="Register{T}"/> to a compatible one-dimensional <see cref="Array"/>, 
        /// starting at the specified index of the target array.
        /// </summary>
        /// <param name="array">
        /// The one-dimensional <see cref="Array"/> that is the destination of the elements copied
        /// from <see cref="SearchableListBase{T}"/>. The <see cref="Array"/> must have zero-based indexing.</param>
        /// <param name="index">The zero-based index in array at which copying begins.</param>
        /// <exception cref="ArgumentException" />
        /// <exception cref="ArgumentNullException" />
        /// <exception cref="ArgumentOutOfRangeException" />
        [Pure]
        public virtual void CopyTo([DisallowNull] Array array, Int32 index)
        {
            if (array is T[] arr)
            {
                this.CopyTo(arr, index);
            }
        }

        /// <summary>
        /// Adds an object to the end of the <see cref="SearchableListBase{T}"/>.
        /// </summary>
        /// <param name="item">The object to be added to the end of the <see cref="SearchableListBase{T}"/>. The value can be null for reference types.</param>
        /// <exception cref="InvalidOperationException" />
#pragma warning disable
        public virtual void Add([DisallowNull] T item) => this.AddInternal(item);
#pragma warning restore

        /// <summary>
        /// Removes all elements from the <see cref="SearchableListBase{T}"/>.
        /// </summary>
        /// <exception cref="InvalidOperationException" />
        public virtual void Clear()
        {
            if (this.IsReadOnly)
            {
                throw new InvalidOperationException("The list is read-only.");
            }

            lock (this._syncRoot)
            {
                if (this._size > 0)
                {
                    Array.Clear(this._items, 0, this._size);
                    this._size = 0;
                }
                this._version++;
            }
        }

        /// <summary>
        /// Removes the first occurrence of a specific object from the <see cref="SearchableListBase{T}"/>.
        /// </summary>
        /// <param name="item">The object to remove from the <see cref="SearchableListBase{T}"/>. The value can be null for reference types.</param>
        /// <returns>
        /// <see langword="true"/> if item is successfully removed; otherwise, <see langword="false"/>.
        /// This method also returns <see langword="false"/> if item was not found in the original <see cref="SearchableListBase{T}"/>.
        /// </returns>
        /// <exception cref="InvalidOperationException" />
#pragma warning disable
        public virtual Boolean Remove([DisallowNull] T item) => this.RemoveInternal(item);
#pragma warning restore

        #region Properties

        /// <inheritdoc />
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public virtual Boolean IsSynchronized => true;

        /// <inheritdoc />
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public override Boolean IsReadOnly { get; } = false;

        #endregion

        #endregion

        #region IList

        /// <summary>
        /// Inserts an element into the <see cref="Register{T}"/> at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index at which item should be inserted.</param>
        /// <param name="item">The object to insert. The value can be null for reference types.</param>
        /// <exception cref="ArgumentOutOfRangeException" />
        /// <exception cref="InvalidCastException" />
        /// <exception cref="InvalidOperationException" />
#pragma warning disable
        public virtual void Insert(Int32 index, [DisallowNull] T item) => this.InsertInternal(index, item);
#pragma warning restore

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

        /// <summary>
        /// Removes the element at the specified index of the <see cref="Register{T}"/>.
        /// </summary>
        /// <param name="index">The zero-based index of the element to remove.</param>
        /// <exception cref="ArgumentOutOfRangeException" />
        /// <exception cref="InvalidOperationException" />
        public virtual void RemoveAt(Int32 index)
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
                this._size--;
                if (index < this._size)
                {
                    Array.Copy(this._items, index + 1, this._items, index, this._size - index);
                }
                this._items[this._size] = default;
                this._version++;
            }
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the amount of items the <see cref="Register{T}"/> can hold without resizing itself.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException" />
        /// <exception cref="InvalidOperationException" />
        [DebuggerBrowsable(DebuggerBrowsableState.Collapsed)]
        public virtual Int32 Capacity
        {
            get
            {
                lock (this._syncRoot)
                {
                    return this._items.Length;
                }
            }
            set
            {
                if (this.IsFixedSize)
                {
                    throw new InvalidOperationException("The size for the list is fixed.");
                }
                if (value < this._size)
                {
                    throw new ArgumentOutOfRangeException(nameof(value), "The new value was smaller than the count of items in the list.");
                }
                if (value != this.Capacity)
                {
                    if (value > 0)
                    {
                        T[] array = new T[value];
                        lock (this._syncRoot)
                        {
                            if (this._size > 0)
                            {
                                Array.Copy(this._items, 0, array, 0, this._size);
                            }
                            this._items = array;
                        }
                    }
                    else
                    {
                        lock (this._syncRoot)
                        {
                            this._items = _emptyArray;
                        }
                    }
                }
            }
        }

        #endregion
    }
}
