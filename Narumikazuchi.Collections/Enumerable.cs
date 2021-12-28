namespace Narumikazuchi.Collections;

/// <summary>
/// Extends the <see cref="IEnumerable{T}"/> interface.
/// </summary>
public static class Enumerable
{
    /// <summary>
    /// Creates a <see cref="ObservableList{TElement}"/> from an <see cref="IEnumerable{T}"/>.
    /// </summary>
    [return: NotNull]
    public static ObservableList<TElement> ToObservableList<TElement>(this IEnumerable<TElement> source)
    {
        if (source is ObservableList<TElement> original)
        {
            return original;
        }
        return new(collection: source);
    }


    /// <summary>
    /// Creates a <see cref="ObservableSet{TElement}"/> from an <see cref="IEnumerable{T}"/>.
    /// </summary>
    [return: NotNull]
    public static ObservableSet<TElement> ToObservableSet<TElement>(this IEnumerable<TElement> source)
    {
        if (source is ObservableSet<TElement> original)
        {
            return original;
        }
        return new(collection: source);
    }
    /// <summary>
    /// Creates a <see cref="ObservableSet{TElement}"/> from an <see cref="IEnumerable{T}"/>.
    /// </summary>
    [return: NotNull]
    public static ObservableSet<TElement> ToObservableSet<TElement>(this IEnumerable<TElement> source, 
                                                                    [AllowNull] IEqualityComparer<TElement>? comparer)
    {
        if (source is ObservableSet<TElement> original &&
            (original.Comparer is null &&
            comparer is null ||
            comparer is not null &&
            comparer.Equals(original.Comparer)))
        {
            return original;
        }
        return new(collection: source,
                   comparer: comparer!);
    }
    /// <summary>
    /// Creates a <see cref="ObservableSet{TElement}"/> from an <see cref="IEnumerable{T}"/>.
    /// </summary>
    [return: NotNull]
    public static ObservableSet<TElement> ToObservableSet<TElement>(this IEnumerable<TElement> source, 
                                                                    [DisallowNull] EqualityComparison<TElement> comparison) 
    {
        ExceptionHelpers.ThrowIfArgumentNull(comparison);
        if (source is ObservableSet<TElement> original &&
            original.Comparer is __FuncEqualityComparer<TElement> func &&
            func.Comparison == comparison)
        {
            return original;
        }
        return new(collection: source,
                   comparison: comparison!);
    }

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

    /// <summary>
    /// Adds the specified collection to this collection.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="other">The collection to add.</param>
    public static void AddRange<TElement>(this ICollection<TElement> source, 
                                          [DisallowNull] IEnumerable<TElement> other)
    {
        ExceptionHelpers.ThrowIfArgumentNull(other);

        foreach (TElement item in other)
        {
            source.Add(item);
        }
    }

    /// <summary>
    /// Moves the item at the given index one position in the specified direction in the <see cref="IList{T}"/>.
    /// </summary>
    public static void MoveItem<TElement>(this IList<TElement> list, 
                                          in Int32 index, 
                                          in ItemMoveDirection direction)
    {
        TElement tmp;
        if (direction == ItemMoveDirection.ToLowerIndex)
        {
            if (index > 0)
            {
                tmp = list[index];
                list[index] = list[index - 1];
                list[index - 1] = tmp;
            }
        }
        else
        {
            if (index > -1 && index < list.Count - 1)
            {
                tmp = list[index];
                list[index] = list[index + 1];
                list[index + 1] = tmp;
            }
        }
    }

    /// <summary>
    /// Moves the item at the given index the given amount of positions in the specified direction in the <see cref="IList{T}"/>.
    /// </summary>
    public static void MoveItem<TElement>(this IList<TElement> list, 
                                          in Int32 index, 
                                          in ItemMoveDirection direction, 
                                          Int32 positions)
    {
        if (positions == 0)
        {
            return;
        }

        TElement item = list[index];
        list.RemoveAt(index);
        if (positions > 0)
        {
            list.Insert(index: index + positions,
                        item: item);
            return;
        }
        list.Insert(index: index - positions,
                    item: item);
    }

    /// <summary>
    /// Moves the item one position in the specified direction in the <see cref="IList{T}"/>.
    /// </summary>
    public static void MoveItem<TElement>(this IList<TElement> list, 
                                          [MaybeNull] TElement item, 
                                          in ItemMoveDirection direction)
    {
        Int32 index = list.IndexOf(item);
        if (index == -1)
        {
            return;
        }
        if (direction == ItemMoveDirection.ToLowerIndex)
        {
            if (index > 0)
            {
                list[index] = list[index - 1];
                list[index - 1] = item;
            }
        }
        else
        {
            if (index < list.Count - 1)
            {
                list[index] = list[index + 1];
                list[index + 1] = item;
            }
        }
    }

    /// <summary>
    /// Moves the item at the given index the given amount of positions in the specified direction in the <see cref="IList{T}"/>.
    /// </summary>
    public static void MoveItem<TElement>(this IList<TElement> list, 
                                          [MaybeNull] TElement item, 
                                          in ItemMoveDirection direction, 
                                          Int32 positions)
    {
        if (positions == 0)
        {
            return;
        }

        Int32 index = list.IndexOf(item);
        if (index == -1)
        {
            return;
        }

        list.RemoveAt(index);
        if (positions > 0)
        {
            list.Insert(index: index + positions,
                        item: item);
            return;
        }
        list.Insert(index: index - positions,
                    item: item);
    }
}