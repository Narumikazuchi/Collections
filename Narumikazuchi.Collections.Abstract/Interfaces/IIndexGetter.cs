namespace Narumikazuchi.Collections.Abstract
{
    /// <summary>
    /// Represents an <see cref="System.Collections.Generic.IEnumerable{T}"/> that has a getter for an indexer.
    /// </summary>
    public interface IIndexGetter<TIndex, TElement> : System.Collections.Generic.IEnumerable<TElement> where TIndex : System.IComparable<TIndex>
    {
        #region Indexer

        /// <summary>
        /// Gets the element at the specified index.
        /// </summary>
        /// <param name="index">The index to retrieve.</param>
        /// <returns>The element at the specified index.</returns>
        public TElement this[TIndex index] { get; }

        #endregion
    }
}
