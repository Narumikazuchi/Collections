namespace Narumikazuchi.Collections.Abstract
{
    /// <summary>
    /// Represents a read-only collection of elements that can be accessed by index.
    /// </summary>
    public partial interface IReadOnlyList2<T> : IReadOnlyCollection2<T>, System.Collections.Generic.IReadOnlyList<T>
    {
        #region Management Methods

        /// <summary>
        /// Determines the index of a specific tem in the <see cref="IReadOnlyList2{T}"/>.
        /// </summary>
        /// <param name="item">The object to locate in the <see cref="IReadOnlyList2{T}"/>.</param>
        /// <returns>The index of <paramref name="item"/> if found in the <see cref="IReadOnlyList2{T}"/>; otherwise, -1</returns>
        public System.Int32 IndexOf([System.Diagnostics.CodeAnalysis.DisallowNull] T item);

        #endregion
    }
}
