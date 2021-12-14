namespace Narumikazuchi.Collections.Abstract;

/// <summary>
/// Represents a collection which allows the addition of items to itself.
/// </summary>
public interface IContentAddable<TElement>
{
    /// <summary>
    /// Adds an object to the end of the <see cref="IContentAddable{TElement}"/>.
    /// </summary>
    /// <param name="item">The object to be added to the end of the <see cref="IContentAddable{TElement}"/>. 
    /// The value can be <see langword="null"/> for reference types.</param>
    /// <returns><see langword="true"/> if the item was added; otherwise, <see langword="false"/></returns>
    public Boolean Add(TElement item);

    /// <summary>
    /// Adds the elements of the specified collection to the end of the <see cref="IContentAddable{TElement}"/>.
    /// </summary>
    /// <param name="collection">The collection of items to add.</param>
    public void AddRange([DisallowNull] IEnumerable<TElement> collection);
}