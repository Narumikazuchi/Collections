using System;
using System.Collections;
using System.Collections.Generic;

namespace Narumikazuchi.Collections.Abstract
{
    internal readonly partial struct __StackICollectionWrapper<Type>
    {
        public __StackICollectionWrapper(Stack<Type> source) =>
            this._source = source;

        public static explicit operator Stack<Type>(__StackICollectionWrapper<Type> source) =>
            source._source;

    }

    // Non-Public
    partial struct __StackICollectionWrapper<Type>
    {
        private readonly Stack<Type> _source;
    }

    // IEnumerable
    partial struct __StackICollectionWrapper<Type> : IEnumerable<Type>
    {
        public IEnumerator<Type> GetEnumerator() =>
            this._source.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() =>
            this._source.GetEnumerator();
    }

    // IReadOnlyCollection
    partial struct __StackICollectionWrapper<Type> : IReadOnlyCollection<Type>
    {
        public Int32 Count =>
            this._source.Count;
    }

    // IReadOnlyCollection2
    partial struct __StackICollectionWrapper<Type> : IReadOnlyCollection2<Type>
    {
        public Boolean Contains(Type item) =>
            this._source.Contains(item);

        public void CopyTo(Type[] array, Int32 index) =>
            this._source.CopyTo(array, index);
    }
}
