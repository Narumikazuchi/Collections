namespace Narumikazuchi.Collections;

/// <summary>
/// Implements a way for extensions methods to raise the <see cref="INotifyPropertyChanged.PropertyChanged"/> event.
/// </summary>
public interface INotifyPropertyChangedHelper :
    INotifyPropertyChanged
{
    /// <summary>
    /// Raises the <see cref="INotifyPropertyChanged.PropertyChanged"/> event for the specified property.
    /// </summary>
    /// <param name="propertyName">The name of the property that changed.</param>
    internal protected void OnPropertyChanged([DisallowNull] String propertyName);
}