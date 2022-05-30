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
    /// Returns a read-only <see cref="ReadOnlyDictionary{TKey, TValue}"/> wrapper for the current collection.
    /// </summary>
    /// <returns>An object that contains the objects of the source and is read-only.</returns>
    public static ReadOnlyDictionary<TKey, TValue> AsReadOnlyDictionary<TKey, TValue>(this IEnumerable<KeyValuePair<TKey, TValue>> source)
        where TKey : notnull =>
            ReadOnlyDictionary<TKey, TValue>.CreateFrom(source);
    /// <summary>
    /// Returns a read-only <see cref="ReadOnlyDictionary{TKey, TValue}"/> wrapper for the current collection.
    /// </summary>
    /// <returns>An object that contains the objects of the source and is read-only.</returns>
    public static ReadOnlyDictionary<TKey, TElement> AsReadOnlyDictionary<TElement, TKey>(this IEnumerable<TElement> source,
                                                                                          [DisallowNull] Func<TElement, TKey> keySelector)
        where TKey : notnull
    {
        ArgumentNullException.ThrowIfNull(keySelector);

        return ReadOnlyDictionary<TKey, TElement>.CreateFrom(source.Select(x => new KeyValuePair<TKey, TElement>(key: keySelector.Invoke(x),
                                                                                                                 value: x)));
    }
    /// <summary>
    /// Returns a read-only <see cref="ReadOnlyDictionary{TKey, TValue}"/> wrapper for the current collection.
    /// </summary>
    /// <returns>An object that contains the objects of the source and is read-only.</returns>
    public static ReadOnlyDictionary<TKey, TValue> AsReadOnlyDictionary<TElement, TKey, TValue>(this IEnumerable<TElement> source,
                                                                                                [DisallowNull] Func<TElement, TKey> keySelector,
                                                                                                [DisallowNull] Func<TElement, TValue> valueSelector)
        where TKey : notnull
    {
        ArgumentNullException.ThrowIfNull(keySelector);
        ArgumentNullException.ThrowIfNull(valueSelector);

        return ReadOnlyDictionary<TKey, TValue>.CreateFrom(source.Select(x => new KeyValuePair<TKey, TValue>(key: keySelector.Invoke(x),
                                                                                                             value: valueSelector.Invoke(x))));
    }

    /// <summary>
    /// Returns a read-only <see cref="ReadOnlyList{T}"/> wrapper for the current list.
    /// </summary>
    /// <returns>An object that contains the objects of the source and is read-only.</returns>
    public static ReadOnlyList<TElement> AsReadOnlyList<TElement>(this IEnumerable<TElement> source) =>
        ReadOnlyList<TElement>.CreateFrom(source);

    /// <summary>
    /// Creates a <see cref="BinaryTree{TElement}"/> from an <see cref="IEnumerable{T}"/>.
    /// </summary>
    [return: NotNull]
    public static BinaryTree<TElement> ToBinaryTree<TElement>(this IEnumerable<TElement> source) 
        where TElement : notnull
    {
        if (source is BinaryTree<TElement> original)
        {
            return original;
        }
        return new(collection: source,
                   comparer: Comparer<TElement>.Default);
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