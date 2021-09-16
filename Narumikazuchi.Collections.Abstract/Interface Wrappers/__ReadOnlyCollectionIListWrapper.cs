using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Narumikazuchi.Collections.Abstract
{
    internal readonly partial struct __ReadOnlyCollectionIListWrapper<Type>
    {
        public __ReadOnlyCollectionIListWrapper(ReadOnlyCollection<Type> source) =>
            this._source = source;

        public static explicit operator ReadOnlyCollection<Type>(__ReadOnlyCollectionIListWrapper<Type> source) =>
            source._source;

    }

    // Non-Public
    partial struct __ReadOnlyCollectionIListWrapper<Type>
    {
        private readonly ReadOnlyCollection<Type> _source;
    }

    // IEnumerable
    partial struct __ReadOnlyCollectionIListWrapper<Type> : IEnumerable<Type>
    {
        public IEnumerator<Type> GetEnumerator() =>
            this._source.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() =>
            this._source.GetEnumerator();
    }

    // IReadOnlyCollection
    partial struct __ReadOnlyCollectionIListWrapper<Type> : IReadOnlyCollection<Type>
    {
        public Int32 Count =>
            this._source.Count;
    }

    // IReadOnlyCollection2
    partial struct __ReadOnlyCollectionIListWrapper<Type> : IReadOnlyCollection2<Type>
    {
        public Boolean Contains(Type item) =>
            this._source.Contains(item);

        public void CopyTo(Type[] array, Int32 index) =>
            this._source.CopyTo(array, index);
    }

    // IReadOnlyList
    partial struct __ReadOnlyCollectionIListWrapper<Type> : IReadOnlyList<Type>
    {
        public Type this[Int32 index] =>
            this._source[index];
    }

    // IReadOnlyList2
    partial struct __ReadOnlyCollectionIListWrapper<Type> : IReadOnlyList2<Type>
    {
        public Int32 IndexOf(Type item) =>
            this._source.IndexOf(item);
    }
}
