namespace Narumikazuchi.Collections;

/// <summary>
/// Allows for the implementing object to optimize the generated IL code by
/// providing a specific type instead of an interface for the <see cref="GetEnumerator"/>
/// method. Using this interface will reduce the delegation cost of virtual IL calls
/// and therefore improve iteration times of <see langword="foreach"/>-loops.
/// </summary>
/// <remarks>
/// The idea for this interface and it's implementation is taken from the official source
/// code of the <see cref="System.Collections.Immutable"/> namespace.
/// </remarks>
public interface IStrongEnumerable<out TElement, out TEnumerator> :
    IEnumerable<TElement>
        where TEnumerator : struct, IStrongEnumerator<TElement>
{
    /// <summary>
    /// Returns an enumerator that iterates through the collection.
    /// </summary>
    /// <returns>An enumerator that can be used to iterate through the collection.</returns>
    public new TEnumerator GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() =>
        this.GetEnumerator();

    IEnumerator<TElement> IEnumerable<TElement>.GetEnumerator() =>
        this.GetEnumerator();
}