namespace Narumikazuchi.Collections.Abstract;

/// <summary>
/// Represents a collection that has <typeparamref name="TContainer"/> as a backing container to store the actual data.
/// </summary>
public interface IBackingContainer<TContainer>
{
    /// <summary>
    /// Gets or sets the backing container for the collection.
    /// </summary>
    [NotNull]
    internal protected TContainer Items { get; set; }
    /// <summary>
    /// Gets or sets the item count of the backing collection.
    /// </summary>
    internal protected Int32 ItemCount { get; set; }
}