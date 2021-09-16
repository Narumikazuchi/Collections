using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;

namespace Narumikazuchi.Collections.Abstract
{
    /// <summary>
    /// Represents a strongly typed list of objects, which can be accessed by index. 
    /// </summary>
    public abstract partial class ListBase<TElement> : ReadOnlyListBase<TElement>
    {
        /// <summary>
        /// Adds the elements of the specified collection to the end of the <see cref="ListBase{T}"/>.
        /// </summary>
        /// <param name="collection">The collection of items to add.</param>
        /// <exception cref="ArgumentNullException" />
        /// <exception cref="ArgumentOutOfRangeException" />
        /// <exception cref="InvalidOperationException" />
        public virtual void AddRange([DisallowNull] IEnumerable<TElement> collection) => 
            this.InsertRange(this._size, 
                             collection);

        /// <summary>
        /// Inserts the items from the specified collection into this <see cref="ListBase{T}"/> starting at the specified index.
        /// </summary>
        /// <param name="index">The index where to start inserting the new items.</param>
        /// <param name="collection">The collection of items to insert.</param>
        /// <exception cref="ArgumentNullException" />
        /// <exception cref="ArgumentOutOfRangeException" />
        /// <exception cref="InvalidOperationException" />
        public virtual void InsertRange(Int32 index, 
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
            if ((UInt32)index > (UInt32)this._size)
            {
                throw new ArgumentOutOfRangeException(nameof(index), 
                                                      INDEX_GREATER_THAN_COUNT);
            }

            if (collection is ICollection<TElement> c)
            {
                Int32 count = c.Count;
                if (count > 0)
                {
                    this.EnsureCapacity(this.Count + count);

                    if (index < this.Count)
                    {
                        lock (this._syncRoot)
                        {
                            Array.Copy(this._items, 
                                       index, 
                                       this._items, 
                                       index + count, 
                                       this._size - index);
                        }
                    }

                    if (ReferenceEquals(this, c))
                    {
                        lock (this._syncRoot)
                        {
                            // Copy first part of _items to insert location
                            Array.Copy(this._items, 
                                       0, 
                                       this._items, 
                                       index, 
                                       index);
                            // Copy last part of _items back to inserted location
                            Array.Copy(this._items, 
                                       index + count, 
                                       this._items, 
                                       index * 2, 
                                       this._size - index);
                        }
                    }
                    else
                    {
                        TElement[] insert = new TElement[count];
                        c.CopyTo(insert, 0);
                        lock (this._syncRoot)
                        {
                            insert.CopyTo(this._items, 
                                          index);
                        }
                    }

                    lock (this._syncRoot)
                    {
                        this._size += count;
                    }
                }
            }
            else
            {
                using IEnumerator<TElement> enumerator = collection.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    if (enumerator.Current is null)
                    {
                        continue;
                    }
                    this.Insert(index++, 
                                enumerator.Current);
                }
            }
            lock (this._syncRoot)
            {
                this._version++;
            }
        }

        /// <summary>
        /// Moves the item at the given index one position in the specified direction in the <see cref="ListBase{T}"/>.
        /// </summary>
        public virtual Boolean MoveItem(in Int32 index, 
                                        in ItemMoveDirection direction) => 
            this.MoveItem(index, 
                          direction, 
                          1);

        /// <summary>
        /// Moves the item at the given index the given amount of positions in the specified direction in the <see cref="ListBase{T}"/>.
        /// </summary>
        public virtual Boolean MoveItem(in Int32 index, 
                                        in ItemMoveDirection direction, 
                                        Int32 positions)
        {
            TElement tmp;
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
        /// Moves the item one position in the specified direction in the <see cref="ListBase{T}"/>.
        /// </summary>
        public virtual Boolean MoveItem([DisallowNull] in TElement item, 
                                        in ItemMoveDirection direction) => 
            this.MoveItem(item, 
                          direction, 
                          1);

        /// <summary>
        /// Moves the item at the given index the given amount of positions in the specified direction in the <see cref="ListBase{T}"/>.
        /// </summary>
        public virtual Boolean MoveItem([DisallowNull] in TElement item, 
                                        in ItemMoveDirection direction, 
                                        Int32 positions)
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
        /// Removes all items from the <see cref="ListBase{T}"/> that match the specified condition.
        /// </summary>
        /// <param name="predicate">The condition to determine if an item should be removed.</param>
        /// <returns>The number of items removed from the list</returns>
        /// <exception cref="ArgumentNullException" />
        /// <exception cref="IndexOutOfRangeException" />
        /// <exception cref="InvalidOperationException" />
        public virtual Int32 RemoveAll([DisallowNull] Func<TElement, Boolean> predicate)
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
                while (current < this._size)
                {
                    if (this._version != v)
                    {
                        throw new InvalidOperationException(COLLECTION_CHANGED);
                    }
                    while (current < this._size &&
                           predicate(this._items[current]))
                    {
                        current++;
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
                return result;
            }
        }

        /// <summary>
        /// Removes a range of items from the <see cref="ListBase{T}"/>.
        /// </summary>
        /// <param name="index">The index of the first item to remove.</param>
        /// <param name="count">The amount of items to remove.</param>
        /// <exception cref="ArgumentException" />
        /// <exception cref="ArgumentOutOfRangeException" />
        /// <exception cref="IndexOutOfRangeException" />
        /// <exception cref="InvalidOperationException" />
        public virtual void RemoveRange(in Int32 index, 
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
        }

        /// <summary>
        /// Reverses the order of items in the entire <see cref="ListBase{T}"/>.
        /// </summary>
        /// <exception cref="ArgumentException" />
        /// <exception cref="ArgumentOutOfRangeException" />
        /// <exception cref="InvalidOperationException" />
        public virtual void Reverse() => 
            this.Reverse(0, 
                         this._size);
        /// <summary>
        /// Reverses the order of items in the specified range in the <see cref="ListBase{T}"/>.
        /// </summary>
        /// <param name="index">The index of the first item in the range.</param>
        /// <param name="count">The length of the range to reverse.</param>
        /// <exception cref="ArgumentException" />
        /// <exception cref="ArgumentOutOfRangeException" />
        /// <exception cref="InvalidOperationException" />
        public virtual void Reverse(in Int32 index, 
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
                Array.Reverse(this._items, 
                              index, 
                              count);
                this._version++;
            }
        }

        /// <summary>
        /// Gets or sets the amount of items the <see cref="ListBase{T}"/> can hold without resizing itself.
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
                    throw new InvalidOperationException(SIZE_IS_FIXED);
                }
                if (value < this._size)
                {
                    throw new ArgumentOutOfRangeException(nameof(value),
                                                          CAPACITY_SMALLER_THAN_COUNT);
                }
                if (value != this.Capacity)
                {
                    if (value > 0)
                    {
                        TElement[] array = new TElement[value];
                        lock (this._syncRoot)
                        {
                            if (this._size > 0)
                            {
                                Array.Copy(this._items, 
                                           0, 
                                           array, 
                                           0, 
                                           this._size);
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
    }

    // Non-Public
    partial class ListBase<TElement>
    {
        /// <summary>
        /// Initializes a new empty instance of the <see cref="ListBase{T}"/> class.
        /// </summary>
        protected ListBase() : 
            base() 
        { }
        /// <summary>
        /// Initializes a new empty instance of the <see cref="ListBase{T}"/> class having the specified capacity.
        /// </summary>
        /// <param name="capacity">The capacity of this list.</param>
        /// <exception cref="ArgumentOutOfRangeException" />
        protected ListBase(in Int32 capacity)
        {
            if (capacity < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(capacity), 
                                                      "The capacity of the list cannot be negative.");
            }

            this._items = capacity == 0 
                                ? _emptyArray 
                                : new TElement[capacity];
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="ListBase{T}"/> class having the specified collection of items.
        /// </summary>
        /// <param name="collection">The initial collection of items in this list.</param>
        /// <exception cref="ArgumentException" />
        /// <exception cref="ArgumentNullException" />
        /// <exception cref="InvalidOperationException" />
        protected ListBase([DisallowNull] IEnumerable<TElement> collection) : 
            base(collection) 
        { }

#pragma warning disable
        /// <summary>
        /// Error message, when the index parameter is greater than the amount of items in the collection.
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        protected const String INDEX_GREATER_THAN_COUNT = "The specified index is greater than the amount of items in the list.";
        /// <summary>
        /// Error message, when the new capacity is smaller than the amount of items in the collection.
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        protected const String CAPACITY_SMALLER_THAN_COUNT = "The specified capacity is smaller than the amount of items in the list.";
#pragma warning restore
    }

    // ICollection
    partial class ListBase<TElement> : ICollection<TElement>
    {
        /// <summary>
        /// Copies the entire <see cref="ListBase{T}"/> to a compatible one-dimensional <see cref="Array"/>, 
        /// starting at the specified index of the target array.
        /// </summary>
        /// <param name="array">
        /// The one-dimensional <see cref="Array"/> that is the destination of the elements copied
        /// from <see cref="ListBase{T}"/>. The <see cref="Array"/> must have zero-based indexing.</param>
        /// <param name="index">The zero-based index in array at which copying begins.</param>
        /// <exception cref="ArgumentException" />
        /// <exception cref="ArgumentNullException" />
        /// <exception cref="ArgumentOutOfRangeException" />
        [Pure]
        public virtual void CopyTo([DisallowNull] Array array, Int32 index)
        {
            if (array is null)
            {
                throw new ArgumentNullException(nameof(array));
            }

            if (array is TElement[] arr)
            {
                this.CopyTo(arr, 
                            index);
            }
        }

        /// <summary>
        /// Adds an object to the end of the <see cref="ListBase{T}"/>.
        /// </summary>
        /// <param name="item">The object to be added to the end of the <see cref="ListBase{T}"/>. The value can be null for reference types.</param>
        /// <exception cref="ArgumentNullException" />
        /// <exception cref="InvalidOperationException" />
        public virtual void Add([DisallowNull] TElement item)
        {
            if (item is null)
            {
                throw new ArgumentNullException(nameof(item));
            }
            if (this.IsReadOnly)
            {
                throw new InvalidOperationException(LIST_IS_READONLY);
            }

            this.EnsureCapacity(this.Count + 1);
            lock (this._syncRoot)
            {
                this._items[this._size++] = item;
                this._version++;
            }
        }

        /// <summary>
        /// Removes all elements from the <see cref="ListBase{T}"/>.
        /// </summary>
        /// <exception cref="InvalidOperationException" />
        public virtual void Clear()
        {
            if (this.IsReadOnly)
            {
                throw new InvalidOperationException(LIST_IS_READONLY);
            }

            lock (this._syncRoot)
            {
                if (this._size > 0)
                {
                    Array.Clear(this._items, 
                                0, 
                                this._size);
                    this._size = 0;
                }
                this._version++;
            }
        }

        /// <summary>
        /// Removes the first occurrence of a specific object from the <see cref="ListBase{T}"/>.
        /// </summary>
        /// <param name="item">The object to remove from the <see cref="ListBase{T}"/>. The value can be null for reference types.</param>
        /// <returns>
        /// <see langword="true"/> if item is successfully removed; otherwise, <see langword="false"/>.
        /// This method also returns <see langword="false"/> if item was not found in the original <see cref="ListBase{T}"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException" />
        /// <exception cref="InvalidOperationException" />
        public virtual Boolean Remove([DisallowNull] TElement item)
        {
            if (item is null)
            {
                throw new ArgumentNullException(nameof(item));
            }
            if (this.IsReadOnly)
            {
                throw new InvalidOperationException(LIST_IS_READONLY);
            }

            lock (this._syncRoot)
            {
                Int32 index = this.IndexOf(item);
                if (index > -1)
                {
                    this.RemoveAt(index);
                    return true;
                }
                return false;
            }
        }

        /// <inheritdoc />
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public virtual Boolean IsSynchronized => true;
        /// <inheritdoc />
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public override Boolean IsReadOnly { get; } = false;
    }

    // IIndexSetter
    partial class ListBase<TElement> : IIndexSetter<Int32, TElement>
    {
        /// <inheritdoc />
        /// <exception cref="IndexOutOfRangeException" />
        /// <exception cref="InvalidOperationException" />
        public override TElement this[Int32 index]
        {
            get
            {
                lock (this._syncRoot)
                {
                    return (UInt32)index >= (UInt32)this._size 
                                        ? throw new IndexOutOfRangeException() 
                                        : this._items[index];
                }
            }
            set
            {
                lock (this._syncRoot)
                {
                    if ((UInt32)index >= (UInt32)this._size)
                    {
                        throw new IndexOutOfRangeException();
                    }
                    this._items[index] = value;
                }
            }
        }
    }

    // IList
    partial class ListBase<TElement> : IList<TElement>
    {
        /// <summary>
        /// Inserts an element into the <see cref="ListBase{T}"/> at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index at which item should be inserted.</param>
        /// <param name="item">The object to insert. The value can be null for reference types.</param>
        /// <exception cref="ArgumentOutOfRangeException" />
        /// <exception cref="InvalidCastException" />
        /// <exception cref="InvalidOperationException" />
        public virtual void Insert(Int32 index, [DisallowNull] TElement item)
        {
            if (item is null)
            {
                throw new ArgumentNullException(nameof(item));
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
                this._items[index] = item;
                this._size++;
                this._version++;
            }
        }

        /// <summary>
        /// Removes the element at the specified index of the <see cref="ListBase{T}"/>.
        /// </summary>
        /// <param name="index">The zero-based index of the element to remove.</param>
        /// <exception cref="ArgumentOutOfRangeException" />
        /// <exception cref="InvalidOperationException" />
        public virtual void RemoveAt(Int32 index)
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
            }
        }
    }
}