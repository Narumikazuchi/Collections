namespace Narumikazuchi.Collections;

public partial class Trie<TContent> : INotifyCollectionChangedHelper
{
    void INotifyCollectionChangedHelper.OnCollectionChanged(NotifyCollectionChangedEventArgs eventArgs)
    {
#if NET6_0_OR_GREATER
        ArgumentNullException.ThrowIfNull(eventArgs);
#else
        if (eventArgs is null)
        {
            throw new ArgumentNullException(nameof(eventArgs));
        }
#endif

        if (this.CollectionChanged is not null)
        {
            this.CollectionChanged.Invoke(sender: this,
                                          e: eventArgs);
        }
    }
}

public partial class Trie<TContent> : INotifyCollectionChanged
{
    /// <inheritdoc />
    public event NotifyCollectionChangedEventHandler? CollectionChanged;
}