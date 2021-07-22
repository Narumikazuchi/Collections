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
    public abstract class ReadOnlyCollectionBase<T> : ArrayBasedCollection<T>, IReadOnlyCollection2<T>
    {
        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="ReadOnlyCollectionBase{T}"/> class.
        /// </summary>
        protected ReadOnlyCollectionBase() => this._items = _emptyArray;
        /// <summary>
        /// Initializes a new instance of the <see cref="ReadOnlyCollectionBase{T}"/> class containing the specified collection of items.
        /// </summary>
        /// <param name="collection">The collection of items in this list.</param>
        /// <exception cref="ArgumentException" />
        /// <exception cref="ArgumentNullException" />
        /// <exception cref="InvalidOperationException" />
        protected ReadOnlyCollectionBase([DisallowNull] IEnumerable<T> collection)
        {
            if (collection is null)
            {
                throw new ArgumentNullException(nameof(collection));
            }

            if (collection is ICollection<T> c)
            {
                if (c.Count == 0)
                {
                    this._items = _emptyArray;
                }
                else
                {
                    this._items = new T[c.Count];
#pragma warning disable 
                    c.CopyTo(this._items, 0);
#pragma warning restore
                    this._size = c.Count;
                }
            }
            else
            {
                this._size = 0;
                this._items = _emptyArray;

                using IEnumerator<T> enumerator = collection.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    lock (this._syncRoot)
                    {
                        if (this._items.Length == this._size)
                        {
                            Int32 capacity = this._items.Length == 0 ? DEFAULTCAPACITY : this._items.Length * 2;
                            T[] array = new T[capacity];
                            Array.Copy(this._items, 0, array, 0, this._size);
                            this._items = array;
                        }
                        this._items[this._size++] = enumerator.Current;
                    }
                }
                if (this._items.Length != this._size)
                {
                    T[] array = new T[this._size];
                    Array.Copy(this._items, 0, array, 0, this._size);
                    this._items = array;
                }
            }
        }

        #endregion

        #region Conversion

        /// <summary>
        /// Converts all elements in the <see cref="ReadOnlyCollectionBase{T}"/> into another type and returns an <see cref="IList{T}"/>
        /// containing the converted objects.
        /// </summary>
        /// <param name="converter">A delegate which converts every item into the new type.</param>
        /// <returns>An <see cref="IList{T}"/> which contains the converted objects</returns>
        /// <exception cref="ArgumentNullException" />
        [Pure]
        public virtual IList<TOutput> ConvertAll<TOutput>([DisallowNull] Converter<T, TOutput> converter)
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
                        throw new InvalidOperationException("The collection changed during enumeration.");
                    }
#pragma warning disable
                    result.Add(converter.Invoke(this._items[i]));
#pragma warning restore
                }
            }
            return result;
        }

        #endregion

        #region Collection Management

        /// <summary>
        /// Performs the specified action for every element of this <see cref="ReadOnlyCollectionBase{T}"/>.
        /// </summary>
        /// <param name="action">The action to perform on each item.</param>
        /// <exception cref="ArgumentNullException" />
        /// <exception cref="InvalidOperationException" />
        [Pure]
        public virtual void ForEach([DisallowNull] Action<T> action)
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
                        throw new InvalidOperationException("The collection changed during enumeration.");
                    }
#pragma warning disable
                    action.Invoke(this._items[i]);
#pragma warning restore
                }
            }
        }

        #endregion

        #region IReadOnlyCollection

        /// <inheritdoc />
        /// <exception cref="ArgumentNullException" />
        /// <exception cref="ArgumentOutOfRangeException" />
        [Pure]
        public virtual Boolean Contains([DisallowNull] T item)
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
                    if (this._items[i].Equals(item))
                    {
                        return true;
                    }
#pragma warning restore
                }
                return false;
            }
        }

        /// <inheritdoc />
        /// <exception cref="ArgumentException" />
        /// <exception cref="ArgumentNullException" />
        /// <exception cref="ArgumentOutOfRangeException" />
        [Pure]
        public virtual void CopyTo([DisallowNull] T?[] array, Int32 index)
        {
            if (array is null)
            {
                throw new ArgumentNullException(nameof(array));
            }
            if (array.Rank != 1)
            {
                throw new ArgumentException("Multidimensional array are not supported.");
            }

            lock (this._syncRoot)
            {
                Array.Copy(this._items, 0, array, index, this._size);
            }
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets a value indicating whether the <see cref="ReadOnlyCollectionBase{T}"/> is read-only.
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Collapsed)]
        public virtual Boolean IsReadOnly { get; } = true;

        #endregion
    }
}
