namespace Narumikazuchi.Collections;

internal interface __IReadOnlyDictionary<TKey, TValue, TEqualityComparer>
    where TKey : notnull
    where TEqualityComparer : IEqualityComparer<TKey>
{
    internal Int32 Size { get; }

    internal Int32[] Buckets { get; }

    internal __DictionaryEntry<TKey, TValue>[] Entries { get; }

    internal TEqualityComparer EqualityComparer { get; }

    internal ReadOnlyCollection<TKey> Keys { get; }

    internal ReadOnlyCollection<TValue> Values { get; }
}