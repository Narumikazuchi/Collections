using System;
using System.Collections;
using System.Collections.Generic;

namespace Narumikazuchi.Collections.Abstract
{
    internal readonly partial struct __QueueICollectionWrapper<Type>
    {
        public __QueueICollectionWrapper(Queue<Type> source) =>
            this._source = source;

        public static explicit operator Queue<Type>(__QueueICollectionWrapper<Type> source) =>
            source._source;

    }

    // Non-Public
    partial struct __QueueICollectionWrapper<Type>
    {
        private readonly Queue<Type> _source;
    }

    // IEnumerable
    partial struct __QueueICollectionWrapper<Type> : IEnumerable<Type>
    {
        public IEnumerator<Type> GetEnumerator() =>
            this._source.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() =>
            this._source.GetEnumerator();
    }

    // IReadOnlyCollection
    partial struct __QueueICollectionWrapper<Type> : IReadOnlyCollection<Type>
    {
        public Int32 Count =>
            this._source.Count;
    }

    // IReadOnlyCollection2
    partial struct __QueueICollectionWrapper<Type> : IReadOnlyCollection2<Type>
    {
        public Boolean Contains(Type item) =>
            this._source.Contains(item);

        public void CopyTo(Type[] array, Int32 index) =>
            this._source.CopyTo(array, index);
    }
}
