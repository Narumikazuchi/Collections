namespace Narumikazuchi.Collections;

/// <summary>
/// Implements a way for extensions methods to raise the <see cref="INotifyCollectionChanged.CollectionChanged"/> event.
/// </summary>
public interface INotifyCollectionChangedHelper :
    INotifyCollectionChanged
{
    /// <summary>
    /// Raises the <see cref="INotifyCollectionChanged.CollectionChanged"/> event with the specified event args.
    /// </summary>
    internal protected void OnCollectionChanged([DisallowNull] NotifyCollectionChangedEventArgs eventArgs);
}