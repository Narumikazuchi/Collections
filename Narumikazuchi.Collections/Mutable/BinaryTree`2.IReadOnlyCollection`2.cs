namespace Narumikazuchi.Collections;

public partial class BinaryTree<TValue, TComparer> : IReadOnlyCollection<TValue, BinaryTree<TValue, TComparer>.Enumerator>
{
    /// <inheritdoc/>
    /// <exception cref="ArgumentNullException"/>
    public Boolean Contains(
#if NETCOREAPP3_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        [DisallowNull]
#endif
        NotNull<TValue> element)
    {
        return this.FindInternal(element) is not null;
    }
    /// <inheritdoc/>
    /// <exception cref="ArgumentNullException"/>
    public Boolean Contains<TEqualityComparer>(
#if NETCOREAPP3_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        [DisallowNull]
#endif
        NotNull<TValue> element,
#if NETCOREAPP3_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        [DisallowNull]
#endif
        NotNull<TEqualityComparer> equalityComparer)
            where TEqualityComparer : IEqualityComparer<TValue>
    {
        return this.FindInternal<TEqualityComparer>(value: element,
                                                    equalityComparer: equalityComparer) is not null;
    }

    /// <inheritdoc/>
    public void CopyTo(
#if NETCOREAPP3_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        [DisallowNull]
#endif
        NotNull<TValue[]> array)
    {
        this.CopyTo(array: array,
                    destinationIndex: 0);
    }
    /// <inheritdoc/>
    public void CopyTo(
#if NETCOREAPP3_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        [DisallowNull]
#endif
        NotNull<TValue[]> array,
        Int32 destinationIndex)
    {
        TValue[] destination = array;
        Int32 index = 0;
        foreach (TValue value in this.TraverseInOrder())
        {
            destination[destinationIndex + index++] = value;
        }
    }
}