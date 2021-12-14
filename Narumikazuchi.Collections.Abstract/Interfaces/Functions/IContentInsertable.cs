namespace Narumikazuchi.Collections.Abstract;

/// <summary>
/// Allows the emplacement of an item at a specified index instead of the end of a collection.
/// </summary>
public interface IContentInsertable<TIndex>
{
    /// <summary>
    /// Inserts the specified object into the <see cref="IContentInsertable{TIndex, TElement}"/> at the specified index.
    /// </summary>
    /// <param name="index">The index at which to place the object.</param>
    /// <param name="item">The object to insert.</param>
    public void Insert([DisallowNull] in TIndex index,
                       Object item);
}

/// <summary>
/// Allows the emplacement of an item at a specified index instead of the end of a collection.
/// </summary>
public interface IContentInsertable<TIndex, TElement> :
    IContentInsertable<TIndex>
{
    /// <summary>
    /// Inserts the specified object into the <see cref="IContentInsertable{TIndex, TElement}"/> at the specified index.
    /// </summary>
    /// <param name="index">The index at which to place the object.</param>
    /// <param name="item">The object to insert.</param>
    public void Insert([DisallowNull] in TIndex index,
                       TElement item);

    /// <summary>
    /// Inserts the items from the specified collection into this <see cref="IContentInsertable{TIndex, TElement}"/> starting at the specified index.
    /// </summary>
    /// <param name="index">The index where to start inserting the new items.</param>
    /// <param name="collection">The collection of items to insert.</param>
    public void InsertRange([DisallowNull] in TIndex index,
                            [DisallowNull] IEnumerable<TElement?> collection);
}