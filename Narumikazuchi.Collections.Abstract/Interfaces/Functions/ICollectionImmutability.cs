namespace Narumikazuchi.Collections.Abstract;

/// <summary>
/// Expresses the immutability of the capacity of a collection.
/// </summary>
public interface ICollectionImmutability
{
    /// <summary>
    /// Gets whether the capacity of this <see cref="ICollectionImmutability"/> can be edited.
    /// </summary>
    [Pure]
    public Boolean IsFixedSize { get; }
}