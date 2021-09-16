using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;

namespace Narumikazuchi.Collections.Abstract
{
    /// <summary>
    /// Represents a collection whose underlying item storage is an array.
    /// </summary>
    [DebuggerDisplay("Count = {_size}")]
    public abstract partial class ArrayBasedCollection<TElement>
    {
        /// <summary>
        /// Copies the entire <see cref="ArrayBasedCollection{T}"/> into a new array.
        /// </summary>
        /// <returns>A new array containing all items from this collection</returns>
        [Pure]
        [return: NotNull]
        public virtual TElement[] ToArray()
        {
            lock (this._syncRoot)
            {
                TElement[] array = new TElement[this._size];
                Array.Copy(this._items, 
                           0, 
                           array, 
                           0, 
                           this._size);
                return array;
            }
        }
        /// <summary>
        /// Gets the number of elements contained in the <see cref="ArrayBasedCollection{T}"/>.
        /// </summary>
        [Pure]
        public virtual Int32 Count
        {
            get
            {
                lock (this._syncRoot)
                {
                    return this._size;
                }
            }
        }
        /// <summary>
        /// Gets a value indicating whether the <see cref="ArrayBasedCollection{T}"/> has a fixed size.
        /// </summary>
        public virtual Boolean IsFixedSize => false;
        /// <summary>
        /// Gets the object mutex used to synchronize acces to this <see cref="ArrayBasedCollection{T}"/>.
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        [DisallowNull]
        public virtual Object SyncRoot => this._syncRoot;
    }

    // Non-Public
    partial class ArrayBasedCollection<TElement>
    {
        private protected ArrayBasedCollection() { }

        /// <summary>
        /// Checks whether the specified object can be added to the collection.
        /// </summary>
        /// <param name="value">The object to check.</param>
        /// <returns><see langword="true"/> if the object can be added to the list; else <see langword="false"/></returns>
        [Pure]
        protected static Boolean IsCompatibleObject([AllowNull] Object? value) =>
            value is TElement || 
            (value is null && 
            default(TElement) is null);

        /// <summary>
        /// Expands the underlying array to fit the specified number of items, if necessary.
        /// </summary>
        /// <param name="capacity">The number of items to fit into this collection.</param>
        /// <exception cref="InvalidOperationException" />
        protected void EnsureCapacity(in Int32 capacity)
        {
            if (this._items.Length < capacity)
            {
                if (this.IsFixedSize)
                {
                    throw new InvalidOperationException(SIZE_IS_FIXED);
                }
                Int32 bigger = this._items.Length == 0 
                                    ? DEFAULTCAPACITY 
                                    : this._items.Length * 2;
                if ((UInt32)bigger > MAXARRAYSIZE)
                {
                    bigger = MAXARRAYSIZE;
                }
                if (bigger < capacity)
                {
                    bigger = capacity;
                }
                TElement[] array = new TElement[bigger];
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
        }

        /// <summary>
        /// Statically allocates an empty array to use for every <see cref="ArrayBasedCollection{T}"/> using the type <typeparamref name="TElement"/>.
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        protected static readonly TElement[] _emptyArray = Array.Empty<TElement>();

        /// <summary>
        /// The internal mutex for synchronizing multi-threaded access.
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        protected readonly Object _syncRoot = new();
        /// <summary>
        /// Internally manages and contains the items for the <see cref="ArrayBasedCollection{T}"/>.
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
        protected TElement[] _items = new TElement[1];
        /// <summary>
        /// Internally manages and contains the actual amount of items in the <see cref="ArrayBasedCollection{T}"/>.
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        protected Int32 _size = 0;
        /// <summary>
        /// Keeps track of changes done to the collection, preventing invalid operations during enumeration.
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        protected Int32 _version = 0;

#pragma warning disable
        /// <summary>
        /// Represents the default capacity for a new <see cref="ArrayBasedCollection{T}"/>.
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        protected const Int32 DEFAULTCAPACITY = 4;
        /// <summary>
        /// Represents the max. size to which the internal array can expand to.
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        protected const Int32 MAXARRAYSIZE = 0X7FEFFFFF;
        /// <summary>
        /// Error message, when trying to change _item size while <see cref="IsFixedSize"/> is <see langword="true"/>.
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        protected const String SIZE_IS_FIXED = "The maximum capacity for the collection has been reached, since its size is fixed.";
        /// <summary>
        /// Error message, when the amount of items in the collection have changed during enumeration.
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        protected const String COLLECTION_CHANGED = "The collection changed during enumeration.";
#pragma warning restore
    }

    // IEnumerable
    partial class ArrayBasedCollection<TElement> : IEnumerable<TElement>
    {
        /// <inheritdoc />
        [Pure]
        public IEnumerator<TElement> GetEnumerator()
        {
            lock (this._syncRoot)
            {
                Int32 v = this._version;
                for (Int32 i = 0; i < this._size; i++)
                {
                    yield return this._version == v 
                                        ? this._items[i] 
                                        : throw new InvalidOperationException(COLLECTION_CHANGED);
                }
            }
        }

        /// <inheritdoc />
        [Pure]
        IEnumerator IEnumerable.GetEnumerator() => 
            this.GetEnumerator();
    }
}
