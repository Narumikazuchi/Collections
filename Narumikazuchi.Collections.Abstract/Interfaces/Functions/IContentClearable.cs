namespace Narumikazuchi.Collections.Abstract;

/// <summary>
/// Represents a collection which allows the removal of every item from itself.
/// </summary>
public interface IContentClearable
{
    /// <summary>
    /// Removes all elements from the <see cref="IContentClearable"/>.
    /// </summary>
    public void Clear();
}