namespace Narumikazuchi.Collections;

public partial class ObservableList<TElement> : INotifyCollectionChanged
{
    /// <inheritdoc/>
    public event NotifyCollectionChangedEventHandler? CollectionChanged;
}

public partial class ObservableList<TElement> : INotifyCollectionChangedHelper
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