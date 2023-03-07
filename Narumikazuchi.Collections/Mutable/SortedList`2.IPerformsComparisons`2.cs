namespace Narumikazuchi.Collections;

public partial class SortedList<TElement, TComparer> : IPerformsComparisons<TElement, TComparer>
{
    /// <summary>
    /// Gets the <typeparamref name="TComparer"/> which is used by the <see cref="SortedList{TElement, TComparer}"/>.
    /// </summary>
    [NotNull]
    public TComparer Comparer { get; }
}