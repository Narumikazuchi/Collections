namespace Narumikazuchi.Collections;

/// <summary>
/// Represents any tree data structure where the nodes contain other content besides their value.
/// </summary>
public interface IContentTree<TNode, TValue, TContent> : ITree<TNode, TValue> 
    where TNode : IContentTreeNode<TNode, TValue, TContent> 
    where TContent : class
{
    /// <summary>
    /// Gets or sets if a parent <typeparamref name="TNode"/> should have a reference to the items of it's child-nodes in it's own <see cref="IContentTreeNode{TNode, TValue, TContent}.Items"/>.
    /// </summary>
    [Pure]
    public Boolean ParentsKnowChildItems { get; set; }
}