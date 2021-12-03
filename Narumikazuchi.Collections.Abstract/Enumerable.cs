namespace Narumikazuchi.Collections.Abstract;

/// <summary>
/// Extends the <see cref="IEnumerable{T}"/> interface.
/// </summary>
public static class Enumerable
{
    /// <summary>
    /// Represents this object as the <see cref="IElementContainer{TElement}"/> interface.
    /// </summary>
    [return: NotNull]
    public static IElementContainer AsIReadOnlyCollection<TCollection, TElement>(this TCollection source)
        where TCollection : ICollection, ICollection<TElement?>, IReadOnlyCollection<TElement?> =>
            new __CollectionWrapper<TCollection, TElement>(source: source);

    /// <summary>
    /// Represents this object as the <see cref="IElementContainer{TElement}"/> interface.
    /// </summary>
    [return: NotNull]
    public static IElementContainer<TElement> AsIReadOnlyCollection2<TCollection, TElement>(this TCollection source)
        where TCollection : ICollection, ICollection<TElement?>, IReadOnlyCollection<TElement?> =>
            new __CollectionWrapper<TCollection, TElement>(source: source);

    /// <summary>
    /// Represents this object as the <see cref="IIndexedReadOnlyCollection{TElement}"/> interface.
    /// </summary>
    [return: NotNull]
    public static IIndexedReadOnlyCollection<Int32, TElement> AsIReadOnlyList2<TList, TElement>(this TList source)
        where TList : ICollection, ICollection<TElement?>, IList, IList<TElement?>, IReadOnlyCollection<TElement?>, IReadOnlyList<TElement?> =>
            new __ListWrapper<TList, TElement>(source: source);

    //public static IReadOnlyMatrix2D<Int32, TElement> ToReadOnlyMatrix2D<TElement>(this IEnumerable<TElement?> source,
    //                                                                              [DisallowNull] Mapping2D<Int32, TElement> mapping) =>
    //    source.ToMatrix2D(mapping);

    //public static IReadOnlyMatrix3D<Int32, TElement> ToReadOnlyMatrix3D<TElement>(this IEnumerable<TElement?> source,
    //                                                                              [DisallowNull] Mapping3D<Int32, TElement> mapping) =>
    //    source.ToMatrix3D(mapping);

    //public static IReadOnlyMatrix4D<Int32, TElement> ToReadOnlyMatrix4D<TElement>(this IEnumerable<TElement?> source,
    //                                                                              [DisallowNull] Mapping4D<Int32, TElement> mapping) =>
    //    source.ToMatrix4D(mapping);

    //public static IMatrix2D<Int32, TElement> ToMatrix2D<TElement>(this IEnumerable<TElement?> source,
    //                                                              [DisallowNull] Mapping2D<Int32, TElement> mapping)
    //{
    //    Matrix2D<Int32, TElement> result = new();

    //    Vector2D<Int32> last = new(x: 0,
    //                               y: 0);
    //    foreach (TElement? element in source)
    //    {
    //        Vector2D<Int32> index = mapping.Invoke(element: element,
    //                                               lastIndex: last);
    //        result.Insert(index: index,
    //                      value: element);
    //        last = index;
    //    }

    //    return result;
    //}

    //public static IMatrix3D<Int32, TElement> ToMatrix3D<TElement>(this IEnumerable<TElement?> source,
    //                                                              [DisallowNull] Mapping3D<Int32, TElement> mapping)
    //{
    //    Matrix3D<Int32, TElement> result = new();

    //    Vector3D<Int32> last = new(x: 0,
    //                               y: 0,
    //                               z: 0);
    //    foreach (TElement? element in source)
    //    {
    //        Vector3D<Int32> index = mapping.Invoke(element: element,
    //                                               lastIndex: last);
    //        result.Insert(index: index,
    //                      value: element);
    //        last = index;
    //    }

    //    return result;
    //}

    //public static IMatrix4D<Int32, TElement> ToMatrix4D<TElement>(this IEnumerable<TElement?> source,
    //                                                              [DisallowNull] Mapping4D<Int32, TElement> mapping)
    //{
    //    Matrix4D<Int32, TElement> result = new();

    //    Vector4D<Int32> last = new(x: 0,
    //                               y: 0,
    //                               z: 0,
    //                               w: 0);
    //    foreach (TElement? element in source)
    //    {
    //        Vector4D<Int32> index = mapping.Invoke(element: element,
    //                                               lastIndex: last);
    //        result.Insert(index: index,
    //                      value: element);
    //        last = index;
    //    }

    //    return result;
    //}
}