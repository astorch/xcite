using System.IO;
using NUnit.Framework;

namespace xcite.tdn.tests {
    [TestFixture]
    public class TdnReaderTests {
        [Test]
        public void Iterate() {
            // Arrange
            string binFolder = TestContext.CurrentContext.TestDirectory;
            string filePath = Path.Combine(binFolder, "sample1.tdn.txt");
            string content = File.ReadAllText(filePath);

            // Act
            TdnReader tdnReader = new TdnReader();
            using (var itr = tdnReader.Read(content)) {
                while (itr.MoveNext()) {
                    // TdnProperty tdnProp = itr.Current;

                }
            }

            // Assert
            Assert.Inconclusive("Not fully implemented yet");
        }
    }
}
