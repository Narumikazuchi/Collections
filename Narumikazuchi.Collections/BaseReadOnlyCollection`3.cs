namespace Narumikazuchi.Collections;

/// <summary>
/// Provides the basic functionality for the readonly collections in this library.
/// </summary>
[EditorBrowsable(EditorBrowsableState.Never)]
[Browsable(false)]
public abstract partial class BaseReadOnlyCollection<TElement, TCollection, TEnumerator>
    : StrongEnumerable<TElement, TEnumerator>
        where TCollection : IList<TElement>
        where TEnumerator : struct, IStrongEnumerator<TElement>
{
    internal BaseReadOnlyCollection(TCollection items,
                                    Int32 sectionStart = default,
                                    Int32 sectionEnd = -1)
    {
        if (sectionEnd is -1)
        {
            sectionEnd = items.Count;
        }

        if (sectionStart > sectionEnd)
        {
            (sectionStart, sectionEnd) = (sectionEnd, sectionStart);
        }

        m_SectionStart = sectionStart;
        m_SectionEnd = sectionEnd;
        m_Items = items;
    }

    internal readonly TCollection m_Items;
    internal readonly Int32 m_SectionStart;
    internal readonly Int32 m_SectionEnd;
}