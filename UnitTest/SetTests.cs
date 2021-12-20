using Microsoft.VisualStudio.TestTools.UnitTesting;
using Narumikazuchi.Collections.Abstract;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace UnitTest
{
    [TestClass]
    public class SetTests
    {
        [TestMethod]
        public void Add()
        {
            var set = PrepareTest();
            Assert.IsTrue(set.Add(0.875d));
            Assert.AreEqual(set[16], 0.875d);
        }

        [TestMethod]
        public void ExceptWith()
        {
            var set = PrepareTest();
            var range = set.GetRange(0, 3);

            set.ExceptWith(range);
            foreach (var item in range)
            {
                Assert.IsFalse(set.Contains(item));
            }
        }

        [TestMethod]
        public void IntersectWith()
        {
            var set = PrepareTest(8);
            var range = set.GetRange(3, 6);

            set.IntersectWith(range);
            Assert.ThrowsException<KeyNotFoundException>(() => set[0]);
            Assert.ThrowsException<KeyNotFoundException>(() => set[1]);
            Assert.ThrowsException<KeyNotFoundException>(() => set[2]);
            foreach (var item in range)
            {
                Assert.IsTrue(set.Contains(item));
            }
            Assert.ThrowsException<KeyNotFoundException>(() => set[7]);
        }

        [TestMethod]
        public void SymmetricExceptWith()
        {
            var set = PrepareTest();
            var range = set.GetRange(3, 6);

            set.SymmetricExceptWith(range);
            Assert.ThrowsException<KeyNotFoundException>(() => set[3]);
            Assert.ThrowsException<KeyNotFoundException>(() => set[4]);
            Assert.ThrowsException<KeyNotFoundException>(() => set[5]);
            Assert.ThrowsException<KeyNotFoundException>(() => set[6]);
        }

        [TestMethod]
        public void UnionWith()
        {
            var set = PrepareTest();
            Double[] array = { 0.125d, 0.375d, 0.875d };

            set.UnionWith(array);
            Assert.IsTrue(set.Contains(0.125d));
            Assert.IsTrue(set.Contains(0.375d));
            Assert.IsTrue(set.Contains(0.875d));
        }

        private static Set PrepareTest(Int32 count = 16)
        {
            List<(Int32, Double)> array = new();
            for (Int32 i = 0; i < count; i++)
            {
                array.Add((i, Random.NextDouble()));
            }
            Set result = new(array);
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
