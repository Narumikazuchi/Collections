using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Narumikazuchi.Collections.Abstract
{
    partial interface IReadOnlyCollection2<T>
    {
        #region Collection Wrapper

        internal sealed class __CollectionICollectionWrapper<Type> : IReadOnlyCollection2<Type>
        {
            #region Constructor

            public __CollectionICollectionWrapper(Collection<Type> source) => this._source = source;

            #endregion

            #region IEnumerable

            /// <inheritdoc/>
            public IEnumerator<Type> GetEnumerator() => this._source.GetEnumerator();

            /// <inheritdoc/>
            IEnumerator IEnumerable.GetEnumerator() => this._source.GetEnumerator();

            #endregion

            #region IReadOnlyCollection

            /// <inheritdoc/>
            public Int32 Count => this._source.Count;

            #endregion

            #region IReadOnlyCollection2

            /// <inheritdoc/>
            public Boolean Contains(Type item) => this._source.Contains(item);

            /// <inheritdoc/>
#pragma warning disable
            public void CopyTo(Type?[] array, Int32 index) => this._source.CopyTo(array, index);
#pragma warning restore

            #endregion

            #region Operators

#pragma warning disable
            public static explicit operator Collection<Type>(__CollectionICollectionWrapper<Type> source) => source._source;
#pragma warning restore

            #endregion

            #region Fields

            private readonly Collection<Type> _source;

            #endregion
        }

        internal sealed class __LinkedListICollectionWrapper<Type> : IReadOnlyCollection2<Type>
        {
            #region Constructor

            public __LinkedListICollectionWrapper(LinkedList<Type> source) => this._source = source;

            #endregion

            #region IEnumerable

            /// <inheritdoc/>
            public IEnumerator<Type> GetEnumerator() => this._source.GetEnumerator();

            /// <inheritdoc/>
            IEnumerator IEnumerable.GetEnumerator() => this._source.GetEnumerator();

            #endregion

            #region IReadOnlyCollection

            /// <inheritdoc/>
            public Int32 Count => this._source.Count;

            #endregion

            #region IReadOnlyCollection2

            /// <inheritdoc/>
            public Boolean Contains(Type item) => this._source.Contains(item);

            /// <inheritdoc/>
#pragma warning disable
            public void CopyTo(Type?[] array, Int32 index) => this._source.CopyTo(array, index);
#pragma warning restore

            #endregion

            #region Operators

#pragma warning disable
            public static explicit operator LinkedList<Type>(__LinkedListICollectionWrapper<Type> source) => source._source;
#pragma warning restore

            #endregion

            #region Fields

            private readonly LinkedList<Type> _source;

            #endregion
        }

        internal sealed class __ListICollectionWrapper<Type> : IReadOnlyCollection2<Type>
        {
            #region Constructor

            public __ListICollectionWrapper(List<Type> source) => this._source = source;

            #endregion

            #region IEnumerable

            /// <inheritdoc/>
            public IEnumerator<Type> GetEnumerator() => this._source.GetEnumerator();

            /// <inheritdoc/>
            IEnumerator IEnumerable.GetEnumerator() => this._source.GetEnumerator();

            #endregion

            #region IReadOnlyCollection

            /// <inheritdoc/>
            public Int32 Count => this._source.Count;

            #endregion

            #region IReadOnlyCollection2

            /// <inheritdoc/>
            public Boolean Contains(Type item) => this._source.Contains(item);

            /// <inheritdoc/>
#pragma warning disable
            public void CopyTo(Type?[] array, Int32 index) => this._source.CopyTo(array, index);
#pragma warning restore

            #endregion

            #region Operators

#pragma warning disable
            public static explicit operator List<Type>(__ListICollectionWrapper<Type> source) => source._source;
#pragma warning restore

            #endregion

            #region Fields

            private readonly List<Type> _source;

            #endregion
        }

        internal sealed class __QueueICollectionWrapper<Type> : IReadOnlyCollection2<Type>
        {
            #region Constructor

            public __QueueICollectionWrapper(Queue<Type> source) => this._source = source;

            #endregion

            #region IEnumerable

            /// <inheritdoc/>
            public IEnumerator<Type> GetEnumerator() => this._source.GetEnumerator();

            /// <inheritdoc/>
            IEnumerator IEnumerable.GetEnumerator() => this._source.GetEnumerator();

            #endregion

            #region IReadOnlyCollection

            /// <inheritdoc/>
            public Int32 Count => this._source.Count;

            #endregion

            #region IReadOnlyCollection2

            /// <inheritdoc/>
            public Boolean Contains(Type item) => this._source.Contains(item);

            /// <inheritdoc/>
#pragma warning disable
            public void CopyTo(Type?[] array, Int32 index) => this._source.CopyTo(array, index);
#pragma warning restore

            #endregion

            #region Operators

#pragma warning disable
            public static explicit operator Queue<Type>(__QueueICollectionWrapper<Type> source) => source._source;
#pragma warning restore

            #endregion

            #region Fields

            private readonly Queue<Type> _source;

            #endregion
        }

        internal sealed class __ReadOnlyCollectionICollectionWrapper<Type> : IReadOnlyCollection2<Type>
        {
            #region Constructor

            public __ReadOnlyCollectionICollectionWrapper(ReadOnlyCollection<Type> source) => this._source = source;

            #endregion

            #region IEnumerable

            /// <inheritdoc/>
            public IEnumerator<Type> GetEnumerator() => this._source.GetEnumerator();

            /// <inheritdoc/>
            IEnumerator IEnumerable.GetEnumerator() => this._source.GetEnumerator();

            #endregion

            #region IReadOnlyCollection

            /// <inheritdoc/>
            public Int32 Count => this._source.Count;

            #endregion

            #region IReadOnlyCollection2

            /// <inheritdoc/>
            public Boolean Contains(Type item) => this._source.Contains(item);

            /// <inheritdoc/>
#pragma warning disable
            public void CopyTo(Type?[] array, Int32 index) => this._source.CopyTo(array, index);
#pragma warning restore

            #endregion

            #region Operators

#pragma warning disable
            public static explicit operator ReadOnlyCollection<Type>(__ReadOnlyCollectionICollectionWrapper<Type> source) => source._source;
#pragma warning restore

            #endregion

            #region Fields

            private readonly ReadOnlyCollection<Type> _source;

            #endregion
        }

        internal sealed class __SortedSetICollectionWrapper<Type> : IReadOnlyCollection2<Type>
        {
            #region Constructor

            public __SortedSetICollectionWrapper(SortedSet<Type> source) => this._source = source;

            #endregion

            #region IEnumerable

            /// <inheritdoc/>
            public IEnumerator<Type> GetEnumerator() => this._source.GetEnumerator();

            /// <inheritdoc/>
            IEnumerator IEnumerable.GetEnumerator() => this._source.GetEnumerator();

            #endregion

            #region IReadOnlyCollection

            /// <inheritdoc/>
            public Int32 Count => this._source.Count;

            #endregion

            #region IReadOnlyCollection2

            /// <inheritdoc/>
            public Boolean Contains(Type item) => this._source.Contains(item);

            /// <inheritdoc/>
#pragma warning disable
            public void CopyTo(Type?[] array, Int32 index) => this._source.CopyTo(array, index);
#pragma warning restore

            #endregion

            #region Operators

#pragma warning disable
            public static explicit operator SortedSet<Type>(__SortedSetICollectionWrapper<Type> source) => source._source;
#pragma warning restore

            #endregion

            #region Fields

            private readonly SortedSet<Type> _source;

            #endregion
        }

        internal sealed class __StackICollectionWrapper<Type> : IReadOnlyCollection2<Type>
        {
            #region Constructor

            public __StackICollectionWrapper(Stack<Type> source) => this._source = source;

            #endregion

            #region IEnumerable

            /// <inheritdoc/>
            public IEnumerator<Type> GetEnumerator() => this._source.GetEnumerator();

            /// <inheritdoc/>
            IEnumerator IEnumerable.GetEnumerator() => this._source.GetEnumerator();

            #endregion

            #region IReadOnlyCollection

            /// <inheritdoc/>
            public Int32 Count => this._source.Count;

            #endregion

            #region IReadOnlyCollection2

            /// <inheritdoc/>
            public Boolean Contains(Type item) => this._source.Contains(item);

            /// <inheritdoc/>
#pragma warning disable
            public void CopyTo(Type?[] array, Int32 index) => this._source.CopyTo(array, index);
#pragma warning restore

            #endregion

            #region Operators

#pragma warning disable
            public static explicit operator Stack<Type>(__StackICollectionWrapper<Type> source) => source._source;
#pragma warning restore

            #endregion

            #region Fields

            private readonly Stack<Type> _source;

            #endregion
        }

        #endregion
    }

    partial interface IReadOnlyList2<T>
    {
        #region List Wrapper

        internal sealed class __CollectionIListWrapper<Type> : IReadOnlyList2<Type>
        {
            #region Constructor

            public __CollectionIListWrapper(Collection<Type> source) => this._source = source;

            #endregion

            #region IEnumerable

            /// <inheritdoc/>
            public IEnumerator<Type> GetEnumerator() => this._source.GetEnumerator();

            /// <inheritdoc/>
            IEnumerator IEnumerable.GetEnumerator() => this._source.GetEnumerator();

            #endregion

            #region IReadOnlyCollection

            /// <inheritdoc/>
            public Int32 Count => this._source.Count;

            #endregion

            #region IReadOnlyCollection2

            /// <inheritdoc/>
            public Boolean Contains(Type item) => this._source.Contains(item);

            /// <inheritdoc/>
#pragma warning disable
            public void CopyTo(Type?[] array, Int32 index) => this._source.CopyTo(array, index);
#pragma warning restore

            #endregion

            #region IReadOnlyList

            /// <inheritdoc/>
            public Type this[Int32 index] => this._source[index];

            #endregion

            #region IReadOnlyList2

            /// <inheritdoc/>
            public Int32 IndexOf(Type item) => this._source.IndexOf(item);

            #endregion

            #region Operators

#pragma warning disable
            public static explicit operator Collection<Type>(__CollectionIListWrapper<Type> source) => source._source;
#pragma warning restore

            #endregion

            #region Fields

            private readonly Collection<Type> _source;

            #endregion
        }

        internal sealed class __ListIListWrapper<Type> : IReadOnlyList2<Type>
        {
            #region Constructor

            public __ListIListWrapper(List<Type> source) => this._source = source;

            #endregion

            #region IEnumerable

            /// <inheritdoc/>
            public IEnumerator<Type> GetEnumerator() => this._source.GetEnumerator();

            /// <inheritdoc/>
            IEnumerator IEnumerable.GetEnumerator() => this._source.GetEnumerator();

            #endregion

            #region IReadOnlyCollection

            /// <inheritdoc/>
            public Int32 Count => this._source.Count;

            #endregion

            #region IReadOnlyCollection2

            /// <inheritdoc/>
            public Boolean Contains(Type item) => this._source.Contains(item);

            /// <inheritdoc/>
#pragma warning disable
            public void CopyTo(Type?[] array, Int32 index) => this._source.CopyTo(array, index);
#pragma warning restore

            #endregion

            #region IReadOnlyList

            /// <inheritdoc/>
            public Type this[Int32 index] => this._source[index];

            #endregion

            #region IReadOnlyList2

            /// <inheritdoc/>
            public Int32 IndexOf(Type item) => this._source.IndexOf(item);

            #endregion

            #region Operators

#pragma warning disable
            public static explicit operator List<Type>(__ListIListWrapper<Type> source) => source._source;
#pragma warning restore

            #endregion

            #region Fields

            private readonly List<Type> _source;

            #endregion
        }

        internal sealed class __ReadOnlyCollectionIListWrapper<Type> : IReadOnlyList2<Type>
        {
            #region Constructor

            public __ReadOnlyCollectionIListWrapper(ReadOnlyCollection<Type> source) => this._source = source;

            #endregion

            #region IEnumerable

            /// <inheritdoc/>
            public IEnumerator<Type> GetEnumerator() => this._source.GetEnumerator();

            /// <inheritdoc/>
            IEnumerator IEnumerable.GetEnumerator() => this._source.GetEnumerator();

            #endregion

            #region IReadOnlyCollection

            /// <inheritdoc/>
            public Int32 Count => this._source.Count;

            #endregion

            #region IReadOnlyCollection2

            /// <inheritdoc/>
            public Boolean Contains(Type item) => this._source.Contains(item);

            /// <inheritdoc/>
#pragma warning disable
            public void CopyTo(Type?[] array, Int32 index) => this._source.CopyTo(array, index);
#pragma warning restore

            #endregion

            #region IReadOnlyList

            /// <inheritdoc/>
            public Type this[Int32 index] => this._source[index];

            #endregion

            #region IReadOnlyList2

            /// <inheritdoc/>
            public Int32 IndexOf(Type item) => this._source.IndexOf(item);

            #endregion

            #region Operators

#pragma warning disable
            public static explicit operator ReadOnlyCollection<Type>(__ReadOnlyCollectionIListWrapper<Type> source) => source._source;
#pragma warning restore

            #endregion

            #region Fields

            private readonly ReadOnlyCollection<Type> _source;

            #endregion
        }

        #endregion
    }
}
