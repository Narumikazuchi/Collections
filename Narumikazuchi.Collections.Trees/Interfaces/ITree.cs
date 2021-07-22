namespace Narumikazuchi.Collections.Trees
{
    /// <summary>
    /// Represents any tree data structure.
    /// </summary>
    public interface ITree<TNode> where TNode : ITreeNode<TNode>
    {
        #region Properties

        /// <summary>
        /// The root node of this tree structure.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.DisallowNull]
        public TNode RootNode { get; }

        #endregion
    }
}
