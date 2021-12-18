namespace Narumikazuchi.Collections;

/// <summary>
/// Represents a node inside a tree structure where nodes can hold contents.
/// </summary>
public interface IContentTreeNode<TNode, TValue, TContent> : ITreeNode<TNode, TValue> 
    where TNode : ITreeNode<TNode, TValue> 
    where TContent : class
{
    /// <summary>
    /// Gets a collection of items which are either attached to this <typeparamref name="TNode"/> or 
    /// are attached to any of its child-nodes (only when <see cref="IContentTree{TNode, TValue, TContent}.ParentsKnowChildItems"/> for the parent is set to <see langword="true"/>).
    /// </summary>
    [NotNull]
    public IEnumerable<TContent?> Items { get; }
}