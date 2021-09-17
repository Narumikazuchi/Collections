namespace Narumikazuchi.Collections
{
    /// <summary>
    /// Represents an <see cref="System.Collections.Generic.IList{T}"/> where every object is only contained once.
    /// </summary>
    public interface IReadOnlyRegister<TElement> : Abstract.IReadOnlyCollection2<TElement>, Abstract.IReadOnlyList2<TElement>, System.Collections.Generic.IReadOnlySet<TElement>
    {
        /// <summary>
        /// Gets or sets the <see cref="System.Collections.Generic.IEqualityComparer{T}"/> that the <see cref="IReadOnlyRegister{TElement}"/> uses for duplicate-checks.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.MaybeNull]
        public System.Collections.Generic.IEqualityComparer<TElement>? Comparer { get; set; }
    }
}