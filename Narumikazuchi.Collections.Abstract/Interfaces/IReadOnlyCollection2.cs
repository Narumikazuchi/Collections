namespace Narumikazuchi.Collections.Abstract
{
    /// <summary>
    /// Represents a strongly-typed, read-only collection of elements.
    /// </summary>
    public partial interface IReadOnlyCollection2<TElement> : System.Collections.Generic.IReadOnlyCollection<TElement>
    {
        /// <summary>
        /// Determines whether the <see cref="IReadOnlyCollection2{TElement}"/> contains a specific value.
        /// </summary>
        /// <param name="item">The object to locate in the <see cref="IReadOnlyCollection2{T}"/>.</param>
        /// <returns><see langword="true"/> if <paramref name="item"/> is found in the <see cref="IReadOnlyCollection2{T}"/>; otherwise, <see langword="false"/></returns>
        public System.Boolean Contains([System.Diagnostics.CodeAnalysis.DisallowNull] TElement item);

        /// <summary>
        /// Copies the elements of the <see cref="IReadOnlyCollection2{TElement}"/> to an <see cref="System.Array"/>, starting at a particular <see cref="System.Array"/> index.
        /// </summary>
        /// <param name="array">The one-dimensional <see cref="System.Array"/> that is the destination of the elements copied from the <see cref="IReadOnlyCollection2{TElement}"/>.
        /// The <see cref="System.Array"/> must have zero-based indexing.</param>
        /// <param name="index">The zero-based index in <paramref name="array"/> at which copying begins.</param>
        public void CopyTo([System.Diagnostics.CodeAnalysis.DisallowNull] TElement[] array, 
                           System.Int32 index);
    }
}
