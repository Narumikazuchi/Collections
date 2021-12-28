namespace Narumikazuchi.Collections;

/// <summary>
/// Represents a collection which reports changes and contains every object is only once. The procedure to check whether the object is already in the <see cref="ObservableSet{TElement}"/> can be specified
/// with a corresponding <see cref="IEqualityComparer{T}"/> or <see cref="EqualityComparison{T}"/> delegate.
/// </summary>
/// <remarks>
/// If neither <see cref="IEqualityComparer{T}"/> nor <see cref="EqualityComparison{T}"/> are specified, the register will compare the references for classes or check each field/property for values types.
/// </remarks>
public interface IObservableSet<TElement> :
    ICollection,
    ICollection<TElement>,
    ICollectionImmutability,
    IContentAddable<TElement>,
    IContentClearable,
    IContentConvertable<TElement>,
    IContentCopyable<Int32, TElement[]>,
    IContentForEach<TElement>,
    IContentRemovable,
    IContentRemovable<TElement>,
    IContentSegmentable<Int32, TElement>,
    IConvertToArray<TElement[]>,
    IElementContainer,
    IElementContainer<TElement>,
    IElementFinder<TElement, TElement>,
    IEnumerable,
    IEnumerable<TElement>,
    IIndexedReadOnlyCollection<Int32>,
    IIndexedReadOnlyCollection<Int32, TElement>,
    IIndexFinder<Int32, TElement>,
    INotifyCollectionChanged,
    INotifyPropertyChanged,
    INotifyPropertyChanging,
    IReadOnlyCollection<TElement>,
    IReadOnlyList<TElement>,
    IReadOnlySet<TElement>,
    ISet<TElement>,
    ISynchronized
{ }