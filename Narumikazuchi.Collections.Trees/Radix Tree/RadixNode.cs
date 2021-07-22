using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;

namespace Narumikazuchi.Collections.Trees
{
    /// <summary>
    /// Represents a node in a <see cref="RadixTree{T}"/>.
    /// </summary>
    [DebuggerDisplay("{Label}")]
    public sealed class RadixNode<T> : ITreeNode<RadixNode<T>>
    {
        #region Constructor

        internal RadixNode() : this(String.Empty) { }
        internal RadixNode(String label)
        {
            if (label is null)
            {
                throw new ArgumentNullException(nameof(label));
            }

            this._label = label;
            this._parent = null;
            this._children = new NodeCollection<RadixNode<T>>(AreNodesEqual);
            this._items = new List<T>();
        }
        internal RadixNode(String label, RadixNode<T>? parent)
        {
            if (label is null)
            {
                throw new ArgumentNullException(nameof(label));
            }

            this._label = label;
            this._parent = parent;
            this._children = new NodeCollection<RadixNode<T>>(AreNodesEqual);
            this._items = new List<T>();
        }
        internal RadixNode(String label, T item) : this(label, null, item) { }
        internal RadixNode(String label, RadixNode<T>? parent, T item) : this(label, parent) => this._items.Add(item);
        internal RadixNode(String label, IEnumerable<T> items) : this(label, null, items) { }
        internal RadixNode(String label, RadixNode<T>? parent, IEnumerable<T> items) : this(label, parent)
        {
            if (items is null)
            {
                return;
            }
            foreach (T item in items)
            {
                this._items.Add(item);
            }
        }

        #endregion

        #region Node Management

        internal void AddChild(RadixNode<T> child) => this._children.Add(child);
        internal void AddChildren(IEnumerable<RadixNode<T>> children)
        {
            foreach (RadixNode<T> child in children)
            {
                this._children.Add(child);
            }
        }

        internal void RemoveChild(RadixNode<T> child) => this._children.Remove(child);

        internal void ClearChildren() => this._children.Clear();

        #endregion

        #region Item Management

        /// <summary>
        /// Attaches an item of type <typeparamref name="T"/> to the <see cref="RadixNode{T}"/>.
        /// </summary>
        /// <param name="item">The item to add to the <see cref="RadixNode{T}"/>.</param>
        /// <exception cref="ArgumentException"></exception>
        public void AddItem([DisallowNull] in T item) => this.AddItem(item, true, true);
        /// <summary>
        /// Attaches an item of type <typeparamref name="T"/> to the <see cref="RadixNode{T}"/>.
        /// </summary>
        /// <param name="item">The item to add to the <see cref="RadixNode{T}"/>.</param>
        /// <param name="addToParent">Defines if the added item should also be added to the parents <see cref="RadixNode{T}.Items"/>.</param>
        /// <exception cref="ArgumentException"></exception>
        public void AddItem([DisallowNull] in T item, in Boolean addToParent) => this.AddItem(item, true, addToParent);
        /// <summary>
        /// Attaches an item of type <typeparamref name="T"/> to the <see cref="RadixNode{T}"/>.
        /// </summary>
        /// <param name="item">The item to add to the <see cref="RadixNode{T}"/>.</param>
        /// <param name="throwOnDuplicate">Determines if the method should throw an exception if the item already exists.</param>
        /// <param name="addToParent">Defines if the added item should also be added to the parents <see cref="RadixNode{T}.Items"/>.</param>
        /// <exception cref="ArgumentException"></exception>
        public void AddItem([DisallowNull] in T item, in Boolean throwOnDuplicate, in Boolean addToParent)
        {
            if (item is null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            this._items.Add(item);
            if (addToParent)
            {
                this.Parent?.AddItem(item, throwOnDuplicate);
            }
        }

        /// <summary>
        /// Attaches an item of type <typeparamref name="T"/> to the <see cref="RadixNode{T}"/>.
        /// </summary>
        /// <param name="index">The index at which the item will be inserted into the <see cref="RadixNode{T}.Items"/>.</param>
        /// <param name="item">The item to add to the <see cref="RadixNode{T}"/>.</param>
        /// <exception cref="ArgumentException"></exception>
        public void InsertItem([DisallowNull] in Int32 index, [DisallowNull] in T item) => this.InsertItem(index, item, true, true);
        /// <summary>
        /// Attaches an item of type <typeparamref name="T"/> to the <see cref="RadixNode{T}"/>.
        /// </summary>
        /// <param name="index">The index at which the item will be inserted into the <see cref="RadixNode{T}.Items"/>.</param>
        /// <param name="item">The item to add to the <see cref="RadixNode{T}"/>.</param>
        /// <param name="addToParent">Defines if the added item should also be added to the parents <see cref="RadixNode{T}.Items"/>.</param>
        /// <exception cref="ArgumentException"></exception>
        public void InsertItem(in Int32 index, [DisallowNull] in T item, in Boolean addToParent) => this.InsertItem(index, item, true, addToParent);
        /// <summary>
        /// Attaches an item of type <typeparamref name="T"/> to the <see cref="RadixNode{T}"/>.
        /// </summary>
        /// <param name="index">The index at which the item will be inserted into the <see cref="RadixNode{T}.Items"/>.</param>
        /// <param name="item">The item to add to the <see cref="RadixNode{T}"/>.</param>
        /// <param name="throwOnDuplicate">Determines if the method should throw an exception if the item already exists.</param>
        /// <param name="addToParent">Defines if the added item should also be added to the parents <see cref="RadixNode{T}.Items"/>.</param>
        /// <exception cref="ArgumentException"></exception>
        public void InsertItem(in Int32 index, [DisallowNull] in T item, in Boolean throwOnDuplicate, in Boolean addToParent)
        {
            if (item is null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            this._items.Insert(index, item);
            if (addToParent)
            {
                this.Parent?.AddItem(item, throwOnDuplicate);
            }
        }

        /// <summary>
        /// Removes the specified item from the <see cref="RadixNode{T}"/>, if it's attached to it.
        /// </summary>
        /// <param name="item">The item to remove.</param>
        public void RemoveItem([DisallowNull] in T item) => this.RemoveItem(item, true);
        /// <summary>
        /// Removes the specified item from the <see cref="RadixNode{T}"/>, if it's attached to it. If <paramref name="removeFromParent"/> is also set, the item will also be removed from the parent <see cref="RadixNode{T}"/>.
        /// </summary>
        /// <param name="item">The item to remove.</param>
        /// <param name="removeFromParent">Defines if the item should also be removed from the parent <see cref="RadixNode{T}"/>.</param>
        public void RemoveItem([DisallowNull] in T item, in Boolean removeFromParent)
        {
            if (item is null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            this._items.Remove(item);
            foreach (RadixNode<T> child in this.Children)
            {
                child.RemoveItem(item, false);
            }
            if (removeFromParent)
            {
                this.Parent?.RemoveItem(item);
            }
        }

        /// <summary>
        /// Determines whether the specified item is attached to the <see cref="RadixNode{T}"/>. 
        /// </summary>
        /// <param name="item">The item to determine the attachment of.</param>
#pragma warning disable
        [Pure]
        public Boolean ContainsItem([DisallowNull] in T item) => this._items.Contains(item);
#pragma warning restore

        /// <summary>
        /// Empties the list of attached items to the <see cref="RadixNode{T}"/>.
        /// </summary>
        public void ClearItems() => this._items.Clear();

        #endregion

        #region Register Check

        private static Boolean AreNodesEqual(RadixNode<T> left, RadixNode<T> right) =>
            left.Label == right.Label;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the label of the <see cref="RadixNode{T}"/>.
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        [DisallowNull]
        public String Label
        {
            get => this._label;
            set
            {
                if (value is null)
                {
                    throw new ArgumentNullException(nameof(value));
                }
                this._label = value;
            }
        }
        /// <summary>
        /// Gets the parent node of the <see cref="RadixNode{T}"/>. Returns <see langword="null"/> if the <see cref="RadixNode{T}"/> is a root node.
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        [Pure]
        public RadixNode<T>? Parent => this._parent;
        /// <summary>
        /// Gets a collection of all child-nodes of the <see cref="RadixNode{T}"/>.
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
        [DisallowNull]
        [Pure]
        public NodeCollection<RadixNode<T>> Children => this._children;
        /// <summary>
        /// Gets a collection of items which are either attached to this <see cref="RadixNode{T}"/> or are attached to any of its child-nodes.
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Collapsed)]
        [DisallowNull]
        [Pure]
        public IReadOnlyList<T> Items => this._items;
        /// <summary>
        /// Gets if this <see cref="RadixNode{T}"/> has no more child-nodes.
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Collapsed)]
        [Pure]
        public Boolean IsLeaf => this._children.Count == 0;

        #endregion

        #region Fields

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly RadixNode<T>? _parent;
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly NodeCollection<RadixNode<T>> _children;
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly List<T> _items;
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private String _label;

        #endregion
    }
}
