namespace Narumikazuchi.Collections;

internal readonly struct __FuncEqualityComparer<TElement> : IEqualityComparer<TElement>
{
    public __FuncEqualityComparer(EqualityComparison<TElement> comparison)
    {
        ExceptionHelpers.ThrowIfArgumentNull(comparison);

        this.Comparison = comparison;
    }

    public Boolean Equals(TElement? left, 
                          TElement? right) => 
        this.Comparison.Invoke(first: left!, 
                               second: right!);

    public Int32 GetHashCode(TElement obj) => 
        obj is null
            ? 0
            : obj.GetHashCode();

    private static Boolean DefaultCompare(TElement first,
                                          TElement second)
    {
        if (typeof(TElement).IsValueType)
        {
            return first!.Equals(obj: second);
        }
        if (first is IEquatable<TElement> eq)
        {
            return eq.Equals(other: second);
        }
        return ReferenceEquals(objA: first, 
                               objB: second);
    }

    public static ref readonly __FuncEqualityComparer<TElement> Default => ref _default;

    public EqualityComparison<TElement> Comparison { get; }

    private static readonly __FuncEqualityComparer<TElement> _default = new(DefaultCompare);
}