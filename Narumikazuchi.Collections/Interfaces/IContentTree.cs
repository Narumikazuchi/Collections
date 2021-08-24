namespace Narumikazuchi.Collections
{
    /// <summary>
    /// Represents any tree data structure where the nodes contain other content besides their value.
    /// </summary>
    public interface IContentTree<TNode, TValue, TContent> : ITree<TNode, TValue> where TNode : IContentTreeNode<TNode, TValue, TContent> where TContent : class
    {
        #region Management

        /// <summary>
        /// Inserts the specified value into the <see cref="IContentTree{TNode, TValue, TContent}"/> and adds the specified content to the resulting node.
        /// </summary>
        /// <param name="value">The value to be inserted.</param>
        /// <param name="content">The content to add to the resulting node.</param>
        public System.Boolean Insert([System.Diagnostics.CodeAnalysis.DisallowNull] in TValue value, [System.Diagnostics.CodeAnalysis.DisallowNull] TContent content);
        /// <summary>
        /// Inserts the specified value into the <see cref="IContentTree{TNode, TValue, TContent}"/> and adds the specified content to the resulting node.
        /// </summary>
        /// <param name="value">The value to be inserted.</param>
        /// <param name="content">The content to add to the resulting node.</param>
        public System.Boolean Insert([System.Diagnostics.CodeAnalysis.DisallowNull] in TValue value, [System.Diagnostics.CodeAnalysis.DisallowNull] System.Collections.Generic.IEnumerable<TContent> content);
        /// <summary>
        /// Determines if the specified content exists in the <see cref="IContentTree{TNode, TValue, TContent}"/>.
        /// </summary>
        /// <param name="content">The content to look for.</param>
        /// <returns><see langword="true"/> if the specified content is found in the tree; otherwise, <see langword="false"/></returns>
        [System.Diagnostics.Contracts.Pure]
        public System.Boolean Exists([System.Diagnostics.CodeAnalysis.DisallowNull] TContent content);
        /// <summary>
        /// Finds the <typeparamref name="TNode"/> with the highest depth having the specified content.
        /// </summary>
        /// <param name="content">The content to look for in the tree.</param>
        /// <returns>The <typeparamref name="TNode"/> which contains the specified content or <see langword="null"/> if no node with such value exists in the tree.</returns>
        [System.Diagnostics.Contracts.Pure]
        public TNode? Find([System.Diagnostics.CodeAnalysis.DisallowNull] TContent content);
        /// <summary>
        /// Removes the specified content from the <typeparamref name="TNode"/> it's attached to.
        /// </summary>
        /// <param name="content">The content to remove from the tree.</param>
        public System.Boolean Remove([System.Diagnostics.CodeAnalysis.DisallowNull] TContent content);

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets if a parent <typeparamref name="TNode"/> should have a reference to the items of it's child-nodes in it's own <see cref="IContentTreeNode{TNode, TValue, TContent}.Items"/>.
        /// </summary>
        [System.Diagnostics.Contracts.Pure]
        public System.Boolean ParentsKnowChildItems { get; set; }

        #endregion
    }
}
