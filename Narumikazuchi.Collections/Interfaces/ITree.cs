namespace Narumikazuchi.Collections;

/// <summary>
/// Represents any tree data structure.
/// </summary>
public interface ITree<TNode, TValue> 
    where TNode : ITreeNode<TNode, TValue>
{
    /// <summary>
    /// Gets the root node of this tree structure.
    /// </summary>
    [NotNull]
    public TNode RootNode { get; }
}