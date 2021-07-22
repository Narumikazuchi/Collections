using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;

namespace Narumikazuchi.Collections.Abstract
{
    /// <summary>
    /// Represents a strongly typed list of objects, which can be accessed by index and searched. 
    /// </summary>
    public abstract class SearchableReadOnlyListBase<T> : ReadOnlyListBase<T>, ISearchableList<T>
    {
        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="SearchableReadOnlyListBase{T}"/> class.
        /// </summary>
        protected SearchableReadOnlyListBase() : base() { }
        /// <summary>
        /// Initializes a new instance of the <see cref="SearchableReadOnlyListBase{T}"/> class containing the specified collection of items.
        /// </summary>
        /// <param name="collection">The collection of items in this list.</param>
        /// <exception cref="ArgumentException" />
        /// <exception cref="ArgumentNullException" />
        /// <exception cref="InvalidOperationException" />
        protected SearchableReadOnlyListBase([DisallowNull] IEnumerable<T> collection) : base(collection) { }

        #endregion

        #region ISearchableCollection

        /// <inheritdoc/>
        /// <exception cref="ArgumentNullException" />
        /// <exception cref="ArgumentOutOfRangeException" />
        [Pure]
        public virtual Boolean Exists([DisallowNull] Func<T, Boolean> predicate)
        {
            lock (this._syncRoot)
            {
                Int32 v = this._version;
                for (Int32 i = 0; i < this._size; i++)
                {
                    if (this._version != v)
                    {
                        throw new InvalidOperationException("The collection changed during enumeration.");
                    }
#pragma warning disable
                    if (predicate.Invoke(this._items[i]))
                    {
                        return true;
                    }
#pragma warning restore
                }
            }
            return false;
        }

        /// <inheritdoc/>
        /// <exception cref="ArgumentNullException" />
        [Pure]
        public virtual T? Find([DisallowNull] Func<T, Boolean> predicate)
        {
            if (predicate is null)
            {
                throw new ArgumentNullException(nameof(predicate));
            }

            lock (this._syncRoot)
            {
                Int32 v = this._version;
                for (Int32 i = 0; i < this._size; i++)
                {
                    if (this._version != v)
                    {
                        throw new InvalidOperationException("The collection changed during enumeration.");
                    }
#pragma warning disable
                    if (predicate.Invoke(this._items[i]))
                    {
                        return this._items[i];
                    }
#pragma warning restore
                }
            }
            return default;
        }

        /// <inheritdoc/>
        /// <exception cref="ArgumentNullException" />
        [Pure]
        public virtual IReadOnlyList2<T> FindAll([DisallowNull] Func<T, Boolean> predicate)
        {
            if (predicate is null)
            {
                throw new ArgumentNullException(nameof(predicate));
            }

            List<T> result = new();
            lock (this._syncRoot)
            {
                Int32 v = this._version;
                for (Int32 i = 0; i < this._size; i++)
                {
                    if (this._version != v)
                    {
                        throw new InvalidOperationException("The collection changed during enumeration.");
                    }
#pragma warning disable
                    if (predicate.Invoke(this._items[i]))
                    {
                        result.Add(this._items[i]);
                    }
#pragma warning restore
                }
            }
            return result.AsIReadOnlyList2();
        }

        /// <inheritdoc/>
        /// <exception cref="ArgumentNullException" />
        [Pure]
        public virtual IReadOnlyList2<T> FindExcept([DisallowNull] Func<T, Boolean> predicate)
        {
            if (predicate is null)
            {
                throw new ArgumentNullException(nameof(predicate));
            }

            List<T> result = new();
            lock (this._syncRoot)
            {
                Int32 v = this._version;
                for (Int32 i = 0; i < this._size; i++)
                {
                    if (this._version != v)
                    {
                        throw new InvalidOperationException("The collection changed during enumeration.");
                    }
#pragma warning disable
                    if (!predicate.Invoke(this._items[i]))
                    {
                        result.Add(this._items[i]);
                    }
#pragma warning restore
                }
            }
            return result.AsIReadOnlyList2();
        }

        /// <inheritdoc/>
        /// <exception cref="ArgumentNullException" />
        [Pure]
        public virtual T? FindLast([DisallowNull] Func<T, Boolean> predicate)
        {
            if (predicate is null)
            {
                throw new ArgumentNullException(nameof(predicate));
            }

            lock (this._syncRoot)
            {
                Int32 v = this._version;
                for (Int32 i = this._size - 1; i >= 0; i--)
                {
                    if (this._version != v)
                    {
                        throw new InvalidOperationException("The collection changed during enumeration.");
                    }
#pragma warning disable
                    if (predicate.Invoke(this._items[i]))
                    {
                        return this._items[i];
                    }
#pragma warning restore
                }
            }
            return default;
        }

        #endregion

        #region ISearchableList

        /// <inheritdoc/>
        /// <exception cref="ArgumentException" />
        /// <exception cref="ArgumentNullException" />
        /// <exception cref="ArgumentOutOfRangeException" />
        /// <exception cref="InvalidOperationException" />
        [Pure]
        public virtual Int32 BinarySearch([DisallowNull] in T item) => this.BinarySearch(0, this._size, item, null);
        /// <inheritdoc/>
        /// <exception cref="ArgumentException" />
        /// <exception cref="ArgumentNullException" />
        /// <exception cref="ArgumentOutOfRangeException" />
        /// <exception cref="InvalidOperationException" />
        [Pure]
        public virtual Int32 BinarySearch([DisallowNull] in T item, IComparer<T>? comparer) => this.BinarySearch(0, this._size, item, comparer);
        /// <inheritdoc/>
        /// <exception cref="ArgumentException" />
        /// <exception cref="ArgumentNullException" />
        /// <exception cref="ArgumentOutOfRangeException" />
        /// <exception cref="InvalidOperationException" />
        [Pure]
        public virtual Int32 BinarySearch(in Int32 index, in Int32 count, [DisallowNull] in T item, IComparer<T>? comparer)
        {
            if (index < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(index), "Start index cannot be negative.");
            }
            if (count < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(count), "Search count cannot be negative.");
            }
            if (item is null)
            {
                throw new ArgumentNullException(nameof(item));
            }
            if (this.Count - index < count)
            {
                throw new ArgumentException("Not enough items to cover the specified count.");
            }

            lock (this._syncRoot)
            {
#pragma warning disable
                return Array.BinarySearch(this._items, index, count, item, comparer);
#pragma warning restore
            }
        }

        /// <inheritdoc/>
        /// <exception cref="ArgumentNullException" />
        /// <exception cref="ArgumentOutOfRangeException" />
        [Pure]
        public virtual Int32 FindIndex([DisallowNull] Func<T, Boolean> predicate) => this.FindIndex(0, this._size, predicate);
        /// <inheritdoc/>
        /// <exception cref="ArgumentNullException" />
        /// <exception cref="ArgumentOutOfRangeException" />
        [Pure]
        public virtual Int32 FindIndex(in Int32 startIndex, in Int32 count, [DisallowNull] Func<T, Boolean> predicate)
        {
            if (predicate is null)
            {
                throw new ArgumentNullException(nameof(predicate));
            }
            if ((UInt32)startIndex > (UInt32)this._size)
            {
                throw new ArgumentOutOfRangeException(nameof(startIndex), "The specified index exceeds the item count.");
            }
            if (count < 0 ||
                startIndex > this.Count - count)
            {
                throw new ArgumentOutOfRangeException(nameof(count), "The specified count is invalid.");
            }
            lock (this._syncRoot)
            {
                Int32 v = this._version;
                Int32 end = startIndex + count;
                for (Int32 i = startIndex; i < end; i++)
                {
                    if (this._version != v)
                    {
                        throw new InvalidOperationException("The collection changed during enumeration.");
                    }
#pragma warning disable
                    if (predicate.Invoke(this._items[i]))
                    {
                        return i;
                    }
#pragma warning restore
                }
                return -1;
            }
        }

        /// <inheritdoc/>
        /// <exception cref="ArgumentNullException" />
        /// <exception cref="ArgumentOutOfRangeException" />
        [Pure]
        public virtual Int32 FindLastIndex([DisallowNull] Func<T, Boolean> predicate) => this.FindLastIndex(0, this._size, predicate);
        /// <inheritdoc/>
        /// <exception cref="ArgumentNullException" />
        /// <exception cref="ArgumentOutOfRangeException" />
        [Pure]
        public virtual Int32 FindLastIndex(in Int32 startIndex, in Int32 count, [DisallowNull] Func<T, Boolean> predicate)
        {
            if (predicate is null)
            {
                throw new ArgumentNullException(nameof(predicate));
            }
            if ((UInt32)startIndex >= (UInt32)this._size)
            {
                throw new ArgumentOutOfRangeException(nameof(startIndex), "The specified index exceeds the item count.");
            }
            if (count < 0 ||
                startIndex - count + 1 < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(count), "The specified count is invalid.");
            }

            if (this.Count == 0)
            {
                return -1;
            }

            lock (this._syncRoot)
            {
                Int32 v = this._version;
                Int32 end = startIndex - count;
                for (Int32 i = startIndex; i > end; i--)
                {
                    if (this._version != v)
                    {
                        throw new InvalidOperationException("The collection changed during enumeration.");
                    }
#pragma warning disable
                    if (predicate.Invoke(this._items[i]))
                    {
                        return i;
                    }
#pragma warning restore
                }
                return -1;
            }
        }

        /// <inheritdoc/>
        /// <exception cref="ArgumentNullException" />
        /// <exception cref="ArgumentOutOfRangeException" />
        [Pure]
        public virtual Int32 LastIndexOf([DisallowNull] in T item) => 
            item is null
                ? throw new ArgumentNullException(nameof(item))
                : this._size == 0 
                    ? -1 
                    : Array.LastIndexOf(this._items, item, 0, this._size);

        #endregion
    }
}
