using System;
using System.Collections.Generic;

namespace Narumikazuchi.Collections.Trees
{
    /// <summary>
    /// Extends the <see cref="IEnumerable{T}"/> interface.
    /// </summary>
    public static class Enumerable
    {
        #region Conversion to introduced Collections

        /// <summary>
        /// Creates a <see cref="BinaryTree{T}"/> from an <see cref="IEnumerable{T}"/>.
        /// </summary>
        public static BinaryTree<T> ToBinaryTree<T>(this IEnumerable<T> source) where T : IComparable<T> =>
            source is BinaryTree<T> tree ? tree : new BinaryTree<T>(source);

        #endregion
    }
}
