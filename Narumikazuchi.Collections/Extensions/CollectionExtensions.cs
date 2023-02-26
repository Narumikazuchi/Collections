namespace Narumikazuchi.Collections.Extensions;

/// <summary>
/// Extends the <see cref="ICollection{T}"/> interface.
/// </summary>
static public class CollectionExtensions
{
    /// <summary>
    /// Adds the elements of the specified collection to the end of the <see cref="ICollection{T}"/>.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="other">The collection to add.</param>
    /// <exception cref="ArgumentNullException"/>
    static public void AddRange<TElement, TOther>(
        this ICollection<TElement> source,
#if NETCOREAPP3_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        [DisallowNull]
#endif
        NotNull<TOther> other)
            where TOther : IEnumerable<TElement>
    {
        TOther enumerable = other;
        foreach (TElement element in enumerable)
        {
            source.Add(element);
        }
    }

    /// <summary>
    /// Converts the elements in the current <see cref="ICollection{T}"/> to another type, and returns a list containing the converted elements.
    /// </summary>
    /// <param name="source"></param>
    /// <returns>A <see cref="IReadOnlyCollection{T}"/> of the target type containing the converted elements from the current <see cref="ICollection{T}"/>.</returns>
    /// <exception cref="ArgumentNullException"/>
    static public NotNull<ReadOnlyList<TOther>> ConvertAll<TElement, TOther>(this ICollection<TElement> source)
        where TElement : IConvertible<TOther>
    {
        TOther[] result = new TOther[source.Count];
        Int32 index = 0;
        foreach (TElement element in source)
        {
            result[index++] = Converter<TOther>.ToType<TElement>(element);
        }

        return new(result);
    }
    /// <summary>
    /// Converts the elements in the current <see cref="ICollection{T}"/> to another type, and returns a list containing the converted elements.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="converter">A <see cref="Converter{TInput, TOutput}"/> delegate that converts each element from one type to another type.</param>
    /// <returns>A <see cref="IReadOnlyCollection{T}"/> of the target type containing the converted elements from the current <see cref="ICollection{T}"/>.</returns>
    /// <exception cref="ArgumentNullException"/>
    static public NotNull<ReadOnlyList<TOther>> ConvertAll<TElement, TOther>(
        this ICollection<TElement> source,
#if NETCOREAPP3_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        [DisallowNull]
#endif
        Converter<TElement, TOther> converter)
    {
#if NET6_0_OR_GREATER
        ArgumentNullException.ThrowIfNull(converter);
#else
        if (converter is null)
        {
            throw new ArgumentNullException(nameof(converter));
        }
#endif

        TOther[] result = new TOther[source.Count];
        Int32 index = 0;
        foreach (TElement element in source)
        {
            result[index++] = converter.Invoke(element);
        }

        return new(result);
    }

    /// <summary>
    /// Determines whether the <see cref="ICollection{T}"/> contains elements that match the conditions defined by the specified predicate.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="predicate">The <see cref="Func{T, TResult}"/> delegate that defines the conditions of the elements to search for.</param>
    /// <returns><see langword="true"/> if the <see cref="ICollection{T}"/> contains one or more elements that match the conditions defined by the specified predicate; otherwise, <see langword="false"/>.</returns>
    /// <exception cref="ArgumentNullException"/>
    static public Boolean Exists<TElement>(
        this ICollection<TElement> source,
#if NETCOREAPP3_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        [DisallowNull]
#endif
        Func<TElement, Boolean> predicate)
    {
#if NET6_0_OR_GREATER
        ArgumentNullException.ThrowIfNull(predicate);
#else
        if (predicate is null)
        {
            throw new ArgumentNullException(nameof(predicate));
        }
#endif

        foreach (TElement element in source)
        {
            if (predicate.Invoke(element))
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// Removes all the elements that match the conditions defined by the specified predicate.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="predicate">The <see cref="Func{T, TResult}"/> delegate that defines the conditions of the elements to remove.</param>
    /// <returns>The number of elements removed from the <see cref="ICollection{T}"/>.</returns>
    /// <exception cref="ArgumentNullException"/>
    static public Int32 RemoveAll<TElement>(
        this ICollection<TElement> source,
#if NETCOREAPP3_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        [DisallowNull]
#endif
        Func<TElement, Boolean> predicate)
    {
#if NET6_0_OR_GREATER
        ArgumentNullException.ThrowIfNull(predicate);
#else
        if (predicate is null)
        {
            throw new ArgumentNullException(nameof(predicate));
        }
#endif

        Int32 count = 0;
        foreach (TElement element in source.Where(predicate))
        {
            source.Remove(element);
            count++;
        }

        return count;
    }
}