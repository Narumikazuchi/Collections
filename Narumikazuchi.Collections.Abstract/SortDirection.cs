namespace Narumikazuchi.Collections
{

    /// <summary>
    /// Defines the direction in which an <see cref="ISortable"/> is sorted.
    /// </summary>
    public enum SortDirection
    {
        /// <summary>
        /// The <see cref="ISortable"/> is not sorted.
        /// </summary>
        NotSorted = 0,
        /// <summary>
        /// The <see cref="ISortable"/> is sorted in ascending order.
        /// </summary>
        Ascending = 1,
        /// <summary>
        /// The <see cref="ISortable"/> is sorted in descending order.
        /// </summary>
        Descending = 2
    }
}
