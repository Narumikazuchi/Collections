﻿namespace Narumikazuchi.Collections;

/// <summary>
/// Represents a strongly-typed collection of key-value pairs with a significantly faster enumerator to
/// improve iteration times for <see langword="foreach"/>-loops.
/// </summary>
/// <remarks>
/// The faster enumeration only works if the collection is used as-is or by using the
/// <see cref="IStrongEnumerable{TElement, TEnumerator}"/> interface. If you you plan on using 
/// the <see cref="IEnumerable{T}"/> or any other derivative interface in your code then the 
/// efficiency of the enumerator will be lost due to call virtualization in the compiler generated IL.
/// </remarks>
public sealed partial class ReadOnlyDictionary<TKey, TValue, TEqualityComparer> : StrongEnumerable<KeyValuePair<TKey, TValue>, CommonDictionaryEnumerator<TKey, TValue>>
    where TKey : notnull
    where TEqualityComparer : IEqualityComparer<TKey>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ReadOnlyDictionary{TKey, TValue, TEqualityComparer}"/> type.
    /// </summary>
    /// <param name="items">The items that the resulting collection shall hold.</param>
    /// <param name="equalityComparer">The comparer that will be used to compare two instances of type <typeparamref name="TKey"/> for equality.</param>
    /// <exception cref="ArgumentNullException" />
    static public ReadOnlyDictionary<TKey, TValue, TEqualityComparer> CreateFrom<TEnumerable>(
#if NETCOREAPP3_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        [DisallowNull]
#endif
        NotNull<TEnumerable> items,
#if NETCOREAPP3_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        [DisallowNull]
#endif
        NotNull<TEqualityComparer> equalityComparer)
            where TEnumerable : IEnumerable<KeyValuePair<TKey, TValue>>
    {
        TEnumerable source = items;

        if (source is KeyValuePair<TKey, TValue>[] array)
        {
            return new(items: array,
                       equalityComparer: equalityComparer);
        }
#if NETCOREAPP3_1_OR_GREATER
        else if (source is ImmutableArray<KeyValuePair<TKey, TValue>> immutableArray)
        {
            return new(items: immutableArray.ToArray(),
                       equalityComparer: equalityComparer);
        }
#endif
        else if (source is List<KeyValuePair<TKey, TValue>> list)
        {
            return new(items: list.ToArray(),
                       equalityComparer: equalityComparer);
        }
        else if (source is Dictionary<TKey, TValue> dictionary)
        {
            return new(items: dictionary,
                       equalityComparer: equalityComparer);
        }
        else if (source is ICollection<KeyValuePair<TKey, TValue>> iCollection)
        {
            KeyValuePair<TKey, TValue>[] elements = new KeyValuePair<TKey, TValue>[iCollection.Count];
            iCollection.CopyTo(elements, 0);
            return new(items: elements,
                       equalityComparer: equalityComparer);
        }
        else if (source is IReadOnlyList<KeyValuePair<TKey, TValue>> iReadOnlyList)
        {
            KeyValuePair<TKey, TValue>[] elements = new KeyValuePair<TKey, TValue>[iReadOnlyList.Count];
            Int32 index = 0;
            while (index < iReadOnlyList.Count)
            {
                elements[index] = iReadOnlyList[index++];
            }

            return new(items: elements,
                       equalityComparer: equalityComparer);
        }
        else if (source is IHasCount counted)
        {
            KeyValuePair<TKey, TValue>[] elements = new KeyValuePair<TKey, TValue>[counted.Count];
            Int32 index = 0;
            foreach (KeyValuePair<TKey, TValue> element in source)
            {
                elements[index++] = element;
            }

            return new(items: elements,
                       equalityComparer: equalityComparer);
        }
        else
        {
            return new(items: source.ToArray(),
                       equalityComparer: equalityComparer);
        }
    }

    /// <inheritdoc/>
    public sealed override CommonDictionaryEnumerator<TKey, TValue> GetEnumerator()
    {
        return new(elements: m_Entries,
                   count: this.Count);
    }
    /// <inheritdoc/>
    public Boolean ContainsKey(
#if NETCOREAPP3_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        [DisallowNull]
#endif
        NotNull<TKey> key)
    {
        return this.FindEntry(key) > -1;
    }

    /// <inheritdoc/>
    public Boolean ContainsValue(
#if NETCOREAPP3_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        [DisallowNull]
#endif
        NotNull<TValue> value)
    {
        return this.Values.Contains(value);
    }

    /// <inheritdoc/>
    public Boolean TryGetValue(
#if NETCOREAPP3_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        [DisallowNull]
#endif
        NotNull<TKey> key,
#if NETCOREAPP3_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        [NotNullWhen(true)]
#endif
        out TValue? value)
    {
        Int32 index = this.FindEntry(key);
        if (index > -1)
        {
            value = m_Entries[index].Value!;
            return true;
        }
        else
        {
            value = default;
            return false;
        }
    }

    /// <inheritdoc/>
    public NotNull<TValue> this[NotNull<TKey> key]
    {
        get
        {
            Int32 index = this.FindEntry(key);
            if (index > -1)
            {
                return m_Entries[index].Value!;
            }
            else
            {
                throw new KeyNotFoundException();
            }
        }
    }

    /// <inheritdoc/>
    public ReadOnlyList<TKey> Keys { get; }

    /// <inheritdoc/>
    public ReadOnlyList<TValue> Values { get; }

    /// <inheritdoc/>
    public TEqualityComparer EqualityComparer { get; }
}