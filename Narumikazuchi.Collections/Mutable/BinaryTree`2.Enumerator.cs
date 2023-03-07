namespace Narumikazuchi.Collections;

public partial class BinaryTree<TValue, TComparer>
{
    /// <summary>
    /// An enumerator that iterates through the <see cref="BinaryTree{TValue, TComparer}"/>.
    /// </summary>
    public struct Enumerator :
        IStrongEnumerable<TValue, BinaryTree<TValue, TComparer>.Enumerator>,
        IStrongEnumerator<TValue>,
        IEnumerator<TValue>
    {
        /// <summary>
        /// The default constructor for the <see cref="Enumerator"/> is not allowed.
        /// </summary>
        /// <exception cref="NotAllowed"></exception>
        public Enumerator()
        {
            throw new NotAllowed();
        }
        internal Enumerator(BinaryTree<TValue, TComparer> tree,
                            BinaryTraversalMethod method)
        {
            m_Elements = method switch
            {
                BinaryTraversalMethod.PreOrder => tree.TraversePreOrder(),
                BinaryTraversalMethod.PostOrder => tree.TraversePostOrder(),
                _ => tree.TraverseInOrder(),
            };
            m_Index = -1;
        }
        internal Enumerator(List<BinaryNode<TValue>> elements)
        {
            m_Elements = elements;
            m_Index = -1;
        }

        /// <inheritdoc/>
        public Boolean MoveNext()
        {
            return ++m_Index < m_Elements.Count;
        }

        /// <inheritdoc/>
        public Enumerator GetEnumerator()
        {
            if (m_Index == -1)
            {
                return this;
            }
            else
            {
                return new(m_Elements);
            }
        }

        /// <inheritdoc/>
        public TValue Current
        {
            get
            {
                return m_Elements[m_Index].Value;
            }
        }

#if !NET6_0_OR_GREATER
        void IDisposable.Dispose()
        { }

        void IEnumerator.Reset()
        { }

        Object? IEnumerator.Current
        {
            get
            {
                return this.Current;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        IEnumerator<TValue> IEnumerable<TValue>.GetEnumerator()
        {
            return this.GetEnumerator();
        }
#endif

        private readonly List<BinaryNode<TValue>> m_Elements;
        private Int32 m_Index;
    }
}