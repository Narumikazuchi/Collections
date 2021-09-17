namespace Narumikazuchi.Collections
{
    /// <summary>
    /// Represents any tree data structure.
    /// </summary>
    public interface ITree<TNode, TValue> 
        where TNode : ITreeNode<TNode, TValue>
    {
        /// <summary>
        /// Inserts the specified value into the <see cref="ITree{TNode, TValue}"/>.
        /// </summary>
        /// <param name="value">The value to be inserted.</param>
        /// <returns><see langword="true"/> if the specified value has been added to the tree; otherwise, <see langword="false"/></returns>
        public System.Boolean Insert([System.Diagnostics.CodeAnalysis.DisallowNull] TValue value);
        /// <summary>
        /// Determines if the specified value exists in the <see cref="ITree{TNode, TValue}"/>.
        /// </summary>
        /// <param name="value">The value to lookup.</param>
        /// <returns><see langword="true"/> if the specified value is found in the tree; otherwise, <see langword="false"/></returns>
        [System.Diagnostics.Contracts.Pure]
        public System.Boolean Exists([System.Diagnostics.CodeAnalysis.DisallowNull] TValue value);
        /// <summary>
        /// Finds the <typeparamref name="TNode"/> with the highest depth matching the specified value.
        /// </summary>
        /// <param name="value">The value to lookup in the tree.</param>
        /// <returns>The <typeparamref name="TNode"/> which contains the specified value or <see langword="null"/> if no node with such value exists in the tree.</returns>
        [System.Diagnostics.Contracts.Pure]
        [return: System.Diagnostics.CodeAnalysis.MaybeNull]
        public TNode Find([System.Diagnostics.CodeAnalysis.DisallowNull] TValue value);
        /// <summary>
        /// Removes the specified value from the <see cref="ITree{TNode, TValue}"/>.
        /// </summary>
        /// <param name="value">The value to remove from the tree.</param>
        /// <returns><see langword="true"/> if the specified value has been removed from the tree; otherwise, <see langword="false"/></returns>
        public System.Boolean Remove([System.Diagnostics.CodeAnalysis.DisallowNull] TValue value);
        /// <summary>
        /// Clears the entire <see cref="ITree{TNode, TValue}"/> of any nodes.
        /// </summary>
        public void Clear();

        /// <summary>
        /// Gets the root node of this tree structure.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.NotNull]
        public TNode RootNode { get; }
    }
}
