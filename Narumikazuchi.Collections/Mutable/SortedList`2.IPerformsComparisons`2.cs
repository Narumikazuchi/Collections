namespace Narumikazuchi.Collections;

public partial class SortedList<TElement, TComparer> : IPerformsComparisons<TElement, TComparer>
{
    /// <summary>
    /// Gets the <typeparamref name="TComparer"/> which is used by the <see cref="SortedList{TElement, TComparer}"/>.
    /// </summary>
#if NETCOREAPP3_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
    [NotNull]
#endif
    public NotNull<TComparer> Comparer { get; }
}