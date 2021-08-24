using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;

namespace Narumikazuchi.Collections
{
    /// <summary>
    /// Represents a very fast but memory costly word lookup data structure. Includes the attaching of objects of type <typeparamref name="TContent"/> to any individual <see cref="TrieNode{TContent}"/>.
    /// </summary>
    [DebuggerDisplay("Words: {_words} | Items: {RootNode.ChildItems.Count}")]
    public sealed class Trie<TContent> : IContentTree<TrieNode<TContent>, Char, TContent>, IEnumerable<String> where TContent : class
    {
        #region Constructor

        /// <summary>
        /// Instantiates an empty <see cref="Trie{TContent}"/>.
        /// </summary>
        public Trie() => this._root = new(this, '^', null);

        internal Trie([DisallowNull] IEnumerable<String> collection) : this()
        {
            if (collection is null)
            {
                throw new ArgumentNullException(nameof(collection));
            }
            if (!collection.Any())
            {
                throw new ArgumentException("Passed Collection was empty!");
            }

            foreach (String word in collection.Distinct())
            {
                this.Insert(word);
            }
        }

        #endregion

        #region Trie Management

        /// <summary>
        /// Inserts the specified word into the <see cref="Trie{TContent}"/>.
        /// </summary>
        /// <param name="word">The word to insert into the <see cref="Trie{TContent}"/>.</param>
        public void Insert([DisallowNull] String word) => this.InsertInternal(word, Array.Empty<TContent>(), DefaultSeperators);
        /// <summary>
        /// Inserts the specified word into the <see cref="Trie{TContent}"/>.
        /// </summary>
        /// <param name="word">The word to insert into the <see cref="Trie{TContent}"/>.</param>
        /// <param name="seperators"></param>
        public void Insert([DisallowNull] String word, params Char[] seperators) => this.InsertInternal(word, Array.Empty<TContent>(), seperators);
        /// <summary>
        /// Inserts the specified word into the <see cref="Trie{TContent}"/> and attaches the specified item to the last <see cref="TrieNode{TContent}"/>.
        /// </summary>
        /// <param name="word">The word to insert into the <see cref="Trie{TContent}"/>.</param>
        /// <param name="item">The item to attach to the last <see cref="TrieNode{TContent}"/> in the <see cref="Trie{TContent}"/> for the specified word.</param>
        public void Insert([DisallowNull] String word, [DisallowNull] TContent item) => this.InsertInternal(word, new TContent[] { item }, DefaultSeperators);
        /// <summary>
        /// Inserts the specified word into the <see cref="Trie{TContent}"/> and attaches the specified item to the last <see cref="TrieNode{TContent}"/>.
        /// </summary>
        /// <param name="word">The word to insert into the <see cref="Trie{TContent}"/>.</param>
        /// <param name="item">The item to attach to the last <see cref="TrieNode{TContent}"/> in the <see cref="Trie{TContent}"/> for the specified word.</param>
        /// <param name="seperators"></param>
        public void Insert([DisallowNull] String word, [DisallowNull] TContent item, params Char[] seperators) => this.InsertInternal(word, new TContent[] { item }, seperators);
        /// <summary>
        /// Inserts the specified word into the <see cref="Trie{TContent}"/> and attaches the specified items to the last <see cref="TrieNode{TContent}"/>.
        /// </summary>
        /// <param name="word">The word to insert into the <see cref="Trie{TContent}"/>.</param>
        /// <param name="items">The items to attach to the last <see cref="TrieNode{TContent}"/> in the <see cref="Trie{TContent}"/> for the specified word.</param>
        public void Insert([DisallowNull] String word, [DisallowNull] IEnumerable<TContent> items) => this.InsertInternal(word, items, DefaultSeperators);
        /// <summary>
        /// Inserts the specified word into the <see cref="Trie{TContent}"/> and attaches the specified items to the last <see cref="TrieNode{TContent}"/>.
        /// </summary>
        /// <param name="word">The word to insert into the <see cref="Trie{TContent}"/>.</param>
        /// <param name="items">The items to attach to the last <see cref="TrieNode{TContent}"/> in the <see cref="Trie{TContent}"/> for the specified word.</param>
        /// <param name="seperators"></param>
        public void Insert([DisallowNull] String word, [DisallowNull] IEnumerable<TContent> items, params Char[] seperators) => this.InsertInternal(word, items, seperators);

        private void InsertInternal(String word, IEnumerable<TContent> items, Char[] seperators)
        {
            if (word is null)
            {
                throw new ArgumentNullException(nameof(word));
            }

            String[] words = word.ToLower().Split(seperators, StringSplitOptions.RemoveEmptyEntries);
            foreach (String w in words)
            {
                TrieNode<TContent> current = this.Find(w);
                if (current.Depth < w.Length)
                {
                    this._words++;
                }
                for (Int32 i = (Int32)current.Depth; i < w.Length; i++)
                {
                    TrieNode<TContent> newNode = new(this, w[i], current);
                    current.Children.Add(newNode);
                    current = newNode;
                }
                current.IsWord = true;
                if (items.Any())
                {
                    foreach (TContent item in items)
                    {
                        current.AddItem(item);
                    }
                }
            }
        }

        /// <summary>
        /// Determines if the specified word exists in the <see cref="Trie{TContent}"/>.
        /// </summary>
        /// <param name="word">The word to find in the <see cref="Trie{TContent}"/>.</param>
        [Pure]
        public Boolean Exists([DisallowNull] String word) => this.Exists(word, DefaultSeperators);
        /// <summary>
        /// Determines if the specified word exists in the <see cref="Trie{TContent}"/>.
        /// </summary>
        /// <param name="word">The word to find in the <see cref="Trie{TContent}"/>.</param>
        /// <param name="seperators"></param>
        [Pure]
        public Boolean Exists([DisallowNull] String word, params Char[] seperators)
        {
            if (word is null)
            {
                throw new ArgumentNullException(nameof(word));
            }

            String first = word.ToLower().Split(seperators, StringSplitOptions.RemoveEmptyEntries)[0];
            TrieNode<TContent> prefix = this.Find(first);
            return prefix.Depth == first.Length;
        }

        /// <summary>
        /// Finds the last <see cref="TrieNode{TContent}"/> matching the specified prefix or word.
        /// </summary>
        /// <param name="prefix">Either the prefix or whole word to lookup in the <see cref="Trie{TContent}"/>.</param>
        [Pure]
        public TrieNode<TContent> Find([DisallowNull] String prefix) => this.Find(prefix, DefaultSeperators);
        /// <summary>
        /// Finds the last <see cref="TrieNode{TContent}"/> matching the specified prefix or word.
        /// </summary>
        /// <param name="prefix">Either the prefix or whole word to lookup in the <see cref="Trie{TContent}"/>.</param>
        /// <param name="seperators"></param>
        [Pure]
        public TrieNode<TContent> Find([DisallowNull] String prefix, params Char[] seperators)
        {
            if (prefix is null)
            {
                throw new ArgumentNullException(nameof(prefix));
            }

            TrieNode<TContent>? current = this._root;
            TrieNode<TContent> result = current;
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
        /// Deletes the specified word from the <see cref="Trie{TContent}"/>.
        /// </summary>
        /// <param name="word">The word to delete from the <see cref="Trie{TContent}"/>.</param>
        public Boolean Remove([DisallowNull] String word) => this.Remove(word, DefaultSeperators);
        /// <summary>
        /// Deletes the specified word from the <see cref="Trie{TContent}"/>.
        /// </summary>
        /// <param name="word">The word to delete from the <see cref="Trie{TContent}"/>.</param>
        /// <param name="seperators"></param>
        public Boolean Remove([DisallowNull] String word, params Char[] seperators)
        {
            if (word is null)
            {
                throw new ArgumentNullException(nameof(word));
            }

            Boolean result = false;
            String[] words = word.ToLower().Split(seperators, StringSplitOptions.RemoveEmptyEntries);
            foreach (String w in words)
            {
                this._words--;
                TrieNode<TContent> node = this.Find(w);
                if (node.Depth == w.Length)
                {
                    node.IsWord = false;
                    result = true;
                    while (node.IsLeaf)
                    {
                        TrieNode<TContent>? parent = node.Parent;
                        if (parent is null)
                        {
                            break;
                        }
                        parent.Children.Remove(node);
                        node = parent;
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// Clears the entire <see cref="Trie{TContent}"/> structure of any nodes and items.
        /// </summary>
        public void Clear()
        {
            this._root.Children.Clear();
            this._words = 0;
        }

        #endregion

        #region IContentTree

        /// <inheritdoc/>
        Boolean ITree<TrieNode<TContent>, Char>.Insert(in Char value) =>
            (this as IContentTree<TrieNode<TContent>, Char, TContent>).Insert(value, Array.Empty<TContent>());
        /// <inheritdoc/>
        Boolean IContentTree<TrieNode<TContent>, Char, TContent>.Insert(in Char value, [DisallowNull] TContent content) =>
            (this as IContentTree<TrieNode<TContent>, Char, TContent>).Insert(value, new TContent[] { content });
        /// <inheritdoc/>
        Boolean IContentTree<TrieNode<TContent>, Char, TContent>.Insert(in Char value, [DisallowNull] IEnumerable<TContent> content)
        {
            this.InsertInternal(value.ToString(), content, DefaultSeperators);
            return true;
        }

        /// <inheritdoc/>
        Boolean ITree<TrieNode<TContent>, Char>.Exists(in Char value) => 
            this.Exists(value.ToString());
        /// <inheritdoc/>
        public Boolean Exists([DisallowNull] TContent content) => 
            content is null
                ? throw new ArgumentNullException(nameof(content))
                : this.ParentsKnowChildItems
                ? this._root.Items.Contains(content)
                : this.ExistsInternal(this._root, content);

        private Boolean ExistsInternal(TrieNode<TContent> node, TContent content)
        {
            if (node.ContainsItem(content))
            {
                return true;
            }

            foreach (TrieNode<TContent>? child in node.Children)
            {
                if (this.ExistsInternal(child, content))
                {
                    return true;
                }
            }

            return false;
        }

        /// <inheritdoc/>
        TrieNode<TContent>? ITree<TrieNode<TContent>, Char>.Find(in Char value) =>
            this.Find(value.ToString());
        /// <inheritdoc/>
        public TrieNode<TContent>? Find([DisallowNull] TContent content) =>
            this.FindInternal(this._root, content);

        private TrieNode<TContent>? FindInternal(TrieNode<TContent> node, TContent content)
        {
            if (content is null)
            {
                throw new ArgumentNullException(nameof(content));
            }

            if (node.ContainsItem(content))
            {
                return node;
            }

            foreach (TrieNode<TContent>? child in node.Children)
            {
                TrieNode<TContent>? find = this.FindInternal(child, content);
                if (find is not null)
                {
                    return find;
                }
            }

            return null;
        }

        /// <inheritdoc/>
        Boolean ITree<TrieNode<TContent>, Char>.Remove(in Char value) =>
            this.Remove(value.ToString());
        /// <inheritdoc/>
        public Boolean Remove([DisallowNull] TContent content) => 
            this.RemoveInternal(this._root, content);

        private Boolean RemoveInternal(TrieNode<TContent> node, TContent content)
        {
            if (content is null)
            {
                throw new ArgumentNullException(nameof(content));
            }

            if (node.ContainsItem(content))
            {
                node.RemoveItem(content);
                return true;
            }

            foreach (TrieNode<TContent> child in node.Children)
            {
                if (this.RemoveInternal(child, content))
                {
                    return true;
                }
            }

            return false;
        }

        #region Properties

        /// <inheritdoc/>
        [DebuggerBrowsable(DebuggerBrowsableState.Collapsed)]
        [DisallowNull]
        [Pure]
        public TrieNode<TContent> RootNode => this._root;
        /// <inheritdoc/>
        [DebuggerBrowsable(DebuggerBrowsableState.Collapsed)]
        public Boolean ParentsKnowChildItems { get; set; } = true;

        #endregion

        #endregion

        #region IEnumerable

        /// <inheritdoc/>
        public IEnumerator<String> GetEnumerator()
        {
            List<String> words = new();
            this.Traverse(words, this._root);
            for (Int32 i = 0; i < words.Count; i++)
            {
                yield return words[i];
            }
            yield break;
        }

        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();

        private void Traverse(List<String> words, TrieNode<TContent> node)
        {
            if (node.IsLeaf ||
                node.IsWord)
            {
                words.Add(node.ToString());
            }

            foreach (TrieNode<TContent> child in node.Children)
            {
                this.Traverse(words, child);
            }
        }

        #endregion

        #region Fields

        /// <summary>
        /// An array containing the default seperators used by the <see cref="Insert(String)"/> functions.
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public static readonly Char[] DefaultSeperators = new Char[] { ' ', '.', ',', ';', '(', ')', '[', ']', '{', '}', '/', '\\', '-', '_' };

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly TrieNode<TContent> _root;
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private UInt32 _words = 0;

        #endregion
    }
}
