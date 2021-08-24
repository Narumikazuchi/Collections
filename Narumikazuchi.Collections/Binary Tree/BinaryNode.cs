using System;
using System.Diagnostics;
using System.Diagnostics.Contracts;

namespace Narumikazuchi.Collections
{
    /// <summary>
    /// Represents a node of a <see cref="BinaryTree{T}"/>.
    /// </summary>
    [DebuggerDisplay("{Value}")]
    public sealed class BinaryNode<TValue> : ITreeNode<BinaryNode<TValue>, TValue> where TValue : IComparable<TValue>
    {
        #region Constructor

        internal BinaryNode(in TValue value, BinaryNode<TValue>? parent)
        {
            if (value is null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            this._value = value;
            this.Parent = parent;
            this.Depth = parent is null ? 0 : parent.Depth + 1;
        }

        #endregion

        #region Node Management

#pragma warning disable
        internal BinaryNode<TValue> SetToMinBranchValue()
        {
            TValue min = this.Value;
            BinaryNode<TValue> node = this.RightChild;
            while (node.LeftChild is not null)
            {
                min = node.LeftChild.Value;
                node = node.LeftChild;
            }
            this._value = min;
            return node;
        }
#pragma warning restore

        #endregion

        #region Register Check

        private static Boolean AreNodesEqual(BinaryNode<TValue> left, BinaryNode<TValue> right) =>
            left.Value.CompareTo(right.Value) == 0;

        #endregion

        #region ITreeNode

        /// <inheritdoc/>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        [Pure]
        public TValue Value => this._value;
        /// <inheritdoc/>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        [Pure]
        public BinaryNode<TValue>? Parent { get; }
        /// <inheritdoc/>
        [DebuggerBrowsable(DebuggerBrowsableState.Collapsed)]
        [Pure]
        public UInt32 Depth { get; }
        /// <inheritdoc/>
        [DebuggerBrowsable(DebuggerBrowsableState.Collapsed)]
        [Pure]
        public Boolean IsLeaf => this._children.Count == 0;
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        [Pure]
        NodeCollection<BinaryNode<TValue>, TValue> ITreeNode<BinaryNode<TValue>, TValue>.Children => this._children;

        #endregion

        #region Operators

#pragma warning disable
        public static implicit operator TValue(BinaryNode<TValue> node) => node.Value;
#pragma warning restore

        #endregion

        #region Properties

        /// <summary>
        /// Gets the left child <see cref="BinaryNode{T}"/>. Returns <see langword="null"/> if the <see cref="BinaryNode{T}"/> has no left sided child node.
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        [Pure]
        public BinaryNode<TValue>? LeftChild => this.Left;
        /// <summary>
        /// Gets the right child <see cref="BinaryNode{T}"/>. Returns <see langword="null"/> if the <see cref="BinaryNode{T}"/> has no right sided child node.
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        [Pure]
        public BinaryNode<TValue>? RightChild => this.Right;

        [DebuggerBrowsable(DebuggerBrowsableState.Collapsed)]
        internal BinaryNode<TValue>? Left
        {
            get => this._left;
            set
            {
                this._left = value;
                this._children.Clear();
                if (this._left is not null)
                {
                    this._children.Add(this._left);
                }
                if (this._right is not null)
                {
                    this._children.Add(this._right);
                }
            }
        }
        [DebuggerBrowsable(DebuggerBrowsableState.Collapsed)]
        internal BinaryNode<TValue>? Right
        {
            get => this._right;
            set
            {
                this._right = value;
                this._children.Clear();
                if (this._left is not null)
                {
                    this._children.Add(this._left);
                }
                if (this._right is not null)
                {
                    this._children.Add(this._right);
                }
            }
        }


        #endregion

        #region Fields

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly NodeCollection<BinaryNode<TValue>, TValue> _children = new(AreNodesEqual);
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private TValue _value;
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private BinaryNode<TValue>? _left = null;
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private BinaryNode<TValue>? _right = null;

        #endregion
    }
}
