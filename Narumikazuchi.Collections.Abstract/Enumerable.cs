using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Narumikazuchi.Collections.Abstract
{
    /// <summary>
    /// Extends the <see cref="IEnumerable{T}"/> interface.
    /// </summary>
    public static partial class Enumerable
    {
        #region Conversion to introduced Interfaces

        /// <summary>
        /// Represents this object as the <see cref="IReadOnlyCollection2{T}"/> interface.
        /// </summary>
        public static IReadOnlyCollection2<T> AsIReadOnlyCollection2<T>(this Collection<T> source) =>
            new __CollectionICollectionWrapper<T>(source);

        /// <summary>
        /// Represents this object as the <see cref="IReadOnlyCollection2{T}"/> interface.
        /// </summary>
        public static IReadOnlyCollection2<T> AsIReadOnlyCollection2<T>(this LinkedList<T> source) =>
            new __LinkedListICollectionWrapper<T>(source);

        /// <summary>
        /// Represents this object as the <see cref="IReadOnlyCollection2{T}"/> interface.
        /// </summary>
        public static IReadOnlyCollection2<T> AsIReadOnlyCollection2<T>(this List<T> source) =>
            new __ListICollectionWrapper<T>(source);

        /// <summary>
        /// Represents this object as the <see cref="IReadOnlyCollection2{T}"/> interface.
        /// </summary>
        public static IReadOnlyCollection2<T> AsIReadOnlyCollection2<T>(this Queue<T> source) =>
            new __QueueICollectionWrapper<T>(source);

        /// <summary>
        /// Represents this object as the <see cref="IReadOnlyCollection2{T}"/> interface.
        /// </summary>
        public static IReadOnlyCollection2<T> AsIReadOnlyCollection2<T>(this ReadOnlyCollection<T> source) =>
            new __ReadOnlyCollectionICollectionWrapper<T>(source);

        /// <summary>
        /// Represents this object as the <see cref="IReadOnlyCollection2{T}"/> interface.
        /// </summary>
        public static IReadOnlyCollection2<T> AsIReadOnlyCollection2<T>(this SortedSet<T> source) =>
            new __SortedSetICollectionWrapper<T>(source);

        /// <summary>
        /// Represents this object as the <see cref="IReadOnlyCollection2{T}"/> interface.
        /// </summary>
        public static IReadOnlyCollection2<T> AsIReadOnlyCollection2<T>(this Stack<T> source) =>
            new __StackICollectionWrapper<T>(source);

        /// <summary>
        /// Represents this object as the <see cref="IReadOnlyList2{T}"/> interface.
        /// </summary>
        public static IReadOnlyList2<T> AsIReadOnlyList2<T>(this Collection<T> source) =>
            new __CollectionIListWrapper<T>(source);

        /// <summary>
        /// Represents this object as the <see cref="IReadOnlyList2{T}"/> interface.
        /// </summary>
        public static IReadOnlyList2<T> AsIReadOnlyList2<T>(this List<T> source) =>
            new __ListIListWrapper<T>(source);

        /// <summary>
        /// Represents this object as the <see cref="IReadOnlyList2{T}"/> interface.
        /// </summary>
        public static IReadOnlyList2<T> AsIReadOnlyList2<T>(this ReadOnlyCollection<T> source) =>
            new __ReadOnlyCollectionIListWrapper<T>(source);

        #endregion
    }
}
