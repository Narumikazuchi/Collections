using Narumikazuchi.Collections.Abstract;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using System.Linq;

namespace Narumikazuchi.Collections
{
    /// <summary>
    /// Represents a fast binary lookup data structure.
    /// </summary>
    [DebuggerDisplay("Depth = {GetDepth()}")]
    public sealed class BinaryTree<TValue> : IEnumerable<TValue>, ITree<BinaryNode<TValue>, TValue> where TValue : IComparable<TValue>
    {
        #region Constructor

        /// <summary>
        /// Instantiates a new <see cref="BinaryTree{T}"/> with the <paramref name="rootValue"/> as root node.
        /// </summary>
        /// <param name="rootValue">The value of the root node.</param>
        public BinaryTree([DisallowNull] TValue rootValue) => this._root = new BinaryNode<TValue>(rootValue, null);

#pragma warning disable
        internal BinaryTree([DisallowNull] IEnumerable<TValue> collection)
#pragma warning restore
        {
            if (collection is null)
            {
                throw new ArgumentNullException(nameof(collection));
            }
            if (!collection.Any())
            {
                throw new ArgumentException("Passed Collection was empty!");
            }

            IOrderedEnumerable<TValue> distinct = collection.Distinct().OrderBy(i => i);
            TValue median = distinct.Median();
            this._root = new(median, null);
            foreach (TValue item in distinct)
            {
                if (item.CompareTo(median) == 0)
                {
                    continue;
                }
                this.Insert(item);
            }
        }

        #endregion

        #region Node Management

        /// <summary>
        /// Gets the depth of the <see cref="BinaryTree{T}"/>.
        /// </summary>
        /// <returns>The depth of the deepest node in the tree</returns>
        public UInt32 GetDepth() => this.GetDepth(this._root);

#pragma warning disable
        private UInt32 GetDepth(BinaryNode<TValue> node) => 
            node.LeftChild is null &&
            node.RightChild is null
                ? node.Depth
                : node.RightChild is null &&
                  node.LeftChild is not null
                    ? this.GetDepth(node.LeftChild)
                    : node.LeftChild is null &&
                      node.RightChild is not null
                        ? this.GetDepth(node.RightChild)
                        : Math.Max(this.GetDepth(node.LeftChild), this.GetDepth(node.RightChild));
#pragma warning restore

        /// <summary>
        /// Determines the lowest value in the <see cref="BinaryTree{T}"/>.
        /// </summary>
        [Pure]
        public TValue LowBound()
        {
            BinaryNode<TValue> node = this._root;
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
        public TValue HighBound()
        {
            BinaryNode<TValue> node = this._root;
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
        public IEnumerable<BinaryNode<TValue>> Traverse(BinaryTraversalMethod method) => method == BinaryTraversalMethod.PreOrder ?
                                                                                        this.TraversePreOrder() :
                                                                                        method == BinaryTraversalMethod.InOrder ?
                                                                                            this.TraverseInOrder() :
                                                                                            this.TraversePostOrder();

        [Pure]
        private IEnumerable<BinaryNode<TValue>> TraversePreOrder()
        {
            List<BinaryNode<TValue>> nodes = new();
            this.TraversePreOrder(nodes, this._root);
            return nodes;
        }

        [Pure]
        private void TraversePreOrder(List<BinaryNode<TValue>> nodes, BinaryNode<TValue> current)
        {
            nodes.Add(current);
            if (current.LeftChild is not null)
            {
                this.TraversePreOrder(nodes, current.LeftChild);
            }
            if (current.RightChild is not null)
            {
                this.TraversePreOrder(nodes, current.RightChild);
            }
        }

        [Pure]
        private IEnumerable<BinaryNode<TValue>> TraverseInOrder()
        {
            List<BinaryNode<TValue>> nodes = new();
            this.TraverseInOrder(nodes, this._root);
            return nodes;
        }

        [Pure]
        private void TraverseInOrder(List<BinaryNode<TValue>> nodes, BinaryNode<TValue> current)
        {
            if (current.LeftChild is not null)
            {
                this.TraverseInOrder(nodes, current.LeftChild);
            }
            nodes.Add(current);
            if (current.RightChild is not null)
            {
                this.TraverseInOrder(nodes, current.RightChild);
            }
        }

        [Pure]
        private IEnumerable<BinaryNode<TValue>> TraversePostOrder()
        {
            List<BinaryNode<TValue>> nodes = new();
            this.TraversePostOrder(nodes, this._root);
            return nodes.ToArray();
        }

        [Pure]
        private void TraversePostOrder(List<BinaryNode<TValue>> nodes, BinaryNode<TValue> current)
        {
            if (current.LeftChild is not null)
            {
                this.TraverseInOrder(nodes, current.LeftChild);
            }
            if (current.RightChild is not null)
            {
                this.TraverseInOrder(nodes, current.RightChild);
            }
            nodes.Add(current);
        }

        #endregion

        #region IEnumerable

        /// <inheritdoc/>
        public IEnumerator<TValue> GetEnumerator()
        {
            foreach (BinaryNode<TValue> node in this.TraverseInOrder())
            {
                yield return node.Value;
            }
            yield break;
        }

        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();

        #endregion

        #region ITree

        /// <inheritdoc/>
        public Boolean Insert([DisallowNull] in TValue value)
        {
            if (value is null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            BinaryNode<TValue>? node = this._root;
            Int32 compare = value.CompareTo(node.Value);
            while (node is not null)
            {
                if (compare == 0)
                {
                    return this.ThrowExceptionOnDuplicate 
                        ? throw new ArgumentException("A node with the specified value already exists.") 
                        : false;
                }
                else if (compare < 0)
                {
                    if (node.LeftChild is null)
                    {
                        node.Left = new(value, node);
                        return true;
                    }
                    node = node.LeftChild;
                }
                else if (compare > 0)
                {
                    if (node.RightChild is null)
                    {
                        node.Right = new(value, node);
                        return true;
                    }
                    node = node.RightChild;
                }
                if (node is not null)
                {
                    compare = value.CompareTo(node.Value);
                }
            }
            return false;
        }

        /// <inheritdoc/>
        [Pure]
        public Boolean Exists([DisallowNull] in TValue value) => this.Find(value) is not null;

        /// <inheritdoc/>
        [Pure]
        public BinaryNode<TValue>? Find([DisallowNull] in TValue value)
        {
            if (value is null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            BinaryNode<TValue>? node = this._root;
            Int32 compare = value.CompareTo(node.Value);
            while (node is not null &&
                   compare != 0)
            {
                if (compare < 0)
                {
                    node = node.LeftChild;
                }
                else if (compare > 0)
                {
                    node = node.RightChild;
                }
                if (node is not null)
                {
                    compare = value.CompareTo(node.Value);
                }
            }
            return node;
        }

        /// <inheritdoc/>
        public Boolean Remove([DisallowNull] in TValue value)
        {
            if (value is null)
            {
                throw new ArgumentNullException(nameof(value));
            }
            if (value.CompareTo(this._root.Value) == 0)
            {
                throw new ArgumentException("Cannot remove the root of the BinaryTree.");
            }

            BinaryNode<TValue>? node = this.Find(value);
            if (node is null)
            {
                return false;
            }

            if (node.LeftChild is null)
            {
                if (node.Parent is null)
                {
                    throw new InvalidOperationException("Only roots are supposed to have no parent.");
                }
                if (node.Parent.LeftChild == node)
                {
                    node.Parent.Left = node.RightChild;
                }
                else if (node.Parent.RightChild == node)
                {
                    node.Parent.Right = node.RightChild;
                }
            }
            else if (node.RightChild is null)
            {
                if (node.Parent is null)
                {
                    throw new InvalidOperationException("Only roots are supposed to have no parent.");
                }
                if (node.Parent.LeftChild == node)
                {
                    node.Parent.Left = node.LeftChild;
                }
                else if (node.Parent.RightChild == node)
                {
                    node.Parent.Right = node.LeftChild;
                }
            }
            else
            {
                BinaryNode<TValue> min = node.SetToMinBranchValue();
                if (min.Parent is null)
                {
                    throw new InvalidOperationException("Only roots are supposed to have no parent.");
                }
                min.Parent.Left = null;
            }

            return true;
        }

        /// <inheritdoc/>
        public void Clear()
        {
            this._root.Left = null;
            this._root.Right = null;
        }

        #region Properties

        /// <summary>
        /// Gets the root for the <see cref="BinaryTree{T}"/>.
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Collapsed)]
        [DisallowNull]
        [Pure]
        public BinaryNode<TValue> RootNode => this._root;

        #endregion

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets if an exception should be thrown when trying to add a duplicate.
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Collapsed)]
        public Boolean ThrowExceptionOnDuplicate { get; set; } = true;

        #endregion

        #region Fields

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private BinaryNode<TValue> _root;

        #endregion
    }
}
