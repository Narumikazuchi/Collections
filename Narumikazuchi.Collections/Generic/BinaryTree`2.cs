namespace Narumikazuchi.Collections;

/// <summary>
/// Represents a fast binary lookup data structure.
/// </summary>
[DebuggerDisplay("Depth = {GetDepth()}")]
public sealed partial class BinaryTree<TValue, TComparer>
    where TValue : notnull
    where TComparer : IComparer<TValue>
{
    /// <summary>
    /// Adds the elements of the specified <typeparamref name="TEnumerable"/> to the <see cref="BinaryTree{TValue, TComparer}"/>.
    /// </summary>
    /// <param name="enumerable">The collection of items to add.</param>
    /// <exception cref="ArgumentNullException"/>
    public void AddRange<TEnumerable>(
#if NETCOREAPP3_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        [DisallowNull]
#endif
        TEnumerable enumerable)
            where TEnumerable : IEnumerable<TValue>
    {
#if NET6_0_OR_GREATER
        ArgumentNullException.ThrowIfNull(enumerable);
#else
        if (enumerable is null)
        {
            throw new ArgumentNullException(nameof(enumerable));
        }
#endif

        foreach (TValue value in enumerable)
        {
            this.Add(value);
        }
    }

    /// <summary>
    /// Finds the first <see cref="BinaryNode{TValue}"/> matching the specified value.
    /// </summary>
    /// <param name="value">The value to lookup in the tree.</param>
    /// <returns>The <see cref="BinaryNode{TValue}"/> which contains the specified value or <see langword="null"/> if no node with such value exists in the tree.</returns>
    /// <exception cref="ArgumentNullException"/>
    [Pure]
#if NETCOREAPP3_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
    [return: MaybeNull]
#endif
    public BinaryNode<TValue>? Find(
#if NETCOREAPP3_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        [DisallowNull]
#endif
        TValue value)
    {
#if NET6_0_OR_GREATER
        ArgumentNullException.ThrowIfNull(value);
#else
        if (value is null)
        {
            throw new ArgumentNullException(nameof(value));
        }
#endif

        BinaryNode<TValue>? node = m_Root;
        Int32 compare = this.Comparer.Compare(value, node.Value);
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
                compare = this.Comparer.Compare(value, node.Value);
            }
        }
        return node;
    }

    /// <summary>
    /// Removes all <see cref="BinaryNode{TValue}"/> objects from the <see cref="BinaryTree{TValue, TComparer}"/>, only when their <typeparamref name="TValue"/> matches the specified condition.
    /// </summary>
    /// <param name="predicate">The condition that the <typeparamref name="TValue"/> needs to meet to be deleted.</param>
    /// <returns>The number of <see cref="BinaryNode{TValue}"/> objects that have been removed.</returns>
    /// <exception cref="ArgumentNullException"/>
    public Int32 RemoveAll(
#if NETCOREAPP3_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        [DisallowNull]
#endif
        Func<TValue, Boolean> predicate)
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
    /// Gets the depth of the <see cref="BinaryTree{TValue, TComparer}"/>.
    /// </summary>
    /// <returns>The depth of the deepest <see cref="BinaryNode{TValue}"/> in the tree.</returns>
    [Pure]
    public UInt32 GetDepth() => 
        this.GetDepth(node: m_Root);

    /// <summary>
    /// Determines the lowest value in the <see cref="BinaryTree{TValue, TComparer}"/>.
    /// </summary>
    /// <returns>The <typeparamref name="TValue"/> of the lowest element in the <see cref="BinaryTree{TValue, TComparer}"/>.</returns>
    [Pure]
#if NETCOREAPP3_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
    [return: NotNull]
#endif
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
    /// Determines the highest value in the <see cref="BinaryTree{TValue, TComparer}"/>.
    /// </summary>
    /// <returns>The <typeparamref name="TValue"/> of the highest element in the <see cref="BinaryTree{TValue, TComparer}"/>.</returns>
    [Pure]
#if NETCOREAPP3_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
    [return: NotNull]
#endif
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
    /// Returns an <see cref="IStrongEnumerable{TElement, TEnumerator}"/> containing the <typeparamref name="TValue"/> in the traversed order.
    /// </summary>
    /// <param name="traverseMethod">The method to use when traversing.</param>
    /// <returns>An <see cref="IStrongEnumerable{TElement, TEnumerator}"/> containing all <typeparamref name="TValue"/> in this <see cref="BinaryTree{TValue, TComparer}"/> in the order specified by the <paramref name="traverseMethod"/>.</returns>
    [Pure]
    public Enumerator Traverse(in BinaryTraversalMethod traverseMethod) =>
        new(tree: this,
            method: traverseMethod);

    /// <summary>
    /// Instantiates a new <see cref="BinaryTree{TValue, TComparer}"/> with the <paramref name="root"/> as root node.
    /// </summary>
    /// <param name="root">The value of the root node.</param>
    /// <param name="comparer">The comparer that will be used to compare two <typeparamref name="TValue"/> instances.</param>
    /// <exception cref="ArgumentNullException"/>
    public static BinaryTree<TValue, TComparer> Create(
#if NETCOREAPP3_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        [DisallowNull]
#endif
        TValue root,
#if NETCOREAPP3_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        [DisallowNull]
#endif
        TComparer comparer)
    {
#if NET6_0_OR_GREATER
        ArgumentNullException.ThrowIfNull(root);
        ArgumentNullException.ThrowIfNull(comparer);
#else
        if (root is null)
        {
            throw new ArgumentNullException(nameof(root));
        }

        if (comparer is null)
        {
            throw new ArgumentNullException(nameof(comparer));
        }
#endif

        return new(root: root,
                   comparer: comparer);
    }

    /// <summary>
    /// Gets the root for the <see cref="BinaryTree{TValue, TComparer}"/>.
    /// </summary>
#if NETCOREAPP3_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
    [NotNull]
#endif
    [Pure]
    public BinaryNode<TValue> RootNode =>
        m_Root;

    /// <summary>
    /// Gets or sets if an exception should be thrown when trying to add a duplicate <typeparamref name="TValue"/>.
    /// </summary>
    public Boolean ThrowExceptionOnDuplicate
    {
        get;
        set;
    } = true;
}

// Non-Public
partial class BinaryTree<TValue, TComparer>
{
    internal BinaryTree(TValue root,
                        TComparer comparer)
    {
        m_Root = new(value: root,
                     parent: null);
        this.Comparer = comparer;
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

// ICollectionWithCount<T, U>
partial class BinaryTree<TValue, TComparer> : ICollectionWithCount<TValue, BinaryTree<TValue, TComparer>.Enumerator>
{
    /// <inheritdoc/>
    public Int32 Count =>
        m_Count;
}

// IEnumerable
partial class BinaryTree<TValue, TComparer> : IEnumerable
{
    IEnumerator IEnumerable.GetEnumerator() =>
        this.GetEnumerator();
}

// IEnumerable<T>
partial class BinaryTree<TValue, TComparer> : IEnumerable<TValue>
{
    IEnumerator<TValue> IEnumerable<TValue>.GetEnumerator() =>
        this.GetEnumerator();
}

// IModifyableCollection<T, U>
partial class BinaryTree<TValue, TComparer> : IModifyableCollection<TValue, BinaryTree<TValue, TComparer>.Enumerator>
{
    /// <summary>
    /// Adds an element to the <see cref="BinaryTree{TValue, TComparer}"/>.
    /// </summary>
    /// <param name="element">The element to add to the <see cref="BinaryTree{TValue, TComparer}"/>.</param>
    /// <returns><see langword="true"/> if the element was added to the <see cref="BinaryTree{TValue, TComparer}"/>; otherwise, <see langword="false"/>.</returns>
    /// <exception cref="ArgumentNullException"/>
    public Boolean Add(
#if NETCOREAPP3_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        [DisallowNull]
#endif
        TValue element)
    {
#if NET6_0_OR_GREATER
        ArgumentNullException.ThrowIfNull(element);
#else
        if (element is null)
        {
            throw new ArgumentNullException(nameof(element));
        }
#endif

        BinaryNode<TValue>? node = m_Root;
        Int32 compare = this.Comparer.Compare(element, node.Value);
        while (node is not null)
        {
            if (compare == 0)
            {
                if (this.ThrowExceptionOnDuplicate)
                {
                    ArgumentException exception = new(message: ALREADY_EXISTS);
                    exception.Data .Add(key: "Duplicate Value",
                                        value: element);
                    throw exception;
                }
                return false;
            }
            else if (compare < 0)
            {
                if (node.LeftChild is null)
                {
                    node.SetLeftChild(node: new(value: element,
                                                parent: node),
                                      comparer: this.Comparer);
                    m_Count++;
                    return true;
                }
                node = node.LeftChild;
            }
            else if (compare > 0)
            {
                if (node.RightChild is null)
                {
                    node.SetRightChild(node: new(value: element,
                                                 parent: node),
                                       comparer: this.Comparer);
                    m_Count++;
                    return true;
                }
                node = node.RightChild;
            }
            if (node is not null)
            {
                compare = this.Comparer.Compare(element, node.Value);
            }
        }
        return false;
    }

    /// <summary>
    /// Adds all elements of an enumerable to the <see cref="BinaryTree{TValue, TComparer}"/>.
    /// </summary>
    /// <param name="enumerable">The elements to add to the <see cref="BinaryTree{TValue, TComparer}"/>.</param>
    /// <exception cref="ArgumentNullException" />
    public void AddRange<TEnumerable, TEnumerator>(
#if NETCOREAPP3_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        [DisallowNull]
#endif
        TEnumerable enumerable)
            where TEnumerator : struct, IStrongEnumerator<TValue>
            where TEnumerable : IStrongEnumerable<TValue, TEnumerator>
    {
#if NET6_0_OR_GREATER
        ArgumentNullException.ThrowIfNull(enumerable);
#else
        if (enumerable is null)
        {
            throw new ArgumentNullException(nameof(enumerable));
        }
#endif

        foreach (TValue item in enumerable)
        {
            this.Add(item);
        }
    }

    /// <summary>
    /// Removes all elements from the <see cref="BinaryTree{TValue, TComparer}"/>.
    /// </summary>
    public void Clear()
    {
        m_Root.SetLeftChild(node: null,
                            comparer: this.Comparer);
        m_Root.SetRightChild(node: null,
                             comparer: this.Comparer);
        m_Count = 0;
    }

    /// <summary>
    /// Removes the first occurrence of an element from the <see cref="BinaryTree{TValue, TComparer}"/>.
    /// </summary>
    /// <param name="element">Tehe element to remove from the <see cref="BinaryTree{TValue, TComparer}"/>.</param>
    /// <returns><see langword="true"/> if the element was removed from the <see cref="BinaryTree{TValue, TComparer}"/>; otherwise, <see langword="false"/>.</returns>
    /// <exception cref="ArgumentNullException"/>
    public Boolean Remove(
#if NETCOREAPP3_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        [DisallowNull]
#endif
        TValue element)
    {
#if NET6_0_OR_GREATER
        ArgumentNullException.ThrowIfNull(element);
#else
        if (element is null)
        {
            throw new ArgumentNullException(nameof(element));
        }
#endif

        if (this.Comparer.Compare(element, m_Root.Value) == 0)
        {
            throw new ArgumentException(message: CANNOT_REMOVE_ROOT);
        }

        BinaryNode<TValue>? node = this.Find(element);
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
            if (node.Parent.LeftChild == node)
            {
                node.Parent.SetLeftChild(node: node.RightChild,
                                         comparer: this.Comparer);
                if (node.RightChild is not null)
                {
                    node.RightChild.SetParent(node.Parent);
                }
            }
            else if (node.Parent.RightChild == node)
            {
                node.Parent.SetRightChild(node: node.RightChild,
                                          comparer: this.Comparer);
                if (node.RightChild is not null)
                {
                    node.RightChild.SetParent(node.Parent);
                }
            }
        }
        else if (node.RightChild is null)
        {
            if (node.Parent is null)
            {
                throw new NotAllowed(message: NO_PARENT);
            }
            if (node.Parent.LeftChild == node)
            {
                node.Parent.SetLeftChild(node: node.LeftChild,
                                         comparer: this.Comparer);
                if (node.LeftChild is not null)
                {
                    node.LeftChild.SetParent(node.Parent);
                }
            }
            else if (node.Parent.RightChild == node)
            {
                node.Parent.SetRightChild(node: node.LeftChild,
                                          comparer: this.Comparer);
                if (node.LeftChild is not null)
                {
                    node.LeftChild.SetParent(node.Parent);
                }
            }
        }
        else
        {
            BinaryNode<TValue> min = node.SetToMinBranchValue();
            if (min.Parent is null)
            {
                throw new NotAllowed(message: NO_PARENT);
            }
            min.Parent.SetLeftChild(node: null,
                                    comparer: this.Comparer);
        }

        m_Count--;
        return true;
    }
}

// IReadOnlyCollection<T, U>
partial class BinaryTree<TValue, TComparer> : IReadOnlyCollection<TValue, BinaryTree<TValue, TComparer>.Enumerator>
{
    /// <inheritdoc/>
    /// <exception cref="ArgumentNullException"/>
    public Boolean Contains(
#if NETCOREAPP3_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        [DisallowNull]
#endif
        TValue element)
    {
#if NET6_0_OR_GREATER
        ArgumentNullException.ThrowIfNull(element);
#else
        if (element is null)
        {
            throw new ArgumentNullException(nameof(element));
        }
#endif

        return this.Find(element) is not null;
    }

    /// <inheritdoc/>
    public void CopyTo(
#if NETCOREAPP3_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        [DisallowNull]
#endif
        TValue[] array)
    {
#if NET6_0_OR_GREATER
        ArgumentNullException.ThrowIfNull(array);
#else
        if (array is null)
        {
            throw new ArgumentNullException(nameof(array));
        }
#endif

        Int32 index = 0;
        foreach (TValue value in this.TraverseInOrder())
        {
            array[index++] = value;
        }
    }
    /// <inheritdoc/>
    public void CopyTo(
#if NETCOREAPP3_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        [DisallowNull]
#endif
        TValue[] array,
        Int32 destinationIndex)
    {
#if NET6_0_OR_GREATER
        ArgumentNullException.ThrowIfNull(array);
#else
        if (array is null)
        {
            throw new ArgumentNullException(nameof(array));
        }
#endif

        Int32 index = 0;
        foreach (TValue value in this.TraverseInOrder())
        {
            array[destinationIndex + index++] = value;
        }
    }
}

// ISortedCollection<T, U>
partial class BinaryTree<TValue, TComparer> : ISortedCollection<TValue, BinaryTree<TValue, TComparer>.Enumerator, TComparer>
{
    /// <inheritdoc/>
#if NETCOREAPP3_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
    [NotNull]
#endif
    [Pure]
    public TComparer Comparer { get; }
}

// IStrongEnumerable<T, U>
partial class BinaryTree<TValue, TComparer> : IStrongEnumerable<TValue, BinaryTree<TValue, TComparer>.Enumerator>
{
    /// <inheritdoc/>
    public Enumerator GetEnumerator() =>
        new(tree: this,
            method: BinaryTraversalMethod.InOrder);
}

// Enumerator
partial class BinaryTree<TValue, TComparer>
{
    /// <summary>
    /// An enumerator that iterates through the <see cref="BinaryTree{TValue, TComparer}"/>.
    /// </summary>
    public struct Enumerator :
        IStrongEnumerable<TValue, BinaryTree<TValue, TComparer>.Enumerator>,
        IStrongEnumerator<TValue>,
        IEnumerator<TValue>
    {
        /// <summary>
        /// The default constructor for the <see cref="Enumerator"/> is not allowed.
        /// </summary>
        /// <exception cref="NotAllowed"></exception>
        public Enumerator()
        {
            throw new NotAllowed();
        }
        internal Enumerator(BinaryTree<TValue, TComparer> tree,
                            in BinaryTraversalMethod method)
        {
            m_Elements = method switch
            {
                BinaryTraversalMethod.PreOrder => tree.TraversePreOrder(),
                BinaryTraversalMethod.PostOrder => tree.TraversePostOrder(),
                _ => tree.TraverseInOrder(),
            };
            m_Index = -1;
        }
        internal Enumerator(List<BinaryNode<TValue>> elements)
        {
            m_Elements = elements;
            m_Index = -1;
        }

        /// <inheritdoc/>
        public Boolean MoveNext() => 
            ++m_Index < m_Elements.Count;

        /// <inheritdoc/>
        public Enumerator GetEnumerator()
        {
            if (m_Index == -1)
            {
                return this;
            }
            else
            {
                return new(m_Elements);
            }
        }

#if !NETCOREAPP3_1_OR_GREATER
        void IDisposable.Dispose()
        { }

        void IEnumerator.Reset()
        { }

        Object? IEnumerator.Current =>
            this.Current;

        IEnumerator IEnumerable.GetEnumerator() =>
        this.GetEnumerator();
        IEnumerator<TValue> IEnumerable<TValue>.GetEnumerator() =>
            this.GetEnumerator();
#endif

        /// <inheritdoc/>
        public TValue Current =>
            m_Elements[m_Index].Value;

        private readonly List<BinaryNode<TValue>> m_Elements;
        private Int32 m_Index;
    }
}