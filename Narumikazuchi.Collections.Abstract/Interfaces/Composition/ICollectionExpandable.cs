namespace Narumikazuchi.Collections.Abstract;

/// <summary>
/// Represents a collection whose capacity can be increased.
/// </summary>
public interface ICollectionExpandable<TElement>
{
    /// <summary>
    /// Increases the capacity of the collection to enable it to fit the specified amount of items.
    /// </summary>
    /// <param name="capacity">The desired amount of items to fit in the collection.</param>
    internal protected void EnsureCapacity(in Int32 capacity);

    /// <summary>
    /// Increases the capacity of the collection to enable it to fit the specified amount of items.
    /// </summary>
    /// <param name="source">The instance to expand.</param>
    /// <param name="capacity">The desired amount of items to fit in the collection.</param>
    /// <exception cref="ArgumentNullException" />
    /// <exception cref="ArgumentOutOfRangeException" />
    /// <exception cref="NotAllowed" />
    protected static void EnsureCapacity<TCollection>([DisallowNull] TCollection source,
                                                      in Int32 capacity)
        where TCollection : IBackingContainer<TElement?[]>,
                            ICollectionImmutability,
                            ISynchronized
    {
        ExceptionHelpers.ThrowIfArgumentNull(source);
        if (source.Items.Length < capacity)
        {
            if (source.IsFixedSize)
            {
                throw new NotAllowed(auxMessage: "The collection can't be resized, since it's size is fixed.");
            }
            Int32 bigger = source.Items.Length == 0
                                ? 4
                                : source.Items.Length * 2;
            bigger = bigger.Clamp(1,
                                  0x7FFFFFFF);
            TElement[] array = new TElement[bigger];
            lock (source.SyncRoot)
            {
                if (source.ItemCount > 0)
                {
                    Array.Copy(sourceArray: source.Items,
                               sourceIndex: 0,
                               destinationArray: array,
                               destinationIndex: 0,
                               length: source.ItemCount);
                }
                source.Items = array;
            }
        }
    }
}