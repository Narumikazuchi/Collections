﻿using System;
using System.Collections;
using System.Collections.Generic;

namespace Narumikazuchi.Collections.Abstract
{
    internal readonly partial struct __ListICollectionWrapper<Type>
    {
        public __ListICollectionWrapper(List<Type> source) =>
            this._source = source;

        public static explicit operator List<Type>(__ListICollectionWrapper<Type> source) =>
            source._source;

    }

    // Non-Public
    partial struct __ListICollectionWrapper<Type>
    {
        private readonly List<Type> _source;
    }

    // IEnumerable
    partial struct __ListICollectionWrapper<Type> : IEnumerable<Type>
    {
        public IEnumerator<Type> GetEnumerator() =>
            this._source.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() =>
            this._source.GetEnumerator();
    }

    // IReadOnlyCollection
    partial struct __ListICollectionWrapper<Type> : IReadOnlyCollection<Type>
    {
        public Int32 Count =>
            this._source.Count;
    }

    // IReadOnlyCollection2
    partial struct __ListICollectionWrapper<Type> : IReadOnlyCollection2<Type>
    {
        public Boolean Contains(Type item) =>
            this._source.Contains(item);

        public void CopyTo(Type[] array, Int32 index) =>
            this._source.CopyTo(array, index);
    }
}
