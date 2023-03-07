namespace Narumikazuchi.Collections;

public partial class BinaryTree<TValue, TComparer> : IReadOnlyCollection<TValue, BinaryTree<TValue, TComparer>.Enumerator>
{
    /// <inheritdoc/>
    /// <exception cref="ArgumentNullException"/>
    public Boolean Contains([DisallowNull] TValue element)
    {
#if NET6_0_OR_GREATER
        ArgumentNullException.ThrowIfNull(element);
#else
        if (element is null)
        {
            throw new ArgumentNullException(nameof(element));
        }
#endif

        return this.FindInternal(element) is not null;
    }
    /// <inheritdoc/>
    /// <exception cref="ArgumentNullException"/>
    public Boolean Contains<TEqualityComparer>([DisallowNull] TValue element,
                                               [DisallowNull] TEqualityComparer equalityComparer)
        where TEqualityComparer : IEqualityComparer<TValue>
    {
#if NET6_0_OR_GREATER
        ArgumentNullException.ThrowIfNull(element);
        ArgumentNullException.ThrowIfNull(equalityComparer);
#else
        if (element is null)
        {
            throw new ArgumentNullException(nameof(element));
        }
        
        if (equalityComparer is null)
        {
            throw new ArgumentNullException(nameof(equalityComparer));
        }
#endif

        return this.FindInternal(value: element,
                                 equalityComparer: equalityComparer) is not null;
    }

    /// <inheritdoc/>
    public void CopyTo([DisallowNull] TValue[] array)
    {
#if NET6_0_OR_GREATER
        ArgumentNullException.ThrowIfNull(array);
#else
        if (array is null)
        {
            throw new ArgumentNullException(nameof(array));
        }
#endif

        this.CopyTo(array: array,
                    destinationIndex: 0);
    }
    /// <inheritdoc/>
    public void CopyTo([DisallowNull] TValue[] array,
                       Int32 destinationIndex)
    {
#if NET6_0_OR_GREATER
        ArgumentNullException.ThrowIfNull(array);
#else
        if (array is null)
        {
            throw new ArgumentNullException(nameof(array));
        }
#endif

        Int32 index = 0;
        foreach (TValue value in this.TraverseInOrder())
        {
            array[destinationIndex + index++] = value;
        }
    }
}