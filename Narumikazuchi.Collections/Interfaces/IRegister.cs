namespace Narumikazuchi.Collections
{
    /// <summary>
    /// Represents an <see cref="System.Collections.Generic.IList{T}"/> where every object is only contained once.
    /// </summary>
    public interface IRegister<TElement> : System.Collections.Generic.IList<TElement>, IReadOnlyRegister<TElement>, System.Collections.Generic.ISet<TElement>
    { }
}