using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using NUnit.Framework;
using NUnit.Framework.Constraints;

namespace xcite.collections.tests {
    [TestFixture]
    public class LinearListSortingTests {
        [Test]
        public void Sort() {
            LinearList<int> list = new LinearList<int>(new[] {2, 5, 1, 7, 3, 4, 6});
            list.Sort((i1, i2) => i1.CompareTo(i2));

            int[] sortedList = list.ToArray();
            CollectionAssert.AreEqual(new[] {1, 2, 3, 4, 5, 6, 7}, sortedList);
        }

        [Test]
        public void SortWithZero() {
            LinearList<int> list = new LinearList<int>(new[]{2,0,1});
            list.Sort((i1, i2) => i1.CompareTo(i2));

            int[] sortedList = list.ToArray();
            CollectionAssert.AreEqual(new[] {0, 1, 2}, sortedList);
        }

        [Test]
        public void SortWithProperty() {
            IndexObject[] objetSet = new IndexObject[] {
                new IndexObject {Id = "eins", Index = 2},
                new IndexObject {Id = "zwei", Index = 0},
                new IndexObject {Id = "drei", Index = 1}
            };

            LinearList<IndexObject> list = new LinearList<IndexObject>(objetSet);
            list.Sort(item => item.Index);

            IndexObject[] sortedSet = list.ToArray();
            CollectionAssert.AreEqual(new IndexObject[] {objetSet[1], objetSet[2], objetSet[0]}, sortedSet);
        }

        [Test]
        public void SortHuge() {
            int[] array = GetRandomArray(5000, 10000);

            LinearList<int> list = new LinearList<int>(array);
            LinkedList<int> linkedList = new LinkedList<int>(array);

            Stopwatch sw1 = Stopwatch.StartNew();
            list.Sort((i1, i2) => i1 < i2 ? -1 : 0);
            sw1.Stop();

            Console.WriteLine($"Duration linear list: {sw1.ElapsedMilliseconds}ms");

            Stopwatch sw2 = Stopwatch.StartNew();
            var llSorted = linkedList.OrderBy(i => i, Comparer<int>.Default).ToArray();
            sw2.Stop();

            Console.WriteLine($"Duration linked list: {sw2.ElapsedMilliseconds}ms");

            Stopwatch sw3 = Stopwatch.StartNew();
            Array.Sort(array);
            sw3.Stop();

            Console.WriteLine($"Duration array: {sw3.ElapsedMilliseconds}ms");

            CollectionAssert.AreEqual(array, list.ToArray());
            CollectionAssert.AreEqual(array, llSorted);
        }

        private int[] GetRandomArray(int size, int max) {
            Random r = new Random();
            int[] dataSet = new int[size];
            for (int i = -1; ++i != dataSet.Length;) {
                dataSet[i] = r.Next(max) + 1;
            }
            return dataSet;
        }

        class IndexObject {
            public string Id { get; set; }

            public int Index { get; set; }
        }
    }
}
