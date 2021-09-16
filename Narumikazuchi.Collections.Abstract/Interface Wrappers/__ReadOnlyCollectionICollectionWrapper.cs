using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Narumikazuchi.Collections.Abstract
{
    internal readonly partial struct __ReadOnlyCollectionICollectionWrapper<Type>
    {
        public __ReadOnlyCollectionICollectionWrapper(ReadOnlyCollection<Type> source) =>
            this._source = source;

        public static explicit operator ReadOnlyCollection<Type>(__ReadOnlyCollectionICollectionWrapper<Type> source) =>
            source._source;

    }

    // Non-Public
    partial struct __ReadOnlyCollectionICollectionWrapper<Type>
    {
        private readonly ReadOnlyCollection<Type> _source;
    }

    // IEnumerable
    partial struct __ReadOnlyCollectionICollectionWrapper<Type> : IEnumerable<Type>
    {
        public IEnumerator<Type> GetEnumerator() =>
            this._source.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() =>
            this._source.GetEnumerator();
    }

    // IReadOnlyCollection
    partial struct __ReadOnlyCollectionICollectionWrapper<Type> : IReadOnlyCollection<Type>
    {
        public Int32 Count =>
            this._source.Count;
    }

    // IReadOnlyCollection2
    partial struct __ReadOnlyCollectionICollectionWrapper<Type> : IReadOnlyCollection2<Type>
    {
        public Boolean Contains(Type item) =>
            this._source.Contains(item);

        public void CopyTo(Type[] array, Int32 index) =>
            this._source.CopyTo(array, index);
    }
}
