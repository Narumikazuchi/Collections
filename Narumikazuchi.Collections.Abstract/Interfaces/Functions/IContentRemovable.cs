namespace Narumikazuchi.Collections.Abstract;

/// <summary>
/// Represents a collection which allows removal of items from itself.
/// </summary>
public interface IContentRemovable
{
    /// <summary>
    /// Removes the first occurrence of the specified item from the <see cref="IContentRemovable{TElement}"/>.
    /// </summary>
    /// <param name="item">The item to remove.</param>
    /// <returns><see langword="true"/> if the item was found and removed; otherwise, <see langword="false"/></returns>
    public Boolean Remove(Object item);
}

/// <summary>
/// Represents a collection which allows removal of items from itself.
/// </summary>
public interface IContentRemovable<TElement> :
    IContentRemovable
{
    /// <summary>
    /// Removes all objects from the <see cref="IContentRemovable{TElement}"/> that match the specified condition.
    /// </summary>
    /// <param name="predicate">The condition that objects need to meet to be deleted.</param>
    /// <returns>The amount of items removed</returns>
    public Int32 RemoveAll([DisallowNull] Func<TElement, Boolean> predicate);

    /// <summary>
    /// Removes the first occurrence of the specified item from the <see cref="IContentRemovable{TElement}"/>.
    /// </summary>
    /// <param name="item">The item to remove.</param>
    /// <returns><see langword="true"/> if the item was found and removed; otherwise, <see langword="false"/></returns>
    public Boolean Remove(TElement item);
}