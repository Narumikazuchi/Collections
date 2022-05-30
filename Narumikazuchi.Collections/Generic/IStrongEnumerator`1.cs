namespace Narumikazuchi.Collections;

/// <summary>
/// Allows for the implementing object to optimize the generated IL code by
/// providing a specific type instead of an interface for the 
/// <see cref="IStrongEnumerable{TElement, TEnumerator}.GetEnumerator"/> method. 
/// Using this interface will reduce the delegation cost of virtual IL calls
/// and therefore improve iteration times of <see langword="foreach"/>-loops.
/// This interface also removes the requirement of the implementation of the
/// <see cref="IDisposable.Dispose"/> method, making the resulting type 
/// even more memory efficient.
/// </summary>
/// <remarks>
/// The idea for this interface and it's implementation is taken from the official source
/// code of the <see cref="System.Collections.Immutable"/> namespace.
/// </remarks>
public interface IStrongEnumerator<out TElement>
{
    /// <summary>
    /// Advances the enumerator to the next element in the collection.
    /// </summary>
    /// <returns><see langword="true"/> if the enumerator has advanced to the next element; otherwise, <see langword="false"/></returns>
    /// <exception cref="InvalidOperationException"/>
    public Boolean MoveNext();

    /// <summary>
    /// Gets the element in the collection that is at the current position of the enumerator.
    /// </summary>
    public TElement Current { get; }
}