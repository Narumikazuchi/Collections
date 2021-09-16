namespace Narumikazuchi.Collections.Abstract
{
    /// <summary>
    /// Represents a strongly-typed collection that can be searched through.
    /// </summary>
    public interface ISearchableCollection<T> : IReadOnlyCollection2<T>
    {
        /// <summary>
        /// Determines if the <see cref="ISearchableCollection{T}"/> contains item matching the specified condition.
        /// </summary>
        /// <param name="predicate">The condition to check items against.</param>
        /// <returns><see langword="true"/> if at least one item matching the condition exists in this list; else <see langword="false"/></returns>
        [System.Diagnostics.Contracts.Pure]
        public System.Boolean Exists([System.Diagnostics.CodeAnalysis.DisallowNull] System.Func<T, System.Boolean> predicate);

        /// <summary>
        /// Searches the <see cref="ISearchableCollection{T}"/> for an item matching the specified condition and
        /// returns the first occurence of a matching item.
        /// </summary>
        /// <param name="predicate">The condition to check items against.</param>
        /// <returns>The first item matching the condition or <see langword="default"/> if no item in this list matches the condition</returns>
        [System.Diagnostics.Contracts.Pure]
        [return: System.Diagnostics.CodeAnalysis.MaybeNull]
        public T Find([System.Diagnostics.CodeAnalysis.DisallowNull] System.Func<T, System.Boolean> predicate);
        /// <summary>
        /// Retrieves all items in the <see cref="ISearchableCollection{T}"/> that match the specified condition.
        /// </summary>
        /// <param name="predicate">The condition to check items against.</param>
        /// <returns>An <see cref="IReadOnlyList2{T}"/> containing all items matching the condition</returns>
        [System.Diagnostics.Contracts.Pure]
        [return: System.Diagnostics.CodeAnalysis.NotNull]
        public IReadOnlyList2<T> FindAll([System.Diagnostics.CodeAnalysis.DisallowNull] System.Func<T, System.Boolean> predicate);
        /// <summary>
        /// Retrieves all items in the <see cref="ISearchableCollection{T}"/> that do not match the specified condition.
        /// </summary>
        /// <param name="predicate">The condition to check items against.</param>
        /// <returns>An <see cref="IReadOnlyList2{T}"/> containing all items not matching the condition</returns>
        [System.Diagnostics.Contracts.Pure]
        [return: System.Diagnostics.CodeAnalysis.NotNull]
        public IReadOnlyList2<T> FindExcept([System.Diagnostics.CodeAnalysis.DisallowNull] System.Func<T, System.Boolean> predicate);

        /// <summary>
        /// Searches the <see cref="ISearchableCollection{T}"/> for an item matching the specified condition and
        /// returns the last occurence of a matching item.
        /// </summary>
        /// <param name="predicate">The condition to check items against.</param>
        /// <returns>The last item matching the condition or <see langword="default"/> if no item in this list matches the condition</returns>
        [System.Diagnostics.Contracts.Pure]
        [return: System.Diagnostics.CodeAnalysis.MaybeNull]
        public T FindLast([System.Diagnostics.CodeAnalysis.DisallowNull] System.Func<T, System.Boolean> predicate);
    }
}