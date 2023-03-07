namespace Narumikazuchi.Collections;

/// <summary>
/// Represents a node of a <see cref="BinaryTree{TValue, TComparer}"/>.
/// </summary>
[DebuggerDisplay("{Value}")]
public sealed partial class BinaryNode<TValue>
{
#pragma warning disable CS1591
    static public implicit operator TValue(BinaryNode<TValue> node)
    {
        return node.Value;
    }
#pragma warning restore

    /// <summary>
    /// Gets the <typeparamref name="TValue"/> value of this <see cref="BinaryNode{TValue}"/>.
    /// </summary>
    [NotNull]
    public TValue Value
    {
        get
        {
            return m_Value!;
        }
    }

    /// <summary>
    /// Gets the parent of the current <see cref="BinaryNode{TValue}"/>. Should return <see langword="null"/> for root nodes.
    /// </summary>
    [MaybeNull]
    public BinaryNode<TValue>? Parent
    {
        get
        {
            return m_Parent;
        }
    }

    /// <summary>
    /// Gets the left child <see cref="BinaryNode{TValue}"/>. Returns <see langword="null"/> if the <see cref="BinaryNode{TValue}"/> has no left sided child node.
    /// </summary>
    [MaybeNull]
    public BinaryNode<TValue>? LeftChild
    {
        get
        {
            return m_Left;
        }
    }

    /// <summary>
    /// Gets the right child <see cref="BinaryNode{TValue}"/>. Returns <see langword="null"/> if the <see cref="BinaryNode{TValue}"/> has no right sided child node.
    /// </summary>
    [MaybeNull]
    public BinaryNode<TValue>? RightChild
    {
        get
        {
            return m_Right;
        }
    }

    /// <summary>
    /// Gets the depth of this node in it's corresponding <see cref="BinaryTree{TValue, TComparer}"/>. Should be 0 for root nodes.
    /// </summary>
    public UInt32 Depth { get; }

    /// <summary>
    /// Gets whether this <see cref="BinaryNode{TValue}"/> has no more child-nodes.
    /// </summary>
    public Boolean IsLeaf
    {
        get
        {
            return m_Left is null &&
                   m_Right is null;
        }
    }
}