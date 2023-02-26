namespace Narumikazuchi.Collections;

/// <summary>
/// Represents an instance that has some sort of count, usually a count elements in a collection.
/// </summary>
public interface IHasCount
{
    /// <summary>
    /// Gets the number of elements in this instance.
    /// </summary>
    public Int32 Count { get; }
}