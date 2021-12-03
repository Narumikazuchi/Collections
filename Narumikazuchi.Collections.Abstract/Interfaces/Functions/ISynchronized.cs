namespace Narumikazuchi.Collections.Abstract;

/// <summary>
/// Represents an object that is synchronized (thread safe).
/// </summary>
public interface ISynchronized
{
    /// <summary>
    /// Gets a value indicating whether access to this <see cref="ISynchronized"/> is synchronized (thread safe).
    /// </summary>
    [Pure]
    public Boolean IsSynchronized { get; }
    /// <summary>
    /// Gets the object mutex used to synchronize access to this <see cref="ISynchronized"/>.
    /// </summary>
    [Pure]
    [NotNull]
    public Object SyncRoot { get; }
}