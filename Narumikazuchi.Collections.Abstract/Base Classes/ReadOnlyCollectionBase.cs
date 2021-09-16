using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;

namespace Narumikazuchi.Collections.Abstract
{
    /// <summary>
    /// Represents a strongly typed collection of objects. 
    /// </summary>
    public abstract partial class ReadOnlyCollectionBase<TElement> : ArrayBasedCollection<TElement>
    {
        /// <summary>
        /// Converts all elements in the <see cref="ReadOnlyCollectionBase{T}"/> into another type and returns an <see cref="IList{T}"/>
        /// containing the converted objects.
        /// </summary>
        /// <param name="converter">A delegate which converts every item into the new type.</param>
        /// <returns>An <see cref="IList{T}"/> which contains the converted objects</returns>
        /// <exception cref="ArgumentNullException" />
        [Pure]
        [return: NotNull]
        public virtual ICollection<TOutput> ConvertAll<TOutput>([DisallowNull] Converter<TElement, TOutput> converter)
        {
            if (converter is null)
            {
                throw new ArgumentNullException(nameof(converter));
            }

            List<TOutput> result = new();
            lock (this._syncRoot)
            {
                Int32 v = this._version;

                for (Int32 i = 0; i < this._size; i++)
                {
                    if (this._version != v)
                    {
                        throw new InvalidOperationException(COLLECTION_CHANGED);
                    }
                    result.Add(converter.Invoke(this._items[i]));
                }
            }
            return result;
        }

        /// <summary>
        /// Performs the specified action for every element of this <see cref="ReadOnlyCollectionBase{T}"/>.
        /// </summary>
        /// <param name="action">The action to perform on each item.</param>
        /// <exception cref="ArgumentNullException" />
        /// <exception cref="InvalidOperationException" />
        [Pure]
        public virtual void ForEach([DisallowNull] Action<TElement> action)
        {
            if (action is null)
            {
                throw new ArgumentNullException(nameof(action));
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
                    action.Invoke(this._items[i]);
                }
            }
        }

        /// <summary>
        /// Gets a value indicating whether the <see cref="ReadOnlyCollectionBase{T}"/> is read-only.
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Collapsed)]
        public virtual Boolean IsReadOnly { get; } = true;
    }

    // Non-Public
    partial class ReadOnlyCollectionBase<TElement>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ReadOnlyCollectionBase{T}"/> class.
        /// </summary>
        protected ReadOnlyCollectionBase() => 
            this._items = _emptyArray;
        /// <summary>
        /// Initializes a new instance of the <see cref="ReadOnlyCollectionBase{T}"/> class containing the specified collection of items.
        /// </summary>
        /// <param name="collection">The collection of items in this list.</param>
        /// <exception cref="ArgumentException" />
        /// <exception cref="ArgumentNullException" />
        /// <exception cref="InvalidOperationException" />
        protected ReadOnlyCollectionBase([DisallowNull] IEnumerable<TElement> collection)
        {
            if (collection is null)
            {
                throw new ArgumentNullException(nameof(collection));
            }

            if (collection is ICollection<TElement> c)
            {
                if (c.Count == 0)
                {
                    this._items = _emptyArray;
                }
                else
                {
                    this._items = new TElement[c.Count];
                    c.CopyTo(this._items, 
                             0);
                    this._size = c.Count;
                }
            }
            else
            {
                this._size = 0;
                this._items = _emptyArray;

                using IEnumerator<TElement> enumerator = collection.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    if (this._items.Length == this._size)
                    {
                        Int32 capacity = this._items.Length == 0 
                                                ? DEFAULTCAPACITY 
                                                : this._items.Length * 2;
                        TElement[] array = new TElement[capacity];
                        Array.Copy(this._items, 
                                   0, 
                                   array, 
                                   0, 
                                   this._size);
                        this._items = array;
                    }
                    this._items[this._size++] = enumerator.Current;
                }
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

#pragma warning disable
        /// <summary>
        /// Error message, when trying to copy this collection to a multidimensional array.
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        protected const String MULTI_DIMENSIONAL_ARRAYS = "Multidimensional array are not supported.";
#pragma warning restore
    }

    // IReadOnlyCollection2
    partial class ReadOnlyCollectionBase<TElement> : IReadOnlyCollection2<TElement>
    {
        /// <inheritdoc />
        /// <exception cref="ArgumentNullException" />
        /// <exception cref="ArgumentOutOfRangeException" />
        [Pure]
        public virtual Boolean Contains([DisallowNull] TElement item)
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
                    if (item.Equals(this._items[i]))
                    {
                        return true;
                    }
                }
                return false;
            }
        }

        /// <inheritdoc />
        /// <exception cref="ArgumentException" />
        /// <exception cref="ArgumentNullException" />
        /// <exception cref="ArgumentOutOfRangeException" />
        [Pure]
        public virtual void CopyTo([DisallowNull] TElement[] array, Int32 index)
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
                           0, 
                           array, 
                           index, 
                           this._size);
            }
        }
    }
}
