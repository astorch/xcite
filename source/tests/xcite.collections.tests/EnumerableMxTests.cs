using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace xcite.collections.tests {
    [TestFixture]
    public class EnumerableMxTests {
        [Test]
        public void CountList() {
            // Arrange
            List<int> list = new List<int>() {1, 2, 3, 5, 12};

            // Act
            int count = nogen.EnumerableMx.Count(list);

            // Assert
            Assert.AreEqual(list.Count, count);
        }

        [Test]
        public void CountArray() {
            // Arrange
            int[] list = {1, 2, 3, 5, 12};

            // Act
            int count = nogen.EnumerableMx.Count(list);

            // Assert
            Assert.AreEqual(list.Length, count);
        }

        [Test]
        public void CountEnumerable() {
            // Arrange
            int[] list = { 1, 2, 3, 5, 12 };
            IEnumerable<int> sequence = list.Where(i => i % 2 == 0);

            // Act
            int count = nogen.EnumerableMx.Count(sequence);

            // Assert
            Assert.AreEqual(2, count);
        }

        [Test]
        public void Any() {
            // Arrange
            List<int> emptyList = new List<int>();
            List<int> filledList = new List<int> {1, 5, 7, 9};
            IEnumerable<int> seq1 = filledList.Where(i => i % 2 == 0);
            IEnumerable<int> seq2 = filledList.Where(i => i % 3 == 0);

            // Act
            bool anyInEmpty = nogen.EnumerableMx.Any(emptyList);
            bool anyInFilled = nogen.EnumerableMx.Any(filledList);
            bool anyInSeq1 = nogen.EnumerableMx.Any(seq1);
            bool anyInSeq2 = nogen.EnumerableMx.Any(seq2);

            // Assert
            Assert.AreEqual(false, anyInEmpty);
            Assert.AreEqual(true, anyInFilled);
            Assert.AreEqual(false, anyInSeq1);
            Assert.AreEqual(true, anyInSeq2);
        }
    }
}