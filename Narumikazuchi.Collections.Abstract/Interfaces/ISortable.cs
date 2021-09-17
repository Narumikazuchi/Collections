namespace Narumikazuchi.Collections.Abstract
{
    /// <summary>
    /// Represents an <see cref="System.Collections.Generic.IEnumerable{T}"/> that can be sorted.
    /// </summary>
    public interface ISortable<TElement> : System.Collections.Generic.IEnumerable<TElement>
    {
        /// <summary>
        /// Sorts the contents of this object in the specifiedorder.
        /// </summary>
        /// <param name="direction">The <see cref="Collections.SortDirection"/> in which to sort the contents of this object.</param>
        public void Sort(SortDirection direction);
        /// <summary>
        /// Sorts the contents of this object in the specified order using the specified comparision.
        /// </summary>
        /// <param name="direction">The <see cref="Collections.SortDirection"/> in which to sort the contents of this object.</param>
        /// <param name="comparison">The comparision to use for the sorting process.</param>
        public void Sort(SortDirection direction, 
                         [System.Diagnostics.CodeAnalysis.DisallowNull] System.Comparison<TElement> comparison);
        /// <summary>
        /// Sorts the contents of this object in the specified order using the specified comparer.
        /// </summary>
        /// <param name="direction">The <see cref="Collections.SortDirection"/> in which to sort the contents of this object.</param>
        /// <param name="comparer">The <see cref="System.Collections.Generic.IComparer{T}"/> to use for the sorting process.</param>
        public void Sort(SortDirection direction,
                         [System.Diagnostics.CodeAnalysis.AllowNull] System.Collections.Generic.IComparer<TElement>? comparer);

        /// <summary>
        /// Gets if the object is currently sorted.
        /// </summary>
        public System.Boolean IsSorted { get; }
        /// <summary>
        /// Gets the direction this collection is sorted in.
        /// </summary>
        public SortDirection SortDirection { get; }
    }
}
