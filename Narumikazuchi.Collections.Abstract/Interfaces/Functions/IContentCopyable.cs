namespace Narumikazuchi.Collections.Abstract;

/// <summary>
/// Allows the content of a collection to be copied over to an array of type <typeparamref name="TArray"/>.
/// </summary>
public interface IContentCopyable<TIndex, TArray>
{
    /// <summary>
    /// Copies the elements of the <see cref="IContentCopyable{TIndex, TArray}"/> to the specified <typeparamref name="TArray"/> starting at the specified index <typeparamref name="TIndex"/>.
    /// </summary>
    /// <param name="array"></param>
    /// <param name="index"></param>
    [Pure]
    public void CopyTo([DisallowNull] TArray array,
                       [DisallowNull] in TIndex index);
}