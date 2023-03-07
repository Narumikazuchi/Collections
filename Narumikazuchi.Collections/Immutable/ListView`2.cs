namespace Narumikazuchi.Collections;

/// <summary>
/// Represents a strongly-typed view of an <see cref="IList{T}"/>, which is basically a read only view of the
/// source list, with a significantly faster enumerator to improve iteration times for <see langword="foreach"/>-loops.
/// </summary>
/// <remarks>
/// The faster enumeration only works if the collection is used as-is or by using the
/// <see cref="IStrongEnumerable{TElement, TEnumerator}"/> interface. If you you plan on using 
/// any derivative of the <see cref="IEnumerable{T}"/> interface (i.e. <see cref="ICollection{T}"/>) 
/// in your code then the efficiency of the enumerator will be lost due to call virtualization in 
/// the compiler generated IL.<para/>
/// It is recommended to either use generics or use the <see cref="StrongEnumerable{TElement, TEnumerator}"/>
/// abstract class with generics to avoid call virutalization.
/// </remarks>
public sealed partial class ListView<TElement, TList> : BaseReadOnlyCollection<TElement, TList, ListView<TElement, TList>.Enumerator>
    where TList : IList<TElement>
{
#pragma warning disable CS1591 // Missing comments
    static public implicit operator ReadOnlyList<TElement>(ListView<TElement, TList> source)
    {
        return ReadOnlyList<TElement>.CreateFrom(source);
    }

    static public implicit operator ReadOnlySortedList<TElement, Comparer<TElement>>(ListView<TElement, TList> source)
    {
        return ReadOnlySortedList<TElement, Comparer<TElement>>.CreateFrom(items: source,
                                                                           comparer: Comparer<TElement>.Default);
    }
#pragma warning restore

    /// <summary>
    /// Initializes a new instance of type <see cref="ListView{TElement, TList}"/>.
    /// </summary>
    /// <param name="source">The list that the resulting view should expose.</param>
    /// <param name="sectionStart">The first index in the source list to view.</param>
    /// <param name="sectionEnd">The last index in the source list to view.</param>
    /// <exception cref="ArgumentNullException" />
    public ListView([DisallowNull] TList source,
                    Int32 sectionStart = default,
                    Int32 sectionEnd = -1)
        : base(items: source,
               sectionStart: sectionStart,
               sectionEnd: sectionEnd)
    { }

    /// <inheritdoc/>
    public sealed override ListView<TElement, TList>.Enumerator GetEnumerator()
    {
        return new(m_Items);
    }
}