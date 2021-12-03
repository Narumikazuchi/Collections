namespace Narumikazuchi.Collections.Abstract;

/// <summary>
/// Allows the conversion of all items in the collection into a different type.
/// </summary>
public interface IContentConvertable<TElement>
{
    /// <summary>
    /// Converts all elements in the <see cref="IContentConvertable{T}"/> into another type and returns an <see cref="ICollection{T}"/>
    /// containing the converted objects.
    /// </summary>
    /// <param name="converter">A delegate which converts every item into the new type.</param>
    /// <returns>An <see cref="ICollection{T}"/> which contains the converted objects</returns>
    /// <exception cref="ArgumentNullException" />
    [Pure]
    [return: NotNull]
    public ICollection<TOutput> ConvertAll<TOutput>([DisallowNull] Converter<TElement, TOutput> converter);
}