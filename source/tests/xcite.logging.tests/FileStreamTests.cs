using System.IO;
using NUnit.Framework;
using FileStream = xcite.logging.streams.FileStream;

namespace xcite.logging.tests {
    [TestFixture]
    public class FileStreamTests {
        [Test]
        public void Write() {
            // Arrange
            string logFilePath = Path.Combine(TestContext.CurrentContext.TestDirectory, "_log.txt");
            string logText = "INFO 30.12.2018 19:44:51 FileStreamTests - Something about logging.\r\n";
            FileStream fileStream = new FileStream {FileName = logFilePath};

            // Act
            fileStream.Write(logText);
            fileStream.Dispose();

            // Assert
            string fileText = File.ReadAllText(logFilePath);
            Assert.AreEqual(logText, fileText);
        }
    }
}