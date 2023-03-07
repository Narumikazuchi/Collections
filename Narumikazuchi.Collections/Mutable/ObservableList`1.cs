namespace Narumikazuchi.Collections;

/// <summary>
/// Represents a strongly typed list of objects of type <typeparamref name="TElement"/>, which reports changes and can be accessed by index. 
/// </summary>
public partial class ObservableList<TElement> : StrongEnumerable<TElement, CommonListEnumerator<TElement>>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ObservableList{TElement}"/> class.
    /// </summary>
    /// <param name="items">The items that the resulting collection shall hold.</param>
    /// <exception cref="ArgumentNullException" />
    static public ObservableList<TElement> CreateFrom<TEnumerable>([DisallowNull] TEnumerable items)
        where TEnumerable : IEnumerable<TElement>
    {
#if NET6_0_OR_GREATER
        ArgumentNullException.ThrowIfNull(items);
#else
        if (items is null)
        {
            throw new ArgumentNullException(nameof(items));
        }
#endif

        return new(new List<TElement>(items));
    }

    /// <inheritdoc/>
    public sealed override CommonListEnumerator<TElement> GetEnumerator()
    {
        return new(m_Items);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ObservableList{TElement}"/> class.
    /// </summary>
    public ObservableList()
    {
        m_Items = new();
    }
}