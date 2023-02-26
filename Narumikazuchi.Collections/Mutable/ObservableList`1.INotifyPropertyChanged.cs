namespace Narumikazuchi.Collections;

public partial class ObservableList<TElement> : INotifyPropertyChanged
{
    /// <inheritdoc/>
    public event PropertyChangedEventHandler? PropertyChanged;
}

public partial class ObservableList<TElement> : INotifyPropertyChangedHelper
{
    void INotifyPropertyChangedHelper.OnPropertyChanged(NotNull<String> propertyName)
    {
        if (this.PropertyChanged is not null)
        {
            this.PropertyChanged.Invoke(sender: this,
                                        e: new(propertyName));
        }
    }
}