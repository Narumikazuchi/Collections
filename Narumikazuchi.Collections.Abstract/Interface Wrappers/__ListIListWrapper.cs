using System;
using System.Collections;
using System.Collections.Generic;

namespace Narumikazuchi.Collections.Abstract
{
    internal readonly partial struct __ListIListWrapper<Type>
    {
        public __ListIListWrapper(List<Type> source) =>
            this._source = source;

        public static explicit operator List<Type>(__ListIListWrapper<Type> source) =>
            source._source;

    }

    // Non-Public
    partial struct __ListIListWrapper<Type>
    {
        private readonly List<Type> _source;
    }

    // IEnumerable
    partial struct __ListIListWrapper<Type> : IEnumerable<Type>
    {
        public IEnumerator<Type> GetEnumerator() =>
            this._source.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() =>
            this._source.GetEnumerator();
    }

    // IReadOnlyCollection
    partial struct __ListIListWrapper<Type> : IReadOnlyCollection<Type>
    {
        public Int32 Count =>
            this._source.Count;
    }

    // IReadOnlyCollection2
    partial struct __ListIListWrapper<Type> : IReadOnlyCollection2<Type>
    {
        public Boolean Contains(Type item) =>
            this._source.Contains(item);

        public void CopyTo(Type[] array, Int32 index) =>
            this._source.CopyTo(array, index);
    }

    // IReadOnlyList
    partial struct __ListIListWrapper<Type> : IReadOnlyList<Type>
    {
        public Type this[Int32 index] =>
            this._source[index];
    }

    // IReadOnlyList2
    partial struct __ListIListWrapper<Type> : IReadOnlyList2<Type>
    {
        public Int32 IndexOf(Type item) =>
            this._source.IndexOf(item);
    }
}
