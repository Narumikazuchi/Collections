namespace Narumikazuchi.Collections;

public partial class ListView<TElement, TList>
{
    /// <summary>
    /// An enumerator that iterates through the <see cref="ListView{TElement, TList}"/>.
    /// </summary>
    public struct Enumerator :
        IStrongEnumerable<TElement, ListView<TElement, TList>.Enumerator>,
        IStrongEnumerator<TElement>,
        IEnumerator<TElement>
    {
        /// <summary>
        /// The default constructor for the <see cref="Enumerator"/> is not allowed.
        /// </summary>
        /// <exception cref="NotAllowed"></exception>
        public Enumerator()
        {
            throw new NotAllowed();
        }
        internal Enumerator(TList source)
        {
            m_Elements = source;
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
        public TElement Current
        {
            get
            {
                return m_Elements[m_Index];
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

        IEnumerator<TElement> IEnumerable<TElement>.GetEnumerator()
        {
            return this.GetEnumerator();
        }
#endif

        private readonly TList m_Elements;
        private Int32 m_Index;
    }
}