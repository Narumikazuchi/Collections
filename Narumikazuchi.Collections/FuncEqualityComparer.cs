using System;
using System.Collections.Generic;
using System.Linq;

namespace Narumikazuchi.Collections
{
    internal readonly struct __FuncEqualityComparer<TElement> : IEqualityComparer<TElement>
    {
        public __FuncEqualityComparer(EqualityComparison<TElement> c) => 
            this.Comparison = c;

        public Boolean Equals(TElement? left, 
                              TElement? right) => 
            this.Comparison.Invoke(left, 
                                   right);

        public Int32 GetHashCode(TElement obj) => 
            obj.GetHashCode();

        public static ref readonly __FuncEqualityComparer<TElement> Default => ref _default;

        public EqualityComparison<TElement> Comparison { get; }

        private static readonly __FuncEqualityComparer<TElement> _default = new((a, b) => typeof(TElement).IsValueType || 
                                                                                          typeof(TElement).GetInterfaces()
                                                                                                          .Contains(typeof(IEquatable<TElement>)) 
                                                                                                ? a.Equals(b) 
                                                                                                : ReferenceEquals(a, b));
    }
}
