namespace Narumikazuchi.Collections.Abstract;

/// <summary>
/// Allows the removal of an item by specifying an index instead of the actual item.
/// </summary>
public interface IContentIndexRemovable<TIndex>
{
    /// <summary>
    /// Removes the object at the specified index from the <see cref="IContentIndexRemovable{TIndex}"/>.
    /// </summary>
    /// <param name="index">The index of the object to delete.</param>
    public void RemoveAt([DisallowNull] in TIndex index);
}