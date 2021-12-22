namespace Narumikazuchi.Collections;

/// <summary>
/// Represents a fast binary lookup data structure.
/// </summary>
[DebuggerDisplay("Depth = {GetDepth()}")]
public sealed partial class BinaryTree<TValue>
{
    /// <summary>
    /// Instantiates a new <see cref="BinaryTree{TValue}"/> with the <paramref name="rootValue"/> as root node.
    /// </summary>
    /// <param name="rootValue">The value of the root node.</param>
    public BinaryTree([DisallowNull] TValue rootValue) => 
        this._root = new BinaryNode<TValue>(value: rootValue, 
                                            parent: null);

    /// <summary>
    /// Determines if the specified value exists in the <see cref="BinaryTree{TValue}"/>.
    /// </summary>
    /// <param name="value">The value to lookup.</param>
    /// <returns><see langword="true"/> if the specified value is found in the tree; otherwise, <see langword="false"/></returns>
    [Pure]
    public Boolean Exists([DisallowNull] TValue value) =>
        this.Find(value: value) is not null;

    /// <summary>
    /// Finds the <see cref="BinaryNode{TValue}"/> with the highest depth matching the specified value.
    /// </summary>
    /// <param name="value">The value to lookup in the tree.</param>
    /// <returns>The <see cref="BinaryNode{TValue}"/> which contains the specified value or <see langword="null"/> if no node with such value exists in the tree.</returns>
    [Pure]
    [return: MaybeNull]
    public BinaryNode<TValue>? Find([DisallowNull] TValue value)
    {
        ExceptionHelpers.ThrowIfArgumentNull(value);

        BinaryNode<TValue>? node = this._root;
        Int32 compare = value.CompareTo(other: node.Value);
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
                compare = value.CompareTo(other: node.Value);
            }
        }
        return node;
    }

    /// <summary>
    /// Gets the depth of the <see cref="BinaryTree{TValue}"/>.
    /// </summary>
    /// <returns>The depth of the deepest node in the tree</returns>
    public UInt32 GetDepth() => 
        this.GetDepth(node: this._root);

    /// <summary>
    /// Determines the lowest value in the <see cref="BinaryTree{TValue}"/>.
    /// </summary>
    [Pure]
    public TValue LowBound()
    {
        BinaryNode<TValue> node = this._root;
        while (node.LeftChild is not null)
        {
            node = node.LeftChild;
        }
        return node.Value;
    }

    /// <summary>
    /// Determines the highest value in the <see cref="BinaryTree{TValue}"/>.
    /// </summary>
    [Pure]
    public TValue HighBound()
    {
        BinaryNode<TValue> node = this._root;
        while (node.RightChild is not null)
        {
            node = node.RightChild;
        }
        return node.Value;
    }

    /// <summary>
    /// Returns an <see cref="IEnumerable{T}"/> containing the traversed <see cref="BinaryTree{TValue}"/> in the traversed order.
    /// </summary>
    /// <param name="method">The method to use when traversing.</param>
    [Pure]
    public IEnumerable<BinaryNode<TValue>> Traverse(BinaryTraversalMethod method) => method switch
    {
        BinaryTraversalMethod.PreOrder => this.TraversePreOrder(),
        BinaryTraversalMethod.InOrder => this.TraverseInOrder(),
        BinaryTraversalMethod.PostOrder => this.TraversePostOrder(),
        _ => Array.Empty<BinaryNode<TValue>>(),
    };

    /// <summary>
    /// Gets or sets if an exception should be thrown when trying to add a duplicate.
    /// </summary>
    [DebuggerBrowsable(DebuggerBrowsableState.Collapsed)]
    public Boolean ThrowExceptionOnDuplicate { get; set; } = true;
}

// Non-Public
partial class BinaryTree<TValue>
{
    internal BinaryTree(IEnumerable<TValue> collection)
    {
        ExceptionHelpers.ThrowIfArgumentNull(collection);
        if (!collection.Any())
        {
            throw new ArgumentException(message: CANNOT_CREATE_FROM_EMPTY_COLLECTION);
        }

        IOrderedEnumerable<TValue> distinct = collection.Distinct()
                                                        .OrderBy(i => i);
        TValue median = distinct.Median()!;
        this._root = new(value: median, 
                         parent: null);
        foreach (TValue item in distinct)
        {
            if (item.CompareTo(other: median) == 0)
            {
                continue;
            }
            this.Add(value: item);
        }
    }

    private UInt32 GetDepth(BinaryNode<TValue> node)
    {
        if (node.LeftChild is null &&
            node.RightChild is null)
        {
            return node.Depth;
        }
        if (node.LeftChild is null &&
            node.RightChild is not null)
        {
            return this.GetDepth(node: node.RightChild);
        }
        if (node.LeftChild is not null &&
            node.RightChild is null)
        {
            return this.GetDepth(node: node.LeftChild);
        }
        return Math.Max(val1: this.GetDepth(node: node.LeftChild!),
                        val2: this.GetDepth(node: node.RightChild!));
    }

    private IEnumerable<BinaryNode<TValue>> TraversePreOrder()
    {
        List<BinaryNode<TValue>> nodes = new();
        this.TraversePreOrder(nodes: nodes, 
                              current: this._root);
        return nodes;
    }

    private void TraversePreOrder(List<BinaryNode<TValue>> nodes, 
                                  BinaryNode<TValue> current)
    {
        nodes.Add(item: current);
        if (current.LeftChild is not null)
        {
            this.TraversePreOrder(nodes: nodes, 
                                  current: current.LeftChild);
        }
        if (current.RightChild is not null)
        {
            this.TraversePreOrder(nodes: nodes, 
                                  current: current.RightChild);
        }
    }

    private IEnumerable<BinaryNode<TValue>> TraverseInOrder()
    {
        List<BinaryNode<TValue>> nodes = new();
        this.TraverseInOrder(nodes: nodes, 
                             current: this._root);
        return nodes;
    }

    private void TraverseInOrder(List<BinaryNode<TValue>> nodes, 
                                 BinaryNode<TValue> current)
    {
        if (current.LeftChild is not null)
        {
            this.TraverseInOrder(nodes: nodes, 
                                 current: current.LeftChild);
        }
        nodes.Add(item: current);
        if (current.RightChild is not null)
        {
            this.TraverseInOrder(nodes: nodes, 
                                 current: current.RightChild);
        }
    }

    private IEnumerable<BinaryNode<TValue>> TraversePostOrder()
    {
        List<BinaryNode<TValue>> nodes = new();
        this.TraversePostOrder(nodes: nodes, 
                               current: this._root);
        return nodes;
    }

    private void TraversePostOrder(List<BinaryNode<TValue>> nodes, 
                                   BinaryNode<TValue> current)
    {
        if (current.LeftChild is not null)
        {
            this.TraverseInOrder(nodes: nodes, 
                                 current: current.LeftChild);
        }
        if (current.RightChild is not null)
        {
            this.TraverseInOrder(nodes: nodes, 
                                 current: current.RightChild);
        }
        nodes.Add(item: current);
    }

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private BinaryNode<TValue> _root;

#pragma warning disable
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private const String CANNOT_CREATE_FROM_EMPTY_COLLECTION = "Cannot create BinaryTree from empty IEnumerable.";
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private const String NO_PARENT = "Only the root node can have no parent.";
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private const String ALREADY_EXISTS = "A node with the specified value already exists.";
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private const String CANNOT_REMOVE_ROOT = "The root of a tree cannot be removed.";
#pragma warning restore
}

// IContentAddable<T>
partial class BinaryTree<TValue> : IContentAddable<TValue>
{
    /// <inheritdoc/>
    public Boolean Add([DisallowNull] TValue value)
    {
        ExceptionHelpers.ThrowIfArgumentNull(value);

        BinaryNode<TValue>? node = this._root;
        Int32 compare = value.CompareTo(other: node.Value);
        while (node is not null)
        {
            if (compare == 0)
            {
                return this.ThrowExceptionOnDuplicate
                                ? throw new ArgumentException(message: ALREADY_EXISTS)
                                : false;
            }
            else if (compare < 0)
            {
                if (node.LeftChild is null)
                {
                    node.Left = new(value: value,
                                    parent: node);
                    return true;
                }
                node = node.LeftChild;
            }
            else if (compare > 0)
            {
                if (node.RightChild is null)
                {
                    node.Right = new(value: value,
                                     parent: node);
                    return true;
                }
                node = node.RightChild;
            }
            if (node is not null)
            {
                compare = value.CompareTo(other: node.Value);
            }
        }
        return false;
    }

    /// <inheritdoc/>
    public void AddRange([DisallowNull] IEnumerable<TValue> collection)
    {
        foreach (TValue item in collection)
        {
            this.Add(item);
        }
    }
}

// IContentClearable
partial class BinaryTree<TValue> : IContentClearable
{
    /// <inheritdoc/>
    public void Clear()
    {
        this._root.Left = null;
        this._root.Right = null;
    }
}

// IContentRemovable
partial class BinaryTree<TValue> : IContentRemovable
{
    Boolean IContentRemovable.Remove(Object item) =>
        item is TValue value &&
        this.Remove(value: value);
}

// IContentRemovable<T>
partial class BinaryTree<TValue> : IContentRemovable<TValue>
{
    /// <inheritdoc/>
    public Boolean Remove([DisallowNull] TValue value)
    {
        ExceptionHelpers.ThrowIfArgumentNull(value);
        if (value.CompareTo(other: this._root.Value) == 0)
        {
            throw new ArgumentException(message: CANNOT_REMOVE_ROOT);
        }

        BinaryNode<TValue>? node = this.Find(value: value);
        if (node is null)
        {
            return false;
        }

        if (node.LeftChild is null)
        {
            if (node.Parent is null)
            {
                throw new NotAllowed(auxMessage: NO_PARENT);
            }
            if (node.Parent.LeftChild == node)
            {
                node.Parent.Left = node.RightChild;
            }
            else if (node.Parent.RightChild == node)
            {
                node.Parent.Right = node.RightChild;
            }
        }
        else if (node.RightChild is null)
        {
            if (node.Parent is null)
            {
                throw new NotAllowed(auxMessage: NO_PARENT);
            }
            if (node.Parent.LeftChild == node)
            {
                node.Parent.Left = node.LeftChild;
            }
            else if (node.Parent.RightChild == node)
            {
                node.Parent.Right = node.LeftChild;
            }
        }
        else
        {
            BinaryNode<TValue> min = node.SetToMinBranchValue();
            if (min.Parent is null)
            {
                throw new NotAllowed(auxMessage: NO_PARENT);
            }
            min.Parent.Left = null;
        }

        return true;
    }

    /// <inheritdoc/>
    public Int32 RemoveAll([DisallowNull] Func<TValue, Boolean> predicate)
    {
        ExceptionHelpers.ThrowIfArgumentNull(predicate);

        Collection<TValue> remove = new();
        foreach (BinaryNode<TValue>? item in this.TraverseInOrder())
        {
            if (predicate.Invoke(arg: item.Value))
            {
                remove.Add(item: item.Value);
            }
        }

        for (Int32 i = 0; i < remove.Count; i++)
        {
            this.Remove(value: remove[i]);
        }
        return remove.Count;
    }
}

// IEnumerable
partial class BinaryTree<TValue> : IEnumerable<TValue>
{
    IEnumerator IEnumerable.GetEnumerator() => 
        this.GetEnumerator();
}

// IEnumerable<T>
partial class BinaryTree<TValue> : IEnumerable<TValue>
{
    /// <inheritdoc/>
    public IEnumerator<TValue> GetEnumerator()
    {
        foreach (BinaryNode<TValue> node in this.TraverseInOrder())
        {
            yield return node.Value;
        }
        yield break;
    }
}

// ITree<T, U>
partial class BinaryTree<TValue> : ITree<BinaryNode<TValue>, TValue> 
    where TValue : IComparable<TValue>
{
    /// <summary>
    /// Gets the root for the <see cref="BinaryTree{TValue}"/>.
    /// </summary>
    [DebuggerBrowsable(DebuggerBrowsableState.Collapsed)]
    [NotNull]
    [Pure]
    public BinaryNode<TValue> RootNode => this._root;
}