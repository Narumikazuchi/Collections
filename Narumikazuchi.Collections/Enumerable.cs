using System;
using System.Collections.Generic;
using System.ComponentModel;

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

        #endregion
    }
}
