namespace Narumikazuchi.Collections;

public partial class TrieNode<TContent> : IReadOnlyCollection<TContent, CommonHashSetEnumerator<TContent>>
{
    /// <summary>
    /// Determines whether the <see cref="TrieNode{TContent}"/> contains a specific value.
    /// </summary>
    /// <param name="item">The object to locate in the <see cref="TrieNode{TContent}"/>.</param>
    /// <returns><see langword="true"/> if <paramref name="item"/> is found in the <see cref="TrieNode{TContent}"/>; otherwise, <see langword="false"/></returns>
    /// <exception cref="ArgumentNullException"/>
    public Boolean Contains(
#if NETCOREAPP3_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        [DisallowNull]
#endif
        NotNull<TContent> item)
    {
        return m_Items.Contains(item);
    }
    /// <summary>
    /// Determines whether the <see cref="TrieNode{TContent}"/> contains a specific value.
    /// </summary>
    /// <param name="item">The object to locate in the <see cref="TrieNode{TContent}"/>.</param>
    /// <param name="equalityComparer">The comparer to use to check for equality.</param>
    /// <returns><see langword="true"/> if <paramref name="item"/> is found in the <see cref="TrieNode{TContent}"/>; otherwise, <see langword="false"/></returns>
    /// <exception cref="ArgumentNullException"/>
    public Boolean Contains<TEqualityComparer>(
#if NETCOREAPP3_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        [DisallowNull]
#endif
        NotNull<TContent> item,
        NotNull<TEqualityComparer> equalityComparer)
            where TEqualityComparer : IEqualityComparer<TContent>
    {
        TEqualityComparer comparer = equalityComparer;
        foreach (TContent content in m_Items)
        {
            if (comparer.Equals(x: content,
                                y: item))
            {
                return true;
            }
        }

        return false;
    }

    /// <inheritdoc/>
    public void CopyTo(
#if NETCOREAPP3_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        [DisallowNull]
#endif
        NotNull<TContent[]> array)
    {
        m_Items.CopyTo(array);
    }
    /// <inheritdoc/>
    public void CopyTo(
#if NETCOREAPP3_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        [DisallowNull]
#endif
        NotNull<TContent[]> array,
        Int32 destinationIndex)
    {
#if NETCOREAPP3_0_OR_GREATER
        destinationIndex.ThrowIfOutOfRange(0, Int32.MaxValue);
#else
        if (destinationIndex is < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(destinationIndex));
        }
#endif

        m_Items.CopyTo(array: array,
                       arrayIndex: destinationIndex);
    }
}