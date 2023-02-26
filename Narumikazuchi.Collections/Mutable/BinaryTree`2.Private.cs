namespace Narumikazuchi.Collections;

public partial class BinaryTree<TValue, TComparer>
{
    internal BinaryTree(TValue root,
                        TComparer comparer)
    {
        m_Root = new(value: root,
                     parent: null);
        this.Comparer = comparer;
    }

    private BinaryNode<TValue>? FindInternal(TValue value)
    {
        BinaryNode<TValue>? node = m_Root;
        TComparer comparer = this.Comparer;
        Int32 compare = comparer.Compare(x: value,
                                         y: node.Value);
        while (node is not null &&
               compare != 0)
        {
            if (compare < 0)
            {
                node = node.LeftChild;
            }
            else if (compare > 0)
            {
                node = node.RightChild;
            }

            if (node is not null)
            {
                compare = comparer.Compare(x: value,
                                           y: node.Value);
            }
        }

        return node;
    }
    private BinaryNode<TValue>? FindInternal<TEqualityComparer>(TValue value,
                                                                TEqualityComparer equalityComparer)
        where TEqualityComparer : IEqualityComparer<TValue>
    {
        foreach (BinaryNode<TValue> node in this.TraverseInOrder())
        {
            if (equalityComparer.Equals(value, node.Value))
            {
                return node;
            }
        }

        return null;
    }

    private UInt32 GetDepth(BinaryNode<TValue> node)
    {
        if (node.LeftChild.IsNull &&
            node.RightChild.IsNull)
        {
            return node.Depth;
        }
        else if (node.LeftChild.IsNull &&
                 !node.RightChild.IsNull)
        {
            return this.GetDepth(node.RightChild!);
        }
        else if (!node.LeftChild.IsNull &&
                 node.RightChild.IsNull)
        {
            return this.GetDepth(node.LeftChild!);
        }
        else
        {
            return Math.Max(val1: this.GetDepth(node.LeftChild!),
                            val2: this.GetDepth(node.RightChild!));
        }
    }

    private List<BinaryNode<TValue>> TraversePreOrder()
    {
        List<BinaryNode<TValue>> nodes = new();
        this.TraversePreOrder(nodes: nodes, 
                              current: m_Root);
        return nodes;
    }

    private void TraversePreOrder(List<BinaryNode<TValue>> nodes, 
                                  BinaryNode<TValue> current)
    {
        nodes.Add(current);
        if (!current.LeftChild.IsNull)
        {
            this.TraversePreOrder(nodes: nodes, 
                                  current: current.LeftChild!);
        }

        if (!current.RightChild.IsNull)
        {
            this.TraversePreOrder(nodes: nodes, 
                                  current: current.RightChild!);
        }
    }

    private List<BinaryNode<TValue>> TraverseInOrder()
    {
        List<BinaryNode<TValue>> nodes = new();
        this.TraverseInOrder(nodes: nodes, 
                             current: m_Root);
        return nodes;
    }

    private void TraverseInOrder(List<BinaryNode<TValue>> nodes, 
                                 BinaryNode<TValue> current)
    {
        if (!current.LeftChild.IsNull)
        {
            this.TraverseInOrder(nodes: nodes, 
                                 current: current.LeftChild!);
        }

        nodes.Add(current);
        if (!current.RightChild.IsNull)
        {
            this.TraverseInOrder(nodes: nodes, 
                                 current: current.RightChild!);
        }
    }

    private List<BinaryNode<TValue>> TraversePostOrder()
    {
        List<BinaryNode<TValue>> nodes = new();
        this.TraversePostOrder(nodes: nodes, 
                               current: m_Root);
        return nodes;
    }

    private void TraversePostOrder(List<BinaryNode<TValue>> nodes, 
                                   BinaryNode<TValue> current)
    {
        if (!current.LeftChild.IsNull)
        {
            this.TraverseInOrder(nodes: nodes, 
                                 current: current.LeftChild!);
        }

        if (!current.RightChild.IsNull)
        {
            this.TraverseInOrder(nodes: nodes, 
                                 current: current.RightChild!);
        }

        nodes.Add(current);
    }

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private readonly BinaryNode<TValue> m_Root;
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private Int32 m_Count;

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private const String CANNOT_CREATE_FROM_EMPTY_COLLECTION = "Cannot create BinaryTree from empty IEnumerable.";
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private const String NO_PARENT = "Only the root node can have no parent.";
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private const String ALREADY_EXISTS = "A node with the specified value already exists.";
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private const String CANNOT_REMOVE_ROOT = "The root of a tree cannot be removed.";
}