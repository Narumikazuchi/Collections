using System;
using System.Collections.Generic;
using Narumikazuchi.Collections.Abstract;

namespace UnitTest
{
    public class ReadOnlyCollection : ReadOnlyCollectionBase<Int32, Double>
    {
        public ReadOnlyCollection(IEnumerable<(Int32, Double)> collection) :
            base(collection: collection)
        { }
    }

    public class Collection : CollectionBase<Int32, Double>
    {
        public Collection(IEnumerable<(Int32, Double)> collection) :
            base(collection: collection)
        { }
    }

    public class ReadOnlySet : ReadOnlySetBase<Int32, Double>, IEnumerable<(Int32, Double)>
    {
        public ReadOnlySet(IEnumerable<(Int32, Double)> collection) :
            base(collection: collection)
        { }

        IEnumerator<(Int32, Double)> IEnumerable<(Int32, Double)>.GetEnumerator()
        {
            foreach (var kv in this.GetKeyValuePairsFirstToLast())
            {
                yield return (kv.Key, kv.Value);
            }
            yield break;
        }
    }

    public class Set : SetBase<Int32, Double>, IEnumerable<(Int32, Double)>
    {
        public Set(IEnumerable<(Int32, Double)> collection) :
            base(collection: collection)
        { }

        IEnumerator<(Int32, Double)> IEnumerable<(Int32, Double)>.GetEnumerator()
        {
            foreach (var kv in this.GetKeyValuePairsFirstToLast())
            {
                yield return (kv.Key, kv.Value);
            }
            yield break;
        }
    }
}
