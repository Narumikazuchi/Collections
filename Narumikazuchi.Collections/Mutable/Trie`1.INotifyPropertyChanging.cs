namespace Narumikazuchi.Collections;

public partial class Trie<TContent> : INotifyPropertyChangingHelper
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

public partial class Trie<TContent> : INotifyPropertyChanging
{
    /// <inheritdoc />
    public event PropertyChangingEventHandler? PropertyChanging;
}