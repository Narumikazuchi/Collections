using System;

namespace Narumikazuchi.Collections
{
    /// <summary>
    /// Represents a class that contains event data for item changes inside a collection.
    /// </summary>
    public sealed class ItemChangedEventArgs<T> : EventArgs
    {
        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="ItemChangedEventArgs{T}"/> class.
        /// </summary>
        /// <param name="item">The item that changed.</param>
        /// <param name="previousValue">The value of the changed property before it had changed.</param>
        /// <param name="property">The name of the property that changed.</param>
        public ItemChangedEventArgs(in T? item, Object? previousValue, String? property)
        {
            this.ChangedItem = item;
            this.PreviousValue = previousValue;
            this.ChangedProperty = property;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the item that has changed.
        /// </summary>
        public T? ChangedItem { get; }
        /// <summary>
        /// Gets the value of the changed property before the change happened.
        /// </summary>
        public Object? PreviousValue { get; }
        /// <summary>
        /// Gets the name of the property that changed.
        /// </summary>
        public String? ChangedProperty { get; }

        #endregion
    }
}
