using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Narumikazuchi.Collections.Abstract
{
    internal readonly partial struct __ReadOnlyCollectionICollectionWrapper<TElement>
    {
        public __ReadOnlyCollectionICollectionWrapper(ReadOnlyCollection<TElement> source) =>
            this._source = source;

        public static explicit operator ReadOnlyCollection<TElement>(__ReadOnlyCollectionICollectionWrapper<TElement> source) =>
            source._source;

    }

    // Non-Public
    partial struct __ReadOnlyCollectionICollectionWrapper<TElement>
    {
        private readonly ReadOnlyCollection<TElement> _source;
    }

    // IEnumerable
    partial struct __ReadOnlyCollectionICollectionWrapper<TElement> : IEnumerable<TElement>
    {
        public IEnumerator<TElement> GetEnumerator() =>
            this._source.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() =>
            this._source.GetEnumerator();
    }

    // IReadOnlyCollection
    partial struct __ReadOnlyCollectionICollectionWrapper<TElement> : IReadOnlyCollection<TElement>
    {
        public Int32 Count =>
            this._source.Count;
    }

    // IReadOnlyCollection2
    partial struct __ReadOnlyCollectionICollectionWrapper<TElement> : IReadOnlyCollection2<TElement>
    {
        public Boolean Contains(TElement item) =>
            this._source.Contains(item);

        public void CopyTo(TElement[] array, Int32 index) =>
            this._source.CopyTo(array, index);
    }
}
