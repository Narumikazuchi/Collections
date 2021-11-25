namespace Narumikazuchi.Collections.Abstract;

/// <summary>
/// Represents an <see cref="IEnumerable{T}"/> that can be sorted.
/// </summary>
public interface ISortable<TElement> : IEnumerable<TElement>
{
    /// <summary>
    /// Sorts the contents of this object in the specifiedorder.
    /// </summary>
    /// <param name="direction">The <see cref="Collections.SortDirection"/> in which to sort the contents of this object.</param>
    public void Sort(in SortDirection direction);
    /// <summary>
    /// Sorts the contents of this object in the specified order using the specified comparision.
    /// </summary>
    /// <param name="direction">The <see cref="Collections.SortDirection"/> in which to sort the contents of this object.</param>
    /// <param name="comparison">The comparision to use for the sorting process.</param>
    public void Sort(in SortDirection direction, 
                     [DisallowNull] Comparison<TElement> comparison);
    /// <summary>
    /// Sorts the contents of this object in the specified order using the specified comparer.
    /// </summary>
    /// <param name="direction">The <see cref="Collections.SortDirection"/> in which to sort the contents of this object.</param>
    /// <param name="comparer">The <see cref="IComparer{T}"/> to use for the sorting process.</param>
    public void Sort(in SortDirection direction,
                     [AllowNull] IComparer<TElement>? comparer);

    /// <summary>
    /// Gets if the object is currently sorted.
    /// </summary>
    public Boolean IsSorted { get; }
    /// <summary>
    /// Gets the direction this collection is sorted in.
    /// </summary>
    public SortDirection SortDirection { get; }
}