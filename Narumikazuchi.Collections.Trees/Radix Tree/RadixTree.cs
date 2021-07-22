using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;

namespace Narumikazuchi.Collections.Trees
{
    /// <summary>
    /// Represents a word lookup data structure, which is slower than the <see cref="Trie.Trie{T}"/> but less memory intensive. Includes the attaching of objects of type <typeparamref name="T"/> to any individual <see cref="RadixNode{T}"/>.
    /// </summary>
    [DebuggerDisplay("Words: {_words} | Items: {RootNode.Items.Count}")]
    public sealed class RadixTree<T> : ITree<RadixNode<T>>
    {
        #region Constructor

        /// <summary>
        /// Creates a new empty <see cref="RadixTree{T}"/>.
        /// </summary>
        public RadixTree() => this._root = new RadixNode<T>();

        #endregion

        #region Node Management

        /// <summary>
        /// Inserts the specified word into the <see cref="RadixTree{T}"/>.
        /// </summary>
        /// <param name="word">The word to be inserted.</param>
        public void Insert([DisallowNull] String word) => this.InsertInternal(word, this._root, System.Linq.Enumerable.Empty<T>());
        /// <summary>
        /// Inserts the specified word into the <see cref="RadixTree{T}"/> and attaches the specified item to the resulting <see cref="RadixNode{T}"/>.
        /// </summary>
        /// <param name="word">The word to be inserted.</param>
        /// <param name="item">The item to attach.</param>
        public void Insert([DisallowNull] String word, in T item) => this.InsertInternal(word, this._root, new T[] { item });
        /// <summary>
        /// Inserts the specified word into the <see cref="RadixTree{T}"/> and attaches the specified items to the resulting <see cref="RadixNode{T}"/>.
        /// </summary>
        /// <param name="word">The word to be inserted.</param>
        /// <param name="items">The items to attach.</param>
        public void Insert([DisallowNull] String word, [DisallowNull] IEnumerable<T> items) => this.InsertInternal(word, this._root, items);

        private void InsertInternal(String part, RadixNode<T> node, IEnumerable<T> items)
        {
            if (part is null)
            {
                throw new ArgumentNullException(nameof(part));
            }
            if (items is null)
            {
                throw new ArgumentNullException(nameof(items));
            }

            String wordPart = part;
            Int32 matches = AmountMatchingConsecutiveCharacters(wordPart, node);

            if (matches == 0 ||
                ReferenceEquals(this._root, node) ||
                (matches > 0 &&
                matches < wordPart.Length &&
                matches >= node.Label.Length))
            {
                Boolean inserted = false;
                String subWord = wordPart[matches..];
                foreach (RadixNode<T> child in node.Children)
                {
                    if (child.Label[0] == subWord[0])
                    {
                        inserted = true;
                        this.InsertInternal(subWord, child, items);
                    }
                }
                if (!inserted)
                {
                    node.AddChild(new RadixNode<T>(subWord, node, items));
                }
            }
            else if (matches < wordPart.Length)
            {
                String commonRoot = wordPart.Substring(0, matches);
                String branchParent = node.Label[matches..];
                String branchNew = wordPart[matches..];

                node.Label = commonRoot;

                RadixNode<T> newNodeParent = new(branchParent, node);
                newNodeParent.AddChildren(node.Children);

                node.ClearChildren();
                node.AddChild(newNodeParent);

                RadixNode<T> newNode = new(branchNew, node, items);
                node.AddChild(newNode);
            }
            else if (matches > node.Label.Length)
            {
                String branchNew = node.Label.Substring(node.Label.Length, wordPart.Length);
                RadixNode<T> newNode = new(branchNew, node, items);
                node.AddChild(newNode);
            }
        }

        /// <summary>
        /// Determines if the specified word exists in the <see cref="RadixTree{T}"/>.
        /// </summary>
        /// <param name="word">The word to lookup.</param>
        [Pure]
        public Boolean WordExists([DisallowNull] String word) => this.WordExistsInternal(word, this._root);

        [Pure]
        private Boolean WordExistsInternal(String part, RadixNode<T> node)
        {
            if (part is null)
            {
                throw new ArgumentNullException(nameof(part));
            }

            String wordPart = part;
            Int32 matches = AmountMatchingConsecutiveCharacters(wordPart, node);

            if (matches == 0 ||
                ReferenceEquals(this._root, node) ||
                (matches > 0 &&
                matches < wordPart.Length &&
                matches > node.Label.Length))
            {
                String newLabel = wordPart[matches..];
                foreach (RadixNode<T> child in node.Children)
                {
                    if (child.Label[0] == newLabel[0])
                    {
                        return this.WordExistsInternal(newLabel, child);
                    }
                }
                return false;
            }
            else
            {
                return matches == node.Label.Length;
            }
        }

        /// <summary>
        /// Locates and returns the smallest string greater than the specified word, by lexicographic order.
        /// </summary>
        /// <param name="word">The word to find the successor of.</param>
        [Pure]
        public String FindSuccessor([DisallowNull] String word) => this.FindSuccessorInternal(word, this._root, String.Empty);

        [Pure]
        private String FindSuccessorInternal(String part, RadixNode<T> node, String carry)
        {
            if (part is null)
            {
                throw new ArgumentNullException(nameof(part));
            }

            String wordPart = part;
            Int32 matches = AmountMatchingConsecutiveCharacters(wordPart, node);

            if (matches == 0 ||
                ReferenceEquals(this._root, node) ||
                (matches > 0 &&
                matches < wordPart.Length))
            {
                String newLabel = wordPart[matches..];
                foreach (RadixNode<T> child in node.Children)
                {
                    if (child.Label[0] == newLabel[0])
                    {
                        return this.FindSuccessorInternal(newLabel, child, carry + node.Label);
                    }
                }
                return node.Label;
            }
            else if (matches < node.Label.Length)
            {
                return carry + node.Label;
            }
            else if (matches == node.Label.Length)
            {
                carry += node.Label;

                Int32 min = Int32.MaxValue;
                RadixNode<T>? current = null;
                foreach (RadixNode<T> child in node.Children)
                {
                    if (child.Label.Length < min)
                    {
                        min = child.Label.Length;
                        current = child;
                    }
                }
                return current is not null ?
                         carry + current.Label :
                         carry;
            }
            return String.Empty;
        }

        /// <summary>
        /// Locates and returns the largest string less than the specified word, by lexicographic order.
        /// </summary>
        /// <param name="word">The word to find the predeccessor of.</param>
        [Pure]
        public String FindPredeccessor([DisallowNull] String word) => this.FindPredeccessorInternal(word, this._root, String.Empty);

        [Pure]
        private String FindPredeccessorInternal(String part, RadixNode<T> node, String carry)
        {
            if (part is null)
            {
                throw new ArgumentNullException(nameof(part));
            }

            String wordPart = part;
            Int32 matches = AmountMatchingConsecutiveCharacters(wordPart, node);

            if (matches == 0 ||
                ReferenceEquals(this._root, node) ||
                (matches > 0 &&
                matches < wordPart.Length &&
                matches >= node.Label.Length))
            {
                String newLabel = wordPart[matches..];
                foreach (RadixNode<T> child in node.Children)
                {
                    if (child.Label[0] == newLabel[0])
                    {
                        return this.FindPredeccessorInternal(newLabel, child, carry + node.Label);
                    }
                }
                return carry + node.Label;
            }
            else if (matches == node.Label.Length)
            {
                return carry + node.Label;
            }
            return String.Empty;
        }

        /// <summary>
        /// Deletes the specified word from the <see cref="RadixTree{T}"/>.
        /// </summary>
        /// <param name="word">The word to be deleted.</param>
        public void Delete([DisallowNull] String word) => this.DeleteInternal(word, this._root);
        /// <summary>
        /// Deletes the specified item all node of the <see cref="RadixTree{T}"/>.
        /// </summary>
        /// <param name="item">The item to be deleted.</param>
        public void Delete([DisallowNull] in T item) => this.DeleteInternal(item, this._root);

        private void DeleteInternal(String part, RadixNode<T> node)
        {
            if (part is null)
            {
                throw new ArgumentNullException(nameof(part));
            }

            String wordPart = part;
            Int32 matches = AmountMatchingConsecutiveCharacters(wordPart, node);

            if (matches == 0 ||
                ReferenceEquals(this._root, node) ||
                (matches > 0 &&
                matches < wordPart.Length &&
                matches >= node.Label.Length))
            {
                String newLabel = wordPart[matches..];
                foreach (RadixNode<T> child in node.Children)
                {
                    if (child.Label[0] == newLabel[0])
                    {
                        if (newLabel == child.Label)
                        {
                            if (child.Children.Count == 0)
                            {
                                node.RemoveChild(child);
                                return;
                            }
                        }
                        this.DeleteInternal(newLabel, child);
                    }
                }
            }
        }

        private void DeleteInternal(in T item, RadixNode<T> node)
        {
            if (item is null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            node.RemoveItem(item, false);
            foreach (RadixNode<T> child in node.Children)
            {
                this.DeleteInternal(item, child);
            }
        }

        #endregion

        #region Node Support

        [Pure]
        private static Int32 AmountMatchingConsecutiveCharacters(String word, RadixNode<T> node)
        {
            Int32 matches = 0;
            Int32 min = node.Label.Length >= word.Length ? word.Length : node.Label.Length;
            if (min > 0)
            {
                for (Int32 i = 0; i < min; i++)
                {
                    if (word[i] == node.Label[i])
                    {
                        matches++;
                    }
                    else
                    {
                        break;
                    }
                }
            }
            return matches;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets if a parent <see cref="RadixNode{T}"/> should have a reference to the items of it's child-nodes in it's own <see cref="RadixNode{T}.Items"/>.
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Collapsed)]
        public Boolean ParentsKnowChildItems { get; set; } = true;
        /// <summary>
        /// Gets or stes if an exception should be thrown, when trying to add a duplicate <see cref="RadixNode{T}"/> or item <typeparamref name="T"/>.
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Collapsed)]
        public Boolean ThrowExceptionOnDuplicate { get; set; } = true;
        /// <summary>
        /// Gets the root <see cref="RadixNode{T}"/> for the <see cref="RadixTree{T}"/>.
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Collapsed)]
        [DisallowNull]
        [Pure]
        public RadixNode<T> RootNode => this._root;

        #endregion

        #region Fields

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private RadixNode<T> _root;

        #endregion
    }
}
