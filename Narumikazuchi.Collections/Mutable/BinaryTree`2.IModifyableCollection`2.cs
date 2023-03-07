namespace Narumikazuchi.Collections;

public partial class BinaryTree<TValue, TComparer> : IModifyableCollection<TValue, BinaryTree<TValue, TComparer>.Enumerator>
{
    /// <summary>
    /// Adds an element to the <see cref="BinaryTree{TValue, TComparer}"/>.
    /// </summary>
    /// <param name="element">The element to add to the <see cref="BinaryTree{TValue, TComparer}"/>.</param>
    /// <returns><see langword="true"/> if the element was added to the <see cref="BinaryTree{TValue, TComparer}"/>; otherwise, <see langword="false"/>.</returns>
    /// <exception cref="ArgumentNullException"/>
    public Boolean Add([DisallowNull] TValue element)
    {
        BinaryNode<TValue>? node = m_Root;
        Int32 compare = this.Comparer.Compare(x: element,
                                              y: node.Value);
        while (node is not null)
        {
            if (compare == 0)
            {
                if (this.ThrowExceptionOnDuplicate)
                {
                    ArgumentException exception = new(message: ALREADY_EXISTS);
                    exception.Data.Add(key: "Duplicate Value",
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
                compare = this.Comparer.Compare(x: element,
                                                y: node.Value);
            }
        }

        return false;
    }

    /// <summary>
    /// Adds the elements of the specified <typeparamref name="TEnumerable"/> to the <see cref="BinaryTree{TValue, TComparer}"/>.
    /// </summary>
    /// <param name="enumerable">The collection of items to add.</param>
    /// <exception cref="ArgumentNullException"/>
    public void AddRange<TEnumerable>([DisallowNull] TEnumerable enumerable)
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
            this.Add(value!);
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
    public Boolean Remove([DisallowNull] TValue element)
    {
#if NET6_0_OR_GREATER
        ArgumentNullException.ThrowIfNull(element);
#else
        if (element is null)
        {
            throw new ArgumentNullException(nameof(element));
        }
#endif

        TComparer comparer = this.Comparer;
        if (this.Comparer.Compare(x: element,
                                  y: m_Root.Value) == 0)
        {
            throw new ArgumentException(message: CANNOT_REMOVE_ROOT);
        }

        BinaryNode<TValue>? node = this.FindInternal(element);
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

            BinaryNode<TValue> parent = node.Parent;
            if (parent.LeftChild == node)
            {
                parent.SetLeftChild(node: node.RightChild,
                                    comparer: this.Comparer);
                if (node.RightChild is not null)
                {
                    node.RightChild.SetParent(parent);
                }
            }
            else if (parent.RightChild == node)
            {
                parent.SetRightChild(node: node.RightChild,
                                     comparer: this.Comparer);
                if (node.RightChild is not null)
                {
                    node.RightChild.SetParent(parent);
                }
            }
        }
        else if (node.RightChild is not null)
        {
            if (node.Parent is null)
            {
                throw new NotAllowed(message: NO_PARENT);
            }

            BinaryNode<TValue> parent = node.Parent;
            if (parent.LeftChild == node)
            {
                parent.SetLeftChild(node: node.LeftChild,
                                    comparer: this.Comparer);
                if (node.LeftChild is not null)
                {
                    node.LeftChild.SetParent(parent);
                }
            }
            else if (parent.RightChild == node)
            {
                parent.SetRightChild(node: node.LeftChild,
                                     comparer: this.Comparer);
                if (node.LeftChild is not null)
                {
                    node.LeftChild.SetParent(parent);
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

            BinaryNode<TValue> parent = min.Parent!;
            parent.SetLeftChild(node: null,
                                comparer: this.Comparer);
        }

        m_Count--;
        return true;
    }
}