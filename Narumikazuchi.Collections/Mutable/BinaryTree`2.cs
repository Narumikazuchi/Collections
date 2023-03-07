namespace Narumikazuchi.Collections;

/// <summary>
/// Represents a fast binary lookup data structure.
/// </summary>
[DebuggerDisplay("Depth = {GetDepth()}")]
public sealed partial class BinaryTree<TValue, TComparer> : StrongEnumerable<TValue, BinaryTree<TValue, TComparer>.Enumerator>
    where TComparer : IComparer<TValue>
{
    /// <summary>
    /// Instantiates a new <see cref="BinaryTree{TValue, TComparer}"/> with the <paramref name="root"/> as root node.
    /// </summary>
    /// <param name="root">The value of the root node.</param>
    /// <param name="comparer">The comparer that will be used to compare two <typeparamref name="TValue"/> instances.</param>
    /// <exception cref="ArgumentNullException"/>
    static public BinaryTree<TValue, TComparer> Create([DisallowNull] TValue root,
                                                       [DisallowNull] TComparer comparer)
    {
        return new(root: root,
                   comparer: comparer);
    }

    /// <inheritdoc/>
    public sealed override BinaryTree<TValue, TComparer>.Enumerator GetEnumerator()
    {
        return new(tree: this,
                   method: BinaryTraversalMethod.InOrder);
    }

    /// <summary>
    /// Finds the first <typeparamref name="TValue"/> matching the specified value.
    /// </summary>
    /// <param name="value">The value to lookup in the tree.</param>
    /// <returns>The <typeparamref name="TValue"/> or <see langword="null"/> if no node with such value exists in the tree.</returns>
    /// <exception cref="ArgumentNullException"/>
    [return: MaybeNull]
    public TValue? Find([DisallowNull] TValue value)
    {
        BinaryNode<TValue>? node = this.FindInternal(value);
        if (node is null)
        {
            return default;
        }
        else
        {
            return node.Value;
        }
    }

    /// <summary>
    /// Removes all <see cref="BinaryNode{TValue}"/> objects from the <see cref="BinaryTree{TValue, TComparer}"/>, only when their <typeparamref name="TValue"/> matches the specified condition.
    /// </summary>
    /// <param name="predicate">The condition that the <typeparamref name="TValue"/> needs to meet to be deleted.</param>
    /// <returns>The number of <see cref="BinaryNode{TValue}"/> objects that have been removed.</returns>
    /// <exception cref="ArgumentNullException"/>
    public Int32 RemoveAll([DisallowNull] Func<TValue, Boolean> predicate)
    {
#if NET6_0_OR_GREATER
        ArgumentNullException.ThrowIfNull(predicate);
#else
        if (predicate is null)
        {
            throw new ArgumentNullException(nameof(predicate));
        }
#endif

        List<TValue> remove = new();
        foreach (BinaryNode<TValue>? item in this.TraverseInOrder())
        {
            if (predicate.Invoke(item.Value))
            {
                remove.Add(item.Value);
            }
        }

        Int32 skipped = 0;
        for (Int32 index = 0;
             index < remove.Count;
             index++)
        {
            if (!this.Remove(remove[index]!))
            {
                ++skipped;
            }
        }

        return remove.Count - skipped;
    }

    /// <summary>
    /// Gets the depth of the <see cref="BinaryTree{TValue, TComparer}"/>.
    /// </summary>
    /// <returns>The depth of the deepest <see cref="BinaryNode{TValue}"/> in the tree.</returns>
    public UInt32 GetDepth()
    {
        return this.GetDepth(m_Root);
    }

    /// <summary>
    /// Determines the lowest value in the <see cref="BinaryTree{TValue, TComparer}"/>.
    /// </summary>
    /// <returns>The <typeparamref name="TValue"/> of the lowest element in the <see cref="BinaryTree{TValue, TComparer}"/>.</returns>
    [return: NotNull]
    public TValue Minimum()
    {
        BinaryNode<TValue> node = m_Root;
        while (node.LeftChild is not null)
        {
            node = node.LeftChild;
        }

        return node.Value;
    }

    /// <summary>
    /// Determines the highest value in the <see cref="BinaryTree{TValue, TComparer}"/>.
    /// </summary>
    /// <returns>The <typeparamref name="TValue"/> of the highest element in the <see cref="BinaryTree{TValue, TComparer}"/>.</returns>
    [return: NotNull]
    public TValue Maximum()
    {
        BinaryNode<TValue> node = m_Root;
        while (node.RightChild is not null)
        {
            node = node.RightChild;
        }

        return node.Value;
    }

    /// <summary>
    /// Returns an <see cref="IStrongEnumerable{TElement, TEnumerator}"/> containing the <typeparamref name="TValue"/> in the traversed order.
    /// </summary>
    /// <param name="traverseMethod">The method to use when traversing.</param>
    /// <returns>An <see cref="IStrongEnumerable{TElement, TEnumerator}"/> containing all <typeparamref name="TValue"/> in this <see cref="BinaryTree{TValue, TComparer}"/> in the order specified by the <paramref name="traverseMethod"/>.</returns>
    public Enumerator Traverse(BinaryTraversalMethod traverseMethod)
    {
        return new(tree: this,
                   method: traverseMethod);
    }

    /// <summary>
    /// Gets the root for the <see cref="BinaryTree{TValue, TComparer}"/>.
    /// </summary>
    [NotNull]
    public BinaryNode<TValue> RootNode
    {
        get
        {
            return m_Root;
        }
    }

    /// <summary>
    /// Gets or sets if an exception should be thrown when trying to add a duplicate <typeparamref name="TValue"/>.
    /// </summary>
    public Boolean ThrowExceptionOnDuplicate
    {
        get;
        set;
    } = true;
}