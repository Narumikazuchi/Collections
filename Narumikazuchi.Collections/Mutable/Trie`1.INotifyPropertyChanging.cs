namespace Narumikazuchi.Collections;

public partial class Trie<TContent> : INotifyPropertyChangingHelper
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

public partial class Trie<TContent> : INotifyPropertyChanging
{
    /// <inheritdoc />
    public event PropertyChangingEventHandler? PropertyChanging;
}