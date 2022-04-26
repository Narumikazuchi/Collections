namespace Narumikazuchi.Collections;

/// <summary>
/// Implements a way for extensions methods to raise the <see cref="INotifyPropertyChanging.PropertyChanging"/> event.
/// </summary>
public interface INotifyPropertyChangingHelper :
    INotifyPropertyChanging
{
    /// <summary>
    /// Raises the <see cref="INotifyPropertyChanging.PropertyChanging"/> event for the specified property.
    /// </summary>
    /// <param name="propertyName">The name of the property that is changing.</param>
    internal protected void OnPropertyChanging([DisallowNull] String propertyName);
}