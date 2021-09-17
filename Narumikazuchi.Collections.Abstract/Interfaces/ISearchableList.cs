namespace Narumikazuchi.Collections.Abstract
{
    /// <summary>
    /// Represents a strongly-typed, index-accessible collection that can be searched through.
    /// </summary>
    public interface ISearchableList<TElement> : IReadOnlyList2<TElement>, ISearchableCollection<TElement>
    {
        /// <summary>
        /// Searches the sorted <see cref="ISearchableList{TElement}"/> for the specified item in the entire <see cref="ISearchableList{TElement}"/>.
        /// </summary>
        /// <remarks>
        /// If the <see cref="ISearchableList{TElement}"/> is not previously sorted, this method will sort it before searching.
        /// </remarks>
        /// <param name="item">The item to search.</param>
        /// <returns>The zero-based index of the item or -1 if the list doesn't contain the item</returns>
        [System.Diagnostics.Contracts.Pure]
        public System.Int32 BinarySearch([System.Diagnostics.CodeAnalysis.DisallowNull] in TElement item);
        /// <summary>
        /// Searches the sorted <see cref="ISearchableList{TElement}"/> for the specified item using the specified <see cref="System.Collections.Generic.IComparer{T}"/>.
        /// </summary>
        /// <param name="item">The item to search.</param>
        /// <param name="comparer">The comparer to determine a match.</param>
        /// <returns>The zero-based index of the item or -1 if the list doesn't contain the item</returns>
        [System.Diagnostics.Contracts.Pure]
        public System.Int32 BinarySearch([System.Diagnostics.CodeAnalysis.DisallowNull] in TElement item, 
                                         [System.Diagnostics.CodeAnalysis.AllowNull] System.Collections.Generic.IComparer<TElement>? comparer);
        /// <summary>
        /// Searches the sorted <see cref="ISearchableList{TElement}"/> for the specified item in the specified range using the
        /// specified <see cref="System.Collections.Generic.IComparer{T}"/>.
        /// </summary>
        /// <remarks>
        /// If the <see cref="ISearchableList{TElement}"/> is not previously sorted, this method will sort it before searching.
        /// </remarks>
        /// <param name="index">The index of the first item in the range.</param>
        /// <param name="count">The length of the range.</param>
        /// <param name="item">The item to search.</param>
        /// <param name="comparer">The comparer to determine a match.</param>
        /// <returns>The zero-based index of the item or -1 if the list doesn't contain the item</returns>
        [System.Diagnostics.Contracts.Pure]
        public System.Int32 BinarySearch(in System.Int32 index, 
                                         in System.Int32 count, 
                                         [System.Diagnostics.CodeAnalysis.DisallowNull] in TElement item, 
                                         [System.Diagnostics.CodeAnalysis.AllowNull] System.Collections.Generic.IComparer<TElement>? comparer);

        /// <summary>
        /// Searches the entire <see cref="ISearchableList{TElement}"/> for an item matching the specified condition and
        /// returns the index of the first occurence of a matching item.
        /// </summary>
        /// <param name="predicate">The condition to check items against.</param>
        /// <returns>The index of the first item matching the condition or -1 if no item in this list matches the condition</returns>
        [System.Diagnostics.Contracts.Pure]
        public System.Int32 FindIndex([System.Diagnostics.CodeAnalysis.DisallowNull] System.Func<TElement, System.Boolean> predicate);
        /// <summary>
        /// Searches the specified range in the <see cref="ISearchableList{TElement}"/> for an item matching the specified condition and
        /// returns the index of the first occurence of a matching item.
        /// </summary>
        /// <param name="startIndex">The index of the first item in the range.</param>
        /// <param name="count">The length of the range to search.</param>
        /// <param name="predicate">The condition to check items against.</param>
        /// <returns>The index of the first item matching the condition or -1 if no item in this list matches the condition</returns>
        [System.Diagnostics.Contracts.Pure]
        public System.Int32 FindIndex(in System.Int32 startIndex, 
                                      in System.Int32 count, 
                                      [System.Diagnostics.CodeAnalysis.DisallowNull] System.Func<TElement, System.Boolean> predicate);
        /// <summary>
        /// Searches the entire <see cref="ISearchableList{TElement}"/> for an item matching the specified condition and
        /// returns the index of the last occurence of a matching item.
        /// </summary>
        /// <param name="predicate">The condition to check items against.</param>
        /// <returns>The index of the last item matching the condition or -1 if no item in this list matches the condition</returns>
        [System.Diagnostics.Contracts.Pure]
        public System.Int32 FindLastIndex([System.Diagnostics.CodeAnalysis.DisallowNull] System.Func<TElement, System.Boolean> predicate);
        /// <summary>
        /// Searches the specified range in the <see cref="ISearchableList{TElement}"/> for an item matching the specified condition and
        /// returns the index of the last occurence of a matching item.
        /// </summary>
        /// <param name="startIndex">The index of the first item in the range.</param>
        /// <param name="count">The length of the range to search.</param>
        /// <param name="predicate">The condition to check items against.</param>
        /// <returns>The index of the last item matching the condition or -1 if no item in this list matches the condition</returns>
        [System.Diagnostics.Contracts.Pure]
        public System.Int32 FindLastIndex(in System.Int32 startIndex, 
                                          in System.Int32 count, 
                                          [System.Diagnostics.CodeAnalysis.DisallowNull] System.Func<TElement, System.Boolean> predicate);

        /// <summary>
        /// Searches for the specified item and returns the index of the last occurence in the <see cref="ISearchableList{TElement}"/>.
        /// </summary>
        /// <param name="item">The item to search for.</param>
        /// <returns>The index of the last occurence of the specified item or -1 if the item does not exist in the list</returns>
        [System.Diagnostics.Contracts.Pure]
        public System.Int32 LastIndexOf([System.Diagnostics.CodeAnalysis.DisallowNull] in TElement item);
    }
}