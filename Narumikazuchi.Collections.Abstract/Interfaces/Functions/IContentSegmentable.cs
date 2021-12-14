namespace Narumikazuchi.Collections.Abstract;

/// <summary>
/// Represents the functionality to retrieve only a part of an <see cref="IEnumerable"/> that is accessible by index.
/// </summary>
public interface IContentSegmentable<TIndex, TElement>
    where TIndex : IComparable<TIndex>
{
    /// <summary>
    /// Creates a shallow copy of the elements of this <see cref="IContentSegmentable{TIndex, TElement}"/> from the specified range.
    /// </summary>
    /// <remarks>
    /// Both of the elements at <paramref name="startIndex"/> as well as <paramref name="endIndex"/> are included in the result.
    /// </remarks>
    /// <param name="startIndex">The index of the first item in the resulting range.</param>
    /// <param name="endIndex">The index of the last item in the resulting range.</param>
    /// <returns>An <see cref="ICollection{T}"/> containing the items from the specified range</returns>
    [Pure]
    [return: NotNull]
    public ICollection<TElement> GetRange([DisallowNull] in TIndex startIndex,
                                          [DisallowNull] in TIndex endIndex);
}