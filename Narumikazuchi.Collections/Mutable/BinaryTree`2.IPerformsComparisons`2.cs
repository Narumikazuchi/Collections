namespace Narumikazuchi.Collections;

public partial class BinaryTree<TValue, TComparer> : IPerformsComparisons<TValue, TComparer>
{
    /// <inheritdoc/>
    [NotNull]
    public TComparer Comparer { get; }
}