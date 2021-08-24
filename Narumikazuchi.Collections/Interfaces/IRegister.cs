namespace Narumikazuchi.Collections
{
    /// <summary>
    /// Represents an <see cref="System.Collections.Generic.IList{T}"/> where every object is only contained once.
    /// </summary>
    public interface IRegister<T> : System.Collections.Generic.IList<T>, IReadOnlyRegister<T>, System.Collections.Generic.ISet<T>
    { }
}