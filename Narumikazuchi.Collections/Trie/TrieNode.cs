using Narumikazuchi.Collections.Abstract;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using System.Linq;

namespace Narumikazuchi.Collections
{
    /// <summary>
    /// Represents a node in a <see cref="Trie{T}"/> data structure. Items of type <typeparamref name="T"/> can be attached to this <see cref="TrieNode{T}"/>.
    /// </summary>
    [DebuggerDisplay("{Value}")]
    public sealed class TrieNode<T> : ITreeNode<TrieNode<T>> where T : class
    {
        #region Constructor

        internal TrieNode(Trie<T> trie, in Char value, in Int32 depth) : this(trie, value, depth, null) { }
        internal TrieNode(Trie<T> trie, in Char value, in Int32 depth, TrieNode<T>? parent)
        {
            this._trie = trie;
            this._value = Char.ToLower(value);
            this._depth = depth;
            this._parent = parent;
            this._children = new(AreNodesEqual);
            this._items = new Register<T>(AreItemsEqual);
            this._children.EnableAutoSort(SortDirection.Ascending, (a, b) => a.Value.CompareTo(b.Value));
        }
        internal TrieNode(Trie<T> trie, in Char value, in Int32 depth, TrieNode<T>? parent, T item) : this(trie, value, depth, parent) => this._items.Add(item);
        internal TrieNode(Trie<T> trie, in Char value, in Int32 depth, TrieNode<T>? parent, IEnumerable<T> items) : this(trie, value, depth, parent)
        {
            foreach (T item in items)
            {
                this._items.Add(item);
            }
        }

        #endregion

        #region Node Management

        internal void AddChild(TrieNode<T> node)
        {
            this._children.Add(node);
            this._children.Sort(SortDirection.Ascending, (a, b) => a.Value.CompareTo(b.Value));
        }

#pragma warning disable
        internal void DeleteChildNode(in Char value) => this.DeleteChildNode(this.FindChildNode(value));
#pragma warning restore
        internal void DeleteChildNode(TrieNode<T> node)
        {
            if (node is null ||
                !this._children.Contains(node))
            {
                return;
            }
            this._children.Remove(node);
        }

        internal void Clear()
        {
            this._children.Clear();
            this._items.Clear();
        }

        /// <summary>
        /// Finds the child-node with the specified value. Returns <see langword="null"/> if no <see cref=" TrieNode{T}"/> with the specified value exists.
        /// </summary>
        /// <param name="value">The value to lookup in the child-nodes of the <see cref="TrieNode{T}"/>.</param>
        [Pure]
        public TrieNode<T>? FindChildNode(Char value)
        {
            value = Char.ToLower(value);
            foreach (TrieNode<T> child in this.Children)
            {
                if (child.Value == value)
                {
                    return child;
                }
            }
            return null;
        }

        #endregion

        #region Item Management

        /// <summary>
        /// Attaches an item of type <typeparamref name="T"/> to the <see cref="TrieNode{T}"/>.
        /// </summary>
        /// <param name="item">The item to add to the <see cref="TrieNode{T}"/>.</param>
        /// <exception cref="ArgumentException"></exception>
        public void AddItem(T item) => this._items.Add(item);

        /// <summary>
        /// Attaches an item of type <typeparamref name="T"/> to the <see cref="TrieNode{T}"/>.
        /// </summary>
        /// <param name="index">The index at which the item will be inserted into the <see cref="TrieNode{T}.Items"/>.</param>
        /// <param name="item">The item to add to the <see cref="TrieNode{T}"/>.</param>
        /// <exception cref="ArgumentException"></exception>
        public void InsertItem(in Int32 index, T item) => this._items.Insert(index, item);

        /// <summary>
        /// Removes the specified item from the <see cref="TrieNode{T}"/>, if it's attached to it.
        /// </summary>
        /// <param name="item">The item to remove.</param>
        public void RemoveItem(T item)
        {
            this._items.Remove(item);
            foreach (TrieNode<T> child in this.Children)
            {
                child.RemoveItem(item);
            }
        }

        /// <summary>
        /// Determines whether the specified item is attached to the <see cref="TrieNode{T}"/>. 
        /// </summary>
        /// <param name="item">The item to determine the attachment of.</param>
        [Pure]
        public Boolean ContainsItem(T item) => this._items.Contains(item);

        /// <summary>
        /// Empties the list of attached items to the <see cref="TrieNode{T}"/>.
        /// </summary>
        public void ClearItems() => this._items.Clear();

        #endregion

        #region Register Check

        private static Boolean AreItemsEqual(T left, T right) => 
            left is IEquatable<T> eq 
                ? eq.Equals(right) 
                : ReferenceEquals(left, right);

        private static Boolean AreNodesEqual(TrieNode<T> left, TrieNode<T> right) =>
            left.Value == right.Value;

        #endregion

        #region ITreeNode

        /// <summary>
        /// Gets the parent <see cref="TrieNode{T}"/> for this <see cref="TrieNode{T}"/>. Returns <see langword="null"/> for a root node.
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        [Pure]
        public TrieNode<T>? Parent => this._parent;
        /// <summary>
        /// Gets a collection containing the child-nodes for this <see cref="TrieNode{T}"/>.
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
        [DisallowNull]
        [Pure]
        public NodeCollection<TrieNode<T>> Children => this._children;

        #endregion

        #region Properties

        /// <summary>
        /// Gets the <see cref="Char"/> value of this <see cref="TrieNode{T}"/>.
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        [Pure]
        public Char Value => this._value;
        /// <summary>
        /// Gets the depth in the <see cref="Trie{T}"/>.
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Collapsed)]
        [Pure]
        public Int32 Depth => this._depth;
        /// <summary>
        /// Gets a collection of items which are either attached to this <see cref="TrieNode{T}"/> or are attached to any of its child-nodes (only when <see cref="Trie{T}.ParentsKnowChildItems"/> for the parent <see cref="Trie{T}"/> is set to <see langword="true"/>).
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Collapsed)]
        [DisallowNull]
        public IReadOnlyList2<T> Items => this._trie.ParentsKnowChildItems
                    ? this.ChildItems
                    : this._items;
        /// <summary>
        /// Gets if this <see cref="TrieNode{T}"/> has no more child-nodes.
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Collapsed)]
        [Pure]
        public Boolean IsLeaf => this._children.Count == 0;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        internal IReadOnlyList2<T> ChildItems => this._children.SelectMany(child => child.Items).Union(this._items).ToRegister(AreItemsEqual);

        #endregion

        #region Fields

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        internal readonly Register<T> _items;
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly Char _value;
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly Int32 _depth;
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly TrieNode<T>? _parent;
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly NodeCollection<TrieNode<T>> _children;
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly Trie<T> _trie;

        #endregion
    }
}
