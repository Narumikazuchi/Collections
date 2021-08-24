namespace Narumikazuchi.Collections
{
    /// <summary>
    /// Represents an <see cref="System.Collections.Generic.IList{T}"/> where every object is only contained once.
    /// </summary>
    public interface IReadOnlyRegister<T> : Abstract.IReadOnlyCollection2<T>, Abstract.IReadOnlyList2<T>, System.Collections.Generic.IReadOnlySet<T>
    {
        #region Properties

        /// <summary>
        /// Gets or sets the <see cref="System.Collections.Generic.IEqualityComparer{T}"/> that the <see cref="IRegister{T}"/> uses for duplicate-checks.
        /// </summary>
        public System.Collections.Generic.IEqualityComparer<T>? Comparer { get; set; }

        #endregion
    }
}