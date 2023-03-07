namespace Narumikazuchi.Collections;

public partial class ObservableList<TElement> : INotifyPropertyChanged
{
    /// <inheritdoc/>
    public event PropertyChangedEventHandler? PropertyChanged;
}

public partial class ObservableList<TElement> : INotifyPropertyChangedHelper
{
    void INotifyPropertyChangedHelper.OnPropertyChanged(String propertyName)
    {
#if NET6_0_OR_GREATER
        ArgumentNullException.ThrowIfNull(propertyName);
#else
        if (propertyName is null)
        {
            throw new ArgumentNullException(nameof(propertyName));
        }
#endif

        if (this.PropertyChanged is not null)
        {
            this.PropertyChanged.Invoke(sender: this,
                                        e: new(propertyName));
        }
    }
}