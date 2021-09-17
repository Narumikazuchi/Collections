using System;
using System.Collections;
using System.Collections.Generic;

namespace Narumikazuchi.Collections.Abstract
{
    internal readonly partial struct __SortedSetICollectionWrapper<TElement>
    {
        public __SortedSetICollectionWrapper(SortedSet<TElement> source) =>
            this._source = source;

        public static explicit operator SortedSet<TElement>(__SortedSetICollectionWrapper<TElement> source) =>
            source._source;

    }

    // Non-Public
    partial struct __SortedSetICollectionWrapper<TElement>
    {
        private readonly SortedSet<TElement> _source;
    }

    // IEnumerable
    partial struct __SortedSetICollectionWrapper<TElement> : IEnumerable<TElement>
    {
        public IEnumerator<TElement> GetEnumerator() =>
            this._source.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() =>
            this._source.GetEnumerator();
    }

    // IReadOnlyCollection
    partial struct __SortedSetICollectionWrapper<TElement> : IReadOnlyCollection<TElement>
    {
        public Int32 Count =>
            this._source.Count;
    }

    // IReadOnlyCollection2
    partial struct __SortedSetICollectionWrapper<TElement> : IReadOnlyCollection2<TElement>
    {
        public Boolean Contains(TElement item) =>
            this._source.Contains(item);

        public void CopyTo(TElement[] array, Int32 index) =>
            this._source.CopyTo(array, index);
    }
}
