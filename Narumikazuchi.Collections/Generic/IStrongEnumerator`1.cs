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
public interface IStrongEnumerator<out TElement> :
    IEnumerator<TElement>
{
#if NETCOREAPP3_1_OR_GREATER
    void IDisposable.Dispose()
    { }

    void IEnumerator.Reset()
    { }

    Object? IEnumerator.Current =>
        this.Current;
#endif
}