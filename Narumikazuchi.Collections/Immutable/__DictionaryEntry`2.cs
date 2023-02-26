namespace Narumikazuchi.Collections;

internal struct __DictionaryEntry<TKey, TValue>
    where TKey : notnull
{
    public Int32 HashCode { get; set; }

    public Int32 Next { get; set; }

    public TKey Key { get; set; }

    public TValue Value { get; set; }
}