namespace Narumikazuchi.Collections.Extensions;

/// <summary>
/// Extends the <see cref="IEnumerable{T}"/> interface.
/// </summary>
static public class EnumerableExtensions
{
    /// <summary>
    /// Retrieves all the elements that match the conditions defined by the specified predicate.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="predicate">The <see cref="Func{T, TResult}"/> delegate that defines the conditions of the elements to search for.</param>
    /// <returns>A <see cref="ReadOnlyList{T}"/> containing all the elements that match the conditions defined by the specified predicate, if found; otherwise, an empty <see cref="ReadOnlyList{T}"/>.</returns>
    /// <exception cref="ArgumentNullException"/>
    static public ReadOnlyList<TElement> FindAll<TElement>(this IEnumerable<TElement> source,
                                                          [DisallowNull] Func<TElement, Boolean> predicate)
    {
#if NET6_0_OR_GREATER
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.ThrowIfNull(predicate);
#else
        if (source is null)
        {
            throw new ArgumentNullException(nameof(source));
        }
        
        if (predicate is null)
        {
            throw new ArgumentNullException(nameof(predicate));
        }
#endif

        return new ReadOnlyList<TElement>(source.Where(predicate)
                                                .ToArray());
    }

    /// <summary>
    /// Performs the specified action on each element of the <see cref="IEnumerable{T}"/>.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="action">The <see cref="Action{T}"/> delegate to perform on each element of the <see cref="IEnumerable{T}"/>.</param>
    /// <exception cref="ArgumentNullException"/>
    static public void ForEach<TElement>(this IEnumerable<TElement> source,
                                         [DisallowNull] Action<TElement> action)
    {
#if NET6_0_OR_GREATER
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.ThrowIfNull(action);
#else
        if (source is null)
        {
            throw new ArgumentNullException(nameof(source));
        }
        
        if (action is null)
        {
            throw new ArgumentNullException(nameof(action));
        }
#endif

        foreach (TElement element in source)
        {
            action.Invoke(element);
        }
    }

    /// <summary>
    /// Determines whether every element in the <see cref="IEnumerable{T}"/> matches the conditions defined by the specified predicate.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="predicate">The <see cref="Func{T, TResult}"/> delegate that defines the conditions to check against the elements.</param>
    /// <returns>
    /// <see langword="true"/> if every element in the <see cref="IEnumerable{T}"/> matches the conditions
    /// defined by the specified predicate; otherwise, <see langword="false"/>. If the collection has no elements,
    /// the return value is <see langword="true"/>.
    /// </returns>
    /// <exception cref="ArgumentNullException"/>
    static public Boolean TrueForAll<TElement>(this IEnumerable<TElement> source,
                                               [DisallowNull] Func<TElement, Boolean> predicate)
    {
#if NET6_0_OR_GREATER
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.ThrowIfNull(predicate);
#else
        if (source is null)
        {
            throw new ArgumentNullException(nameof(source));
        }
        
        if (predicate is null)
        {
            throw new ArgumentNullException(nameof(predicate));
        }
#endif

        foreach (TElement element in source)
        {
            if (!predicate.Invoke(element))
            {
                return false;
            }
        }

        return true;
    }

    /// <summary>
    /// Returns the item <typeparamref name="TElement"/> at the center of the collection.
    /// </summary>
    /// <returns>The item <typeparamref name="TElement"/> at the center of the collection</returns>
#if NETCOREAPP3_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
    [return: MaybeNull]
#endif
    static public TElement Median<TElement>(this IEnumerable<TElement> source)
    {
#if NET6_0_OR_GREATER
        ArgumentNullException.ThrowIfNull(source);
#else
        if (source is null)
        {
            throw new ArgumentNullException(nameof(source));
        }
#endif

        if (!source.Any())
        {
            return default!;
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