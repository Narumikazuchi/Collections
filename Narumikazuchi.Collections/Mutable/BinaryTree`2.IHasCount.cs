namespace Narumikazuchi.Collections;

// ICollectionWithCount<T, U>
public partial class BinaryTree<TValue, TComparer> : IHasCount
{
    /// <inheritdoc/>
    public Int32 Count
    {
        get
        {
            return m_Count;
        }
    }
}