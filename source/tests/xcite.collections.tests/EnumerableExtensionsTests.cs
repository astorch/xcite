using NUnit.Framework;

namespace xcite.collections.tests {
    [TestFixture]
    public class EnumerableExtensionsTests {
        [Test]
        public void Skip() {
            // Arrange
            int[] noList = {5, 13, 12, 10, 9};

            // Act
            int[] sl1 = noList.Skip(0).ToArray();
            int[] sl2 = noList.Skip(1).ToArray();
            int[] sl3 = noList.Skip(4).ToArray();

            // Assert
            CollectionAssert.AreEqual(new[] {5, 13, 12, 10, 9}, sl1);
            CollectionAssert.AreEqual(new[] {13, 12, 10, 9}, sl2);
            CollectionAssert.AreEqual(new[] {9}, sl3);
        }
    }
}