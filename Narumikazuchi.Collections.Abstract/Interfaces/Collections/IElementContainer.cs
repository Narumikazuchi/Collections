namespace Narumikazuchi.Collections.Abstract;

/// <summary>
/// Represents a strongly-typed, read-only collection of elements.
/// </summary>
public interface IElementContainer : 
    ICollection
{
    /// <summary>
    /// Determines whether the <see cref="IElementContainer"/> contains a specific value.
    /// </summary>
    /// <param name="item">The object to locate in the <see cref="IElementContainer"/>.</param>
    /// <returns><see langword="true"/> if <paramref name="item"/> is found in the <see cref="IElementContainer"/>; otherwise, <see langword="false"/></returns>
    [Pure]
    public Boolean Contains([AllowNull] Object? item);
}