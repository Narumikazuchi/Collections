namespace Narumikazuchi.Collections;

/// <summary>
/// Represents a strongly typed list of objects, which reports changes, can be accessed by index and searched. 
/// </summary>
public interface IObservableList<TElement> :
    IAsyncEnumerable<TElement>,
    ICollection,
    ICollection<TElement>,
    ICollectionImmutability,
    IContentClearable,
    IContentConvertable<TElement>,
    IContentCopyable<Int32, TElement[]>,
    IContentForEach<TElement>,
    IContentIndexRemovable<Int32>,
    IContentInsertable<Int32>,
    IContentInsertable<Int32, TElement>,
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
    ISynchronized
{ }