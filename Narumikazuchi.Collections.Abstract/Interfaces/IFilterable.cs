namespace Narumikazuchi.Collections.Abstract
{
    /// <summary>
    /// Represents a collection that can be filtered.
    /// </summary>
    public interface IFilterable<out T> : System.Collections.Generic.IEnumerable<T>
    {
        #region Filtering Methods

        /// <summary>
        /// Filters the <see cref="IFilterable{T}"/> through the specified predicate.
        /// </summary>
        /// <param name="predicate">The predicate to filter the collection by.</param>
        /// <returns><see langword="true"/> if the filtering succeeded, else <see langword="false"/></returns>
        public System.Boolean Filter(System.Func<T, System.Boolean> predicate);

        /// <summary>
        /// Resets the filtering for the <see cref="IFilterable{T}"/>.
        /// </summary>
        public void ResetFilter();

        #endregion

        #region Properties

        /// <summary>
        /// Gets if the <see cref="IFilterable{T}"/> is currently filtered.
        /// </summary>
        public System.Boolean IsFiltered { get; }

        #endregion
    }
}
