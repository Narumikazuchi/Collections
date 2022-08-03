namespace Narumikazuchi.Collections.Extensions;

/// <summary>
/// Extends the <see cref="IEnumerable{T}"/> interface.
/// </summary>
public static class EnumerableExtensions
{
    /// <summary>
    /// Returns a read-only <see cref="ReadOnlyCollection{T}"/> wrapper for the current collection.
    /// </summary>
    /// <returns>An object that contains the objects of the source and is read-only.</returns>
    public static ReadOnlyCollection<TElement> AsReadOnlyCollection<TElement>(this IEnumerable<TElement> source) =>
        ReadOnlyCollection<TElement>.CreateFrom(source);

    /// <summary>
    /// Returns a read-only <see cref="ReadOnlyDictionary{TKey, TValue, TEqualityComparer}"/> wrapper for the current collection.
    /// </summary>
    /// <returns>An object that contains the objects of the source and is read-only.</returns>
    public static ReadOnlyDictionary<TKey, TValue, TEqualityComparer> AsReadOnlyDictionary<TKey, TValue, TEqualityComparer>(this IEnumerable<KeyValuePair<TKey, TValue>> source,
                                                                                                                            [DisallowNull] TEqualityComparer equalityComparer)
        where TKey : notnull
        where TEqualityComparer : IEqualityComparer<TKey> =>
            ReadOnlyDictionary<TKey, TValue, TEqualityComparer>.CreateFrom(items: source,
                                                                           equalityComparer: equalityComparer);
    /// <summary>
    /// Returns a read-only <see cref="ReadOnlyDictionary{TKey, TValue, TEqualityComparer}"/> wrapper for the current collection.
    /// </summary>
    /// <returns>An object that contains the objects of the source and is read-only.</returns>
    public static ReadOnlyDictionary<TKey, TElement, TEqualityComparer> AsReadOnlyDictionary<TElement, TKey, TEqualityComparer>(this IEnumerable<TElement> source,
                                                                                                                                [DisallowNull] TEqualityComparer equalityComparer,
                                                                                                                                [DisallowNull] Func<TElement, TKey> keySelector)
        where TKey : notnull
        where TEqualityComparer : IEqualityComparer<TKey>
    {
        ArgumentNullException.ThrowIfNull(keySelector);

        return ReadOnlyDictionary<TKey, TElement, TEqualityComparer>.CreateFrom(items: source.Select(x => new KeyValuePair<TKey, TElement>(key: keySelector.Invoke(x),
                                                                                                                                           value: x)),
                                                                                equalityComparer: equalityComparer);
    }
    /// <summary>
    /// Returns a read-only <see cref="ReadOnlyDictionary{TKey, TValue, TEqualityComparer}"/> wrapper for the current collection.
    /// </summary>
    /// <returns>An object that contains the objects of the source and is read-only.</returns>
    public static ReadOnlyDictionary<TKey, TValue, TEqualityComparer> AsReadOnlyDictionary<TElement, TKey, TValue, TEqualityComparer>(this IEnumerable<TElement> source,
                                                                                                                                      [DisallowNull] TEqualityComparer equalityComparer,
                                                                                                                                      [DisallowNull] Func<TElement, TKey> keySelector,
                                                                                                                                      [DisallowNull] Func<TElement, TValue> valueSelector)
        where TKey : notnull
        where TEqualityComparer : IEqualityComparer<TKey>
    {
        ArgumentNullException.ThrowIfNull(keySelector);
        ArgumentNullException.ThrowIfNull(valueSelector);

        return ReadOnlyDictionary<TKey, TValue, TEqualityComparer>.CreateFrom(items: source.Select(x => new KeyValuePair<TKey, TValue>(key: keySelector.Invoke(x),
                                                                                                                                       value: valueSelector.Invoke(x))),
                                                                              equalityComparer: equalityComparer);
    }

    /// <summary>
    /// Returns a read-only <see cref="ReadOnlyList{T}"/> wrapper for the current list.
    /// </summary>
    /// <returns>An object that contains the objects of the source and is read-only.</returns>
    public static ReadOnlyList<TElement> AsReadOnlyList<TElement>(this IEnumerable<TElement> source) =>
        ReadOnlyList<TElement>.CreateFrom(source);

    /// <summary>
    /// Creates a <see cref="BinaryTree{TElement, TComparer}"/> from an <see cref="IEnumerable{T}"/>.
    /// </summary>
    [return: NotNull]
    public static BinaryTree<TElement, TComparer> ToBinaryTree<TElement, TComparer>(this IEnumerable<TElement> source,
                                                                                    [DisallowNull] TComparer comparer) 
        where TElement : notnull
        where TComparer : IComparer<TElement>
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.ThrowIfNull(comparer);

        if (source is BinaryTree<TElement, TComparer> original)
        {
            return original;
        }

        if (!source.Any())
        {
            throw new InvalidOperationException("No elements in the source!");
        }

        TElement median = source.Median()!;
        BinaryTree<TElement, TComparer> result = new(root: median,
                                                     comparer: comparer);
        foreach (TElement element in source)
        {
            result.Add(element);
        }

        return result;
    }

    /// <summary>
    /// Creates a <see cref="Trie{TElement}"/> from an <see cref="IEnumerable{String}"/>.
    /// </summary>
    [return: NotNull]
    public static Trie<TElement> ToTrie<TElement>(this IEnumerable<String> source) 
        where TElement : class
    {
        if (source is Trie<TElement> original)
        {
            return original;
        }
        return new(collection: source);
    }

    /// <summary>
    /// Returns the item <typeparamref name="TElement"/> at the center of the collection.
    /// </summary>
    /// <returns>The item <typeparamref name="TElement"/> at the center of the collection</returns>
    [return: MaybeNull]
    public static TElement Median<TElement>(this IEnumerable<TElement> source)
    {
        if (!source.Any())
        {
            return default;
        }

        if (source is IReadOnlyList<TElement> rlist)
        {
            return rlist[rlist.Count / 2];
        }

        if (source is IList<TElement> list)
        {
            return list[list.Count / 2];
        }

        IEnumerator<TElement> enumerator;
        Int32 index = 0;
        if (source is IReadOnlyCollection<TElement> collection)
        {
            enumerator = collection.GetEnumerator();
            while (index < collection.Count / 2)
            {
                enumerator.MoveNext();
                ++index;
            }
            return enumerator.Current;
        }

        IEnumerator<TElement> counter = source.GetEnumerator();
        enumerator = source.GetEnumerator();
        while (enumerator.MoveNext())
        {
            if (index % 2 == 0)
            {
                counter.MoveNext();
            }
            ++index;
        }
        return counter.Current;
    }
}