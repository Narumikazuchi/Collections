﻿namespace Narumikazuchi.Collections.Abstract
{
    /// <summary>
    /// Represents an <see cref="System.Collections.Generic.IEnumerable{T}"/> that can be automatically sorted upon collection change.
    /// </summary>
    public interface IAutoSortable<T> : ISortable<T>
    {
        /// <summary>
        /// Enables automatic sorting for this object.
        /// </summary>
        /// <param name="direction">The <see cref="SortDirection"/> in which to apply automatic sorting.</param>
        public void EnableAutoSort(SortDirection direction);
        /// <summary>
        /// Enables automatic sorting for this object.
        /// </summary>
        /// <param name="direction">The <see cref="SortDirection"/> in which to apply automatic sorting.</param>
        /// <param name="comparison">The comparision to use for the sorting process.</param>
        public void EnableAutoSort(SortDirection direction, 
                                   [System.Diagnostics.CodeAnalysis.DisallowNull] System.Comparison<T> comparison);
        /// <summary>
        /// Enables automatic sorting for this object.
        /// </summary>
        /// <param name="direction">The <see cref="SortDirection"/> in which to apply automatic sorting.</param>
        /// <param name="comparer">The <see cref="System.Collections.Generic.IComparer{T}"/> to use for the sorting process.</param>
        public void EnableAutoSort(SortDirection direction,
                                   [System.Diagnostics.CodeAnalysis.AllowNull] System.Collections.Generic.IComparer<T>? comparer);
        /// <summary>
        /// Disables automatic sorting for this object.
        /// </summary>
        public void DisableAutoSort();

        /// <summary>
        /// Gets if the contents of this object should be automatically sorted upon change.
        /// </summary>
        public System.Boolean AutoSort { get; }
    }
}
