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
    public sealed partial class Trie<TContent>
    {
        /// <summary>
        /// Instantiates an empty <see cref="Trie{TContent}"/>.
        /// </summary>
        public Trie() => 
            this._root = new(this, 
                             '^', 
                             null);

        /// <summary>
        /// Inserts the specified word into the <see cref="Trie{TContent}"/>.
        /// </summary>
        /// <param name="word">The word to insert into the <see cref="Trie{TContent}"/>.</param>
        public void Insert([DisallowNull] String word) => 
            this.InsertInternal(word, 
                                Array.Empty<TContent>(), 
                                DefaultSeperators);
        /// <summary>
        /// Inserts the specified word into the <see cref="Trie{TContent}"/>.
        /// </summary>
        /// <param name="word">The word to insert into the <see cref="Trie{TContent}"/>.</param>
        /// <param name="seperators"></param>
        public void Insert([DisallowNull] String word, 
                           [DisallowNull] params Char[] seperators) => 
            this.InsertInternal(word, 
                                Array.Empty<TContent>(), 
                                seperators);
        /// <summary>
        /// Inserts the specified word into the <see cref="Trie{TContent}"/> and attaches the specified item to the last <see cref="TrieNode{TContent}"/>.
        /// </summary>
        /// <param name="word">The word to insert into the <see cref="Trie{TContent}"/>.</param>
        /// <param name="item">The item to attach to the last <see cref="TrieNode{TContent}"/> in the <see cref="Trie{TContent}"/> for the specified word.</param>
        public void Insert([DisallowNull] String word, 
                           [DisallowNull] TContent item) => 
            this.InsertInternal(word, 
                                new TContent[] { item }, 
                                DefaultSeperators);
        /// <summary>
        /// Inserts the specified word into the <see cref="Trie{TContent}"/> and attaches the specified item to the last <see cref="TrieNode{TContent}"/>.
        /// </summary>
        /// <param name="word">The word to insert into the <see cref="Trie{TContent}"/>.</param>
        /// <param name="item">The item to attach to the last <see cref="TrieNode{TContent}"/> in the <see cref="Trie{TContent}"/> for the specified word.</param>
        /// <param name="seperators"></param>
        public void Insert([DisallowNull] String word, 
                           [DisallowNull] TContent item, 
                           [DisallowNull] params Char[] seperators) => 
            this.InsertInternal(word, 
                                new TContent[] { item }, 
                                seperators);
        /// <summary>
        /// Inserts the specified word into the <see cref="Trie{TContent}"/> and attaches the specified items to the last <see cref="TrieNode{TContent}"/>.
        /// </summary>
        /// <param name="word">The word to insert into the <see cref="Trie{TContent}"/>.</param>
        /// <param name="items">The items to attach to the last <see cref="TrieNode{TContent}"/> in the <see cref="Trie{TContent}"/> for the specified word.</param>
        public void Insert([DisallowNull] String word, 
                           [DisallowNull] IEnumerable<TContent> items) => 
            this.InsertInternal(word, 
                                items, 
                                DefaultSeperators);
        /// <summary>
        /// Inserts the specified word into the <see cref="Trie{TContent}"/> and attaches the specified items to the last <see cref="TrieNode{TContent}"/>.
        /// </summary>
        /// <param name="word">The word to insert into the <see cref="Trie{TContent}"/>.</param>
        /// <param name="items">The items to attach to the last <see cref="TrieNode{TContent}"/> in the <see cref="Trie{TContent}"/> for the specified word.</param>
        /// <param name="seperators"></param>
        public void Insert([DisallowNull] String word, 
                           [DisallowNull] IEnumerable<TContent> items, 
                           [DisallowNull] params Char[] seperators) => 
            this.InsertInternal(word, 
                                items, 
                                seperators);

        /// <summary>
        /// Determines if the specified word exists in the <see cref="Trie{TContent}"/>.
        /// </summary>
        /// <param name="word">The word to find in the <see cref="Trie{TContent}"/>.</param>
        [Pure]
        public Boolean Exists([DisallowNull] String word) => 
            this.Exists(word, 
                        DefaultSeperators);
        /// <summary>
        /// Determines if the specified word exists in the <see cref="Trie{TContent}"/>.
        /// </summary>
        /// <param name="word">The word to find in the <see cref="Trie{TContent}"/>.</param>
        /// <param name="seperators"></param>
        [Pure]
        public Boolean Exists([DisallowNull] String word, 
                              [DisallowNull] params Char[] seperators)
        {
            if (word is null)
            {
                throw new ArgumentNullException(nameof(word));
            }
            if (seperators is null)
            {
                throw new ArgumentNullException(nameof(seperators));
            }

            String first = word.ToLower()
                               .Split(seperators, 
                                      StringSplitOptions.RemoveEmptyEntries)[0];
            TrieNode<TContent> prefix = this.Find(first);
            return prefix.Depth == first.Length;
        }

        /// <summary>
        /// Finds the last <see cref="TrieNode{TContent}"/> matching the specified prefix or word.
        /// </summary>
        /// <param name="prefix">Either the prefix or whole word to lookup in the <see cref="Trie{TContent}"/>.</param>
        [Pure]
        [return: NotNull]
        public TrieNode<TContent> Find([DisallowNull] String prefix) => 
            this.Find(prefix, 
                      DefaultSeperators);
        /// <summary>
        /// Finds the last <see cref="TrieNode{TContent}"/> matching the specified prefix or word.
        /// </summary>
        /// <param name="prefix">Either the prefix or whole word to lookup in the <see cref="Trie{TContent}"/>.</param>
        /// <param name="seperators"></param>
        [Pure]
        [return: NotNull]
        public TrieNode<TContent> Find([DisallowNull] String prefix, 
                                       [DisallowNull] params Char[] seperators)
        {
            if (prefix is null)
            {
                throw new ArgumentNullException(nameof(prefix));
            }
            if (seperators is null)
            {
                throw new ArgumentNullException(nameof(seperators));
            }

            TrieNode<TContent>? current = this._root;
            TrieNode<TContent> result = current;
            String first = prefix.ToLower()
                                 .Split(seperators, 
                                        StringSplitOptions.RemoveEmptyEntries)[0];
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
        public Boolean Remove([DisallowNull] String word) => 
            this.Remove(word, 
                        DefaultSeperators);
        /// <summary>
        /// Deletes the specified word from the <see cref="Trie{TContent}"/>.
        /// </summary>
        /// <param name="word">The word to delete from the <see cref="Trie{TContent}"/>.</param>
        /// <param name="seperators"></param>
        public Boolean Remove([DisallowNull] String word, 
                              [DisallowNull] params Char[] seperators)
        {
            if (word is null)
            {
                throw new ArgumentNullException(nameof(word));
            }
            if (seperators is null)
            {
                throw new ArgumentNullException(nameof(seperators));
            }

            Boolean result = false;
            String[] words = word.ToLower()
                                 .Split(seperators, 
                                        StringSplitOptions.RemoveEmptyEntries);
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

        /// <summary>
        /// An array containing the default seperators used by the <see cref="Insert(String)"/> functions.
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public static readonly Char[] DefaultSeperators = new Char[] { ' ', '.', ',', ';', '(', ')', '[', ']', '{', '}', '/', '\\', '-', '_' };
    }

    // Non-Public
    partial class Trie<TContent>
    {
        internal Trie(IEnumerable<String> collection) : this()
        {
            if (collection is null)
            {
                throw new ArgumentNullException(nameof(collection));
            }
            if (!collection.Any())
            {
                throw new ArgumentException(CANNOT_CREATE_FROM_EMPTY_COLLECTION);
            }

            foreach (String word in collection.Distinct())
            {
                this.Insert(word);
            }
        }

        private void InsertInternal(String word, 
                                    IEnumerable<TContent> items, 
                                    Char[] seperators)
        {
            if (word is null)
            {
                throw new ArgumentNullException(nameof(word));
            }

            String[] words = word.ToLower()
                                 .Split(seperators, 
                                        StringSplitOptions.RemoveEmptyEntries);
            foreach (String w in words)
            {
                TrieNode<TContent> current = this.Find(w);
                if (current.Depth < w.Length)
                {
                    this._words++;
                }
                for (Int32 i = (Int32)current.Depth; i < w.Length; i++)
                {
                    TrieNode<TContent> newNode = new(this, 
                                                     w[i], 
                                                     current);
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

        private Boolean ExistsInternal(TrieNode<TContent> node, 
                                       TContent content)
        {
            if (node.ContainsItem(content))
            {
                return true;
            }

            foreach (TrieNode<TContent> child in node.Children)
            {
                if (this.ExistsInternal(child, 
                                        content))
                {
                    return true;
                }
            }
            return false;
        }

        private TrieNode<TContent>? FindInternal(TrieNode<TContent> node, 
                                                 TContent content)
        {
            if (content is null)
            {
                throw new ArgumentNullException(nameof(content));
            }

            if (node.ContainsItem(content))
            {
                return node;
            }

            foreach (TrieNode<TContent> child in node.Children)
            {
                TrieNode<TContent>? find = this.FindInternal(child, 
                                                             content);
                if (find is not null)
                {
                    return find;
                }
            }

            return null;
        }

        private Boolean RemoveInternal(TrieNode<TContent> node, 
                                       TContent content)
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
                if (this.RemoveInternal(child, 
                                        content))
                {
                    return true;
                }
            }

            return false;
        }

        private void Traverse(List<String> words, 
                              TrieNode<TContent> node)
        {
            if (node.IsLeaf ||
                node.IsWord)
            {
                words.Add(node.ToString());
            }

            foreach (TrieNode<TContent> child in node.Children)
            {
                this.Traverse(words, 
                              child);
            }
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly TrieNode<TContent> _root;
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private UInt32 _words = 0;

#pragma warning disable
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private const String CANNOT_CREATE_FROM_EMPTY_COLLECTION = "Cannot create Trie from empty IEnumerable.";
#pragma warning restore
    }

    // IEnumerable
    partial class Trie<TContent> : IEnumerable<String>
    {
        /// <inheritdoc/>
        public IEnumerator<String> GetEnumerator()
        {
            List<String> words = new();
            this.Traverse(words, 
                          this._root);
            for (Int32 i = 0; i < words.Count; i++)
            {
                yield return words[i];
            }
            yield break;
        }

        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();

    }

    // IContentTree
    partial class Trie<TContent> : IContentTree<TrieNode<TContent>, Char, TContent>
         where TContent : class
    {
        /// <inheritdoc/>
        Boolean ITree<TrieNode<TContent>, Char>.Insert(Char value) =>
            (this as IContentTree<TrieNode<TContent>, Char, TContent>).Insert(value, 
                                                                              Array.Empty<TContent>());
        /// <inheritdoc/>
        Boolean IContentTree<TrieNode<TContent>, Char, TContent>.Insert(Char value, 
                                                                        [DisallowNull] TContent content) =>
            (this as IContentTree<TrieNode<TContent>, Char, TContent>).Insert(value, 
                                                                              new TContent[] { content });
        /// <inheritdoc/>
        Boolean IContentTree<TrieNode<TContent>, Char, TContent>.Insert(Char value, 
                                                                        [DisallowNull] IEnumerable<TContent> content)
        {
            this.InsertInternal(value.ToString(), 
                                content, 
                                DefaultSeperators);
            return true;
        }

        /// <inheritdoc/>
        Boolean ITree<TrieNode<TContent>, Char>.Exists(Char value) =>
            this.Exists(value.ToString());
        /// <inheritdoc/>
        public Boolean Exists([DisallowNull] TContent content)
        {
            if (content is null)
            {
                throw new ArgumentNullException(nameof(content));
            }

            if (this.ParentsKnowChildItems)
            {
                return this._root.Items.Contains(content);
            }
            return this.ExistsInternal(this._root,
                                       content);
        }

        /// <inheritdoc/>
        TrieNode<TContent>? ITree<TrieNode<TContent>, Char>.Find(Char value) =>
            this.Find(value.ToString());
        /// <inheritdoc/>
        [return: MaybeNull]
        public TrieNode<TContent>? Find([DisallowNull] TContent content) =>
            this.FindInternal(this._root, 
                              content);

        /// <inheritdoc/>
        Boolean ITree<TrieNode<TContent>, Char>.Remove(Char value) =>
            this.Remove(value.ToString());
        /// <inheritdoc/>
        public Boolean Remove([DisallowNull] TContent content) =>
            this.RemoveInternal(this._root, 
                                content);

        /// <inheritdoc/>
        [DebuggerBrowsable(DebuggerBrowsableState.Collapsed)]
        [NotNull]
        [Pure]
        public TrieNode<TContent> RootNode => this._root;
        /// <inheritdoc/>
        [DebuggerBrowsable(DebuggerBrowsableState.Collapsed)]
        public Boolean ParentsKnowChildItems { get; set; } = true;
    }
}
