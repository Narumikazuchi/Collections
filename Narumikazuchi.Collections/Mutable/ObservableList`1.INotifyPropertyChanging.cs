namespace Narumikazuchi.Collections;

public partial class ObservableList<TElement> : INotifyPropertyChanging
{
    /// <inheritdoc/>
    public event PropertyChangingEventHandler? PropertyChanging;
}

public partial class ObservableList<TElement> : INotifyPropertyChangingHelper
{
    void INotifyPropertyChangingHelper.OnPropertyChanging(NotNull<String> propertyName)
    {
        if (this.PropertyChanging is not null)
        {
            this.PropertyChanging.Invoke(sender: this,
                                         e: new(propertyName));
        }
    }
}