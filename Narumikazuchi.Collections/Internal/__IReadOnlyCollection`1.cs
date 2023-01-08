namespace Narumikazuchi.Collections;

internal interface __IReadOnlyCollection<TElement>
{
    internal Boolean TryGetReadOnlyArray(
#if NETCOREAPP3_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        [NotNullWhen(true)]
#endif
        out TElement[]? array);

    internal Boolean TryGetReadOnlyList(
#if NETCOREAPP3_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        [NotNullWhen(true)]
#endif
        out List<TElement>? list);
}