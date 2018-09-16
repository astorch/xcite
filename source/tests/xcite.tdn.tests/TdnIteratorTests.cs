using System.IO;
using NUnit.Framework;

namespace xcite.tdn.tests {
    [TestFixture]
    public class TdnIteratorTests {
        [Test]
        public void Iterate() {
            // Arrange
            string binFolder = TestContext.CurrentContext.TestDirectory;
            string filePath = Path.Combine(binFolder, "sample1.tdn.txt");
            string content = File.ReadAllText(filePath);

            // Act
            using (TdnIterator itr = new TdnIterator(content)) {
                while (itr.MoveNext()) {

                }
            }

            // Assert
            Assert.Inconclusive("Not fully implemented yet");
        }
    }
}
