using Narumikazuchi.Collections.Abstract;
using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;

namespace Narumikazuchi.Collections
{
    /// <summary>
    /// Represents a node in a <see cref="Trie{T}"/> data structure. Items of type <typeparamref name="TContent"/> can be attached to this <see cref="TrieNode{T}"/>.
    /// </summary>
    [DebuggerDisplay("{Value}")]
    public sealed partial class TrieNode<TContent>
    {
        /// <summary>
        /// Finds the child-node with the specified value. Returns <see langword="null"/> if no <see cref=" TrieNode{T}"/> with the specified value exists.
        /// </summary>
        /// <param name="value">The value to lookup in the child-nodes of the <see cref="TrieNode{T}"/>.</param>
        [Pure]
        [return: MaybeNull]
        public TrieNode<TContent>? FindChildNode(Char value) => 
            this.Children.FirstOrDefault(n => n.Value == Char.ToLower(value));

        /// <inheritdoc/>
        [return: MaybeNull]
        public override String? ToString()
        {
            StringBuilder builder = new();
            TrieNode<TContent>? current = this;
            do
            {
                builder.Insert(0, 
                               current.Value);
                current = current.Parent;
            } while (current is not null &&
                     current.Parent is not null);
            return builder.ToString();
        }
    }

    // Non-Public
    partial class TrieNode<TContent>
    {
        internal TrieNode(Trie<TContent> trie, 
                          in Char value, 
                          TrieNode<TContent>? parent)
        {
            this._trie = trie;
            this._children = new(AreNodesEqual);
            this._items = new Register<TContent>(AreItemsEqual);
            this.Value = Char.ToLower(value);
            this.Parent = parent;
            this.Depth = parent is null 
                            ? 0 
                            : parent.Depth + 1;

            this._children.EnableAutoSort(SortDirection.Ascending, 
                                          (a, b) => a.Value.CompareTo(b.Value));
        }

        private static Boolean AreItemsEqual(TContent left, 
                                             TContent right) =>
            left is IEquatable<TContent> eq
                ? eq.Equals(right)
                : ReferenceEquals(left, 
                                  right);

        private static Boolean AreNodesEqual(TrieNode<TContent> left, 
                                             TrieNode<TContent> right) =>
            left.Value == right.Value;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        internal Boolean IsWord { get; set; }
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        internal IReadOnlyList2<TContent> ChildItems => 
            this._children.SelectMany(child => child.Items)
                          .Union(this._items)
                          .ToRegister(AreItemsEqual);

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        internal readonly Register<TContent> _items;
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly NodeCollection<TrieNode<TContent>, Char> _children;
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly Trie<TContent> _trie;
    }

    // Non-Public
    partial class TrieNode<TContent> : IContentTreeNode<TrieNode<TContent>, Char, TContent> 
        where TContent : class
    {
        /// <inheritdoc/>
        public void AddItem([DisallowNull] TContent item) => 
            this._items.Add(item);

        /// <inheritdoc/>
        public void InsertItem(in Int32 index, 
                               [DisallowNull] TContent item) => 
            this._items.Insert(index, 
                               item);

        /// <inheritdoc/>
        [Pure]
        public Boolean ContainsItem([DisallowNull] TContent item) => 
            this._items.Contains(item);

        /// <inheritdoc/>
        public void RemoveItem([DisallowNull] TContent item)
        {
            if (this._items.Contains(item))
            {
                this._items.Remove(item);
                return;
            }
            foreach (TrieNode<TContent> child in this.Children)
            {
                child.RemoveItem(item);
            }
        }

        /// <inheritdoc/>
        public void ClearItems() => 
            this._items.Clear();

        /// <inheritdoc/>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        [Pure]
        public Char Value { get; }
        /// <inheritdoc/>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        [Pure]
        [MaybeNull]
        public TrieNode<TContent>? Parent { get; }
        /// <inheritdoc/>
        [DebuggerBrowsable(DebuggerBrowsableState.Collapsed)]
        [Pure]
        public UInt32 Depth { get; }
        /// <inheritdoc/>
        [DebuggerBrowsable(DebuggerBrowsableState.Collapsed)]
        [Pure]
        public Boolean IsLeaf => this._children.Count == 0;
        /// <inheritdoc/>
        [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
        [NotNull]
        [Pure]
        public NodeCollection<TrieNode<TContent>, Char> Children => this._children;

        /// <inheritdoc/>
        [DebuggerBrowsable(DebuggerBrowsableState.Collapsed)]
        [NotNull]
        public IReadOnlyList2<TContent> Items =>
            this._trie.ParentsKnowChildItems
                    ? this.ChildItems
                    : this._items;
    }
}
