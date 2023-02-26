namespace Narumikazuchi.Collections;

public partial class TrieNode<TContent>
{
    /// <summary>
    /// An enumerator that iterates through the contents of the <see cref="TrieNode{TContent}"/>.
    /// </summary>
    public struct Enumerator :
        IStrongEnumerable<TContent, Enumerator>,
        IStrongEnumerator<TContent>
    {
        /// <summary>
        /// The default constructor for the <see cref="Enumerator"/> is not allowed.
        /// </summary>
        /// <exception cref="NotAllowed"></exception>
        public Enumerator()
        {
            throw new NotAllowed();
        }
        internal Enumerator(TrieNode<TContent> parent)
        {
            m_Parent = parent;
            m_ChildEnumerator = parent.Children.GetEnumerator();
            m_Child = null;
            m_Enumerator = default;
            m_State = null;
        }

        /// <inheritdoc/>
        public Boolean MoveNext()
        {
            if (!m_State.HasValue)
            {
                m_Enumerator = m_Parent.m_Items.GetEnumerator();
                m_State = 1;
            }

            while (m_State.Value == 1)
            {
                if (m_Enumerator.MoveNext())
                {
                    return true;
                }
                else if (m_Parent.m_Trie.ParentsKnowChildItems &&
                         m_ChildEnumerator.MoveNext())
                {
                    m_Child = m_ChildEnumerator.Current;
                    m_Enumerator = m_Child.m_Items.GetEnumerator();
                    continue;
                }
                else
                {
                    m_State = -1;
                    return false;
                }
            }

            m_State = -1;
            return false;
        }

        /// <inheritdoc/>
        public Enumerator GetEnumerator()
        {
            if (!m_State.HasValue)
            {
                return this;
            }
            else
            {
                return new(m_Parent);
            }
        }

        /// <inheritdoc/>
        public TContent Current
        {
            get
            {
                return m_Enumerator.Current;
            }
        }

#if !NETCOREAPP3_1_OR_GREATER
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

        IEnumerator<TContent> IEnumerable<TContent>.GetEnumerator()
        {
            return this.GetEnumerator();
        }
#endif

        private readonly TrieNode<TContent> m_Parent;
        private readonly CommonListEnumerator<TrieNode<TContent>> m_ChildEnumerator;
        private TrieNode<TContent>? m_Child;
        private HashSet<TContent>.Enumerator m_Enumerator;
        private Int32? m_State;
    }
}