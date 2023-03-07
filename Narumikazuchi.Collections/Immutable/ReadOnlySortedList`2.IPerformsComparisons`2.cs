namespace Narumikazuchi.Collections;

public partial class ReadOnlySortedList<TElement, TComparer> : IPerformsComparisons<TElement, TComparer>
{
    /// <inheritdoc/>
    [NotNull]
    public TComparer Comparer { get; }
}