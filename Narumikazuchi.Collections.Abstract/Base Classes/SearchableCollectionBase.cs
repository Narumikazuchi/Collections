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
    public abstract partial class SearchableCollectionBase<TElement> : SearchableReadOnlyCollectionBase<TElement>
    {
        /// <summary>
        /// Adds the elements of the specified collection to the end of the <see cref="SearchableCollectionBase{T}"/>.
        /// </summary>
        /// <param name="collection">The collection of items to add.</param>
        /// <exception cref="ArgumentNullException" />
        /// <exception cref="ArgumentOutOfRangeException" />
        /// <exception cref="InvalidOperationException" />
        public virtual void AddRange([DisallowNull] IEnumerable<TElement> collection)
        {
            if (collection is null)
            {
                throw new ArgumentNullException(nameof(collection));
            }
            if (this.IsReadOnly)
            {
                throw new InvalidOperationException(COLLECTION_IS_READONLY);
            }

            if (collection is ICollection<TElement> c)
            {
                Int32 count = c.Count;
                if (count > 0)
                {
                    this.EnsureCapacity(this.Count + count);

                    if (ReferenceEquals(this,
                                        c))
                    {
                        lock (this._syncRoot)
                        {
                            Array.Copy(this._items,
                                       0,
                                       this._items,
                                       this._size,
                                       this._size);
                        }
                    }
                    else
                    {
                        TElement[] insert = new TElement[count];
                        c.CopyTo(insert,
                                 0);
                        lock (this._syncRoot)
                        {
                            insert.CopyTo(this._items,
                                          this._size);
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
                    this.Add(enumerator.Current);
                }
            }
            lock (this._syncRoot)
            {
                this._version++;
            }
        }

        /// <summary>
        /// Removes all items from the <see cref="SearchableCollectionBase{T}"/> that match the specified condition.
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
                throw new InvalidOperationException(COLLECTION_IS_READONLY);
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
        /// Reverses the order of items in the entire <see cref="SearchableCollectionBase{T}"/>.
        /// </summary>
        /// <exception cref="ArgumentException" />
        /// <exception cref="ArgumentOutOfRangeException" />
        /// <exception cref="InvalidOperationException" />
        public virtual void Reverse()
        {
            if (this.IsReadOnly)
            {
                throw new InvalidOperationException(COLLECTION_IS_READONLY);
            }

            lock (this._syncRoot)
            {
                Array.Reverse(this._items,
                              0,
                              this._size);
                this._version++;
            }
        }
    }

    // Non-Public
    partial class SearchableCollectionBase<TElement>
    {
        /// <summary>
        /// Initializes a new empty instance of the <see cref="SearchableCollectionBase{T}"/> class.
        /// </summary>
        protected SearchableCollectionBase() :
            base()
        { }
        /// <summary>
        /// Initializes a new instance of the <see cref="SearchableCollectionBase{T}"/> class having the specified collection of items.
        /// </summary>
        /// <param name="collection">The initial collection of items in this list.</param>
        /// <exception cref="ArgumentException" />
        /// <exception cref="ArgumentNullException" />
        /// <exception cref="InvalidOperationException" />
        protected SearchableCollectionBase([DisallowNull] IEnumerable<TElement> collection) :
            base(collection)
        { }

#pragma warning disable
        /// <summary>
        /// Error message, when trying to write to a readonly list.
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        protected const String COLLECTION_IS_READONLY = "This collection is readonly and cannot be written to.";
#pragma warning restore
    }

    // ICollection
    partial class SearchableCollectionBase<TElement> : ICollection<TElement>
    {
        /// <summary>
        /// Copies the entire <see cref="SearchableCollectionBase{T}"/> to a compatible one-dimensional <see cref="Array"/>, 
        /// starting at the specified index of the target array.
        /// </summary>
        /// <param name="array">
        /// The one-dimensional <see cref="Array"/> that is the destination of the elements copied
        /// from <see cref="SearchableCollectionBase{T}"/>. The <see cref="Array"/> must have zero-based indexing.</param>
        /// <param name="index">The zero-based index in array at which copying begins.</param>
        /// <exception cref="ArgumentException" />
        /// <exception cref="ArgumentNullException" />
        /// <exception cref="ArgumentOutOfRangeException" />
        [Pure]
        public virtual void CopyTo([DisallowNull] Array array,
                                   Int32 index)
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
        /// Adds an object to the end of the <see cref="SearchableCollectionBase{T}"/>.
        /// </summary>
        /// <param name="item">The object to be added to the end of the <see cref="SearchableCollectionBase{T}"/>. The value can be null for reference types.</param>
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
                throw new InvalidOperationException(COLLECTION_IS_READONLY);
            }

            this.EnsureCapacity(this.Count + 1);
            lock (this._syncRoot)
            {
                this._items[this._size++] = item;
                this._version++;
            }
        }

        /// <summary>
        /// Removes all elements from the <see cref="CollectionBase{T}"/>.
        /// </summary>
        /// <exception cref="InvalidOperationException" />
        public virtual void Clear()
        {
            if (this.IsReadOnly)
            {
                throw new InvalidOperationException(COLLECTION_IS_READONLY);
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
        /// Removes the first occurrence of a specific object from the <see cref="SearchableCollectionBase{T}"/>.
        /// </summary>
        /// <param name="item">The object to remove from the <see cref="SearchableCollectionBase{T}"/>. The value can be null for reference types.</param>
        /// <returns>
        /// <see langword="true"/> if item is successfully removed; otherwise, <see langword="false"/>.
        /// This method also returns <see langword="false"/> if item was not found in the original <see cref="SearchableCollectionBase{T}"/>.
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
                throw new InvalidOperationException(COLLECTION_IS_READONLY);
            }

            lock (this._syncRoot)
            {
                Int32 index = Array.IndexOf(this._items,
                                            item);
                if (index > -1)
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
}
