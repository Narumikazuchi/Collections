namespace Narumikazuchi.Collections.Abstract;

internal sealed class __Collection<TElement> : CollectionBase<Int32, TElement>
{
    public __Collection() :
        base()
    { }
    public __Collection(IEnumerable<TElement> source) :
        base()
    {
        foreach (TElement element in source)
        {
            this.Insert(index: this.Count,
                        item: element);
        }
    }
}