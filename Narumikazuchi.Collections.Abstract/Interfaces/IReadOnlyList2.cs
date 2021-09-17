namespace Narumikazuchi.Collections.Abstract
{
    /// <summary>
    /// Represents a read-only collection of elements that can be accessed by index.
    /// </summary>
    public partial interface IReadOnlyList2<TElement> : IIndexGetter<System.Int32, TElement>, IReadOnlyCollection2<TElement>, System.Collections.Generic.IReadOnlyList<TElement>
    {
        /// <summary>
        /// Determines the index of a specific tem in the <see cref="IReadOnlyList2{TElement}"/>.
        /// </summary>
        /// <param name="item">The object to locate in the <see cref="IReadOnlyList2{TElement}"/>.</param>
        /// <returns>The index of <paramref name="item"/> if found in the <see cref="IReadOnlyList2{TElement}"/>; otherwise, -1</returns>
        public System.Int32 IndexOf([System.Diagnostics.CodeAnalysis.DisallowNull] TElement item);
    }
}
