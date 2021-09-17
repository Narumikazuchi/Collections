using System;
using System.Collections.Generic;

namespace Narumikazuchi.Collections
{
    internal readonly struct __FuncComparer<TElement> : IComparer<TElement>
    {
        public __FuncComparer(Comparison<TElement?> c) => 
            this.Comparison = c;

        public Int32 Compare(TElement? left, 
                             TElement? right) => 
            this.Comparison.Invoke(left, 
                                   right);

        public Comparison<TElement?> Comparison { get; }
    }
}
