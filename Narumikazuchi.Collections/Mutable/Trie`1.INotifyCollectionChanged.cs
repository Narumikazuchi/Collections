namespace Narumikazuchi.Collections;

public partial class Trie<TContent> : INotifyCollectionChangedHelper
{
    void INotifyCollectionChangedHelper.OnCollectionChanged(NotNull<NotifyCollectionChangedEventArgs> eventArgs)
    {
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