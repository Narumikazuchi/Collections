namespace Narumikazuchi.Collections;

internal interface __IReadOnlyCollection<TElement>
{
    internal Boolean TryGetReadOnlyArray([NotNullWhen(true)] out TElement[]? array);

    internal Boolean TryGetReadOnlyList([NotNullWhen(true)] out List<TElement>? list);
}