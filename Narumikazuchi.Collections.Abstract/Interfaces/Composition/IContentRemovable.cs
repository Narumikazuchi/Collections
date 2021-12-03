namespace Narumikazuchi.Collections.Abstract;

/// <summary>
/// Represents a collection which allows removal of items from itself.
/// </summary>
public interface IContentRemovable<TElement>
{
    /// <summary>
    /// Removes all elements from the <see cref="IContentRemovable{TElement}"/>.
    /// </summary>
    public void Clear();

    /// <summary>
    /// Removes all elements from the <see cref="IContentRemovable{TElement}"/>.
    /// </summary>
    /// <param name="source">The instance to clear.</param>
    /// <exception cref="ArgumentNullException" />
    /// <exception cref="NotAllowed" />
    protected static void Clear<TCollection>([DisallowNull] TCollection source)
        where TCollection : ICollection<TElement?>,
                            IBackingContainer<TElement?[]>,
                            ISynchronized,
                            IVersioned
    {
        ExceptionHelpers.ThrowIfArgumentNull(source);
        if (source.IsReadOnly)
        {
            throw new NotAllowed(auxMessage: "The collection is read-only and can't be edited.");
        }

        lock (source.SyncRoot)
        {
            if (source.ItemCount > 0)
            {
                Array.Clear(array: source.Items,
                            index: 0,
                            length: source.ItemCount);
                source.ItemCount = 0;
            }
            source.Version++;
        }
    }

    /// <summary>
    /// Removes all objects from the <see cref="IContentRemovable{TElement}"/> that match the specified condition.
    /// </summary>
    /// <param name="predicate">The condition that objects need to meet to be deleted.</param>
    /// <returns>The amount of items removed</returns>
    public Int32 RemoveAll([DisallowNull] Func<TElement, Boolean> predicate);

    /// <summary>
    /// Removes all objects from the <see cref="IContentRemovable{TElement}"/> that match the specified condition.
    /// </summary>
    /// <param name="source">The instance from which to remove items.</param>
    /// <param name="predicate">The condition that objects need to meet to be deleted.</param>
    /// <returns>The amount of items removed</returns>
    /// <exception cref="ArgumentNullException" />
    /// <exception cref="NotAllowed" />
    protected static Int32 RemoveAll<TCollection>([DisallowNull] TCollection source,
                                                  [DisallowNull] Func<TElement?, Boolean> predicate)
        where TCollection : IBackingContainer<TElement?[]>,
                            ICollection<TElement>,
                            ISynchronized,
                            IVersioned
    {
        ExceptionHelpers.ThrowIfArgumentNull(source);
        ExceptionHelpers.ThrowIfArgumentNull(predicate);
        if (source.IsReadOnly)
        {
            throw new NotAllowed(auxMessage: "The collection is read-only and can't be edited.");
        }

        lock (source.SyncRoot)
        {
            Int32 v = source.Version;
            Int32 free = 0;
            while (free < source.ItemCount &&
                   !predicate.Invoke(arg: source.Items[free]))
            {
                if (source.Version != v)
                {
                    NotAllowed ex = new(auxMessage: "The collection changed during the enumeration.");
                    ex.Data.Add(key: "Fixed Version",
                                value: v);
                    ex.Data.Add(key: "Altered Version",
                                value: source.Version);

                    throw ex;
                }
                free++;
            }
            if (free >= source.ItemCount)
            {
                return 0;
            }

            Int32 current = free + 1;
            while (current < source.ItemCount)
            {
                if (source.Version != v)
                {
                    NotAllowed ex = new(auxMessage: "The collection changed during the enumeration.");
                    ex.Data.Add(key: "Fixed Version",
                                value: v);
                    ex.Data.Add(key: "Altered Version",
                                value: source.Version);
                    throw ex;
                }
                while (current < source.ItemCount &&
                       predicate(arg: source.Items[current]))
                {
                    current++;
                }

                if (current < source.ItemCount)
                {
                    source.Items[free++] = source.Items[current++];
                }
            }

            Array.Clear(array: source.Items,
                        index: free,
                        length: source.ItemCount - free);
            Int32 result = source.ItemCount - free;
            source.ItemCount = free;
            source.Version++;
            return result;
        }
    }

    /// <summary>
    /// Removes the first occurrence of a specific object from the <see cref="IContentRemovable{TElement}"/>.
    /// </summary>
    /// <param name="source">The instance to remove the item from.</param>
    /// <param name="item">The object to remove from the <see cref="IContentRemovable{TElement}"/>. 
    /// The value can be <see langword="null"/> for reference types.</param>
    /// <returns>
    /// <see langword="true"/> if item is successfully removed; otherwise, <see langword="false"/>.
    /// This method also returns <see langword="false"/> if item was not found in the <see cref="IContentRemovable{TElement}"/>.
    /// </returns>
    /// <exception cref="ArgumentNullException" />
    /// <exception cref="NotAllowed" />
    protected static Boolean Remove<TCollection>([DisallowNull] TCollection source,
                                                 TElement item)
        where TCollection : IBackingContainer<TElement?[]>,
                            ICollection<TElement>,
                            ISynchronized,
                            IVersioned
    {
        ExceptionHelpers.ThrowIfArgumentNull(source);
        if (source.IsReadOnly)
        {
            throw new NotAllowed(auxMessage: "The collection is read-only and can't be edited.");
        }

        lock (source.SyncRoot)
        {
            Int32 index = Array.IndexOf(array: source.Items,
                                        value: item);
            if (index > -1 &&
                index < source.ItemCount)
            {
                source.ItemCount--;
                if (index < source.ItemCount)
                {
                    Array.Copy(sourceArray: source.Items,
                               sourceIndex: index + 1,
                               destinationArray: source.Items,
                               destinationIndex: index,
                               length: source.ItemCount - index);
                }
                source.Items[source.ItemCount] = default;
                source.Version++;
                return true;
            }
            return false;
        }
    }
}