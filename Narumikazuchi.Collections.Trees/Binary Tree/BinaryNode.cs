using System;
using System.Diagnostics;
using System.Diagnostics.Contracts;

namespace Narumikazuchi.Collections.Trees
{
    /// <summary>
    /// Represents a node of a <see cref="BinaryTree{T}"/>.
    /// </summary>
    [DebuggerDisplay("{Value}")]
    public sealed class BinaryNode<T> : ITreeNode<BinaryNode<T>> where T : IComparable<T>
    {
        #region Constructor

        internal BinaryNode(in T value) : this(value, null) { }
        internal BinaryNode(in T value, BinaryNode<T>? parent)
        {
            if (value is null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            this._value = value;
            this.Parent = parent;
        }

        #endregion

        #region Node Management

        internal void Add(in T value, in Boolean throwOnDuplicate)
        {
            if (value is null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            Int32 compare = value.CompareTo(this.Value);
            if (compare < 0)
            {
                if (this.Left is null)
                {
                    this.Left = new BinaryNode<T>(value, this);
                }
                else
                {
                    this.Left.Add(value, throwOnDuplicate);
                }
            }
            else if (compare > 0)
            {
                if (this.Right is null)
                {
                    this.Right = new BinaryNode<T>(value, this);
                }
                else
                {
                    this.Right.Add(value, throwOnDuplicate);
                }
            }
            else
            {
                if (throwOnDuplicate)
                {
                    throw new ArgumentException("A node with the specified value already exists.");
                }
            }
        }
        internal BinaryNode<T>? Remove(in T value)
        {
            if (value is null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            Int32 compare = value.CompareTo(this.Value);
            if (compare < 0)
            {
                this.Left = this.LeftChild?.Remove(value);
            }
            else if (compare > 0)
            {
                this.Right = this.RightChild?.Remove(value);
            }
            else
            {
                if (this.LeftChild is null)
                {
                    return this.RightChild;
                }
                else if (this.RightChild is null)
                {
                    return this.LeftChild;
                }
                this.SetToMinBranchValue();
                this.Right = this.RightChild.Remove(this.Value);
            }
            return this;
        }

        private void SetToMinBranchValue()
        {
            T min = this.RightChild is null ? this.Value : this.RightChild.Value;
            BinaryNode<T>? node = this.RightChild;
            while (node is not null &&
                   node.LeftChild is not null)
            {
                min = node.LeftChild.Value;
                node = node.LeftChild;
            }
            this._value = min;
        }

        [Pure]
        internal BinaryNode<T>? Find(in T value)
        {
            if (value is null)
            {
                throw new ArgumentNullException(nameof(value));
            }
            Int32 compare = value.CompareTo(this.Value);
            return compare < 0 ? this.LeftChild?.Find(value) : compare > 0 ? this.RightChild?.Find(value) : this;
        }

#pragma warning disable
        [Pure]
        internal Int32 GetDepth() => this.LeftChild is null &&
                                     this.RightChild is null 
                                        ? 1 
                                        : this.RightChild is null 
                                        ? this.LeftChild.GetDepth() + 1 
                                        : this.LeftChild is null 
                                        ? this.RightChild.GetDepth() + 1 
                                        : Math.Max(this.LeftChild.GetDepth(), this.RightChild.GetDepth()) + 1;
#pragma warning restore

        #endregion

        #region Register Check

        private static Boolean AreNodesEqual(BinaryNode<T> left, BinaryNode<T> right) =>
            left.Value.CompareTo(right.Value) == 0;

        #endregion

        #region Operators

#pragma warning disable
        public static implicit operator T(BinaryNode<T> node) => node.Value;
#pragma warning restore

        #endregion

        #region Properties

        /// <summary>
        /// Gets the value of the <see cref="BinaryNode{T}"/>.
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        [Pure]
        public T Value => this._value;
        /// <summary>
        /// Gets the left child <see cref="BinaryNode{T}"/>. Returns <see langword="null"/> if the <see cref="BinaryNode{T}"/> has no left sided child node.
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        [Pure]
        public BinaryNode<T>? LeftChild => this.Left;
        /// <summary>
        /// Gets the right child <see cref="BinaryNode{T}"/>. Returns <see langword="null"/> if the <see cref="BinaryNode{T}"/> has no right sided child node.
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        [Pure]
        public BinaryNode<T>? RightChild => this.Right;
        /// <summary>
        /// Gets the parent <see cref="BinaryNode{T}"/> of the <see cref="BinaryNode{T}"/>. Returns <see langword="null"/> if the <see cref="BinaryNode{T}"/> is a root.
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        [Pure]
        public BinaryNode<T>? Parent { get; }

        [DebuggerBrowsable(DebuggerBrowsableState.Collapsed)]
        private BinaryNode<T>? Left
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
        private BinaryNode<T>? Right
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

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        [Pure]
        NodeCollection<BinaryNode<T>> ITreeNode<BinaryNode<T>>.Children => this._children;

        #endregion

        #region Fields

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private T _value;
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private BinaryNode<T>? _left = null;
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private BinaryNode<T>? _right = null;
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly NodeCollection<BinaryNode<T>> _children = new(AreNodesEqual);

        #endregion
    }
}
