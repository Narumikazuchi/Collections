namespace Narumikazuchi.Collections;

public partial class BinaryTree<TValue, TComparer> : IModifyableCollection<TValue, BinaryTree<TValue, TComparer>.Enumerator>
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
        NotNull<TValue> element)
    {
        BinaryNode<TValue>? node = m_Root;
        TComparer comparer = this.Comparer;
        Int32 compare = comparer.Compare(x: element,
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
                if (node.LeftChild.IsNull)
                {
                    node.SetLeftChild(node: new(value: element,
                                                parent: node),
                                      comparer: comparer);
                    m_Count++;
                    return true;
                }
                node = node.LeftChild;
            }
            else if (compare > 0)
            {
                if (node.RightChild.IsNull)
                {
                    node.SetRightChild(node: new(value: element,
                                                 parent: node),
                                       comparer: comparer);
                    m_Count++;
                    return true;
                }
                node = node.RightChild;
            }

            if (node is not null)
            {
                compare = comparer.Compare(x: element,
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
    public void AddRange<TEnumerable>(
#if NETCOREAPP3_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        [DisallowNull]
#endif
        NotNull<TEnumerable> enumerable)
            where TEnumerable : IEnumerable<TValue>
    {
        TEnumerable source = enumerable;
        foreach (TValue value in source)
        {
            this.Add(value!);
        }
    }

    /// <summary>
    /// Removes all elements from the <see cref="BinaryTree{TValue, TComparer}"/>.
    /// </summary>
    public void Clear()
    {
        TComparer comparer = this.Comparer;
        m_Root.SetLeftChild(node: null,
                            comparer: comparer);
        m_Root.SetRightChild(node: null,
                             comparer: comparer);
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
        NotNull<TValue> element)
    {
        TComparer comparer = this.Comparer;
        if (comparer.Compare(x: element,
                             y: m_Root.Value) == 0)
        {
            throw new ArgumentException(message: CANNOT_REMOVE_ROOT);
        }

        BinaryNode<TValue>? node = this.FindInternal(element);
        if (node is null)
        {
            return false;
        }

        if (node.LeftChild.IsNull)
        {
            if (node.Parent.IsNull)
            {
                throw new NotAllowed(message: NO_PARENT);
            }

            BinaryNode<TValue> parent = node.Parent!;
            if (parent.LeftChild == node)
            {
                parent.SetLeftChild(node: node.RightChild,
                                    comparer: comparer);
                node.RightChild.WhenNotNull(rightChild => rightChild.SetParent(parent));
            }
            else if (parent.RightChild == node)
            {
                parent.SetRightChild(node: node.RightChild,
                                     comparer: comparer);
                node.RightChild.WhenNotNull(rightChild => rightChild.SetParent(parent));
            }
        }
        else if (!node.RightChild.IsNull)
        {
            if (!node.Parent.IsNull)
            {
                throw new NotAllowed(message: NO_PARENT);
            }

            BinaryNode<TValue> parent = node.Parent!;
            if (parent.LeftChild == node)
            {
                parent.SetLeftChild(node: node.LeftChild,
                                    comparer: comparer);
                node.LeftChild.WhenNotNull(leftChild => leftChild.SetParent(parent));
            }
            else if (parent.RightChild == node)
            {
                parent.SetRightChild(node: node.LeftChild,
                                     comparer: comparer);
                node.LeftChild.WhenNotNull(leftChild => leftChild.SetParent(parent));
            }
        }
        else
        {
            BinaryNode<TValue> min = node.SetToMinBranchValue();
            if (min.Parent.IsNull)
            {
                throw new NotAllowed(message: NO_PARENT);
            }

            BinaryNode<TValue> parent = min.Parent!;
            parent.SetLeftChild(node: null,
                                comparer: comparer);
        }

        m_Count--;
        return true;
    }
}