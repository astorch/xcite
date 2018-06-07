using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace xcite.collections.tests {
    [TestFixture]
    public class EnumerableExtensionsBaseTests {
        [Test]
        public void CountList() {
            // Arrange
            List<int> list = new List<int> {1, 2, 3, 5, 12};

            // Act
            int count = list.Count();

            // Assert
            Assert.AreEqual(list.Count, count);
        }

        [Test]
        public void CountArray() {
            // Arrange
            int[] list = {1, 2, 3, 5, 12};

            // Act
            int count = list.Count();

            // Assert
            Assert.AreEqual(list.Length, count);
        }

        [Test]
        public void CountEnumerable() {
            // Arrange
            int[] list = { 1, 2, 3, 5, 12 };
            IEnumerable<int> sequence = list.Where(i => i % 2 == 0);

            // Act
            int count = sequence.Count();

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
            bool anyInEmpty = emptyList.Any();
            bool anyInFilled = filledList.Any();
            bool anyInSeq1 = seq1.Any();
            bool anyInSeq2 = seq2.Any();

            // Assert
            Assert.AreEqual(false, anyInEmpty);
            Assert.AreEqual(true, anyInFilled);
            Assert.AreEqual(false, anyInSeq1);
            Assert.AreEqual(true, anyInSeq2);
        }
    }
}