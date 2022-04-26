namespace Narumikazuchi.Collections;

/// <summary>
/// Represents a fast binary lookup data structure.
/// </summary>
[DebuggerDisplay("Depth = {GetDepth()}")]
public sealed partial class BinaryTree<TValue>
    where TValue : IComparable<TValue>
{
    /// <summary>
    /// Instantiates a new <see cref="BinaryTree{TValue}"/> with the <paramref name="rootValue"/> as root node.
    /// </summary>
    /// <param name="rootValue">The value of the root node.</param>
    /// <exception cref="ArgumentNullException"/>
    public BinaryTree([DisallowNull] TValue rootValue)
    {
        ArgumentNullException.ThrowIfNull(rootValue);

        m_Root = new BinaryNode<TValue>(value: rootValue,
                                        parent: null);
    }

    /// <summary>
    /// Determines if the specified value exists in the <see cref="BinaryTree{TValue}"/>.
    /// </summary>
    /// <param name="value">The value to lookup.</param>
    /// <returns><see langword="true"/> if the specified value is found in the tree; otherwise, <see langword="false"/></returns>
    /// <exception cref="ArgumentNullException"/>
    [Pure]
    public Boolean Exists([DisallowNull] TValue value)
    {
        ArgumentNullException.ThrowIfNull(value);

        return this.Find(value) is not null;
    }

    /// <summary>
    /// Finds the <see cref="BinaryNode{TValue}"/> with the highest depth matching the specified value.
    /// </summary>
    /// <param name="value">The value to lookup in the tree.</param>
    /// <returns>The <see cref="BinaryNode{TValue}"/> which contains the specified value or <see langword="null"/> if no node with such value exists in the tree.</returns>
    /// <exception cref="ArgumentNullException"/>
    [Pure]
    [return: MaybeNull]
    public BinaryNode<TValue>? Find([DisallowNull] TValue value)
    {
        ArgumentNullException.ThrowIfNull(value);

        BinaryNode<TValue>? node = m_Root;
        Int32 compare = value.CompareTo(node.Value);
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
                compare = value.CompareTo(node.Value);
            }
        }
        return node;
    }

    /// <summary>
    /// Adds an object to the <see cref="BinaryTree{TValue}"/>.
    /// </summary>
    /// <param name="value">The value to be added to the <see cref="BinaryTree{TValue}"/>.</param>
    /// <returns><see langword="true"/> if the item was added; otherwise, <see langword="false"/></returns>
    /// <exception cref="ArgumentNullException"/>
    public Boolean Add([DisallowNull] TValue value)
    {
        ArgumentNullException.ThrowIfNull(value);

        BinaryNode<TValue>? node = m_Root;
        Int32 compare = value.CompareTo(node.Value);
        while (node is not null)
        {
            if (compare == 0)
            {
                if (this.ThrowExceptionOnDuplicate)
                {
                    ArgumentException exception = new(message: ALREADY_EXISTS);
                    exception.Data
                             .Add(key: "Duplicate Value",
                                  value: value);
                    throw exception;
                }
                return false;
            }
            else if (compare < 0)
            {
                if (node.LeftChild is null)
                {
                    node.SetLeftChild(new(value: value,
                                          parent: node));
                    return true;
                }
                node = node.LeftChild;
            }
            else if (compare > 0)
            {
                if (node.RightChild is null)
                {
                    node.SetRightChild(new(value: value,
                                           parent: node));
                    return true;
                }
                node = node.RightChild;
            }
            if (node is not null)
            {
                compare = value.CompareTo(node.Value);
            }
        }
        return false;
    }

    /// <summary>
    /// Adds the elements of the specified collection to the <see cref="BinaryTree{TValue}"/>.
    /// </summary>
    /// <param name="collection">The collection of items to add.</param>
    /// <exception cref="ArgumentNullException"/>
    public void AddRange([DisallowNull] IEnumerable<TValue> collection)
    {
        ArgumentNullException.ThrowIfNull(collection);

        foreach (TValue item in collection)
        {
            this.Add(item);
        }
    }

    /// <summary>
    /// Removes all elements from the <see cref="BinaryTree{TValue}"/>.
    /// </summary>
    public void Clear()
    {
        m_Root.SetLeftChild(null);
        m_Root.SetRightChild(null);
    }

    /// <summary>
    /// Removes the first occurrence of the specified item from the <see cref="BinaryTree{TValue}"/>.
    /// </summary>
    /// <param name="value">The item to remove.</param>
    /// <returns><see langword="true"/> if the item was found and removed; otherwise, <see langword="false"/></returns>
    /// <exception cref="ArgumentNullException"/>
    public Boolean Remove([DisallowNull] TValue value)
    {
        ArgumentNullException.ThrowIfNull(value);

        if (value.CompareTo(m_Root.Value) == 0)
        {
            throw new ArgumentException(message: CANNOT_REMOVE_ROOT);
        }

        BinaryNode<TValue>? node = this.Find(value);
        if (node is null)
        {
            return false;
        }

        if (node.LeftChild is null)
        {
            if (node.Parent is null)
            {
                throw new NotAllowed(message: NO_PARENT);
            }
            if (node.Parent
                    .LeftChild == node)
            {
                node.Parent
                    .SetLeftChild(node.RightChild);
            }
            else if (node.Parent
                         .RightChild == node)
            {
                node.Parent
                    .SetRightChild(node.RightChild);
            }
        }
        else if (node.RightChild is null)
        {
            if (node.Parent is null)
            {
                throw new NotAllowed(message: NO_PARENT);
            }
            if (node.Parent
                    .LeftChild == node)
            {
                node.Parent
                    .SetLeftChild(node.LeftChild);
            }
            else if (node.Parent
                         .RightChild == node)
            {
                node.Parent
                    .SetRightChild(node.LeftChild);
            }
        }
        else
        {
            BinaryNode<TValue> min = node.SetToMinBranchValue();
            if (min.Parent is null)
            {
                throw new NotAllowed(message: NO_PARENT);
            }
            min.Parent
               .SetLeftChild(null);
        }

        return true;
    }

    /// <summary>
    /// Removes all objects from the <see cref="BinaryTree{TValue}"/> that match the specified condition.
    /// </summary>
    /// <param name="predicate">The condition that objects need to meet to be deleted.</param>
    /// <returns>The amount of items removed</returns>
    /// <exception cref="ArgumentNullException"/>
    public Int32 RemoveAll([DisallowNull] Func<TValue, Boolean> predicate)
    {
        ArgumentNullException.ThrowIfNull(predicate);

        Collection<TValue> remove = new();
        foreach (BinaryNode<TValue>? item in this.TraverseInOrder())
        {
            if (predicate.Invoke(item.Value))
            {
                remove.Add(item.Value);
            }
        }

        Int32 skipped = 0;
        for (Int32 i = 0;
             i < remove.Count;
             i++)
        {
            if (!this.Remove(remove[i]))
            {
                ++skipped;
            }
        }
        return remove.Count - skipped;
    }

    /// <summary>
    /// Gets the depth of the <see cref="BinaryTree{TValue}"/>.
    /// </summary>
    /// <returns>The depth of the deepest node in the tree</returns>
    public UInt32 GetDepth() => 
        this.GetDepth(node: m_Root);

    /// <summary>
    /// Determines the lowest value in the <see cref="BinaryTree{TValue}"/>.
    /// </summary>
    [Pure]
    public TValue LowBound()
    {
        BinaryNode<TValue> node = m_Root;
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
        BinaryNode<TValue> node = m_Root;
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
    public IEnumerable<BinaryNode<TValue>> Traverse(in BinaryTraversalMethod method) => method switch
    {
        BinaryTraversalMethod.PreOrder => this.TraversePreOrder(),
        BinaryTraversalMethod.InOrder => this.TraverseInOrder(),
        BinaryTraversalMethod.PostOrder => this.TraversePostOrder(),
        _ => Array.Empty<BinaryNode<TValue>>(),
    };

    /// <summary>
    /// Gets the root for the <see cref="BinaryTree{TValue}"/>.
    /// </summary>
    [NotNull]
    [Pure]
    public BinaryNode<TValue> RootNode =>
        m_Root;

    /// <summary>
    /// Gets or sets if an exception should be thrown when trying to add a duplicate.
    /// </summary>
    public Boolean ThrowExceptionOnDuplicate { get; set; } = true;
}

// Non-Public
partial class BinaryTree<TValue>
{
    internal BinaryTree(IEnumerable<TValue> collection)
    {
        ArgumentNullException.ThrowIfNull(collection);

        if (!collection.Any())
        {
            throw new ArgumentException(message: CANNOT_CREATE_FROM_EMPTY_COLLECTION);
        }

        IOrderedEnumerable<TValue> distinct = collection.Distinct()
                                                        .OrderBy(i => i);
        TValue median = distinct.Median()!;
        m_Root = new(value: median, 
                     parent: null);
        foreach (TValue item in distinct)
        {
            if (item.CompareTo(median) == 0)
            {
                continue;
            }
            this.Add(item);
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
                              current: m_Root);
        return nodes;
    }

    private void TraversePreOrder(IList<BinaryNode<TValue>> nodes, 
                                  BinaryNode<TValue> current)
    {
        nodes.Add(current);
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
                             current: m_Root);
        return nodes;
    }

    private void TraverseInOrder(IList<BinaryNode<TValue>> nodes, 
                                 BinaryNode<TValue> current)
    {
        if (current.LeftChild is not null)
        {
            this.TraverseInOrder(nodes: nodes, 
                                 current: current.LeftChild);
        }
        nodes.Add(current);
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
                               current: m_Root);
        return nodes;
    }

    private void TraversePostOrder(IList<BinaryNode<TValue>> nodes, 
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
        nodes.Add(current);
    }

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private readonly BinaryNode<TValue> m_Root;

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private const String CANNOT_CREATE_FROM_EMPTY_COLLECTION = "Cannot create BinaryTree from empty IEnumerable.";
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private const String NO_PARENT = "Only the root node can have no parent.";
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private const String ALREADY_EXISTS = "A node with the specified value already exists.";
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private const String CANNOT_REMOVE_ROOT = "The root of a tree cannot be removed.";
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