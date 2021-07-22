using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using System.Linq;

namespace Narumikazuchi.Collections
{
    internal readonly struct __FuncEqualityComparer<T> : IEqualityComparer<T>
    {
        #region Constructor

        public __FuncEqualityComparer(EqualityComparison<T> c) => this.Comparison = c;

        #endregion

        #region Compare

#pragma warning disable
        [Pure]
        public Boolean Equals(T? left, T? right) => this.Comparison.Invoke(left, right);
#pragma warning restore

        [Pure]
        public Int32 GetHashCode([DisallowNull] T obj) => obj.GetHashCode();

        #endregion

        #region Properties

        public static ref readonly __FuncEqualityComparer<T> Default => ref _default;

        public EqualityComparison<T> Comparison { get; }

        #endregion

        #region Fields

#pragma warning disable
        private static readonly __FuncEqualityComparer<T> _default = new((a, b) => typeof(T).IsValueType || typeof(T).GetInterfaces().Contains(typeof(IEquatable<T>)) ? a.Equals(b) : ReferenceEquals(a, b));
#pragma warning restore

        #endregion
    }
}
