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
    public abstract partial class ReadOnlyListBase<TElement> : ReadOnlyCollectionBase<TElement>
    {
        /// <summary>
        /// Copies the entire <see cref="ReadOnlyListBase{T}"/> to the specified one-dimensional array.
        /// </summary>
        /// <param name="array">An array with a fitting size to copy the items into.</param>
        /// <exception cref="ArgumentException" />
        /// <exception cref="ArgumentNullException" />
        /// <exception cref="ArgumentOutOfRangeException" />
        [Pure]
        public virtual void CopyTo([DisallowNull] TElement[] array) => 
            this.CopyTo(array, 0);
        /// <summary>
        /// Copies a section of the <see cref="ReadOnlyListBase{T}"/> to the specified one-dimensional array.
        /// The section starts at the specified index, entails the specified count of items and begins inserting them
        /// at the specified starting index into the array.
        /// </summary>
        /// <param name="index">The index of the first item of this list to copy.</param>
        /// <param name="count">The amount of items to copy from this list.</param>
        /// <param name="array">An array with a fitting size to copy the items into.</param>
        /// <param name="arrayIndex">The index at which to start inserting items into the array.</param>
        /// <exception cref="ArgumentException" />
        /// <exception cref="ArgumentNullException" />
        /// <exception cref="ArgumentOutOfRangeException" />
        [Pure]
        public virtual void CopyTo(in Int32 index, 
                                   in Int32 count, 
                                   [DisallowNull] TElement[] array, 
                                   in Int32 arrayIndex)
        {
            if (array is null)
            {
                throw new ArgumentNullException(nameof(array));
            }
            if (array.Rank != 1)
            {
                throw new ArgumentException(MULTI_DIMENSIONAL_ARRAYS);
            }

            lock (this._syncRoot)
            {
                Array.Copy(this._items, 
                           index, 
                           array, 
                           arrayIndex, 
                           count);
            }
        }

        /// <summary>
        /// Creates a shallow copy of the elements of this <see cref="ReadOnlyListBase{T}"/> from the specified range.
        /// </summary>
        /// <param name="index">The index of the first item in the resulting range.</param>
        /// <param name="count">The amount of items in the range.</param>
        /// <returns>An <see cref="IList{T}"/> containing the items from the specified range</returns>
        /// <exception cref="ArgumentException" />
        /// <exception cref="ArgumentOutOfRangeException" />
        [Pure]
        public virtual IList<TElement> GetRange(in Int32 index, 
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
            if (this._size - index < count)
            {
                throw new ArgumentException(COUNT_IS_GREATER_THAN_ITEMS);
            }

            lock (this._syncRoot)
            {
                Int32 v = this._version;
                List<TElement> result = new();
                Int32 end = index + count;

                for (Int32 i = index; i < end; i++)
                {
                    if (this._version != v)
                    {
                        throw new InvalidOperationException(COLLECTION_CHANGED);
                    }
                    result.Add(this._items[i]);
                }
                return result;
            }
        }

    }

    // Non-Public
    partial class ReadOnlyListBase<TElement>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ReadOnlyListBase{T}"/> class.
        /// </summary>
        protected ReadOnlyListBase() : 
            base() 
        { }
        /// <summary>
        /// Initializes a new instance of the <see cref="ReadOnlyListBase{T}"/> class containing the specified collection of items.
        /// </summary>
        /// <param name="collection">The collection of items in this list.</param>
        /// <exception cref="ArgumentException" />
        /// <exception cref="ArgumentNullException" />
        /// <exception cref="InvalidOperationException" />
        protected ReadOnlyListBase([DisallowNull] IEnumerable<TElement> collection) : 
            base(collection) 
        { }

#pragma warning disable
        /// <summary>
        /// Error message, when trying to write to a readonly list.
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        protected const String LIST_IS_READONLY = "This list is readonly and cannot be written to.";
        /// <summary>
        /// Error message, when trying to write to a readonly list.
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        protected const String START_INDEX_IS_NEGATIVE = "The start index cannot be negative.";
        /// <summary>
        /// Error message, when trying to write to a readonly list.
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        protected const String COUNT_IS_NEGATIVE = "The count cannot be negative.";
        /// <summary>
        /// Error message, when trying to write to a readonly list.
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        protected const String COUNT_IS_GREATER_THAN_ITEMS = "The specified count is greater than the available number of items.";
#pragma warning restore
    }

    // IReadOnlyList2
    partial class ReadOnlyListBase<TElement> : IReadOnlyList2<TElement>
    {
        /// <inheritdoc />
        /// <exception cref="ArgumentNullException" />
        /// <exception cref="ArgumentOutOfRangeException" />
        [Pure]
        public virtual Int32 IndexOf([DisallowNull] TElement item)
        {
            if (item is null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            lock (this._syncRoot)
            {
                return Array.IndexOf(this._items, 
                                     item, 
                                     0, 
                                     this._size);
            }
        }

        /// <inheritdoc />
        /// <exception cref="IndexOutOfRangeException" />
        /// <exception cref="InvalidOperationException" />
        public virtual TElement this[Int32 index]
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
            set => throw new NotSupportedException(LIST_IS_READONLY);
        }
    }
}
