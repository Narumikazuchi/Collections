using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Narumikazuchi.Collections.Abstract
{
    /// <summary>
    /// Extends the <see cref="IEnumerable{T}"/> interface.
    /// </summary>
    public static class Enumerable
    {
        /// <summary>
        /// Represents this object as the <see cref="IReadOnlyCollection2{TElement}"/> interface.
        /// </summary>
        public static IReadOnlyCollection2<TElement> AsIReadOnlyCollection2<TElement>(this Collection<TElement> source) =>
            new __CollectionICollectionWrapper<TElement>(source);

        /// <summary>
        /// Represents this object as the <see cref="IReadOnlyCollection2{TElement}"/> interface.
        /// </summary>
        public static IReadOnlyCollection2<TElement> AsIReadOnlyCollection2<TElement>(this LinkedList<TElement> source) =>
            new __LinkedListICollectionWrapper<TElement>(source);

        /// <summary>
        /// Represents this object as the <see cref="IReadOnlyCollection2{TElement}"/> interface.
        /// </summary>
        public static IReadOnlyCollection2<TElement> AsIReadOnlyCollection2<TElement>(this List<TElement> source) =>
            new __ListICollectionWrapper<TElement>(source);

        /// <summary>
        /// Represents this object as the <see cref="IReadOnlyCollection2{TElement}"/> interface.
        /// </summary>
        public static IReadOnlyCollection2<TElement> AsIReadOnlyCollection2<TElement>(this Queue<TElement> source) =>
            new __QueueICollectionWrapper<TElement>(source);

        /// <summary>
        /// Represents this object as the <see cref="IReadOnlyCollection2{TElement}"/> interface.
        /// </summary>
        public static IReadOnlyCollection2<TElement> AsIReadOnlyCollection2<TElement>(this ReadOnlyCollection<TElement> source) =>
            new __ReadOnlyCollectionICollectionWrapper<TElement>(source);

        /// <summary>
        /// Represents this object as the <see cref="IReadOnlyCollection2{TElement}"/> interface.
        /// </summary>
        public static IReadOnlyCollection2<TElement> AsIReadOnlyCollection2<TElement>(this SortedSet<TElement> source) =>
            new __SortedSetICollectionWrapper<TElement>(source);

        /// <summary>
        /// Represents this object as the <see cref="IReadOnlyCollection2{TElement}"/> interface.
        /// </summary>
        public static IReadOnlyCollection2<TElement> AsIReadOnlyCollection2<TElement>(this Stack<TElement> source) =>
            new __StackICollectionWrapper<TElement>(source);

        /// <summary>
        /// Represents this object as the <see cref="IReadOnlyList2{TElement}"/> interface.
        /// </summary>
        public static IReadOnlyList2<TElement> AsIReadOnlyList2<TElement>(this Collection<TElement> source) =>
            new __CollectionIListWrapper<TElement>(source);

        /// <summary>
        /// Represents this object as the <see cref="IReadOnlyList2{TElement}"/> interface.
        /// </summary>
        public static IReadOnlyList2<TElement> AsIReadOnlyList2<TElement>(this List<TElement> source) =>
            new __ListIListWrapper<TElement>(source);

        /// <summary>
        /// Represents this object as the <see cref="IReadOnlyList2{TElement}"/> interface.
        /// </summary>
        public static IReadOnlyList2<TElement> AsIReadOnlyList2<TElement>(this ReadOnlyCollection<TElement> source) =>
            new __ReadOnlyCollectionIListWrapper<TElement>(source);
    }
}
