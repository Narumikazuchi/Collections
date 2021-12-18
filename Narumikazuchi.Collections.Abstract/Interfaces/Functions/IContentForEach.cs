namespace Narumikazuchi.Collections.Abstract;

/// <summary>
/// Allows the use of an <see cref="Action{T}"/> instead of a <see langword="foreach"/>-block to perform actions on the items of a collection.
/// </summary>
public interface IContentForEach<out TElement>
{
    /// <summary>
    /// Performs the specified action for every element of this <see cref="IContentForEach{TElement}"/>.
    /// </summary>
    /// <param name="action">The action to perform on each item.</param>
    [Pure]
    public void ForEach([DisallowNull] Action<TElement> action);
}