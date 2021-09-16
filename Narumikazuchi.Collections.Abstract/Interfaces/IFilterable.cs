namespace Narumikazuchi.Collections.Abstract
{
    /// <summary>
    /// Represents a collection that can be filtered.
    /// </summary>
    public interface IFilterable<T> : System.Collections.Generic.IEnumerable<T>
    {
        /// <summary>
        /// Filters the <see cref="IFilterable{T}"/> through the specified predicate.
        /// </summary>
        /// <param name="predicate">The predicate to filter the collection by.</param>
        /// <returns><see langword="true"/> if the filtering succeeded, else <see langword="false"/></returns>
        public System.Boolean Filter([System.Diagnostics.CodeAnalysis.DisallowNull] System.Func<T, System.Boolean> predicate);
        /// <summary>
        /// Resets the filtering for the <see cref="IFilterable{T}"/>.
        /// </summary>
        public void ResetFilter();

        /// <summary>
        /// Gets if the <see cref="IFilterable{T}"/> is currently filtered.
        /// </summary>
        public System.Boolean IsFiltered { get; }
    }
}
