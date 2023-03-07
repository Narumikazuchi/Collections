namespace Narumikazuchi.Collections;

public partial class BinaryNode<TValue>
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

    internal void SetParent(BinaryNode<TValue>? parent)
    {
        m_Parent = parent;
    }

    internal void SetLeftChild<TComparer>(BinaryNode<TValue>? node,
                                          TComparer comparer)
        where TComparer : IComparer<TValue>
    {
        if (m_Right is not null &&
            node is not null &&
            comparer.Compare(x: m_Right.Value,
                             y: node.Value) == 0)
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
            comparer.Compare(x: m_Left.Value,
                             y: node.Value) == 0)
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