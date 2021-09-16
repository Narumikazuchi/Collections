namespace Narumikazuchi.Collections.Abstract
{
    /// <summary>
    /// Represents an <see cref="System.Collections.Generic.IEnumerable{T}"/> that has a setter for an indexer.
    /// </summary>
    public interface IIndexSetter<TIndex, TElement> : System.Collections.Generic.IEnumerable<TElement> 
        where TIndex : System.IComparable<TIndex>
    {
        /// <summary>
        /// Sets the element at the specified index.
        /// </summary>
        /// <param name="index">The index to set the element to.</param>
        public TElement this[TIndex index] { set; }
    }
}
