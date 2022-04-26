namespace Narumikazuchi.Collections.Linq;

/// <summary>
/// Extends the <see cref="IEnumerable{T}"/> interface.
/// </summary>
public static class EnumerableExtensions
{
    /// <summary>
    /// Creates a <see cref="BinaryTree{TElement}"/> from an <see cref="IEnumerable{T}"/>.
    /// </summary>
    [return: NotNull]
    public static BinaryTree<TElement> ToBinaryTree<TElement>(this IEnumerable<TElement> source) 
        where TElement : IComparable<TElement>
    {
        if (source is BinaryTree<TElement> original)
        {
            return original;
        }
        return new(collection: source);
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