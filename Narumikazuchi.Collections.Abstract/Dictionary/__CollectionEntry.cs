namespace Narumikazuchi.Collections.Abstract;

[DebuggerDisplay("IsUsed: {IsUsed}; Key: {Key}; NextKey: {NextKey}; NextValue: {NextValue}; PreviousKey: {PreviousKey}; PreviousValue: {PreviousValue}; Value: {Value}")]
internal struct __CollectionEntry<TKey, TValue>
{
    public void Reset()
    {
        this.Key = default;
        this.Value = default;
        this.IsUsed = false;
        this.KeyHashcode = -1;
        this.ValueHashcode = -1;
        this.NextKey = -1;
        this.PreviousKey = -1;
        this.NextValue = -1;
        this.PreviousValue = -1;
    }

    [MaybeNull]
    public TKey Key { get; set; }
    [MaybeNull]
    public TValue Value { get; set; }
    public Boolean IsUsed { get; set; } = false;
    public Int32 KeyHashcode { get; set; } = -1;
    public Int32 ValueHashcode { get; set; } = -1;
    public Int32 NextKey { get; set; } = -1;
    public Int32 PreviousKey { get; set; } = -1;
    public Int32 NextValue { get; set; } = -1;
    public Int32 PreviousValue { get; set; } = -1;
}