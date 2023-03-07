namespace Narumikazuchi.Collections;

public partial class TrieNode<TContent> : IReadOnlyCollection<TContent, CommonHashSetEnumerator<TContent>>
{
    /// <summary>
    /// Determines whether the <see cref="TrieNode{TContent}"/> contains a specific value.
    /// </summary>
    /// <param name="item">The object to locate in the <see cref="TrieNode{TContent}"/>.</param>
    /// <returns><see langword="true"/> if <paramref name="item"/> is found in the <see cref="TrieNode{TContent}"/>; otherwise, <see langword="false"/></returns>
    /// <exception cref="ArgumentNullException"/>
    public Boolean Contains([DisallowNull] TContent item)
    {
#if NET6_0_OR_GREATER
        ArgumentNullException.ThrowIfNull(item);
#else
        if (item is null)
        {
            throw new ArgumentNullException(nameof(item));
        }
#endif

        return m_Items.Contains(item);
    }
    /// <summary>
    /// Determines whether the <see cref="TrieNode{TContent}"/> contains a specific value.
    /// </summary>
    /// <param name="item">The object to locate in the <see cref="TrieNode{TContent}"/>.</param>
    /// <param name="equalityComparer">The comparer to use to check for equality.</param>
    /// <returns><see langword="true"/> if <paramref name="item"/> is found in the <see cref="TrieNode{TContent}"/>; otherwise, <see langword="false"/></returns>
    /// <exception cref="ArgumentNullException"/>
    public Boolean Contains<TEqualityComparer>([DisallowNull] TContent item,
                                               [DisallowNull] TEqualityComparer equalityComparer)
        where TEqualityComparer : IEqualityComparer<TContent>
    {
#if NET6_0_OR_GREATER
        ArgumentNullException.ThrowIfNull(item);
        ArgumentNullException.ThrowIfNull(equalityComparer);
#else
        if (item is null)
        {
            throw new ArgumentNullException(nameof(item));
        }
        
        if (equalityComparer is null)
        {
            throw new ArgumentNullException(nameof(equalityComparer));
        }
#endif

        foreach (TContent content in m_Items)
        {
            if (equalityComparer.Equals(x: content,
                                        y: item))
            {
                return true;
            }
        }

        return false;
    }

    /// <inheritdoc/>
    public void CopyTo([DisallowNull] TContent[] array)
    {
#if NET6_0_OR_GREATER
        ArgumentNullException.ThrowIfNull(array);
#else
        if (array is null)
        {
            throw new ArgumentNullException(nameof(array));
        }
#endif

        m_Items.CopyTo(array);
    }
    /// <inheritdoc/>
    public void CopyTo([DisallowNull] TContent[] array,
                       Int32 destinationIndex)
    {
#if NETCOREAPP3_0_OR_GREATER
        ArgumentNullException.ThrowIfNull(array);
        destinationIndex.ThrowIfOutOfRange(0, Int32.MaxValue);
#else
        if (array is null)
        {
            throw new ArgumentNullException(nameof(array));
        }

        if (destinationIndex is < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(destinationIndex));
        }
#endif

        m_Items.CopyTo(array: array,
                       arrayIndex: destinationIndex);
    }
}