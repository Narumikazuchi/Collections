namespace Narumikazuchi.Collections;

public partial class BinaryTree<TValue, TComparer> : IPerformsComparisons<TValue, TComparer>
{
    /// <inheritdoc/>
#if NETCOREAPP3_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
    [NotNull]
#endif
    public NotNull<TComparer> Comparer { get; }
}