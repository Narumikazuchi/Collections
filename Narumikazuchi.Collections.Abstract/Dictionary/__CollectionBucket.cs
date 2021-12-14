namespace Narumikazuchi.Collections.Abstract;

[DebuggerDisplay("First: {First}; Last: {Last}")]
internal struct __CollectionBucket
{
    public __CollectionBucket()
    {
        this.First = -1;
        this.Last = -1;
    }

    public Int32 First { get; set; }
    public Int32 Last { get; set; }

    public Boolean IsEmpty =>
        this.First == -1 &&
        this.Last == -1;
}