using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;

namespace Narumikazuchi.Collections.Abstract
{
    /// <summary>
    /// Represents a strongly typed list of objects, which can be accessed by index and searched. 
    /// </summary>
    // Non-Public
    public abstract partial class SearchableReadOnlyListBase<TElement> : ReadOnlyListBase<TElement>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SearchableReadOnlyListBase{T}"/> class.
        /// </summary>
        protected SearchableReadOnlyListBase() : 
            base() 
        { }
        /// <summary>
        /// Initializes a new instance of the <see cref="SearchableReadOnlyListBase{T}"/> class containing the specified collection of items.
        /// </summary>
        /// <param name="collection">The collection of items in this list.</param>
        /// <exception cref="ArgumentException" />
        /// <exception cref="ArgumentNullException" />
        /// <exception cref="InvalidOperationException" />
        protected SearchableReadOnlyListBase([DisallowNull] IEnumerable<TElement> collection) : 
            base(collection) 
        { }

#pragma warning disable
        /// <summary>
        /// Error message, when the index parameter is greater than the amount of items in the collection.
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        protected const String INDEX_GREATER_THAN_COUNT = "The specified index is greater than the amount of items in the list.";
#pragma warning restore
    }

    // ISearchableList
    partial class SearchableReadOnlyListBase<TElement> : ISearchableCollection<TElement>
    {
        /// <inheritdoc/>
        /// <exception cref="ArgumentNullException" />
        /// <exception cref="ArgumentOutOfRangeException" />
        [Pure]
        public virtual Boolean Exists([DisallowNull] Func<TElement, Boolean> predicate)
        {
            lock (this._syncRoot)
            {
                Int32 v = this._version;
                for (Int32 i = 0; i < this._size; i++)
                {
                    if (this._version != v)
                    {
                        throw new InvalidOperationException(COLLECTION_CHANGED);
                    }
                    if (predicate.Invoke(this._items[i]))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        /// <inheritdoc/>
        /// <exception cref="ArgumentNullException" />
        [Pure]
        [return: MaybeNull]
        public virtual TElement Find([DisallowNull] Func<TElement, Boolean> predicate)
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
                        throw new InvalidOperationException(COLLECTION_CHANGED);
                    }
                    if (predicate.Invoke(this._items[i]))
                    {
                        return this._items[i];
                    }
                }
            }
            return default;
        }

        /// <inheritdoc/>
        /// <exception cref="ArgumentNullException" />
        [Pure]
        [return: NotNull]
        public virtual IReadOnlyList2<TElement> FindAll([DisallowNull] Func<TElement, Boolean> predicate)
        {
            if (predicate is null)
            {
                throw new ArgumentNullException(nameof(predicate));
            }

            List<TElement> result = new();
            lock (this._syncRoot)
            {
                Int32 v = this._version;
                for (Int32 i = 0; i < this._size; i++)
                {
                    if (this._version != v)
                    {
                        throw new InvalidOperationException(COLLECTION_CHANGED);
                    }
                    if (predicate.Invoke(this._items[i]))
                    {
                        result.Add(this._items[i]);
                    }
                }
            }
            return result.AsIReadOnlyList2();
        }

        /// <inheritdoc/>
        /// <exception cref="ArgumentNullException" />
        [Pure]
        [return: NotNull]
        public virtual IReadOnlyList2<TElement> FindExcept([DisallowNull] Func<TElement, Boolean> predicate)
        {
            if (predicate is null)
            {
                throw new ArgumentNullException(nameof(predicate));
            }

            List<TElement> result = new();
            lock (this._syncRoot)
            {
                Int32 v = this._version;
                for (Int32 i = 0; i < this._size; i++)
                {
                    if (this._version != v)
                    {
                        throw new InvalidOperationException(COLLECTION_CHANGED);
                    }
                    if (!predicate.Invoke(this._items[i]))
                    {
                        result.Add(this._items[i]);
                    }
                }
            }
            return result.AsIReadOnlyList2();
        }

        /// <inheritdoc/>
        /// <exception cref="ArgumentNullException" />
        [Pure]
        [return: MaybeNull]
        public virtual TElement FindLast([DisallowNull] Func<TElement, Boolean> predicate)
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
                        throw new InvalidOperationException(COLLECTION_CHANGED);
                    }
                    if (predicate.Invoke(this._items[i]))
                    {
                        return this._items[i];
                    }
                }
            }
            return default;
        }
    }

    // ISearchableList
    partial class SearchableReadOnlyListBase<TElement> : ISearchableList<TElement>
    {
        /// <inheritdoc/>
        /// <exception cref="ArgumentException" />
        /// <exception cref="ArgumentNullException" />
        /// <exception cref="ArgumentOutOfRangeException" />
        /// <exception cref="InvalidOperationException" />
        [Pure]
        public virtual Int32 BinarySearch([DisallowNull] in TElement item) => 
            this.BinarySearch(0, 
                              this._size, 
                              item, 
                              null);
        /// <inheritdoc/>
        /// <exception cref="ArgumentException" />
        /// <exception cref="ArgumentNullException" />
        /// <exception cref="ArgumentOutOfRangeException" />
        /// <exception cref="InvalidOperationException" />
        [Pure]
        public virtual Int32 BinarySearch([DisallowNull] in TElement item, 
                                          [AllowNull] IComparer<TElement>? comparer) => 
            this.BinarySearch(0, 
                              this._size, 
                              item, 
                              comparer);
        /// <inheritdoc/>
        /// <exception cref="ArgumentException" />
        /// <exception cref="ArgumentNullException" />
        /// <exception cref="ArgumentOutOfRangeException" />
        /// <exception cref="InvalidOperationException" />
        [Pure]
        public virtual Int32 BinarySearch(in Int32 index, 
                                          in Int32 count, 
                                          [DisallowNull] in TElement item, 
                                          [AllowNull] IComparer<TElement>? comparer)
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
            if (item is null)
            {
                throw new ArgumentNullException(nameof(item));
            }
            if (this.Count - index < count)
            {
                throw new ArgumentException(COUNT_IS_GREATER_THAN_ITEMS);
            }

            lock (this._syncRoot)
            {
                return Array.BinarySearch(this._items, 
                                          index, 
                                          count, 
                                          item, 
                                          comparer);
            }
        }

        /// <inheritdoc/>
        /// <exception cref="ArgumentNullException" />
        /// <exception cref="ArgumentOutOfRangeException" />
        [Pure]
        public virtual Int32 FindIndex([DisallowNull] Func<TElement, Boolean> predicate) => 
            this.FindIndex(0, 
                           this._size, 
                           predicate);
        /// <inheritdoc/>
        /// <exception cref="ArgumentNullException" />
        /// <exception cref="ArgumentOutOfRangeException" />
        [Pure]
        public virtual Int32 FindIndex(in Int32 startIndex, 
                                       in Int32 count, 
                                       [DisallowNull] Func<TElement, Boolean> predicate)
        {
            if (predicate is null)
            {
                throw new ArgumentNullException(nameof(predicate));
            }
            if ((UInt32)startIndex > (UInt32)this._size)
            {
                throw new ArgumentOutOfRangeException(nameof(startIndex), 
                                                      INDEX_GREATER_THAN_COUNT);
            }
            if (count < 0 ||
                startIndex > this.Count - count)
            {
                throw new ArgumentOutOfRangeException(nameof(count),
                                                      COUNT_IS_GREATER_THAN_ITEMS);
            }
            lock (this._syncRoot)
            {
                Int32 v = this._version;
                Int32 end = startIndex + count;
                for (Int32 i = startIndex; i < end; i++)
                {
                    if (this._version != v)
                    {
                        throw new InvalidOperationException(COLLECTION_CHANGED);
                    }
                    if (predicate.Invoke(this._items[i]))
                    {
                        return i;
                    }
                }
                return -1;
            }
        }

        /// <inheritdoc/>
        /// <exception cref="ArgumentNullException" />
        /// <exception cref="ArgumentOutOfRangeException" />
        [Pure]
        public virtual Int32 FindLastIndex([DisallowNull] Func<TElement, Boolean> predicate) => 
            this.FindLastIndex(0, 
                               this._size, 
                               predicate);
        /// <inheritdoc/>
        /// <exception cref="ArgumentNullException" />
        /// <exception cref="ArgumentOutOfRangeException" />
        [Pure]
        public virtual Int32 FindLastIndex(in Int32 startIndex, 
                                           in Int32 count, 
                                           [DisallowNull] Func<TElement, Boolean> predicate)
        {
            if (predicate is null)
            {
                throw new ArgumentNullException(nameof(predicate));
            }
            if ((UInt32)startIndex >= (UInt32)this._size)
            {
                throw new ArgumentOutOfRangeException(nameof(startIndex),
                                                      INDEX_GREATER_THAN_COUNT);
            }
            if (count < 0 ||
                startIndex - count + 1 < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(count),
                                                      COUNT_IS_GREATER_THAN_ITEMS);
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
                        throw new InvalidOperationException(COLLECTION_CHANGED);
                    }
                    if (predicate.Invoke(this._items[i]))
                    {
                        return i;
                    }
                }
                return -1;
            }
        }

        /// <inheritdoc/>
        /// <exception cref="ArgumentNullException" />
        /// <exception cref="ArgumentOutOfRangeException" />
        [Pure]
        public virtual Int32 LastIndexOf([DisallowNull] in TElement item)
        {
            if (item is null)
            {
                throw new ArgumentNullException(nameof(item));
            }
            if (this._size == 0)
            {
                return -1;
            }
            return Array.LastIndexOf(this._items,
                                     item,
                                     0,
                                     this._size);
        }
    }
}
