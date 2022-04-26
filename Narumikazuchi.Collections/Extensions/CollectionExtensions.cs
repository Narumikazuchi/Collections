namespace Narumikazuchi.Collections.Linq;

/// <summary>
/// Extends the <see cref="ICollection{T}"/> interface.
/// </summary>
public static class CollectionExtensions
{
    /// <summary>
    /// Adds the elements of the specified collection to the end of the <see cref="ICollection{T}"/>.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="other">The collection to add.</param>
    /// <exception cref="ArgumentNullException"/>
    public static void AddRange<TElement>(this ICollection<TElement> source,
                                          [DisallowNull] IEnumerable<TElement> other)
        where TElement : notnull
    {
        ArgumentNullException.ThrowIfNull(other);

        foreach (TElement element in other.Where(x => x is not null))
        {
            if (source is INotifyPropertyChangingHelper changing)
            {
                changing.OnPropertyChanging(nameof(source.Count));
            }
            source.Add(element);
            if (source is INotifyPropertyChangedHelper changed)
            {
                changed.OnPropertyChanged(nameof(source.Count));
            }
            if (source is INotifyCollectionChangedHelper collection)
            {
                collection.OnCollectionChanged(new(action: NotifyCollectionChangedAction.Add,
                                                   changedItem: element));
            }
        }
    }

    /// <summary>
    /// Returns a read-only <see cref="IReadOnlyCollection{T}"/> wrapper for the current collection.
    /// </summary>
    /// <returns>An object that acts as a read-only wrapper around the current <see cref="ICollection{T}"/>.</returns>
    public static IReadOnlyCollection<TElement> AsReadOnly<TElement>(this ICollection<TElement> source)
        where TElement : notnull
    {
        if (source is List<TElement> list)
        {
            return list.AsReadOnly();
        }

        return new List<TElement>(source.Where(x => x is not null)).AsReadOnly();
    }

    /// <summary>
    /// Converts the elements in the current <see cref="ICollection{T}"/> to another type, and returns a list containing the converted elements.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="converter">A <see cref="Converter{TInput, TOutput}"/> delegate that converts each element from one type to another type.</param>
    /// <returns>A <see cref="IReadOnlyCollection{T}"/> of the target type containing the converted elements from the current <see cref="ICollection{T}"/>.</returns>
    /// <exception cref="ArgumentNullException"/>
    public static IReadOnlyCollection<TOther> ConvertAll<TElement, TOther>(this ICollection<TElement> source,
                                                                           [DisallowNull] Converter<TElement, TOther> converter)
        where TElement : notnull
    {
        ArgumentNullException.ThrowIfNull(converter);

        List<TOther> list = new();
        foreach (TElement element in source.Where(x => x is not null))
        {
            list.Add(converter.Invoke(element));
        }

        return list.AsReadOnly();
    }

    /// <summary>
    /// Determines whether the <see cref="ICollection{T}"/> contains elements that match the conditions defined by the specified predicate.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="predicate">The <see cref="Func{T, TResult}"/> delegate that defines the conditions of the elements to search for.</param>
    /// <returns><see langword="true"/> if the <see cref="ICollection{T}"/> contains one or more elements that match the conditions defined by the specified predicate; otherwise, <see langword="false"/>.</returns>
    /// <exception cref="ArgumentNullException"/>
    public static Boolean Exists<TElement>(this ICollection<TElement> source,
                                           [DisallowNull] Func<TElement, Boolean> predicate)
        where TElement : notnull
    {
        ArgumentNullException.ThrowIfNull(predicate);

        foreach (TElement element in source.Where(x => x is not null))
        {
            if (predicate.Invoke(element))
            {
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// Retrieves all the elements that match the conditions defined by the specified predicate.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="predicate">The <see cref="Func{T, TResult}"/> delegate that defines the conditions of the elements to search for.</param>
    /// <returns>A <see cref="IReadOnlyCollection{T}"/> containing all the elements that match the conditions defined by the specified predicate, if found; otherwise, an empty <see cref="IReadOnlyCollection{T}"/>.</returns>
    /// <exception cref="ArgumentNullException"/>
    public static IReadOnlyCollection<TElement> FindAll<TElement>(this ICollection<TElement> source,
                                                                  [DisallowNull] Func<TElement, Boolean> predicate)
        where TElement : notnull
    {
        ArgumentNullException.ThrowIfNull(predicate);

        List<TElement> list = new();
        foreach (TElement element in source.Where(x => x is not null))
        {
            if (predicate.Invoke(element))
            {
                list.Add(element);
            }
        }

        return list.AsReadOnly();
    }

    /// <summary>
    /// Performs the specified action on each element of the <see cref="ICollection{T}"/>.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="action">The <see cref="Action{T}"/> delegate to perform on each element of the <see cref="ICollection{T}"/>.</param>
    /// <exception cref="ArgumentNullException"/>
    public static void ForEach<TElement>(this ICollection<TElement> source,
                                         [DisallowNull] Action<TElement> action)
        where TElement : notnull
    {
        ArgumentNullException.ThrowIfNull(action);

        foreach (TElement element in source.Where(x => x is not null))
        {
            action.Invoke(element);
        }
    }

    /// <summary>
    /// Removes all the elements that match the conditions defined by the specified predicate.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="predicate">The <see cref="Func{T, TResult}"/> delegate that defines the conditions of the elements to remove.</param>
    /// <returns>The number of elements removed from the <see cref="ICollection{T}"/>.</returns>
    /// <exception cref="ArgumentNullException"/>
    public static Int32 RemoveAll<TElement>(this ICollection<TElement> source,
                                            [DisallowNull] Func<TElement, Boolean> predicate)
        where TElement : notnull
    {
        ArgumentNullException.ThrowIfNull(predicate);

        List<TElement> remove = new();
        foreach (TElement element in source.Where(x => x is not null))
        {
            if (predicate.Invoke(element))
            {
                remove.Add(element);
            }
        }

        foreach (TElement element in remove)
        {
            if (source is INotifyPropertyChangingHelper changing)
            {
                changing.OnPropertyChanging(nameof(source.Count));
            }
            source.Remove(element);
            if (source is INotifyPropertyChangedHelper changed)
            {
                changed.OnPropertyChanged(nameof(source.Count));
            }
            if (source is INotifyCollectionChangedHelper collection)
            {
                collection.OnCollectionChanged(new(action: NotifyCollectionChangedAction.Remove,
                                                   changedItem: element));
            }
        }

        return remove.Count;
    }

    /// <summary>
    /// Determines whether every element in the <see cref="ICollection{T}"/> matches the conditions defined by the specified predicate.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="predicate">The <see cref="Func{T, TResult}"/> delegate that defines the conditions to check against the elements.</param>
    /// <returns>
    /// <see langword="true"/> if every element in the <see cref="ICollection{T}"/> matches the conditions
    /// defined by the specified predicate; otherwise, <see langword="false"/>. If the collection has no elements,
    /// the return value is <see langword="true"/>.
    /// </returns>
    /// <exception cref="ArgumentNullException"/>
    public static Boolean TrueForAll<TElement>(this ICollection<TElement> source,
                                               [DisallowNull] Func<TElement, Boolean> predicate)
        where TElement : notnull
    {
        ArgumentNullException.ThrowIfNull(predicate);

        foreach (TElement element in source.Where(x => x is not null))
        {
            if (!predicate.Invoke(element))
            {
                return false;
            }
        }

        return true;
    }
}