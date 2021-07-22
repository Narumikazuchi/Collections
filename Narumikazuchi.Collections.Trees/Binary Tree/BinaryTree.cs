using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using System.Linq;

namespace Narumikazuchi.Collections.Trees
{
    /// <summary>
    /// Represents a fast binary lookup data structure.
    /// </summary>
    [DebuggerDisplay("Depth = {Depth}")]
    public sealed class BinaryTree<T> : ITree<BinaryNode<T>> where T : IComparable<T>
    {
        #region Constructor

        /// <summary>
        /// Instantiates a new <see cref="BinaryTree{T}"/> with the <paramref name="rootValue"/> as root node.
        /// </summary>
        /// <param name="rootValue">The value of the root node.</param>
        public BinaryTree([DisallowNull] T rootValue) => this._root = new BinaryNode<T>(rootValue);

        internal BinaryTree([DisallowNull] IEnumerable<T> collection)
        {
            if (collection is null)
            {
                throw new ArgumentNullException(nameof(collection));
            }
            T? first = collection.FirstOrDefault();
            if (first is null)
            {
                throw new ArgumentException("Passed Collection was empty!");
            }
            this._root = new BinaryNode<T>(first);
            foreach (T item in collection)
            {
                if (item.Equals(first))
                {
                    continue;
                }
                this._root.Add(item, false);
            }
        }

        #endregion

        #region Node Management

        /// <summary>
        /// Adds the specified item <typeparamref name="T"/> to the <see cref="BinaryTree{T}"/>.
        /// </summary>
        /// <param name="value">The value to add to the <see cref="BinaryTree{T}"/>.</param>
        /// <exception cref="ArgumentException"></exception>
        public void Add([DisallowNull] in T value) => this._root.Add(value, this.ThrowExceptionOnDuplicate);

        /// <summary>
        /// Finds the <see cref="BinaryNode{T}"/> for the specified value <typeparamref name="T"/> in the <see cref="BinaryTree{T}"/>. Returns <see langword="null"/> if the <see cref="BinaryTree{T}"/> does not contain the value.
        /// </summary>
        /// <param name="value">The value to find in the <see cref="BinaryTree{T}"/>.</param>
        [Pure]
        public BinaryNode<T>? Find([DisallowNull] in T value) => this._root.Find(value);

        /// <summary>
        /// Removes the <see cref="BinaryNode{T}"/> from the <see cref="BinaryTree{T}"/>, which represents the specified value <typeparamref name="T"/>.
        /// </summary>
        /// <param name="value">The value to remove from the <see cref="BinaryTree{T}"/>.</param>
        /// <exception cref="ArgumentException"></exception>
        public void Remove([DisallowNull] in T value)
        {
            if (value.CompareTo(this._root) == 0)
            {
                throw new ArgumentException("Cannot remove the root of the BinaryTree.");
            }
            else
            {
                this._root.Remove(value);
            }
        }

        /// <summary>
        /// Determines the lowest value in the <see cref="BinaryTree{T}"/>.
        /// </summary>
        [Pure]
        public T LowBound()
        {
            BinaryNode<T> node = this._root;
            while (node.LeftChild is not null)
            {
                node = node.LeftChild;
            }
            return node.Value;
        }

        /// <summary>
        /// Determines the highest value in the <see cref="BinaryTree{T}"/>.
        /// </summary>
        [Pure]
        public T HighBound()
        {
            BinaryNode<T> node = this._root;
            while (node.RightChild is not null)
            {
                node = node.RightChild;
            }
            return node.Value;
        }

        /// <summary>
        /// Returns an <see cref="IEnumerable{T}"/> containing the traversed <see cref="BinaryTree{T}"/> in the traversed order.
        /// </summary>
        /// <param name="method">The method to use when traversing.</param>
        [Pure]
        public IEnumerable<BinaryNode<T>> Traverse(BinaryTraversalMethod method) => method == BinaryTraversalMethod.PreOrder ?
                                                                                        this.TraversePreOrder() :
                                                                                        method == BinaryTraversalMethod.InOrder ?
                                                                                            this.TraverseInOrder() :
                                                                                            this.TraversePostOrder();

        [Pure]
        private IEnumerable<BinaryNode<T>> TraversePreOrder()
        {
            List<BinaryNode<T>> nodes = new()
            {
                this._root
            };
            if (this._root.LeftChild is not null)
            {
                this.TraversePreOrderSingle(nodes, this._root.LeftChild);
            }
            if (this._root.RightChild is not null)
            {
                this.TraversePreOrderSingle(nodes, this._root.RightChild);
            }
            return nodes.ToArray();
        }

        [Pure]
        private void TraversePreOrderSingle(List<BinaryNode<T>> nodes, BinaryNode<T>? current)
        {
            if (current is null)
            {
                return;
            }
            nodes.Add(current);
            this.TraversePreOrderSingle(nodes, current.LeftChild);
            this.TraversePreOrderSingle(nodes, current.RightChild);
        }

        [Pure]
        private IEnumerable<BinaryNode<T>> TraverseInOrder()
        {
            List<BinaryNode<T>> nodes = new();
            this.TraverseInOrderSingle(nodes, this._root);
            return nodes.ToArray();
        }

        [Pure]
        private void TraverseInOrderSingle(List<BinaryNode<T>> nodes, BinaryNode<T>? current)
        {
            if (current is null)
            {
                return;
            }
            this.TraverseInOrderSingle(nodes, current.LeftChild);
            nodes.Add(current);
            this.TraverseInOrderSingle(nodes, current.RightChild);
        }

        [Pure]
        private IEnumerable<BinaryNode<T>> TraversePostOrder()
        {
            List<BinaryNode<T>> nodes = new();
            this.TraversePostOrderSingle(nodes, this._root);
            return nodes.ToArray();
        }

        [Pure]
        private void TraversePostOrderSingle(List<BinaryNode<T>> nodes, BinaryNode<T>? current)
        {
            if (current is null)
            {
                return;
            }
            this.TraversePostOrderSingle(nodes, current.LeftChild);
            this.TraversePostOrderSingle(nodes, current.RightChild);
            nodes.Add(current);
        }

        #endregion

        #region ITree

        /// <summary>
        /// Gets the root for the <see cref="BinaryTree{T}"/>.
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Collapsed)]
        [DisallowNull]
        [Pure]
        public BinaryNode<T> RootNode => this._root;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets if an exception should be thrown when trying to add a duplicate.
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Collapsed)]
        public Boolean ThrowExceptionOnDuplicate { get; set; } = true;
        /// <summary>
        /// Gets the depth of the <see cref="BinaryTree{T}"/>.
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        [Pure]
        public Int32 Depth => this._root.GetDepth();

        #endregion

        #region Fields

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private BinaryNode<T> _root;

        #endregion
    }
}
