using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Narumikazuchi.Collections.Abstract
{
    internal readonly partial struct __ReadOnlyCollectionIListWrapper<TElement>
    {
        public __ReadOnlyCollectionIListWrapper(ReadOnlyCollection<TElement> source) =>
            this._source = source;

        public static explicit operator ReadOnlyCollection<TElement>(__ReadOnlyCollectionIListWrapper<TElement> source) =>
            source._source;

    }

    // Non-Public
    partial struct __ReadOnlyCollectionIListWrapper<TElement>
    {
        private readonly ReadOnlyCollection<TElement> _source;
    }

    // IEnumerable
    partial struct __ReadOnlyCollectionIListWrapper<TElement> : IEnumerable<TElement>
    {
        public IEnumerator<TElement> GetEnumerator() =>
            this._source.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() =>
            this._source.GetEnumerator();
    }

    // IReadOnlyCollection
    partial struct __ReadOnlyCollectionIListWrapper<TElement> : IReadOnlyCollection<TElement>
    {
        public Int32 Count =>
            this._source.Count;
    }

    // IReadOnlyCollection2
    partial struct __ReadOnlyCollectionIListWrapper<TElement> : IReadOnlyCollection2<TElement>
    {
        public Boolean Contains(TElement item) =>
            this._source.Contains(item);

        public void CopyTo(TElement[] array, Int32 index) =>
            this._source.CopyTo(array, index);
    }

    // IReadOnlyList
    partial struct __ReadOnlyCollectionIListWrapper<TElement> : IReadOnlyList<TElement>
    {
        public TElement this[Int32 index] =>
            this._source[index];
    }

    // IReadOnlyList2
    partial struct __ReadOnlyCollectionIListWrapper<TElement> : IReadOnlyList2<TElement>
    {
        public Int32 IndexOf(TElement item) =>
            this._source.IndexOf(item);
    }
}
