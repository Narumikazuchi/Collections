namespace Narumikazuchi.Collections;

public partial class Trie<TContent> : INotifyPropertyChangedHelper
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

public partial class Trie<TContent> : INotifyPropertyChanged
{
    /// <inheritdoc />
    public event PropertyChangedEventHandler? PropertyChanged;
}