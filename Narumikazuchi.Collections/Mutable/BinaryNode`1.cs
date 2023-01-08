namespace Narumikazuchi.Collections;

/// <summary>
/// Represents a node of a <see cref="BinaryTree{TValue, TComparer}"/>.
/// </summary>
[DebuggerDisplay("{Value}")]
public sealed partial class BinaryNode<TValue>
    where TValue : notnull
{
#pragma warning disable CS1591
    public static implicit operator TValue(BinaryNode<TValue> node)
    {
        return node.Value;
    }
#pragma warning restore

    /// <summary>
    /// Gets the <typeparamref name="TValue"/> value of this <see cref="BinaryNode{TValue}"/>.
    /// </summary>
    [Pure]
#if NETCOREAPP3_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
    [NotNull]
#endif
    public TValue Value =>
        m_Value;

    /// <summary>
    /// Gets the parent of the current <see cref="BinaryNode{TValue}"/>. Should return <see langword="null"/> for root nodes.
    /// </summary>
    [Pure]
#if NETCOREAPP3_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
    [MaybeNull]
#endif
    public BinaryNode<TValue>? Parent =>
        m_Parent;

    /// <summary>
    /// Gets the left child <see cref="BinaryNode{TValue}"/>. Returns <see langword="null"/> if the <see cref="BinaryNode{TValue}"/> has no left sided child node.
    /// </summary>
    [Pure]
#if NETCOREAPP3_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
    [MaybeNull]
#endif
    public BinaryNode<TValue>? LeftChild =>
        m_Left;

    /// <summary>
    /// Gets the right child <see cref="BinaryNode{TValue}"/>. Returns <see langword="null"/> if the <see cref="BinaryNode{TValue}"/> has no right sided child node.
    /// </summary>
    [Pure]
#if NETCOREAPP3_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
    [MaybeNull]
#endif
    public BinaryNode<TValue>? RightChild =>
        m_Right;

    /// <summary>
    /// Gets the depth of this node in it's corresponding <see cref="BinaryTree{TValue, TComparer}"/>. Should be 0 for root nodes.
    /// </summary>
    [Pure]
    public UInt32 Depth { get; }

    /// <summary>
    /// Gets whether this <see cref="BinaryNode{TValue}"/> has no more child-nodes.
    /// </summary>
    [Pure]
    public Boolean IsLeaf =>
        m_Left is null &&
        m_Right is null;
}

// Non-Public
partial class BinaryNode<TValue>
{
    internal BinaryNode(TValue value, 
                        BinaryNode<TValue>? parent)
    {
        m_Value = value;
        m_Parent = parent;
        if (parent is null)
        {
            this.Depth = 0;
            return;
        }
        this.Depth = parent.Depth + 1;
    }

    internal BinaryNode<TValue> SetToMinBranchValue()
    {
        TValue min = this.Value;
        BinaryNode<TValue>? node = this.RightChild;
        while (node is not null &&
               node.LeftChild is not null)
        {
            min = node.LeftChild.Value;
            node = node.LeftChild;
        }
        m_Value = min;
        return node!;
    }

    internal void SetParent(BinaryNode<TValue>? parent) => 
        m_Parent = parent;

    internal void SetLeftChild<TComparer>(BinaryNode<TValue>? node,
                                          TComparer comparer)
        where TComparer : IComparer<TValue>
    {
        if (m_Right is not null &&
            node is not null &&
            m_Right.Value is not null &&
            comparer.Compare(m_Right.Value, node.Value) == 0)
        {
            return;
        }

        m_Left = node;
    }

    internal void SetRightChild<TComparer>(BinaryNode<TValue>? node,
                                           TComparer comparer)
        where TComparer : IComparer<TValue>
    {
        if (m_Left is not null &&
            node is not null &&
            m_Left.Value is not null &&
            comparer.Compare(m_Left.Value, node.Value) == 0)
        {
            return;
        }

        m_Right = node;
    }

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private TValue m_Value;
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private BinaryNode<TValue>? m_Parent;
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private BinaryNode<TValue>? m_Left = null;
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private BinaryNode<TValue>? m_Right = null;
}