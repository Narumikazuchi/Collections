using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;

namespace Narumikazuchi.Collections
{
    internal readonly struct __FuncComparer<T> : IComparer<T>
    {
        #region Constructor

        public __FuncComparer(Comparison<T?> c) => this.Comparison = c;

        #endregion

        #region Compare

        [Pure]
        public Int32 Compare(T? left, T? right) => this.Comparison.Invoke(left, right);

        #endregion

        #region Properties

        public Comparison<T?> Comparison { get; }

        #endregion
    }
}
