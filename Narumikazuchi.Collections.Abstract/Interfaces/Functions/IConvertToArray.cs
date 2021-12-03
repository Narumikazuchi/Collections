namespace Narumikazuchi.Collections.Abstract;

/// <summary>
/// The functionality to copy the contents of this object into an array of type <typeparamref name="TArray"/>.
/// </summary>
public interface IConvertToArray<TArray>
{
    /// <summary>
    /// Creates a shallow copy of the contents of this <see cref="IConvertToArray{TArray}"/> and returns it as an array.
    /// </summary>
    /// <returns>An array containing the same items as this <see cref="IConvertToArray{TArray}"/></returns>
    [Pure]
    [return: NotNull]
    public TArray ToArray();
}