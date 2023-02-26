namespace Narumikazuchi.Collections;

public partial class ReadOnlySortedList<TElement, TComparer> : IPerformsComparisons<TElement, TComparer>
{
    /// <inheritdoc/>
#if NETCOREAPP3_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
    [NotNull]
#endif
    public NotNull<TComparer> Comparer { get; }
}