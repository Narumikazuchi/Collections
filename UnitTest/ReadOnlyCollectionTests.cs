using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace UnitTest
{
    [TestClass]
    public class ReadOnlyCollectionTests
    {
        [TestMethod]
        public async Task AsyncForEach()
        {
            var collection = PrepareTest();
            var destination = new ConcurrentQueue<Double>();

            await foreach (var item in collection)
            {
                destination.Enqueue(item);
            }

            Assert.IsTrue(collection.SequenceEqual(destination));
        }

        [TestMethod]
        public void Contains()
        {
            var collection = PrepareTest();
            Double item = collection[8];

            Assert.IsTrue(collection.Contains(item));
            Assert.IsFalse(collection.Contains(-item));
        }

        [TestMethod]
        public void ConvertAll()
        {
            var collection = PrepareTest();
            Int32 item = (Int32)(collection[8] * 100d);

            var converted = collection.ConvertAll(input => (Int32)(input * 100d))
                                      .ToList();

            Assert.AreEqual(item, converted[8]);
        }

        [TestMethod]
        public void CopyTo()
        {
            var collection = PrepareTest();
            Double[] destination = new Double[Int16.MaxValue];
            Double item = collection[8];

            collection.CopyTo(destination,
                              8);

            Assert.AreEqual(item, destination[16]);
        }

        [TestMethod]
        public void Exists()
        {
            var collection = PrepareTest();
            Boolean expected = false;
            foreach (Double item in collection)
            {
                if (item >= 0.5d)
                {
                    expected = true;
                    break;
                }
            }

            Boolean result = collection.Exists(input => input >= 0.5d);
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void Find()
        {
            var collection = PrepareTest();
            Double expected = default;
            foreach (Double item in collection)
            {
                if (item >= 0.5d)
                {
                    expected = item;
                    break;
                }
            }

            Double result = collection.Find(input => input >= 0.5d);
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void FindAll()
        {
            var collection = PrepareTest();
            Collection<Double> expected = new();
            foreach (Double item in collection)
            {
                if (item >= 0.5d)
                {
                    expected.Add(item);
                }
            }

            var result = collection.FindAll(input => input >= 0.5d);
            Assert.IsTrue(expected.SequenceEqual(result));
        }

        [TestMethod]
        public void FindExcept()
        {
            var collection = PrepareTest();
            Collection<Double> expected = new();
            foreach (Double item in collection)
            {
                if (item >= 0.5d)
                {
                    expected.Add(item);
                }
            }

            var result = collection.FindExcept(input => input < 0.5d);
            Assert.IsTrue(expected.SequenceEqual(result));
        }

        [TestMethod]
        public void FindIndex()
        {
            var collection = PrepareTest();
            Double item = collection[8];

            Int32 index = collection.FindIndex(input => input == item);
            Assert.AreEqual(8, index);

            index = collection.FindIndex(input => input == -item);
            Assert.AreEqual(-1, index);
        }

        [TestMethod]
        public void FindLast()
        {
            var collection = PrepareTest();
            Double expected = default;
            foreach (Double item in collection)
            {
                if (item >= 0.5d)
                {
                    expected = item;
                }
            }

            Double result = collection.FindLast(input => input >= 0.5d);
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void FindLastIndex()
        {
            var collection = PrepareTest();
            Double item = collection[8];

            Int32 index = collection.FindLastIndex(input => input == item);
            Assert.AreEqual(8, index);

            index = collection.FindLastIndex(input => input == -item);
            Assert.AreEqual(-1, index);
        }

        [TestMethod]
        public void ForEach()
        {
            var collection = PrepareTest();
            collection.ForEach(input => _instance.WriteLine(input.ToString()));
        }

        [TestMethod]
        public void GetRange()
        {
            var collection = PrepareTest();
            Collection<Double> expected = new();
            Int32 count = 0;
            foreach (Double item in collection)
            {
                if (count++ < 5)
                {
                    expected.Add(item);
                }
            }

            var result = collection.GetRange(0, 4);
            Assert.IsTrue(expected.SequenceEqual(result));
        }

        [TestMethod]
        public void IndexOf()
        {
            var collection = PrepareTest();
            Double item = collection[8];

            Int32 index = collection.IndexOf(item);
            Assert.AreEqual(8, index);

            index = collection.IndexOf(-item);
            Assert.AreEqual(-1, index);
        }

        [TestMethod]
        public void LastIndexOf()
        {
            var collection = PrepareTest();
            Double item = collection[8];

            Int32 index = collection.LastIndexOf(item);
            Assert.AreEqual(8, index);

            index = collection.LastIndexOf(-item);
            Assert.AreEqual(-1, index);
        }

        [TestMethod]
        public void ToArray()
        {
            var collection = PrepareTest();
            var array = collection.ToArray();

            Assert.IsTrue(array.SequenceEqual(collection));
        }

        private static ReadOnlyCollection PrepareTest(Int32 count = 16)
        {
            List<(Int32, Double)> array = new();
            for (Int32 i = 0; i < count; i++)
            {
                array.Add((i, Random.NextDouble()));
            }
            ReadOnlyCollection result = new(array);
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
