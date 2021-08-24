using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace Narumikazuchi.Collections
{
    /// <summary>
    /// Extends the <see cref="IEnumerable{T}"/> interface.
    /// </summary>
    public static class Enumerable
    {
        #region Conversion to introduced Collections

        /// <summary>
        /// Creates a <see cref="ObservableList{T}"/> from an <see cref="IEnumerable{T}"/>.
        /// </summary>
        public static ObservableList<T> ToObservableList<T>(this IEnumerable<T> source) where T : INotifyPropertyChanged => 
            source is ObservableList<T> list ? list : new ObservableList<T>(source);

        /// <summary>
        /// Creates a <see cref="Register{T}"/> from an <see cref="IEnumerable{T}"/>.
        /// </summary>
        public static Register<T> ToRegister<T>(this IEnumerable<T> source) =>
            source is Register<T> register ? register : new Register<T>(source);
        /// <summary>
        /// Creates a <see cref="Register{T}"/> from an <see cref="IEnumerable{T}"/>.
        /// </summary>
        public static Register<T> ToRegister<T>(this IEnumerable<T> source, IEqualityComparer<T>? comparer) =>
            source is Register<T> register && ReferenceEquals(register.Comparer, comparer) ? register : new Register<T>(comparer, source);
        /// <summary>
        /// Creates a <see cref="Register{T}"/> from an <see cref="IEnumerable{T}"/>.
        /// </summary>
        public static Register<T> ToRegister<T>(this IEnumerable<T> source, EqualityComparison<T> comparison) =>
            source is Register<T> register && register.Comparer is __FuncEqualityComparer<T> func && func.Comparison == comparison ? register : new Register<T>(comparison, source);

        /// <summary>
        /// Creates a <see cref="ObservableRegister{T}"/> from an <see cref="IEnumerable{T}"/>.
        /// </summary>
        public static ObservableRegister<T> ToObservableRegister<T>(this IEnumerable<T> source) where T : INotifyPropertyChanged =>
            source is ObservableRegister<T> register ? register : new ObservableRegister<T>(source);
        /// <summary>
        /// Creates a <see cref="ObservableRegister{T}"/> from an <see cref="IEnumerable{T}"/>.
        /// </summary>
        public static ObservableRegister<T> ToObservableRegister<T>(this IEnumerable<T> source, IEqualityComparer<T>? comparer) where T : INotifyPropertyChanged =>
            source is ObservableRegister<T> register && ReferenceEquals(register.Comparer, comparer) ? register : new ObservableRegister<T>(comparer, source);
        /// <summary>
        /// Creates a <see cref="ObservableRegister{T}"/> from an <see cref="IEnumerable{T}"/>.
        /// </summary>
        public static ObservableRegister<T> ToObservableRegister<T>(this IEnumerable<T> source, EqualityComparison<T> comparison) where T : INotifyPropertyChanged =>
            source is ObservableRegister<T> register && register.Comparer is __FuncEqualityComparer<T> func && func.Comparison == comparison ? register : new ObservableRegister<T>(comparison, source);

        /// <summary>
        /// Creates a <see cref="BinaryTree{T}"/> from an <see cref="IEnumerable{T}"/>.
        /// </summary>
        public static BinaryTree<T> ToBinaryTree<T>(this IEnumerable<T> source) where T : IComparable<T> =>
            source is BinaryTree<T> tree ? tree : new BinaryTree<T>(source);

        /// <summary>
        /// Creates a <see cref="BinaryTree{T}"/> from an <see cref="IEnumerable{T}"/>.
        /// </summary>
        public static Trie<T> ToTrie<T>(this IEnumerable<String> source) where T : class =>
            source is Trie<T> trie ? trie : new Trie<T>(source);

        #endregion

        #region Enumerables

        /// <summary>
        /// Returns the item <typeparamref name="T"/> at the center of the collection.
        /// </summary>
        /// <returns>The item <typeparamref name="T"/> at the center of the collection</returns>
        public static T? Median<T>(this IEnumerable<T> source)
        {
            if (!source.Any())
            {
                return default;
            }

            if (source is IReadOnlyList<T> list)
            {
                return list[list.Count / 2];
            }

            IEnumerator<T> enumerator;
            Int32 index = 0;
            if (source is IReadOnlyCollection<T> collection)
            {
                enumerator = collection.GetEnumerator();
                while (index < collection.Count / 2)
                {
                    enumerator.MoveNext();
                    ++index;
                }
                return enumerator.Current;
            }

            IEnumerator<T> counter = source.GetEnumerator();
            enumerator = source.GetEnumerator();
            while (enumerator.MoveNext())
            {
                if (index % 2 == 0)
                {
                    counter.MoveNext();
                }
                ++index;
            }
            return counter.Current;
        }

        /// <summary>
        /// Adds the specified collection to this collection.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="other">The collection to add.</param>
        public static void AddRange<T>(this ICollection<T> source, IEnumerable<T> other)
        {
            foreach (T item in other)
            {
                source.Add(item);
            }
        }

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
