﻿namespace Narumikazuchi.Collections.Extensions;

/// <summary>
/// Extends the <see cref="IList{T}"/> interface.
/// </summary>
static public class ListExtensions
{
    /// <summary>
    /// Searches the entire sorted <see cref="IList{T}"/> for an element using the default comparer and returns the zero-based index of the element.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="item">The object to locate. The value can be <see langword="null"/> for reference types.</param>
    /// <returns>
    /// The zero-based index of item the sorted <see cref="IList{T}"/>,
    /// if item is found; otherwise, a negative number that is the bitwise complement
    /// of the index of the next element that is larger than item or, if there is no
    /// larger element, the bitwise complement of <see cref="IReadOnlyCollection{T}.Count"/>.
    /// </returns>
    /// <exception cref="ArgumentNullException" />
    /// <exception cref="InvalidOperationException" />
    static public Int32 BinarySearch<TElement>(this IList<TElement> source,
                                               [DisallowNull] TElement item)
    {
#if NET6_0_OR_GREATER
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.ThrowIfNull(item);
#else
        if (source is null)
        {
            throw new ArgumentNullException(nameof(source));
        }
        
        if (item is null)
        {
            throw new ArgumentNullException(nameof(item));
        }
#endif

        if (source is List<TElement> list)
        {
            return list.BinarySearch(item);
        }
        else if (source is TElement[] array)
        {
            return Array.BinarySearch(array: array,
                                      value: item);
        }
        else
        {
            TElement[] elements = source.ToArray();
            return Array.BinarySearch(array: elements,
                                      value: item);
        }
    }
    /// <summary>
    /// Searches the entire sorted <see cref="IList{T}"/> for an element using the specified comparer and returns the zero-based index of the element.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="item">The object to locate. The value can be <see langword="null"/> for reference types.</param>
    /// <param name="comparer">The <see cref="IComparer{T}"/> implementation to use when comparing elements. -or- <see langword="null"/> to use the default comparer <see cref="Comparer{T}.Default"/>.</param>
    /// <returns>
    /// The zero-based index of item the sorted <see cref="IList{T}"/>,
    /// if item is found; otherwise, a negative number that is the bitwise complement
    /// of the index of the next element that is larger than item or, if there is no
    /// larger element, the bitwise complement of <see cref="IReadOnlyCollection{T}.Count"/>.</returns>
    /// <exception cref="ArgumentException" />
    /// <exception cref="ArgumentNullException" />
    /// <exception cref="InvalidOperationException" />
    static public Int32 BinarySearch<TElement, TComparer>(this IList<TElement> source,
                                                         [DisallowNull] TElement item,
                                                         [AllowNull] TComparer? comparer)
        where TComparer : IComparer<TElement>
    {
#if NET6_0_OR_GREATER
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.ThrowIfNull(item);
#else
        if (source is null)
        {
            throw new ArgumentNullException(nameof(source));
        }
        
        if (item is null)
        {
            throw new ArgumentNullException(nameof(item));
        }
#endif

        if (source is List<TElement> list)
        {
            return list.BinarySearch(item: item,
                                     comparer: comparer);
        }
        else if (source is TElement[] array)
        {
            return Array.BinarySearch(array: array,
                                      value: item,
                                      comparer: comparer);
        }
        else
        {
            TElement[] elements = source.ToArray();
            return Array.BinarySearch(array: elements,
                                      value: item,
                                      comparer: comparer);
        }
    }
    /// <summary>
    /// Searches the entire sorted <see cref="IList{T}"/> for an element using the specified comparer and returns the zero-based index of the element.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="index">The zero-based starting index of the range to search.</param>
    /// <param name="count">The length of the range to search.</param>
    /// <param name="item">The object to locate. The value can be <see langword="null"/> for reference types.</param>
    /// <param name="comparer">The <see cref="IComparer{T}"/> implementation to use when comparing elements. -or- <see langword="null"/> to use the default comparer <see cref="Comparer{T}.Default"/>.</param>
    /// <returns>
    /// The zero-based index of item the sorted <see cref="IList{T}"/>,
    /// if item is found; otherwise, a negative number that is the bitwise complement
    /// of the index of the next element that is larger than item or, if there is no
    /// larger element, the bitwise complement of <see cref="IReadOnlyCollection{T}.Count"/>.</returns>
    /// <exception cref="ArgumentException" />
    /// <exception cref="ArgumentNullException" />
    /// <exception cref="ArgumentOutOfRangeException" />
    /// <exception cref="InvalidOperationException" />
    static public Int32 BinarySearch<TElement, TComparer>(this IList<TElement> source,
                                                          Int32 index,
                                                          Int32 count,
                                                          [DisallowNull] TElement item,
                                                          [AllowNull] TComparer? comparer)
        where TComparer : IComparer<TElement>
    {
#if NET6_0_OR_GREATER
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.ThrowIfNull(item);
#else
        if (source is null)
        {
            throw new ArgumentNullException(nameof(source));
        }
        
        if (item is null)
        {
            throw new ArgumentNullException(nameof(item));
        }
#endif

        if (source is List<TElement> list)
        {
            return list.BinarySearch(item: item,
                                     comparer: comparer,
                                     index: index,
                                     count: count);
        }
        else if (source is TElement[] array)
        {
            return Array.BinarySearch(array: array,
                                      index: index,
                                      length: count,
                                      value: item,
                                      comparer: comparer);
        }
        else
        {
            TElement[] elements = source.ToArray();
            return Array.BinarySearch(array: elements,
                                      value: item,
                                      index: index,
                                      length: count,
                                      comparer: comparer);
        }
    }

    /// <summary>
    /// Searches for the first element that matches the conditions defined by the specified predicate, and returns the first occurrence withthe entire <see cref="IList{T}"/>.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="predicate">The <see cref="Func{T, TResult}"/> delegate that defines the conditions of the element to search for.</param>
    /// <returns>The first element that matches the conditions defined by the specified predicate, if found; otherwise, the default value for type <typeparamref name="TElement"/>.</returns>
    /// <exception cref="ArgumentNullException"/>
    static public TElement? FindFirstOrDefault<TElement>(this IList<TElement> source,
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

        for (Int32 index = 0;
             index < source.Count;
             index++)
        {
            if (predicate.Invoke(source[index]))
            {
                return source[index];
            }
        }

        return default;
    }

    /// <summary>
    /// Searches for the first element that matches the conditions defined by the specified 
    /// predicate, and returns the zero-based index of the first occurrence withthe
    /// entire <see cref="IList{T}"/>.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="predicate">The <see cref="Func{T, TResult}"/> delegate that defines the conditions of the element to search for.</param>
    /// <returns>The zero-based index of the first occurrence of an element that matches the conditions defined by <paramref name="predicate"/>, if found; otherwise, -1.</returns>
    /// <exception cref="ArgumentNullException"/>
    static public Int32 FindFirstIndex<TElement>(this IList<TElement> source,
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

        for (Int32 index = 0;
             index < source.Count;
             index++)
        {
            if (predicate.Invoke(source[index]))
            {
                return index;
            }
        }

        return -1;
    }
    /// <summary>
    /// Searches for the first element that matches the conditions defined by the specified 
    /// predicate, and returns the zero-based index of the first occurrence withthe
    /// entire <see cref="IList{T}"/>.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="startIndex">The zero-based starting index of the search.</param>
    /// <param name="predicate">The <see cref="Func{T, TResult}"/> delegate that defines the conditions of the element to search for.</param>
    /// <returns>The zero-based index of the first occurrence of an element that matches the conditions defined by <paramref name="predicate"/>, if found; otherwise, -1.</returns>
    /// <exception cref="ArgumentNullException"/>
    /// <exception cref="ArgumentOutOfRangeException"/>
    static public Int32 FindFirstIndex<TElement>(this IList<TElement> source,
                                                 Int32 startIndex,
                                                 [DisallowNull] Func<TElement, Boolean> predicate)
    {
#if NET6_0_OR_GREATER
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.ThrowIfNull(predicate);
        startIndex.ThrowIfOutOfRange(0, source.Count - 1);
#else
        if (source is null)
        {
            throw new ArgumentNullException(nameof(source));
        }
        
        if (predicate is null)
        {
            throw new ArgumentNullException(nameof(predicate));
        }
        
        if (startIndex < 0 ||
            startIndex > source.Count - 1)
        {
            throw new ArgumentOutOfRangeException(nameof(startIndex));
        }
#endif

        for (Int32 index = startIndex;
             index < source.Count;
             index++)
        {
            if (predicate.Invoke(source[index]))
            {
                return index;
            }
        }

        return -1;
    }
    /// <summary>
    /// Searches for the first element that matches the conditions defined by the specified 
    /// predicate, and returns the zero-based index of the first occurrence withthe
    /// entire <see cref="IList{T}"/>.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="startIndex">The zero-based starting index of the search.</param>
    /// <param name="count">The number of elements the section to search.</param>
    /// <param name="predicate">The <see cref="Func{T, TResult}"/> delegate that defines the conditions of the element to search for.</param>
    /// <returns>The zero-based index of the first occurrence of an element that matches the conditions defined by <paramref name="predicate"/>, if found; otherwise, -1.</returns>
    /// <exception cref="ArgumentNullException"/>
    /// <exception cref="ArgumentOutOfRangeException"/>
    static public Int32 FindFirstIndex<TElement>(this IList<TElement> source,
                                                 Int32 startIndex,
                                                 Int32 count,
                                                 [DisallowNull] Func<TElement, Boolean> predicate)
    {
#if NET6_0_OR_GREATER
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.ThrowIfNull(predicate);
        startIndex.ThrowIfOutOfRange(0, source.Count - 1);
        count.ThrowIfOutOfRange(1, source.Count - startIndex);
#else
        if (source is null)
        {
            throw new ArgumentNullException(nameof(source));
        }
        
        if (predicate is null)
        {
            throw new ArgumentNullException(nameof(predicate));
        }
        
        if (startIndex < 0 ||
            startIndex > source.Count - 1)
        {
            throw new ArgumentOutOfRangeException(nameof(startIndex));
        }

        if (count < 1 ||
            count > source.Count - startIndex)
        {
            throw new ArgumentOutOfRangeException(nameof(count));
        }
#endif

        for (Int32 index = startIndex;
             index < startIndex + count;
             index++)
        {
            if (predicate.Invoke(source[index]))
            {
                return index;
            }
        }

        return -1;
    }

    /// <summary>
    /// Searches for the last element that matches the conditions defined by the specified predicate, and returns the last occurrence withthe entire <see cref="IList{T}"/>.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="predicate">The <see cref="Func{T, TResult}"/> delegate that defines the conditions of the element to search for.</param>
    /// <returns>The last element that matches the conditions defined by the specified predicate, if found; otherwise, the default value for type <typeparamref name="TElement"/>.</returns>
    /// <exception cref="ArgumentNullException"/>
    static public TElement? FindLastOrDefault<TElement>(this IList<TElement> source,
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

        for (Int32 index = source.Count - 1;
             index > 0;
             index--)
        {
            if (predicate.Invoke(source[index]))
            {
                return source[index];
            }
        }

        return default;
    }

    /// <summary>
    /// Searches for the last element that matches the conditions defined by the specified 
    /// predicate, and returns the zero-based index of the last occurrence withthe
    /// entire <see cref="IList{T}"/>.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="predicate">The <see cref="Func{T, TResult}"/> delegate that defines the conditions of the element to search for.</param>
    /// <returns>The zero-based index of the last occurrence of an element that matches the conditions defined by <paramref name="predicate"/>, if found; otherwise, -1.</returns>
    /// <exception cref="ArgumentNullException"/>
    static public Int32 FindLastIndex<TElement>(this IList<TElement> source,
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

        for (Int32 index = source.Count - 1;
             index > 0;
             index--)
        {
            if (predicate.Invoke(source[index]))
            {
                return index;
            }
        }

        return -1;
    }
    /// <summary>
    /// Searches for the last element that matches the conditions defined by the specified 
    /// predicate, and returns the zero-based index of the last occurrence withthe
    /// entire <see cref="IList{T}"/>.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="startIndex">The zero-based starting index of the search.</param>
    /// <param name="predicate">The <see cref="Func{T, TResult}"/> delegate that defines the conditions of the element to search for.</param>
    /// <returns>The zero-based index of the last occurrence of an element that matches the conditions defined by <paramref name="predicate"/>, if found; otherwise, -1.</returns>
    /// <exception cref="ArgumentNullException"/>
    /// <exception cref="ArgumentOutOfRangeException"/>
    static public Int32 FindLastIndex<TElement>(this IList<TElement> source,
                                                Int32 startIndex,
                                                [DisallowNull] Func<TElement, Boolean> predicate)
    {
#if NET6_0_OR_GREATER
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.ThrowIfNull(predicate);
        startIndex.ThrowIfOutOfRange(0, source.Count - 1);
#else
        if (source is null)
        {
            throw new ArgumentNullException(nameof(source));
        }
        
        if (predicate is null)
        {
            throw new ArgumentNullException(nameof(predicate));
        }
        
        if (startIndex < 0 ||
            startIndex > source.Count - 1)
        {
            throw new ArgumentOutOfRangeException(nameof(startIndex));
        }
#endif

        for (Int32 i = startIndex;
             i > 0;
             i--)
        {
            if (predicate.Invoke(source[i]))
            {
                return i;
            }
        }

        return -1;
    }
    /// <summary>
    /// Searches for the last element that matches the conditions defined by the specified 
    /// predicate, and returns the zero-based index of the last occurrence withthe
    /// entire <see cref="IList{T}"/>.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="startIndex">The zero-based starting index of the search.</param>
    /// <param name="count">The number of elements the section to search.</param>
    /// <param name="predicate">The <see cref="Func{T, TResult}"/> delegate that defines the conditions of the element to search for.</param>
    /// <returns>The zero-based index of the last occurrence of an element that matches the conditions defined by <paramref name="predicate"/>, if found; otherwise, -1.</returns>
    /// <exception cref="ArgumentNullException"/>
    /// <exception cref="ArgumentOutOfRangeException"/>
    static public Int32 FindLastIndex<TElement>(this IList<TElement> source,
                                                Int32 startIndex,
                                                Int32 count,
                                                [DisallowNull] Func<TElement, Boolean> predicate)
    {
#if NET6_0_OR_GREATER
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.ThrowIfNull(predicate);
        startIndex.ThrowIfOutOfRange(0, source.Count - 1);
        count.ThrowIfOutOfRange(1, source.Count - startIndex);
#else
        if (source is null)
        {
            throw new ArgumentNullException(nameof(source));
        }
        
        if (predicate is null)
        {
            throw new ArgumentNullException(nameof(predicate));
        }
        
        if (startIndex < 0 ||
            startIndex > source.Count - 1)
        {
            throw new ArgumentOutOfRangeException(nameof(startIndex));
        }

        if (count < 1 ||
            count > source.Count - startIndex)
        {
            throw new ArgumentOutOfRangeException(nameof(count));
        }
#endif

        for (Int32 index = startIndex;
             index > startIndex - count;
             index--)
        {
            if (predicate.Invoke(source[index]))
            {
                return index;
            }
        }

        return -1;
    }

    /// <summary>
    /// Creates a shallow copy of a range of elements the source <see cref="IList{T}"/>.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="startIndex">The zero-based <see cref="IList{T}"/> index at which the range starts.</param>
    /// <param name="count">The number of elements the range.</param>
    /// <returns>A shallow copy of a range of elements the source <see cref="IList{T}"/>.</returns>
    /// <exception cref="ArgumentException"/>
    /// <exception cref="ArgumentOutOfRangeException"/>
    static public ReadOnlyList<TElement> GetRange<TElement>(this IList<TElement> source,
                                                            Int32 startIndex,
                                                            Int32 count)
    {
#if NET6_0_OR_GREATER
        ArgumentNullException.ThrowIfNull(source);
        startIndex.ThrowIfOutOfRange(0, source.Count - 1);
        count.ThrowIfOutOfRange(1, source.Count - startIndex);
#else
        if (source is null)
        {
            throw new ArgumentNullException(nameof(source));
        }
        
        if (startIndex < 0 ||
            startIndex > source.Count - 1)
        {
            throw new ArgumentOutOfRangeException(nameof(startIndex));
        }

        if (count < 1 ||
            count > source.Count - startIndex)
        {
            throw new ArgumentOutOfRangeException(nameof(count));
        }
#endif

        if (source is List<TElement> list)
        {
            return ReadOnlyList<TElement>.CreateFrom(list.GetRange(index: startIndex,
                                                                   count: count));
        }
        else
        {
            return ReadOnlyList<TElement>.CreateFrom(source.Skip(startIndex)
                                                           .Take(count)
                                                           .ToArray());
        }
    }

    /// <summary>
    /// Searches for the specified object and returns the zero-based index of the first occurrence withthe entire <see cref="IList{T}"/>.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="item">The object to locate the <see cref="IList{T}"/>. The value can be null for reference types.</param>
    /// <param name="equalityComparer">The equality comparer that will be used to compare two instances of <typeparamref name="TElement"/>.</param>
    /// <returns>The zero-based index of the first occurrence of item withthe entire <see cref="IList{T}"/>, if found; otherwise, -1.</returns>
    /// <exception cref="ArgumentNullException"/>
    static public Int32 IndexOf<TElement, TEqualityComparer>(this IList<TElement> source,
                                                             [DisallowNull] TElement item,
                                                             [DisallowNull] TEqualityComparer equalityComparer)
        where TEqualityComparer : IEqualityComparer<TElement>
    {
#if NET6_0_OR_GREATER
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.ThrowIfNull(item);
        ArgumentNullException.ThrowIfNull(equalityComparer);
#else
        if (source is null)
        {
            throw new ArgumentNullException(nameof(source));
        }
        
        if (item is null)
        {
            throw new ArgumentNullException(nameof(item));
        }
        
        if (equalityComparer is null)
        {
            throw new ArgumentNullException(nameof(equalityComparer));
        }
#endif

        for (Int32 index = 0;
             index < source.Count;
             index++)
        {
            if (equalityComparer.Equals(x: item,
                                        y: source[index]))
            {
                return index;
            }
        }

        return -1;
    }

    /// <summary>
    /// Inserts the elements of a collection into the <see cref="IList{T}"/> at the specified index.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="index">The zero-based index at which the new elements should be inserted.</param>
    /// <param name="items">The collection whose elements should be inserted into the <see cref="IList{T}"/>.</param>
    /// <exception cref="ArgumentNullException"/>
    /// <exception cref="ArgumentOutOfRangeException"/>
    static public void InsertRange<TEnumerable, TElement>(this IList<TElement> source,
                                                          Int32 index,
                                                          [DisallowNull] TEnumerable items)
        where TEnumerable : IEnumerable<TElement>
    {
#if NET6_0_OR_GREATER
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.ThrowIfNull(items);
        index.ThrowIfOutOfRange(0, source.Count - 1);
#else
        if (source is null)
        {
            throw new ArgumentNullException(nameof(source));
        }
        
        if (items is null)
        {
            throw new ArgumentNullException(nameof(items));
        }

        if (index < 0 ||
            index > source.Count - 1)
        {
            throw new ArgumentOutOfRangeException(nameof(index));
        }
#endif

        TEnumerable enumerable = items;
        Int32 i = index;
        foreach (TElement item in enumerable)
        {
            if (source is INotifyPropertyChangingHelper changing)
            {
                changing.OnPropertyChanging(nameof(source.Count));
            }
            source.Insert(index: i++,
                          item: item);
            if (source is INotifyPropertyChangedHelper changed)
            {
                changed.OnPropertyChanged(nameof(source.Count));
            }
            if (source is INotifyCollectionChangedHelper collection)
            {
                collection.OnCollectionChanged(new(action: NotifyCollectionChangedAction.Add,
                                                   changedItem: item));
            }
        }
    }

    /// <summary>
    /// Searches for the specified object and returns the zero-based index of the last occurrence withthe entire <see cref="IList{T}"/>.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="item">The object to locate the <see cref="IList{T}"/>.</param>
    /// <returns>The zero-based index of the last occurrence of item withthe entire the <see cref="IList{T}"/>, if found; otherwise, -1.</returns>
    /// <exception cref="ArgumentNullException"/>
    static public Int32 LastIndexOf<TElement>(this IList<TElement> source,
                                              [DisallowNull] TElement item)
    {
#if NET6_0_OR_GREATER
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.ThrowIfNull(item);
#else
        if (source is null)
        {
            throw new ArgumentNullException(nameof(source));
        }
        
        if (item is null)
        {
            throw new ArgumentNullException(nameof(item));
        }
#endif

        return LastIndexOf(source: source,
                           item: item,
                           equalityComparer: EqualityComparer<TElement>.Default);
    }

    /// <summary>
    /// Searches for the specified object and returns the zero-based index of the last occurrence withthe entire <see cref="IList{T}"/>.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="item">The object to locate the <see cref="IList{T}"/>.</param>
    /// <param name="equalityComparer">The equality comparer that will be used to compare two instances of <typeparamref name="TElement"/>.</param>
    /// <returns>The zero-based index of the last occurrence of item withthe entire the <see cref="IList{T}"/>, if found; otherwise, -1.</returns>
    /// <exception cref="ArgumentNullException"/>
    static public Int32 LastIndexOf<TElement, TEqualityComparer>(this IList<TElement> source,
                                                                 [DisallowNull] TElement item,
                                                                 [DisallowNull] TEqualityComparer equalityComparer)
        where TEqualityComparer : IEqualityComparer<TElement>
    {
#if NET6_0_OR_GREATER
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.ThrowIfNull(item);
        ArgumentNullException.ThrowIfNull(equalityComparer);
#else
        if (source is null)
        {
            throw new ArgumentNullException(nameof(source));
        }
        
        if (item is null)
        {
            throw new ArgumentNullException(nameof(item));
        }
        
        if (equalityComparer is null)
        {
            throw new ArgumentNullException(nameof(equalityComparer));
        }
#endif

        for (Int32 index = source.Count - 1;
             index > 0;
             index--)
        {
            if (equalityComparer.Equals(x: item,
                                        y: source[index]))
            {
                return index;
            }
        }

        return -1;
    }

    /// <summary>
    /// Moves the item at the given index one position the specified direction the <see cref="IList{T}"/>.
    /// </summary>
    static public void MoveItem<TElement>(this IList<TElement> source,
                                          Int32 index,
                                          ItemMoveDirection direction)
    {
#if NET6_0_OR_GREATER
        ArgumentNullException.ThrowIfNull(source);
#else
        if (source is null)
        {
            throw new ArgumentNullException(nameof(source));
        }
#endif

        TElement tmp;
        if (direction == ItemMoveDirection.ToLowerIndex)
        {
            if (index > 0)
            {
                tmp = source[index];
                source[index] = source[index - 1];
                source[index - 1] = tmp;
            }
        }
        else
        {
            if (index > -1 && index < source.Count - 1)
            {
                tmp = source[index];
                source[index] = source[index + 1];
                source[index + 1] = tmp;
            }
        }
    }

    /// <summary>
    /// Moves the item at the given index the given amount of positions the specified direction the <see cref="IList{T}"/>.
    /// </summary>
    static public void MoveItem<TElement>(this IList<TElement> source,
                                          Int32 index,
                                          ItemMoveDirection direction,
                                          Int32 positions)
    {
#if NET6_0_OR_GREATER
        ArgumentNullException.ThrowIfNull(source);
#else
        if (source is null)
        {
            throw new ArgumentNullException(nameof(source));
        }
#endif

        if (positions == 0)
        {
            return;
        }

        TElement item = source[index];
        source.RemoveAt(index);
        if (positions > 0)
        {
            source.Insert(index: index + positions,
                          item: item);
            return;
        }
        else
        {
            source.Insert(index: index - positions,
                          item: item);
        }
    }

    /// <summary>
    /// Moves the item one position the specified direction the <see cref="IList{T}"/>.
    /// </summary>
    static public void MoveItem<TElement>(this IList<TElement> source,
                                          [DisallowNull] TElement item,
                                          ItemMoveDirection direction)
    {
#if NET6_0_OR_GREATER
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.ThrowIfNull(item);
#else
        if (source is null)
        {
            throw new ArgumentNullException(nameof(source));
        }
        
        if (item is null)
        {
            throw new ArgumentNullException(nameof(item));
        }
#endif

        Int32 index = source.IndexOf(item);
        if (index == -1)
        {
            return;
        }

        if (direction == ItemMoveDirection.ToLowerIndex)
        {
            if (index > 0)
            {
                source[index] = source[index - 1];
                source[index - 1] = item;
            }
        }
        else
        {
            if (index < source.Count - 1)
            {
                source[index] = source[index + 1];
                source[index + 1] = item;
            }
        }
    }

    /// <summary>
    /// Moves the item at the given index the given amount of positions the specified direction the <see cref="IList{T}"/>.
    /// </summary>
    static public void MoveItem<TElement>(this IList<TElement> source,
                                          [DisallowNull] TElement item,
                                          ItemMoveDirection direction,
                                          Int32 positions)
    {
#if NET6_0_OR_GREATER
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.ThrowIfNull(item);
#else
        if (source is null)
        {
            throw new ArgumentNullException(nameof(source));
        }
        
        if (item is null)
        {
            throw new ArgumentNullException(nameof(item));
        }
#endif

        if (positions == 0)
        {
            return;
        }

        Int32 index = source.IndexOf(item);
        if (index == -1)
        {
            return;
        }

        source.RemoveAt(index);
        if (positions > 0)
        {
            source.Insert(index: index + positions,
                          item: item);
            return;
        }
        else
        {
            source.Insert(index: index - positions,
                          item: item);
        }
    }

    /// <summary>
    /// Removes a range of elements from the <see cref="IList{T}"/>.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="startIndex">The zero-based starting index of the range of elements to remove.</param>
    /// <param name="count">The number of elements to remove.</param>
    /// <exception cref="ArgumentOutOfRangeException"/>
    static public void RemoveRange<TElement>(this IList<TElement> source,
                                             Int32 startIndex,
                                             Int32 count)
    {
#if NET6_0_OR_GREATER
        ArgumentNullException.ThrowIfNull(source);
        startIndex.ThrowIfOutOfRange(0, source.Count - 1);
        count.ThrowIfOutOfRange(1, source.Count - startIndex);
#else
        if (source is null)
        {
            throw new ArgumentNullException(nameof(source));
        }
        
        if (startIndex < 0 ||
            startIndex > source.Count - 1)
        {
            throw new ArgumentOutOfRangeException(nameof(startIndex));
        }

        if (count < 1 ||
            count > source.Count - startIndex)
        {
            throw new ArgumentOutOfRangeException(nameof(count));
        }
#endif

        for (Int32 index = startIndex;
             index > startIndex - count;
             index--)
        {
            if (source is INotifyPropertyChangingHelper changing)
            {
                changing.OnPropertyChanging(nameof(source.Count));
            }
            TElement item = source[index];
            source.RemoveAt(index);
            if (source is INotifyPropertyChangedHelper changed)
            {
                changed.OnPropertyChanged(nameof(source.Count));
            }
            if (source is INotifyCollectionChangedHelper collection)
            {
                collection.OnCollectionChanged(new(action: NotifyCollectionChangedAction.Remove,
                                                   changedItem: item));
            }
        }
    }
}