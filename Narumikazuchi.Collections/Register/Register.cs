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
    /// Represents a collection where every object is only contained once. The procedure to check whether the object is already in the <see cref="Register{TElement}"/> can be specified
    /// with a corresponding <see cref="EqualityComparer{T}"/> or <see cref="EqualityComparison{T}"/> delegate.
    /// </summary>
    public partial class Register<TElement> : ReadOnlyRegister<TElement>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Register{TElement}"/> class.
        /// </summary>
        public Register() : 
            this(4, 
                 (IEqualityComparer<TElement>?)null) 
        { }
        /// <summary>
        /// Initializes a new instance of the <see cref="Register{TElement}"/> class with the specified capacity.
        /// </summary>
        public Register(in Int32 capacity) : 
            this(capacity, 
                 (IEqualityComparer<TElement>?)null) 
        { }
        /// <summary>
        /// Initializes a new instance of the <see cref="Register{TElement}"/> class using the specified function to check items for equality.
        /// </summary>
        public Register([AllowNull] EqualityComparison<TElement> comparison) : 
            this(4, 
                 comparison) 
        { }
        /// <summary>
        /// Initializes a new instance of the <see cref="Register{TElement}"/> class with the specified collection as items.
        /// </summary>
        public Register([DisallowNull] IEnumerable<TElement> collection) : 
            base(collection) 
        { }
        /// <summary>
        /// Initializes a new instance of the <see cref="Register{TElement}"/> class with the specified capacity using the specified function to check items for equality.
        /// </summary>
        public Register(in Int32 capacity, 
                        [AllowNull] EqualityComparison<TElement> comparison) : 
            base(comparison)
        {
            if (capacity < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(capacity), 
                                                      CAPACITY_IS_NEGATIVE);
            }

            this._items = capacity == 0 
                            ? _emptyArray 
                            : new TElement[capacity];
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="Register{TElement}"/> class with the specified collection as items using the specified function to check items for equality.
        /// </summary>
        public Register([AllowNull] EqualityComparison<TElement> comparison, 
                        [DisallowNull] IEnumerable<TElement> collection) : 
            base(comparison, 
                 collection) 
        { }
        /// <summary>
        /// Initializes a new instance of the <see cref="Register{TElement}"/> class using the specified <see cref="IEqualityComparer{T}"/> to check items for equality.
        /// </summary>
        public Register([AllowNull] IEqualityComparer<TElement>? comparer) : 
            this(4, 
                 comparer) 
        { }
        /// <summary>
        /// Initializes a new instance of the <see cref="Register{TElement}"/> class with the specified capacity using the specified <see cref="IEqualityComparer{T}"/> to check items for equality.
        /// </summary>
        public Register(in Int32 capacity, 
                        [AllowNull] IEqualityComparer<TElement>? comparer) : 
            base(comparer)
        {
            if (capacity < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(capacity),
                                                      CAPACITY_IS_NEGATIVE);
            }

            this._items = capacity == 0
                            ? _emptyArray
                            : new TElement[capacity];
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="Register{TElement}"/> class with the specified collection as items using the specified <see cref="IEqualityComparer{T}"/> to check items for equality.
        /// </summary>
        public Register([AllowNull] IEqualityComparer<TElement>? comparer, 
                        [DisallowNull] IEnumerable<TElement> collection) : 
            base(comparer, 
                 collection) 
        { }

        /// <summary>
        /// Adds the elements of the specified collection to the end of the <see cref="Register{TElement}"/>.
        /// </summary>
        /// <param name="collection">The collection of items to add.</param>
        /// <exception cref="ArgumentNullException" />
        /// <exception cref="ArgumentOutOfRangeException" />
        /// <exception cref="InvalidOperationException" />
        public virtual void AddRange([DisallowNull] IEnumerable<TElement> collection) => 
            this.InsertRange(this._size, 
                             collection);

        /// <summary>
        /// Inserts the items from the specified collection into this <see cref="Register{TElement}"/> starting at the specified index.
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
            if ((UInt32)index > (UInt32)this.Count)
            {
                throw new ArgumentOutOfRangeException(nameof(index),
                                                      INDEX_GREATER_THAN_COUNT);
            }

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
                this.Insert(index++, 
                            enumerator.Current);
            }
            lock (this._syncRoot)
            {
                this._version++;
            }
        }

        /// <summary>
        /// Removes all items from the <see cref="Register{TElement}"/> that match the specified condition.
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

            List<TElement> items = new();
            Int32 result = 0;
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
                        items.Add(this._items[current++]);
                    }

                    if (current < this._size)
                    {
                        this._items[free++] = this._items[current++];
                    }
                }

                Array.Clear(this._items,
                            free,
                            this._size - free);
                result = this._size - free;
                this._size = free;
                this._version++;
            }

            for (Int32 i = 0; i < items.Count; i++)
            {
                this.RemoveFromInternalRegister(items[i].GetHashCode());
            }
            return result;
        }

        /// <summary>
        /// Removes a range of items from the <see cref="Register{TElement}"/>.
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

            IList<TElement> list = this.GetRange(index, 
                                                 count);
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
            for (Int32 i = 0; i < list.Count; i++)
            {
                this.RemoveFromInternalRegister(list[i].GetHashCode());
            }
        }

        /// <summary>
        /// Reverses the order of items in the entire <see cref="Register{TElement}"/>.
        /// </summary>
        /// <exception cref="ArgumentException" />
        /// <exception cref="ArgumentOutOfRangeException" />
        /// <exception cref="InvalidOperationException" />
        public virtual void Reverse() => 
            this.Reverse(0, 
                         this._size);
        /// <summary>
        /// Reverses the order of items in the specified range in the <see cref="Register{TElement}"/>.
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
        /// Gets or sets the amount of items the <see cref="Register{TElement}"/> can hold without resizing itself.
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
    partial class Register<TElement> : IRegister<TElement>
    {
#pragma warning disable
        /// <summary>
        /// Error message, when the new capacity is negative.
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        protected const String CAPACITY_IS_NEGATIVE = "The capacity cannot be negative.";
        /// <summary>
        /// Error message, when the new capacity is smaller than the amount of items in the collection.
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        protected const String CAPACITY_SMALLER_THAN_COUNT = "The specified capacity is smaller than the amount of items in the list.";
#pragma warning restore
    }

    // ICollection
    partial class Register<TElement> : ICollection<TElement>
    {

        /// <summary>
        /// Copies the entire <see cref="Register{TElement}"/> to a compatible one-dimensional <see cref="Array"/>, 
        /// starting at the specified index of the target array.
        /// </summary>
        /// <param name="array">
        /// The one-dimensional <see cref="Array"/> that is the destination of the elements copied
        /// from <see cref="Register{TElement}"/>. The <see cref="Array"/> must have zero-based indexing.</param>
        /// <param name="index">The zero-based index in array at which copying begins.</param>
        /// <exception cref="ArgumentException" />
        /// <exception cref="ArgumentNullException" />
        /// <exception cref="ArgumentOutOfRangeException" />
        [Pure]
        public virtual void CopyTo([DisallowNull] Array array, 
                                   Int32 index)
        {
            if (array is TElement[] arr)
            {
                this.CopyTo(arr, 
                            index);
            }
        }

        void ICollection<TElement>.Add(TElement item) => 
            this.Add(item);

        /// <summary>
        /// Removes all elements from the <see cref="Register{TElement}"/>.
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
        /// Removes the first occurrence of a specific object from the <see cref="Register{TElement}"/>.
        /// </summary>
        /// <param name="item">The object to remove from the <see cref="Register{TElement}"/>. The value can be null for reference types.</param>
        /// <returns>
        /// <see langword="true"/> if item is successfully removed; otherwise, <see langword="false"/>.
        /// This method also returns <see langword="false"/> if item was not found in the original <see cref="Register{TElement}"/>.
        /// </returns>
        /// <exception cref="InvalidOperationException" />
        public virtual Boolean Remove([DisallowNull] TElement item) => 
            this.RemoveInternal(item);

        /// <inheritdoc />
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public virtual Boolean IsSynchronized => true;

        /// <inheritdoc />
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public override Boolean IsReadOnly { get; } = false;
    }

    // IList
    partial class Register<TElement> : IList<TElement>
    {
        /// <summary>
        /// Inserts an element into the <see cref="Register{TElement}"/> at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index at which item should be inserted.</param>
        /// <param name="item">The object to insert. The value can be null for reference types.</param>
        /// <exception cref="ArgumentOutOfRangeException" />
        /// <exception cref="InvalidCastException" />
        /// <exception cref="InvalidOperationException" />
        public virtual void Insert(Int32 index, [DisallowNull] TElement item) => 
            this.InsertInternal(index, 
                                item);

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

        /// <summary>
        /// Removes the element at the specified index of the <see cref="Register{TElement}"/>.
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

    // ISet
    partial class Register<TElement> : ISet<TElement>
    {
        /// <summary>
        /// Adds an object to the end of the <see cref="Register{TElement}"/>.
        /// </summary>
        /// <param name="item">The object to be added to the end of the <see cref="Register{TElement}"/>. The value can be null for reference types.</param>
        /// <exception cref="InvalidOperationException" />
        public virtual Boolean Add([DisallowNull] TElement item) => 
            this.AddInternal(item);

        /// <inheritdoc/>
        /// <exception cref="ArgumentNullException"/>
        public virtual void ExceptWith([DisallowNull] IEnumerable<TElement> other)
        {
            if (other is null)
            {
                throw new ArgumentNullException(nameof(other));
            }

            if (this.Count == 0)
            {
                return;
            }

            if (other == this)
            {
                this.Clear();
                return;
            }

            foreach (TElement item in other)
            {
                this.Remove(item);
            }
        }

        /// <inheritdoc/>
        /// <exception cref="ArgumentNullException"/>
        public virtual void IntersectWith([DisallowNull] IEnumerable<TElement> other)
        {
            if (other is null)
            {
                throw new ArgumentNullException(nameof(other));
            }

            if (other == this)
            {
                return;
            }

            if (other is ICollection<TElement> collection)
            {
                if (collection.Count == 0)
                {
                    this.Clear();
                    return;
                }
            }

            if (!other.Any())
            {
                this.Clear();
                return;
            }

            BitArray arr = new(this._size);

            foreach (TElement item in other)
            {
                Int32 index = this.IndexOf(item);
                if (index >= 0)
                {
                    arr.Set(index, 
                            true);
                }
            }

            for (Int32 i = this._size - 1; i > 0; i--)
            {
                if (!arr.Get(i))
                {
                    this.RemoveAt(i);
                }
            }
        }

        /// <inheritdoc/>
        /// <exception cref="ArgumentNullException"/>
        public virtual void SymmetricExceptWith([DisallowNull] IEnumerable<TElement> other)
        {
            if (other is null)
            {
                throw new ArgumentNullException(nameof(other));
            }

            if (this.Count == 0)
            {
                this.UnionWith(other);
                return;
            }

            if (other == this)
            {
                this.Clear();
                return;
            }

            if (other is IReadOnlyRegister<TElement> register)
            {
                if ((this.Comparer is null &&
                    register.Comparer is null) ||
                    (this.Comparer is not null &&
                    this.Comparer.Equals(register.Comparer)))
                {
                    foreach (TElement item in register)
                    {
                        if (!this.Remove(item))
                        {
                            this.Add(item);
                        }
                    }
                    return;
                }
            }

            Int32 current = this._size;
            BitArray toRemove = new(current);
            List<Int32> added = new();

            foreach (TElement item in other)
            {
                Int32 index = this.IndexOf(item);
                if (index >= 0 &&
                    !added.Contains(index))
                {
                    toRemove.Set(index, 
                                 true);
                    continue;
                }
                added.Add(this._size);
                this.Add(item);
            }

            for (Int32 i = current - 1; i > 0; i--)
            {
                if (toRemove.Get(i))
                {
                    this.RemoveAt(i);
                }
            }
        }

        /// <inheritdoc/>
        /// <exception cref="ArgumentNullException"/>
        public virtual void UnionWith([DisallowNull] IEnumerable<TElement> other)
        {
            if (other is null)
            {
                throw new ArgumentNullException(nameof(other));
            }

            if (other == this)
            {
                return;
            }

            foreach (TElement item in other)
            {
                this.AddInternal(item);
            }
        }
    }
}
