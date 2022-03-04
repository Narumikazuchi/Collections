namespace Narumikazuchi.Collections;

/// <summary>
/// Represents a node of a <see cref="BinaryTree{TValue}"/>.
/// </summary>
[DebuggerDisplay("{Value}")]
public sealed partial class BinaryNode<TValue>
{
#pragma warning disable CS1591
    public static implicit operator TValue(BinaryNode<TValue> node)
    {
        return node.Value;
    }
#pragma warning restore

    /// <summary>
    /// Gets the left child <see cref="BinaryNode{TValue}"/>. Returns <see langword="null"/> if the <see cref="BinaryNode{TValue}"/> has no left sided child node.
    /// </summary>
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    [Pure]
    [MaybeNull]
    public BinaryNode<TValue>? LeftChild => 
        this.Left;
    /// <summary>
    /// Gets the right child <see cref="BinaryNode{TValue}"/>. Returns <see langword="null"/> if the <see cref="BinaryNode{TValue}"/> has no right sided child node.
    /// </summary>
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    [Pure]
    [MaybeNull]
    public BinaryNode<TValue>? RightChild => 
        this.Right;
}

// Non-Public
partial class BinaryNode<TValue>
{
    internal BinaryNode(in TValue value!!, 
                        BinaryNode<TValue>? parent)
    {
        m_Value = value;
        this.Parent = parent;
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
            min = node.LeftChild
                      .Value;
            node = node.LeftChild;
        }
        m_Value = min;
        return node!;
    }

    private static Boolean AreNodesEqual(BinaryNode<TValue> left!!, 
                                         BinaryNode<TValue> right!!) =>
        left.Value
            .CompareTo(right.Value) == 0;

    private static Int32 CompareNodes(BinaryNode<TValue>? left,
                                      BinaryNode<TValue>? right)
    {
        if (left is null)
        {
            if (right is null)
            {
                return 0;
            }
            return 1;
        }
        if (right is null)
        {
            return -1;
        }
        return left.Value
                   .CompareTo(right.Value);
    }

    internal BinaryNode<TValue>? Left
    {
        get => m_Left;
        set
        {
            m_Left = value;
            m_Children.Clear();
            if (m_Left is not null)
            {
                m_Children.Add(m_Left);
            }
            if (m_Right is not null)
            {
                m_Children.Add(m_Right);
            }
        }
    }
    internal BinaryNode<TValue>? Right
    {
        get => m_Right;
        set
        {
            m_Right = value;
            m_Children.Clear();
            if (m_Left is not null)
            {
                m_Children.Add(m_Left);
            }
            if (m_Right is not null)
            {
                m_Children.Add(m_Right);
            }
        }
    }

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private readonly NodeCollection<BinaryNode<TValue>, TValue> m_Children = new(equality: AreNodesEqual,
                                                                                 comparison: CompareNodes);
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private TValue m_Value;
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private BinaryNode<TValue>? m_Left = null;
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private BinaryNode<TValue>? m_Right = null;
}

// ITreeNode<T, U>
partial class BinaryNode<TValue> : ITreeNode<BinaryNode<TValue>, TValue> 
    where TValue : IComparable<TValue>
{
    /// <inheritdoc/>
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    [Pure]
    public TValue Value => 
        m_Value;
    /// <inheritdoc/>
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    [Pure]
    [MaybeNull]
    public BinaryNode<TValue>? Parent { get; }
    /// <inheritdoc/>
    [DebuggerBrowsable(DebuggerBrowsableState.Collapsed)]
    [Pure]
    public UInt32 Depth { get; }
    /// <inheritdoc/>
    [DebuggerBrowsable(DebuggerBrowsableState.Collapsed)]
    [Pure]
    public Boolean IsLeaf => 
        m_Children.Count == 0;

    /// <inheritdoc/>
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    [Pure]
    public NodeCollection<BinaryNode<TValue>, TValue> Children => 
        m_Children;
}