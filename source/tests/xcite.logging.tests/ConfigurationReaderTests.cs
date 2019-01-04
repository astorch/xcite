using System.IO;
using NUnit.Framework;
using xcite.logging.streams;
using FileStream = xcite.logging.streams.FileStream;

namespace xcite.logging.tests {
    [TestFixture]
    public class ConfigurationReaderTests {
        [Test]
        public void ReadFile() {
            // Arrange
            string logCfgFn = Path.Combine(TestContext.CurrentContext.TestDirectory, "_utlog.cfg");
            
            // Act
            LogConfiguration lgCfg = ConfigurationReader.ReadFile(logCfgFn, false);
            
            // Assert
            Assert.IsNotNull(lgCfg);
            Assert.AreEqual(ELogLevel.Debug, lgCfg.Level);
            Assert.AreEqual("%date %level %text%nl", lgCfg.Pattern);
            
            Assert.AreEqual(2, lgCfg.Streams.Length);
            
            ILogStream consoleStream = lgCfg.Streams[0];
            Assert.AreEqual(typeof(ConsoleStream), consoleStream.GetType());

            ILogStream fileStream = lgCfg.Streams[1];
            Assert.AreEqual(typeof(FileStream), fileStream.GetType());
            Assert.AreEqual("_lfc.txt", ((FileStream) fileStream).FileName);
        }
    }
}