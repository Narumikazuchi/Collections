using Microsoft.VisualStudio.TestTools.UnitTesting;
using Narumikazuchi.Collections.Abstract;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace UnitTest
{
    [TestClass]
    public class ReadOnlySetTests
    {
        [TestMethod]
        public void IsProperSubsetOf()
        {
            var set = PrepareTest();
            var empty = new ReadOnlySet(Array.Empty<(Int32, Double)>());
            (Int32, Double)[] array = { (27, 0.875), (69, 0.125) };
            var bigger = new ReadOnlySet(set.Union(array));

            Assert.IsTrue(empty.IsProperSubsetOf(set));
            Assert.IsTrue(set.IsProperSubsetOf(bigger));
            Assert.IsFalse(empty.IsProperSubsetOf(empty));
        }

        [TestMethod]
        public void IsProperSupersetOf()
        {
            var set = PrepareTest();
            var empty = new ReadOnlySet(Array.Empty<(Int32, Double)>());
            (Int32, Double)[] array = { (27, 0.875), (69, 0.125) };
            var bigger = new ReadOnlySet(set.Union(array));

            Assert.IsTrue(set.IsProperSupersetOf(empty));
            Assert.IsTrue(bigger.IsProperSupersetOf(set));
            Assert.IsFalse(empty.IsProperSupersetOf(empty));
        }

        [TestMethod]
        public void IsSubsetOf()
        {
            var set = PrepareTest();
            var empty = new ReadOnlySet(Array.Empty<(Int32, Double)>());
            (Int32, Double)[] array = { (27, 0.875), (69, 0.125) };
            var bigger = new ReadOnlySet(set.Union(array));

            Assert.IsTrue(empty.IsSubsetOf(set));
            Assert.IsTrue(set.IsSubsetOf(bigger));
            Assert.IsTrue(empty.IsSubsetOf(empty));
        }

        [TestMethod]
        public void IsSupersetOf()
        {
            var set = PrepareTest();
            var empty = new ReadOnlySet(Array.Empty<(Int32, Double)>());
            (Int32, Double)[] array = { (27, 0.875), (69, 0.125) };
            var bigger = new ReadOnlySet(set.Union(array));

            Assert.IsTrue(set.IsSupersetOf(empty));
            Assert.IsTrue(bigger.IsSupersetOf(set));
            Assert.IsTrue(empty.IsSupersetOf(empty));
        }

        [TestMethod]
        public void Overlaps()
        {
            var set = PrepareTest();
            var range = set.GetRange(3, 6);

            Assert.IsTrue(set.Overlaps(range));
        }

        [TestMethod]
        public void SetEquals()
        {
            var set = PrepareTest();
            var range = set.GetRange(0, 15);

            Assert.IsTrue(set.SetEquals(range));
        }

        private static ReadOnlySet PrepareTest(Int32 count = 16)
        {
            List<(Int32, Double)> array = new();
            for (Int32 i = 0; i < count; i++)
            {
                array.Add((i, Random.NextDouble()));
            }
            ReadOnlySet result = new(array);
            return result;
        }

        public TestContext TestContext
        {
            get => this._instance;
            set => this._instance = value;
        }

        private static Random Random { get; } = new();

        private TestContext _instance;
    }
}
