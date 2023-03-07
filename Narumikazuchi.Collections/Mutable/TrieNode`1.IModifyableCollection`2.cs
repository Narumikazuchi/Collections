namespace Narumikazuchi.Collections;

public partial class TrieNode<TContent> : IModifyableCollection<TContent, CommonHashSetEnumerator<TContent>>
{
    /// <summary>
    /// Adds an object to the <see cref="TrieNode{TContent}"/>.
    /// </summary>
    /// <param name="item">The value to be added to the <see cref="TrieNode{TContent}"/>.</param>
    /// <returns><see langword="true"/> if the item was added; otherwise, <see langword="false"/></returns>
    /// <exception cref="ArgumentNullException"/>
    public Boolean Add([DisallowNull] TContent item)
    {
#if NET6_0_OR_GREATER
        ArgumentNullException.ThrowIfNull(item);
#else
        if (item is null)
        {
            throw new ArgumentNullException(nameof(item));
        }
#endif

        return m_Items.Add(item);
    }

    /// <summary>
    /// Adds the elements of the specified collection to the <see cref="TrieNode{TContent}"/>.
    /// </summary>
    /// <param name="enumerable">The collection of items to add.</param>
    /// <exception cref="ArgumentNullException"/>
    public void AddRange<TEnumerable>([DisallowNull] TEnumerable enumerable)
        where TEnumerable : IEnumerable<TContent>
    {
#if NET6_0_OR_GREATER
        ArgumentNullException.ThrowIfNull(enumerable);
#else
        if (enumerable is null)
        {
            throw new ArgumentNullException(nameof(enumerable));
        }
#endif

        foreach (TContent item in enumerable)
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
    public Boolean Remove([DisallowNull] TContent item)
    {
#if NET6_0_OR_GREATER
        ArgumentNullException.ThrowIfNull(item);
#else
        if (item is null)
        {
            throw new ArgumentNullException(nameof(item));
        }
#endif

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