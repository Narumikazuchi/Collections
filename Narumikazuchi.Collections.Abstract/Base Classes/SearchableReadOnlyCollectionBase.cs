﻿using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;

namespace Narumikazuchi.Collections.Abstract
{
    /// <summary>
    /// Represents a strongly typed collection of objects, which can be searched. 
    /// </summary>
    // Non-Public
    public abstract partial class SearchableReadOnlyCollectionBase<TElement> : ReadOnlyCollectionBase<TElement>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SearchableReadOnlyCollectionBase{T}"/> class.
        /// </summary>
        protected SearchableReadOnlyCollectionBase() : 
            base() 
        { }
        /// <summary>
        /// Initializes a new instance of the <see cref="SearchableReadOnlyCollectionBase{T}"/> class containing the specified collection of items.
        /// </summary>
        /// <param name="collection">The collection of items in this list.</param>
        /// <exception cref="ArgumentException" />
        /// <exception cref="ArgumentNullException" />
        /// <exception cref="InvalidOperationException" />
        protected SearchableReadOnlyCollectionBase([DisallowNull] IEnumerable<TElement> collection) :
            base(collection) 
        { }
    }

    // ISearchableCollection
    partial class SearchableReadOnlyCollectionBase<TElement> : ISearchableCollection<TElement>
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
}
