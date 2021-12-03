namespace Narumikazuchi.Collections.Abstract;

/// <summary>
/// Represents a collection which allows the addition of items to itself.
/// </summary>
public interface IContentAddable<TElement>
{
    /// <summary>
    /// Adds an object to the end of the <see cref="IContentAddable{TElement}"/>.
    /// </summary>
    /// <param name="item">The object to be added to the end of the <see cref="IContentAddable{TElement}"/>. 
    /// The value can be <see langword="null"/> for reference types.</param>
    /// <returns><see langword="true"/> if the item was added; otherwise, <see langword="false"/></returns>
    public Boolean Add(TElement item);

    /// <summary>
    /// Adds an object to the end of the <see cref="IContentAddable{TElement}"/>.
    /// </summary>
    /// <param name="source">The collection to add the item to.</param>
    /// <param name="item">The object to be added to the end of the <see cref="IContentAddable{TElement}"/>. 
    /// The value can be <see langword="null"/> for reference types.</param>
    /// <returns><see langword="true"/> if the item was added; otherwise, <see langword="false"/></returns>
    /// <exception cref="ArgumentNullException" />
    /// <exception cref="NotAllowed" />
    protected static Boolean Add<TCollection>([DisallowNull] TCollection source,
                                              TElement item)
        where TCollection : IBackingContainer<TElement?[]>,
                            ICollection<TElement>,
                            ICollectionExpandable<TElement>,
                            ICollectionImmutability,
                            ISynchronized,
                            IVersioned
    {
        ExceptionHelpers.ThrowIfArgumentNull(source);
        if (source.IsReadOnly)
        {
            throw new NotAllowed(auxMessage: "The collection is read-only and can't be edited.");
        }

        source.EnsureCapacity(source.ItemCount + 1);

        lock (source.SyncRoot)
        {
            source.Items[source.ItemCount++] = item;
            source.Version++;
        }

        return true;
    }

    /// <summary>
    /// Adds the elements of the specified collection to the end of the <see cref="IContentAddable{TElement}"/>.
    /// </summary>
    /// <param name="collection">The collection of items to add.</param>
    public void AddRange([DisallowNull] IEnumerable<TElement> collection);

    /// <summary>
    /// Adds the elements of the specified collection to the end of the <see cref="IContentAddable{TElement}"/>.
    /// </summary>
    /// <param name="source">The collection to add the item to.</param>
    /// <param name="collection">The collection of items to add.</param>
    /// <exception cref="ArgumentNullException" />
    /// <exception cref="ArgumentOutOfRangeException" />
    /// <exception cref="NotAllowed" />
    protected static void AddRange<TCollection>([DisallowNull] TCollection source,
                                                [DisallowNull] IEnumerable<TElement> collection)
        where TCollection : IBackingContainer<TElement?[]>,
                            ICollection<TElement?>,
                            ICollectionExpandable<TElement>,
                            ICollectionImmutability,
                            ISynchronized,
                            IVersioned
    {
        ExceptionHelpers.ThrowIfArgumentNull(source);
        ExceptionHelpers.ThrowIfArgumentNull(collection);
        if (source.IsReadOnly)
        {
            throw new NotAllowed(auxMessage: "The collection is read-only and can't be edited.");
        }

        if (collection is ICollection c)
        {
            Int32 count = c.Count;
            if (count > 0)
            {
                source.EnsureCapacity(source.ItemCount + count);

                if (ReferenceEquals(objA: source,
                                    objB: c))
                {
                    lock (source.SyncRoot)
                    {
                        Array.Copy(sourceArray: source.Items,
                                   sourceIndex: 0,
                                   destinationArray: source.Items,
                                   destinationIndex: source.ItemCount,
                                   length: source.ItemCount);
                    }
                }
                else
                {
                    TElement[] insert = new TElement[count];
                    c.CopyTo(array: insert,
                             index: 0);
                    lock (source.SyncRoot)
                    {
                        insert.CopyTo(array: source.Items,
                                      index: source.ItemCount);
                    }
                }

                lock (source.SyncRoot)
                {
                    source.ItemCount += count;
                }
            }
        }
        else
        {
            using IEnumerator<TElement> enumerator = collection.GetEnumerator();
            while (enumerator.MoveNext())
            {
                if (enumerator.Current is null)
                {
                    continue;
                }
                source.Add(item: enumerator.Current);
            }
        }
        lock (source.SyncRoot)
        {
            source.Version++;
        }
    }
}