namespace Narumikazuchi.Collections.Abstract;

/// <summary>
/// Indicates that the collection tracks changes.
/// </summary>
public interface IVersioned
{
    /// <summary>
    /// Gets or sets the current number of state changing actions performed on this collection.
    /// </summary>
    internal protected Int32 Version { get; set; }
}