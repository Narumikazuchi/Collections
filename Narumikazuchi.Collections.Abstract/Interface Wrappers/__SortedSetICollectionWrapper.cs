using System;
using System.Collections;
using System.Collections.Generic;

namespace Narumikazuchi.Collections.Abstract
{
    internal readonly partial struct __SortedSetICollectionWrapper<Type>
    {
        public __SortedSetICollectionWrapper(SortedSet<Type> source) =>
            this._source = source;

        public static explicit operator SortedSet<Type>(__SortedSetICollectionWrapper<Type> source) =>
            source._source;

    }

    // Non-Public
    partial struct __SortedSetICollectionWrapper<Type>
    {
        private readonly SortedSet<Type> _source;
    }

    // IEnumerable
    partial struct __SortedSetICollectionWrapper<Type> : IEnumerable<Type>
    {
        public IEnumerator<Type> GetEnumerator() =>
            this._source.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() =>
            this._source.GetEnumerator();
    }

    // IReadOnlyCollection
    partial struct __SortedSetICollectionWrapper<Type> : IReadOnlyCollection<Type>
    {
        public Int32 Count =>
            this._source.Count;
    }

    // IReadOnlyCollection2
    partial struct __SortedSetICollectionWrapper<Type> : IReadOnlyCollection2<Type>
    {
        public Boolean Contains(Type item) =>
            this._source.Contains(item);

        public void CopyTo(Type[] array, Int32 index) =>
            this._source.CopyTo(array, index);
    }
}
