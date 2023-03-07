namespace Narumikazuchi.Collections;

/// <summary>
/// Represents a strongly-typed read-only collection of elements.
/// </summary>
public interface IPerformsComparisons<TElement, TComparer>
    where TComparer : IComparer<TElement>
{
    /// <summary>
    /// Gets the <see cref="IComparer{T}"/> which is used by the <see cref="IPerformsComparisons{TElement, TComparer}"/>.
    /// </summary>
    [NotNull]
    public TComparer Comparer { get; }
}