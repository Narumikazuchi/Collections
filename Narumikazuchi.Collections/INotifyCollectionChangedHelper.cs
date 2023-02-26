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
    public void OnCollectionChanged(
#if NETCOREAPP3_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        [DisallowNull]
#endif
        NotNull<NotifyCollectionChangedEventArgs> eventArgs);
}