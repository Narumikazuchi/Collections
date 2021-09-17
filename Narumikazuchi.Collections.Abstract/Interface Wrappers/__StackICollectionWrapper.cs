using System;
using System.Collections;
using System.Collections.Generic;

namespace Narumikazuchi.Collections.Abstract
{
    internal readonly partial struct __StackICollectionWrapper<TElement>
    {
        public __StackICollectionWrapper(Stack<TElement> source) =>
            this._source = source;

        public static explicit operator Stack<TElement>(__StackICollectionWrapper<TElement> source) =>
            source._source;

    }

    // Non-Public
    partial struct __StackICollectionWrapper<TElement>
    {
        private readonly Stack<TElement> _source;
    }

    // IEnumerable
    partial struct __StackICollectionWrapper<TElement> : IEnumerable<TElement>
    {
        public IEnumerator<TElement> GetEnumerator() =>
            this._source.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() =>
            this._source.GetEnumerator();
    }

    // IReadOnlyCollection
    partial struct __StackICollectionWrapper<TElement> : IReadOnlyCollection<TElement>
    {
        public Int32 Count =>
            this._source.Count;
    }

    // IReadOnlyCollection2
    partial struct __StackICollectionWrapper<TElement> : IReadOnlyCollection2<TElement>
    {
        public Boolean Contains(TElement item) =>
            this._source.Contains(item);

        public void CopyTo(TElement[] array, Int32 index) =>
            this._source.CopyTo(array, index);
    }
}
