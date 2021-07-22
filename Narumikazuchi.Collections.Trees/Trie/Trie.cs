using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using System.Linq;

namespace Narumikazuchi.Collections.Trees
{
    /// <summary>
    /// Represents a very fast but memory costly word lookup data structure. Includes the attaching of objects of type <typeparamref name="T"/> to any individual <see cref="TrieNode{T}"/>.
    /// </summary>
    [DebuggerDisplay("Words: {_words} | Items: {RootNode.Items.Count}")]
    public sealed class Trie<T> : ITree<TrieNode<T>> where T : class
    {
        #region Constructor

        /// <summary>
        /// Instantiates an empty <see cref="Trie{T}"/>.
        /// </summary>
        public Trie() => this._root = new TrieNode<T>(this, '^', 0);

        #endregion

        #region Trie Management

        /// <summary>
        /// Finds the last <see cref="TrieNode{T}"/> matching the specified prefix or word.
        /// </summary>
        /// <param name="prefix">Either the prefix or whole word to lookup in the <see cref="Trie{T}"/>.</param>
        [Pure]
        public TrieNode<T> FindNode([DisallowNull] String prefix) => this.FindNode(prefix, DefaultSeperators);
        /// <summary>
        /// Finds the last <see cref="TrieNode{T}"/> matching the specified prefix or word.
        /// </summary>
        /// <param name="prefix">Either the prefix or whole word to lookup in the <see cref="Trie{T}"/>.</param>
        /// <param name="seperators"></param>
        [Pure]
        public TrieNode<T> FindNode([DisallowNull] String prefix, params Char[] seperators)
        {
            if (prefix is null)
            {
                throw new ArgumentNullException(nameof(prefix));
            }

            TrieNode<T>? current = this._root;
            TrieNode<T> result = current;
            String first = prefix.ToLower().Split(seperators, StringSplitOptions.RemoveEmptyEntries)[0];
            foreach (Char c in first)
            {
                current = current.FindChildNode(c);
                if (current is null)
                {
                    break;
                }
                result = current;
            }
            return result;
        }

        /// <summary>
        /// Determines if the specified word exists in the <see cref="Trie{T}"/>.
        /// </summary>
        /// <param name="word">The word to find in the <see cref="Trie{T}"/>.</param>
        [Pure]
        public Boolean WordExists([DisallowNull] String word) => this.WordExists(word, DefaultSeperators);
        /// <summary>
        /// Determines if the specified word exists in the <see cref="Trie{T}"/>.
        /// </summary>
        /// <param name="word">The word to find in the <see cref="Trie{T}"/>.</param>
        /// <param name="seperators"></param>
        [Pure]
        public Boolean WordExists([DisallowNull] String word, params Char[] seperators)
        {
            if (word is null)
            {
                throw new ArgumentNullException(nameof(word));
            }

            String first = word.ToLower().Split(seperators, StringSplitOptions.RemoveEmptyEntries)[0];
            TrieNode<T> prefix = this.FindNode(first);
            return prefix.Depth == first.Length;
        }

        /// <summary>
        /// Inserts the specified word into the <see cref="Trie{T}"/>.
        /// </summary>
        /// <param name="word">The word to insert into the <see cref="Trie{T}"/>.</param>
        public void Insert([DisallowNull] String word) => this.InsertInternal(word, Array.Empty<T>(), DefaultSeperators);
        /// <summary>
        /// Inserts the specified word into the <see cref="Trie{T}"/>.
        /// </summary>
        /// <param name="word">The word to insert into the <see cref="Trie{T}"/>.</param>
        /// <param name="seperators"></param>
        public void Insert([DisallowNull] String word, params Char[] seperators) => this.InsertInternal(word, Array.Empty<T>(), seperators);
        /// <summary>
        /// Inserts the specified word into the <see cref="Trie{T}"/> and attaches the specified item to the last <see cref="TrieNode{T}"/>.
        /// </summary>
        /// <param name="word">The word to insert into the <see cref="Trie{T}"/>.</param>
        /// <param name="item">The item to attach to the last <see cref="TrieNode{T}"/> in the <see cref="Trie{T}"/> for the specified word.</param>
        public void Insert([DisallowNull] String word, [DisallowNull] T item) => this.InsertInternal(word, new T[] { item }, DefaultSeperators);
        /// <summary>
        /// Inserts the specified word into the <see cref="Trie{T}"/> and attaches the specified item to the last <see cref="TrieNode{T}"/>.
        /// </summary>
        /// <param name="word">The word to insert into the <see cref="Trie{T}"/>.</param>
        /// <param name="item">The item to attach to the last <see cref="TrieNode{T}"/> in the <see cref="Trie{T}"/> for the specified word.</param>
        /// <param name="seperators"></param>
        public void Insert([DisallowNull] String word, [DisallowNull] T item, params Char[] seperators) => this.InsertInternal(word, new T[] { item }, seperators);
        /// <summary>
        /// Inserts the specified word into the <see cref="Trie{T}"/> and attaches the specified items to the last <see cref="TrieNode{T}"/>.
        /// </summary>
        /// <param name="word">The word to insert into the <see cref="Trie{T}"/>.</param>
        /// <param name="items">The items to attach to the last <see cref="TrieNode{T}"/> in the <see cref="Trie{T}"/> for the specified word.</param>
        public void Insert([DisallowNull] String word, [DisallowNull] IEnumerable<T> items) => this.InsertInternal(word, items, DefaultSeperators);
        /// <summary>
        /// Inserts the specified word into the <see cref="Trie{T}"/> and attaches the specified items to the last <see cref="TrieNode{T}"/>.
        /// </summary>
        /// <param name="word">The word to insert into the <see cref="Trie{T}"/>.</param>
        /// <param name="items">The items to attach to the last <see cref="TrieNode{T}"/> in the <see cref="Trie{T}"/> for the specified word.</param>
        /// <param name="seperators"></param>
        public void Insert([DisallowNull] String word, [DisallowNull] IEnumerable<T> items, params Char[] seperators) => this.InsertInternal(word, items, seperators);

        private void InsertInternal(String word, IEnumerable<T> items, Char[] seperators)
        {
            if (word is null)
            {
                throw new ArgumentNullException(nameof(word));
            }

            String[] words = word.ToLower().Split(seperators, StringSplitOptions.RemoveEmptyEntries);
            foreach (String w in words)
            {
                TrieNode<T> current = this.FindNode(w);
                if (current.Depth < w.Length)
                {
                    this._words++;
                }
                for (Int32 i = current.Depth; i < w.Length; i++)
                {
                    TrieNode<T> newNode = new(this, w[i], current.Depth + 1, current);
                    current.AddChild(newNode);
                    current = newNode;
                }
                if (items.Any())
                {
                    foreach (T item in items)
                    {
                        current.AddItem(item);
                    }
                }
            }
        }

        /// <summary>
        /// Deletes the specified word from the <see cref="Trie{T}"/>.
        /// </summary>
        /// <param name="word">The word to delete from the <see cref="Trie{T}"/>.</param>
        public void Delete([DisallowNull] String word) => this.Delete(word, DefaultSeperators);
        /// <summary>
        /// Deletes the specified word from the <see cref="Trie{T}"/>.
        /// </summary>
        /// <param name="word">The word to delete from the <see cref="Trie{T}"/>.</param>
        /// <param name="seperators"></param>
        public void Delete([DisallowNull] String word, params Char[] seperators)
        {
            if (word is null)
            {
                throw new ArgumentNullException(nameof(word));
            }

            String[] words = word.ToLower().Split(seperators, StringSplitOptions.RemoveEmptyEntries);
            foreach (String w in words)
            {
                this._words--;
                TrieNode<T> node = this.FindNode(w);
                if (node.Depth == w.Length)
                {
                    while (node.IsLeaf)
                    {
                        TrieNode<T>? parent = node.Parent;
                        if (parent is null)
                        {
                            break;
                        }
                        parent.DeleteChildNode(node);
                        node = parent;
                    }
                }
            }
        }
        /// <summary>
        /// Deletes the specified item from the <see cref="TrieNode{T}"/> it's attached to.
        /// </summary>
        /// <param name="item">The item to delete from the <see cref="Trie{T}"/></param>
        public void Delete([DisallowNull] T item) => this.DeleteInternal(this._root, item, false);
        /// <summary>
        /// Deletes the specified item from the <see cref="TrieNode{T}"/> it's attached to.
        /// </summary>
        /// <param name="item">The item to delete from the <see cref="Trie{T}"/></param>
        /// <param name="deleteWord">If <see langword="true"/> also deletes nodes which would be left with no items.</param>
        public void Delete([DisallowNull] T item, in Boolean deleteWord) => this.DeleteInternal(this._root, item, deleteWord);

        private void DeleteInternal(TrieNode<T> node, T item, in Boolean deleteWord)
        {
            if (node._items.Contains(item))
            {
                node._items.Remove(item);
            }
            if (deleteWord &&
                ((this.ParentsKnowChildItems &&
                    node.Items.Count == 0) ||
                (!this.ParentsKnowChildItems &&
                    node.ChildItems.Count == 0)))
            {
                TrieNode<T>? parent = node.Parent;
                if (parent is not null)
                {
                    parent.DeleteChildNode(node);
                    this._words--;
                }
            }
            foreach (TrieNode<T> child in node.Children)
            {
                this.DeleteInternal(child, item, deleteWord);
            }
        }

        /// <summary>
        /// Clears the entire <see cref="Trie{T}"/> structure of any nodes and items.
        /// </summary>
        public void Clear()
        {
            this._root.Clear();
            this._words = 0;
        }

        #endregion

        #region ITree

        /// <summary>
        /// Gets the root node of the <see cref="Trie{T}"/>.
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Collapsed)]
        [DisallowNull]
        [Pure]
        public TrieNode<T> RootNode => this._root;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets if a parent <see cref="TrieNode{T}"/> should have a reference to the items of it's child-nodes in it's own <see cref="TrieNode{T}.Items"/>.
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Collapsed)]
        public Boolean ParentsKnowChildItems { get; set; } = true;

        #endregion

        #region Fields

        /// <summary>
        /// An array containing the default seperators used by the <see cref="Insert(String)"/> functions.
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public static readonly Char[] DefaultSeperators = new Char[] { ' ', '.', ',', ';', '(', ')', '[', ']', '{', '}', '/', '\\', '-', '_' };

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly TrieNode<T> _root;
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private UInt32 _words = 0;

        #endregion
    }
}
