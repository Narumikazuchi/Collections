namespace Narumikazuchi.Collections.Abstract;

/// <summary>
/// Represents a read-only collection of elements that can be accessed by index.
/// </summary>
public interface IReadOnlyList2<TIndex, TElement> : IReadOnlyCollection2<TElement>
{
    /// <summary>
    /// Determines the index of a specific tem in the <see cref="IReadOnlyList2{TIndex, TElement}"/>.
    /// </summary>
    /// <param name="item">The object to locate in the <see cref="IReadOnlyList2{TIndex, TElement}"/>.</param>
    /// <returns>The index of <paramref name="item"/> if found in the <see cref="IReadOnlyList2{TIndex, TElement}"/></returns>
    public TIndex IndexOf([DisallowNull] TElement item);
}

/// <summary>
/// Represents a read-only collection of elements that can be accessed by index.
/// </summary>
public interface IReadOnlyList2<TElement> : IReadOnlyList2<Int32, TElement>, IReadOnlyList<TElement>
{ }