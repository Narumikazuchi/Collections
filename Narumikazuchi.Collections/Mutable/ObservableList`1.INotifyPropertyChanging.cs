namespace Narumikazuchi.Collections;

public partial class ObservableList<TElement> : INotifyPropertyChanging
{
    /// <inheritdoc/>
    public event PropertyChangingEventHandler? PropertyChanging;
}

public partial class ObservableList<TElement> : INotifyPropertyChangingHelper
{
    void INotifyPropertyChangingHelper.OnPropertyChanging(String propertyName)
    {
#if NET6_0_OR_GREATER
        ArgumentNullException.ThrowIfNull(propertyName);
#else
        if (propertyName is null)
        {
            throw new ArgumentNullException(nameof(propertyName));
        }
#endif

        if (this.PropertyChanging is not null)
        {
            this.PropertyChanging.Invoke(sender: this,
                                         e: new(propertyName));
        }
    }
}