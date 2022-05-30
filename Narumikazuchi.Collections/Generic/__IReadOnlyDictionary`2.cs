namespace Narumikazuchi.Collections;

internal interface __IReadOnlyDictionary<TKey, TValue>
    where TKey : notnull
{
    internal Int32 Size { get; }

    internal Int32[] Buckets { get; }

    internal __DictionaryEntry<TKey, TValue>[] Entries { get; }

    internal IEqualityComparer<TKey> EqualityComparer { get; }

    internal ReadOnlyCollection<TKey> Keys { get; }

    internal ReadOnlyCollection<TValue> Values { get; }
}