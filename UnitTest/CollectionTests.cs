using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace UnitTest
{
    [TestClass]
    public class CollectionTests
    {
        [TestMethod]
        public void Clear()
        {
            var collection = PrepareTest();
            collection.Clear();

            Assert.AreEqual(0, collection.Count);
        }

        [TestMethod]
        public void Insert()
        {
            var collection = PrepareTest();
            collection.Insert(0, 0.875d);

            Assert.AreEqual(0.875d, collection[0]);
        }

        [TestMethod]
        public void InsertRange()
        {
            var collection = PrepareTest();
            Double[] array = new Double[] { 0.125d, 0.25d, 0.375d };
            collection.InsertRange(3, array);

            var range = collection.GetRange(3, 5);
            Assert.IsTrue(range.SequenceEqual(array));
        }

        [TestMethod]
        public void Remove()
        {
            var collection = PrepareTest();
            Double item = collection[4];
            collection.Remove(item);

            Assert.IsFalse(collection.Contains(item));
        }

        [TestMethod]
        public void RemoveAll()
        {
            var collection = PrepareTest();
            Collection<Double> expected = new();
            foreach (Double item in collection)
            {
                if (item < 0.5d)
                {
                    expected.Add(item);
                }
            }

            collection.RemoveAll(input => input >= 0.5d);
            _instance.WriteLine("Expected:");
            foreach (Double item in expected)
            {
                _instance.WriteLine(item.ToString());
            }
            _instance.WriteLine("Removed:");
            foreach (Double item in collection)
            {
                _instance.WriteLine(item.ToString());
            }
            Assert.IsTrue(collection.SequenceEqual(expected));
        }

        [TestMethod]
        public void RemoveAt()
        {
            var collection = PrepareTest();
            Double item = collection[6];
            collection.RemoveAt(6);

            Assert.ThrowsException<KeyNotFoundException>(() => collection[6]);
        }

        private static Collection PrepareTest(Int32 count = 16)
        {
            List<(Int32, Double)> array = new();
            for (Int32 i = 0; i < count; i++)
            {
                array.Add((i, Random.NextDouble()));
            }
            Collection result = new(array);
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
