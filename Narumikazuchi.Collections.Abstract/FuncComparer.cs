namespace Narumikazuchi.Collections;

internal readonly struct __FuncComparer<TElement> : IComparer<TElement>
{
    public __FuncComparer(Comparison<TElement> comparison)
    {
        this.Comparison = comparison;
    }

    public Int32 Compare(TElement? left, 
                         TElement? right) => 
        this.Comparison
            .Invoke(x: left!, 
                    y: right!);

    public Comparison<TElement> Comparison { get; }
}