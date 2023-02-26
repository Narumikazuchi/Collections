namespace Narumikazuchi.Collections;

public partial class TrieNode<TContent> : IModifyableCollection<TContent, CommonHashSetEnumerator<TContent>>
{
    /// <summary>
    /// Adds an object to the <see cref="TrieNode{TContent}"/>.
    /// </summary>
    /// <param name="item">The value to be added to the <see cref="TrieNode{TContent}"/>.</param>
    /// <returns><see langword="true"/> if the item was added; otherwise, <see langword="false"/></returns>
    /// <exception cref="ArgumentNullException"/>
    public Boolean Add(
#if NETCOREAPP3_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        [DisallowNull]
#endif
        NotNull<TContent> item)
    {
        return m_Items.Add(item);
    }

    /// <summary>
    /// Adds the elements of the specified collection to the <see cref="TrieNode{TContent}"/>.
    /// </summary>
    /// <param name="enumerable">The collection of items to add.</param>
    /// <exception cref="ArgumentNullException"/>
    public void AddRange<TEnumerable>(
#if NETCOREAPP3_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        [DisallowNull]
#endif
        NotNull<TEnumerable> enumerable)
            where TEnumerable : IEnumerable<TContent>
    {
        TEnumerable source = enumerable;
        foreach (TContent item in source)
        {
            if (item is null)
            {
                continue;
            }
            m_Items.Add(item);
        }
    }

    /// <summary>
    /// Removes all elements from the <see cref="TrieNode{TContent}"/>.
    /// </summary>
    public void Clear()
    {
        m_Items.Clear();
    }

    /// <summary>
    /// Removes the first occurrence of the specified item from the <see cref="TrieNode{TContent}"/>.
    /// </summary>
    /// <param name="item">The item to remove.</param>
    /// <returns><see langword="true"/> if the item was found and removed; otherwise, <see langword="false"/></returns>
    /// <exception cref="ArgumentNullException"/>
    public Boolean Remove(
#if NETCOREAPP3_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        [DisallowNull]
#endif
        NotNull<TContent> item)
    {
        if (m_Items.Contains(item: item))
        {
            return m_Items.Remove(item);
        }

        foreach (TrieNode<TContent> child in this.Children)
        {
            if (child is null)
            {
                continue;
            }
            if (child.Contains(item: item))
            {
                return child.Remove(item);
            }
        }

        return false;
    }
}