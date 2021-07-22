using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Narumikazuchi.Collections.Abstract
{
    /// <summary>
    /// Extends the <see cref="IEnumerable{T}"/> interface.
    /// </summary>
    public static class Enumerable
    {
        #region Conversion to introduced Interfaces

        /// <summary>
        /// Represents this object as the <see cref="IReadOnlyCollection2{T}"/> interface.
        /// </summary>
        public static IReadOnlyCollection2<T> AsIReadOnlyCollection2<T>(this Collection<T> source) =>
            new IReadOnlyCollection2<T>.__CollectionICollectionWrapper<T>(source);

        /// <summary>
        /// Represents this object as the <see cref="IReadOnlyCollection2{T}"/> interface.
        /// </summary>
        public static IReadOnlyCollection2<T> AsIReadOnlyCollection2<T>(this LinkedList<T> source) =>
            new IReadOnlyCollection2<T>.__LinkedListICollectionWrapper<T>(source);

        /// <summary>
        /// Represents this object as the <see cref="IReadOnlyCollection2{T}"/> interface.
        /// </summary>
        public static IReadOnlyCollection2<T> AsIReadOnlyCollection2<T>(this List<T> source) =>
            new IReadOnlyCollection2<T>.__ListICollectionWrapper<T>(source);

        /// <summary>
        /// Represents this object as the <see cref="IReadOnlyCollection2{T}"/> interface.
        /// </summary>
        public static IReadOnlyCollection2<T> AsIReadOnlyCollection2<T>(this Queue<T> source) =>
            new IReadOnlyCollection2<T>.__QueueICollectionWrapper<T>(source);

        /// <summary>
        /// Represents this object as the <see cref="IReadOnlyCollection2{T}"/> interface.
        /// </summary>
        public static IReadOnlyCollection2<T> AsIReadOnlyCollection2<T>(this ReadOnlyCollection<T> source) =>
            new IReadOnlyCollection2<T>.__ReadOnlyCollectionICollectionWrapper<T>(source);

        /// <summary>
        /// Represents this object as the <see cref="IReadOnlyCollection2{T}"/> interface.
        /// </summary>
        public static IReadOnlyCollection2<T> AsIReadOnlyCollection2<T>(this SortedSet<T> source) =>
            new IReadOnlyCollection2<T>.__SortedSetICollectionWrapper<T>(source);

        /// <summary>
        /// Represents this object as the <see cref="IReadOnlyCollection2{T}"/> interface.
        /// </summary>
        public static IReadOnlyCollection2<T> AsIReadOnlyCollection2<T>(this Stack<T> source) =>
            new IReadOnlyCollection2<T>.__StackICollectionWrapper<T>(source);

        /// <summary>
        /// Represents this object as the <see cref="IReadOnlyList2{T}"/> interface.
        /// </summary>
        public static IReadOnlyList2<T> AsIReadOnlyList2<T>(this Collection<T> source) =>
            new IReadOnlyList2<T>.__CollectionIListWrapper<T>(source);

        /// <summary>
        /// Represents this object as the <see cref="IReadOnlyList2{T}"/> interface.
        /// </summary>
        public static IReadOnlyList2<T> AsIReadOnlyList2<T>(this List<T> source) =>
            new IReadOnlyList2<T>.__ListIListWrapper<T>(source);

        /// <summary>
        /// Represents this object as the <see cref="IReadOnlyList2{T}"/> interface.
        /// </summary>
        public static IReadOnlyList2<T> AsIReadOnlyList2<T>(this ReadOnlyCollection<T> source) =>
            new IReadOnlyList2<T>.__ReadOnlyCollectionIListWrapper<T>(source);

        #endregion

        #region IList Item Shuffeling

        /// <summary>
        /// Moves the item at the given index one position in the specified direction in the <see cref="IList{T}"/>.
        /// </summary>
        public static void MoveItem<T>(this IList<T> list, Int32 index, ItemMoveDirection direction)
        {
            T tmp;
            if (direction == ItemMoveDirection.ToLowerIndex)
            {
                if (index > 0)
                {
                    tmp = list[index];
                    list[index] = list[index - 1];
                    list[index - 1] = tmp;
                }
            }
            else
            {
                if (index > -1 && index < list.Count - 1)
                {
                    tmp = list[index];
                    list[index] = list[index + 1];
                    list[index + 1] = tmp;
                }
            }
        }

        /// <summary>
        /// Moves the item at the given index the given amount of positions in the specified direction in the <see cref="IList{T}"/>.
        /// </summary>
        public static void MoveItem<T>(this IList<T> list, Int32 index, ItemMoveDirection direction, Int32 positions)
        {
            T item = list[index];
            while (positions-- > 0)
            {
                list.MoveItem(item, direction);
            }
        }

        /// <summary>
        /// Moves the item one position in the specified direction in the <see cref="IList{T}"/>.
        /// </summary>
        public static void MoveItem<T>(this IList<T> list, T item, ItemMoveDirection direction)
        {
            Int32 index = list.IndexOf(item);
            if (index == -1)
            {
                return;
            }
            if (direction == ItemMoveDirection.ToLowerIndex)
            {
                if (index > 0)
                {
                    list[index] = list[index - 1];
                    list[index - 1] = item;
                }
            }
            else
            {
                if (index < list.Count - 1)
                {
                    list[index] = list[index + 1];
                    list[index + 1] = item;
                }
            }
        }

        /// <summary>
        /// Moves the item at the given index the given amount of positions in the specified direction in the <see cref="IList{T}"/>.
        /// </summary>
        public static void MoveItem<T>(this IList<T> list, T item, ItemMoveDirection direction, Int32 positions)
        {
            while (positions-- > 0)
            {
                list.MoveItem(item, direction);
            }
        }

        #endregion
    }
}
