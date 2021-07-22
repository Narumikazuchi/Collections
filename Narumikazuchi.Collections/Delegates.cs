using System;

namespace Narumikazuchi.Collections
{
    /// <summary>
    /// Represents the method to compare two instances of the same type for equality.
    /// </summary>
    public delegate Boolean EqualityComparison<T>(T first, T second);
}
