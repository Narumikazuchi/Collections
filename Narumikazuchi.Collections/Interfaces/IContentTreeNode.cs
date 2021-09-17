namespace Narumikazuchi.Collections
{
    /// <summary>
    /// Represents a node inside a tree structure where nodes can hold contents.
    /// </summary>
    public interface IContentTreeNode<TNode, TValue, TContent> : ITreeNode<TNode, TValue> 
        where TNode : ITreeNode<TNode, TValue> 
        where TContent : class
    {
        /// <summary>
        /// Attaches an item of type <typeparamref name="TContent"/> to the <typeparamref name="TNode"/>.
        /// </summary>
        /// <param name="item">The item to add to the <typeparamref name="TNode"/>.</param>
        public void AddItem([System.Diagnostics.CodeAnalysis.DisallowNull] TContent item);
        /// <summary>
        /// Attaches an item of type <typeparamref name="TContent"/> to the <typeparamref name="TNode"/>.
        /// </summary>
        /// <param name="index">The index at which the item will be inserted into the <see cref="IContentTreeNode{TNode, TValue, TContent}.Items"/>.</param>
        /// <param name="item">The item to add to the <typeparamref name="TNode"/>.</param>
        public void InsertItem(in System.Int32 index, 
                               [System.Diagnostics.CodeAnalysis.DisallowNull] TContent item);
        /// <summary>
        /// Determines whether the specified item is attached to the <typeparamref name="TNode"/>. 
        /// </summary>
        /// <param name="item">The item to determine the attachment of.</param>
        /// <returns><see langword="true"/> if the item is atteched; otherwise, <see langword="false"/></returns>
        [System.Diagnostics.Contracts.Pure]
        public System.Boolean ContainsItem([System.Diagnostics.CodeAnalysis.DisallowNull] TContent item);
        /// <summary>
        /// Removes the specified item from the <typeparamref name="TNode"/>, if it's attached to it.
        /// </summary>
        /// <param name="item">The item to remove.</param>
        public void RemoveItem([System.Diagnostics.CodeAnalysis.DisallowNull] TContent item);
        /// <summary>
        /// Empties the list of attached items to the <typeparamref name="TNode"/>.
        /// </summary>
        public void ClearItems();

        /// <summary>
        /// Gets a collection of items which are either attached to this <typeparamref name="TNode"/> or 
        /// are attached to any of its child-nodes (only when <see cref="IContentTree{TNode, TValue, TContent}.ParentsKnowChildItems"/> for the parent is set to <see langword="true"/>).
        /// </summary>
        [System.Diagnostics.CodeAnalysis.NotNull]
        public Abstract.IReadOnlyList2<TContent> Items { get; }
    }
}
