namespace Narumikazuchi.Collections.Trees
{
    /// <summary>
    /// Represents a node inside any tree structure.
    /// </summary>
    public interface ITreeNode<TNode> where TNode : ITreeNode<TNode>
    {
        #region Properties

        /// <summary>
        /// The parent of the current node. Should return <see langword="null"/> for root nodes.
        /// </summary>
        public TNode? Parent { get; }
        /// <summary>
        /// A collection child-nodes attached to this node.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.DisallowNull]
        public NodeCollection<TNode> Children { get; }

        #endregion
    }
}
