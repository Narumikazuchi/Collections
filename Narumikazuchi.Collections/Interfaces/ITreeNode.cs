namespace Narumikazuchi.Collections;

/// <summary>
/// Represents a node inside any tree structure.
/// </summary>
public interface ITreeNode<TNode, TValue> 
    where TNode : ITreeNode<TNode, TValue>
{
    /// <summary>
    /// Gets the <typeparamref name="TValue"/> value of this <typeparamref name="TNode"/>.
    /// </summary>
    [Pure]
    public TValue Value { get; }
    /// <summary>
    /// Gets the parent of the current node. Should return <see langword="null"/> for root nodes.
    /// </summary>
    [Pure]
    [MaybeNull]
    public TNode Parent { get; }
    /// <summary>
    /// Gets the depth of this node in it's corresponding tree. Should be 0 for root nodes.
    /// </summary>
    [Pure]
    public UInt32 Depth { get; }
    /// <summary>
    /// Gets whether this <typeparamref name="TNode"/> has no more child-nodes.
    /// </summary>
    [Pure]
    public Boolean IsLeaf { get; }
    /// <summary>
    /// A collection of child-nodes attached to this node.
    /// </summary>
    [NotNull]
    public NodeCollection<TNode, TValue> Children { get; }
}