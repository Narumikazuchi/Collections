namespace Narumikazuchi.Collections.Abstract;

[DebuggerDisplay("Key: {Key}; NextKey: {NextKey}; NextValue: {NextValue}; PreviousKey: {PreviousKey}; PreviousValue: {PreviousValue}; Value: {Value}")]
internal struct __CollectionEntry<TKey, TValue>
{
    [MaybeNull]
    public TKey Key { get; set; }
    [MaybeNull]
    public TValue Value { get; set; }
    public Int32 KeyHashcode { get; set; } = -1;
    public Int32 ValueHashcode { get; set; } = -1;
    public Int32 NextKey { get; set; } = -1;
    public Int32 PreviousKey { get; set; } = -1;
    public Int32 NextValue { get; set; } = -1;
    public Int32 PreviousValue { get; set; } = -1;
}