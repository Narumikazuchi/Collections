using Narumikazuchi.Collections.Extensions;

namespace Narumikazuchi.Collections;

/// <summary>
/// Represents a fast binary lookup data structure.
/// </summary>
[DebuggerDisplay("Depth = {GetDepth()}")]
public sealed partial class BinaryTree<TValue>
    where TValue : notnull
{
    /// <summary>
    /// Instantiates a new <see cref="BinaryTree{TValue}"/> with the <paramref name="rootValue"/> as root node.
    /// </summary>
    /// <param name="rootValue">The value of the root node.</param>
    /// <exception cref="ArgumentNullException"/>
    public BinaryTree([DisallowNull] TValue rootValue) :
        this(rootValue: rootValue,
             comparer: Comparer<TValue>.Default)
    { }
    /// <summary>
    /// Instantiates a new <see cref="BinaryTree{TValue}"/> with the <paramref name="rootValue"/> as root node.
    /// </summary>
    /// <param name="rootValue">The value of the root node.</param>
    /// <param name="comparer">The comparer that will be used to compare two <typeparamref name="TValue"/> instances.</param>
    /// <exception cref="ArgumentNullException"/>
    public BinaryTree([DisallowNull] TValue rootValue,
                      [DisallowNull] IComparer<TValue> comparer)
    {
        ArgumentNullException.ThrowIfNull(rootValue);
        ArgumentNullException.ThrowIfNull(comparer);

        m_Root = new BinaryNode<TValue>(value: rootValue,
                                        parent: null);
        this.Comparer = comparer;
    }

    /// <summary>
    /// Adds the elements of the specified <typeparamref name="TEnumerable"/> to the <see cref="BinaryTree{TValue}"/>.
    /// </summary>
    /// <param name="enumerable">The collection of items to add.</param>
    /// <exception cref="ArgumentNullException"/>
    public void AddRange<TEnumerable>([DisallowNull] TEnumerable enumerable)
        where TEnumerable : IEnumerable<TValue>
    {
        ArgumentNullException.ThrowIfNull(enumerable);

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
    [return: MaybeNull]
    public BinaryNode<TValue>? Find([DisallowNull] TValue value)
    {
        ArgumentNullException.ThrowIfNull(value);

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
    /// Removes all <see cref="BinaryNode{TValue}"/> objects from the <see cref="BinaryTree{TValue}"/>, only when their <typeparamref name="TValue"/> matches the specified condition.
    /// </summary>
    /// <param name="predicate">The condition that the <typeparamref name="TValue"/> needs to meet to be deleted.</param>
    /// <returns>The number of <see cref="BinaryNode{TValue}"/> objects that have been removed.</returns>
    /// <exception cref="ArgumentNullException"/>
    public Int32 RemoveAll([DisallowNull] Func<TValue, Boolean> predicate)
    {
        ArgumentNullException.ThrowIfNull(predicate);

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
    /// Gets the depth of the <see cref="BinaryTree{TValue}"/>.
    /// </summary>
    /// <returns>The depth of the deepest <see cref="BinaryNode{TValue}"/> in the tree.</returns>
    [Pure]
    public UInt32 GetDepth() => 
        this.GetDepth(node: m_Root);

    /// <summary>
    /// Determines the lowest value in the <see cref="BinaryTree{TValue}"/>.
    /// </summary>
    /// <returns>The <typeparamref name="TValue"/> of the lowest element in the <see cref="BinaryTree{TValue}"/>.</returns>
    [Pure]
    [return: NotNull]
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
    /// <returns>The <typeparamref name="TValue"/> of the highest element in the <see cref="BinaryTree{TValue}"/>.</returns>
    [Pure]
    [return: NotNull]
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
    /// <returns>An <see cref="IStrongEnumerable{TElement, TEnumerator}"/> containing all <typeparamref name="TValue"/> in this <see cref="BinaryTree{TValue}"/> in the order specified by the <paramref name="traverseMethod"/>.</returns>
    [Pure]
    [return: NotNull]
    public IStrongEnumerable<TValue, Enumerator> Traverse(in BinaryTraversalMethod traverseMethod) =>
        new Enumerator(tree: this,
                       method: traverseMethod);

    /// <summary>
    /// Gets the root for the <see cref="BinaryTree{TValue}"/>.
    /// </summary>
    [NotNull]
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
partial class BinaryTree<TValue>
{
    internal BinaryTree(IEnumerable<TValue> collection,
                        IComparer<TValue> comparer)
    {
        ArgumentNullException.ThrowIfNull(collection);
        ArgumentNullException.ThrowIfNull(comparer);

        if (!collection.Any())
        {
            throw new ArgumentException(message: CANNOT_CREATE_FROM_EMPTY_COLLECTION);
        }

        this.Comparer = comparer;

        IOrderedEnumerable<TValue> distinct = collection.Distinct()
                                                        .OrderBy(i => i);
        TValue median = distinct.Median()!;
        m_Root = new(value: median, 
                     parent: null);
        this.AddRange(distinct.Where(x => comparer.Compare(x, median) != 0));
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
partial class BinaryTree<TValue> : ICollectionWithCount<TValue, BinaryTree<TValue>.Enumerator>
{
    /// <inheritdoc/>
    public Int32 Count =>
        m_Count;
}

// IEnumerable
partial class BinaryTree<TValue> : IEnumerable
{
    IEnumerator IEnumerable.GetEnumerator() =>
        this.GetEnumerator();
}

// IEnumerable<T>
partial class BinaryTree<TValue> : IEnumerable<TValue>
{
    IEnumerator<TValue> IEnumerable<TValue>.GetEnumerator() =>
        this.GetEnumerator();
}

// IModifyableCollection<T, U>
partial class BinaryTree<TValue> : IModifyableCollection<TValue, BinaryTree<TValue>.Enumerator>
{
    /// <summary>
    /// Adds an element to the <see cref="BinaryTree{TValue}"/>.
    /// </summary>
    /// <param name="element">The element to add to the <see cref="BinaryTree{TValue}"/>.</param>
    /// <returns><see langword="true"/> if the element was added to the <see cref="BinaryTree{TValue}"/>; otherwise, <see langword="false"/>.</returns>
    /// <exception cref="ArgumentNullException"/>
    public Boolean Add([DisallowNull] TValue element)
    {
        ArgumentNullException.ThrowIfNull(element);

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
    /// Adds all elements of an enumerable to the <see cref="BinaryTree{TValue}"/>.
    /// </summary>
    /// <param name="enumerable">The elements to add to the <see cref="BinaryTree{TValue}"/>.</param>
    /// <exception cref="ArgumentNullException" />
    public void AddRange<TEnumerator>([DisallowNull] IStrongEnumerable<TValue, TEnumerator> enumerable)
        where TEnumerator : struct, IStrongEnumerator<TValue>
    {
        ArgumentNullException.ThrowIfNull(enumerable);

        foreach (TValue item in enumerable)
        {
            this.Add(item);
        }
    }

    /// <summary>
    /// Removes all elements from the <see cref="BinaryTree{TValue}"/>.
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
    /// Removes the first occurrence of an element from the <see cref="BinaryTree{TValue}"/>.
    /// </summary>
    /// <param name="element">Tehe element to remove from the <see cref="BinaryTree{TValue}"/>.</param>
    /// <returns><see langword="true"/> if the element was removed from the <see cref="BinaryTree{TValue}"/>; otherwise, <see langword="false"/>.</returns>
    /// <exception cref="ArgumentNullException"/>
    public Boolean Remove([DisallowNull] TValue element)
    {
        ArgumentNullException.ThrowIfNull(element);

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
partial class BinaryTree<TValue> : IReadOnlyCollection<TValue, BinaryTree<TValue>.Enumerator>
{
    /// <inheritdoc/>
    /// <exception cref="ArgumentNullException"/>
    public Boolean Contains([DisallowNull] TValue element)
    {
        ArgumentNullException.ThrowIfNull(element);

        return this.Find(element) is not null;
    }

    /// <inheritdoc/>
    public void CopyTo([DisallowNull] TValue[] array)
    {
        ArgumentNullException.ThrowIfNull(array);

        Int32 index = 0;
        foreach (TValue value in this.TraverseInOrder())
        {
            array[index++] = value;
        }
    }
    /// <inheritdoc/>
    public void CopyTo([DisallowNull] TValue[] array,
                       Int32 destinationIndex)
    {
        ArgumentNullException.ThrowIfNull(array);

        Int32 index = 0;
        foreach (TValue value in this.TraverseInOrder())
        {
            array[destinationIndex + index++] = value;
        }
    }
}

// ISortedCollection<T, U>
partial class BinaryTree<TValue> : ISortedCollection<TValue, BinaryTree<TValue>.Enumerator>
{
    /// <inheritdoc/>
    [Pure]
    [NotNull]
    public IComparer<TValue> Comparer { get; }
}

// IStrongEnumerable<T, U>
partial class BinaryTree<TValue> : IStrongEnumerable<TValue, BinaryTree<TValue>.Enumerator>
{
    /// <inheritdoc/>
    public Enumerator GetEnumerator() =>
        new(tree: this,
            method: BinaryTraversalMethod.InOrder);
}

// Enumerator
partial class BinaryTree<TValue>
{
    /// <summary>
    /// An enumerator that iterates through the <see cref="BinaryTree{TValue}"/>.
    /// </summary>
    public struct Enumerator :
        IStrongEnumerable<TValue, BinaryTree<TValue>.Enumerator>,
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
        internal Enumerator(BinaryTree<TValue> tree,
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

        void IEnumerator.Reset()
        { }

        void IDisposable.Dispose()
        { }

        /// <inheritdoc/>
        public TValue Current =>
            m_Elements[m_Index].Value;

        Object IEnumerator.Current =>
            this.Current;

        private readonly List<BinaryNode<TValue>> m_Elements;
        private Int32 m_Index;
    }
}