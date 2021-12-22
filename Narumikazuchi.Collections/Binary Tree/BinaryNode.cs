namespace Narumikazuchi.Collections;

/// <summary>
/// Represents a node of a <see cref="BinaryTree{TValue}"/>.
/// </summary>
[DebuggerDisplay("{Value}")]
public sealed partial class BinaryNode<TValue>
{
#pragma warning disable
    public static implicit operator TValue(BinaryNode<TValue> node) => 
        node.Value;
#pragma warning restore

    /// <summary>
    /// Gets the left child <see cref="BinaryNode{TValue}"/>. Returns <see langword="null"/> if the <see cref="BinaryNode{TValue}"/> has no left sided child node.
    /// </summary>
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    [Pure]
    [MaybeNull]
    public BinaryNode<TValue>? LeftChild => this.Left;
    /// <summary>
    /// Gets the right child <see cref="BinaryNode{TValue}"/>. Returns <see langword="null"/> if the <see cref="BinaryNode{TValue}"/> has no right sided child node.
    /// </summary>
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    [Pure]
    [MaybeNull]
    public BinaryNode<TValue>? RightChild => this.Right;
}

// Non-Public
partial class BinaryNode<TValue>
{
    internal BinaryNode(in TValue value, 
                        BinaryNode<TValue>? parent)
    {
        ExceptionHelpers.ThrowIfArgumentNull(value);

        this._value = value;
        this.Parent = parent;
        this.Depth = parent is null 
                        ? 0 
                        : parent.Depth + 1;
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
        this._value = min;
        return node!;
    }

    private static Boolean AreNodesEqual(BinaryNode<TValue> left, 
                                         BinaryNode<TValue> right) =>
        left.Value.CompareTo(other: right.Value) == 0;

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
        return left.Value.CompareTo(other: right.Value);
    }

    internal BinaryNode<TValue>? Left
    {
        get => this._left;
        set
        {
            this._left = value;
            this._children.Clear();
            if (this._left is not null)
            {
                this._children.Add(item: this._left);
            }
            if (this._right is not null)
            {
                this._children.Add(item: this._right);
            }
        }
    }
    internal BinaryNode<TValue>? Right
    {
        get => this._right;
        set
        {
            this._right = value;
            this._children.Clear();
            if (this._left is not null)
            {
                this._children.Add(item: this._left);
            }
            if (this._right is not null)
            {
                this._children.Add(item: this._right);
            }
        }
    }

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private readonly NodeCollection<BinaryNode<TValue>, TValue> _children = new(equality: AreNodesEqual,
                                                                                comparison: CompareNodes);
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private TValue _value;
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private BinaryNode<TValue>? _left = null;
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private BinaryNode<TValue>? _right = null;
}

// ITreeNode<T, U>
partial class BinaryNode<TValue> : ITreeNode<BinaryNode<TValue>, TValue> 
    where TValue : IComparable<TValue>
{
    /// <inheritdoc/>
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    [Pure]
    public TValue Value => 
        this._value;
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
        this._children.Count == 0;

    /// <inheritdoc/>
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    [Pure]
    public NodeCollection<BinaryNode<TValue>, TValue> Children => 
        this._children;
}