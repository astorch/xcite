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

        [Test]
        public void ReadFileWithFlags() {
            // Arrange
            string logCfgFn = Path.Combine(TestContext.CurrentContext.TestDirectory, "_utlog2.cfg");
            
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
            Assert.AreEqual("_lfc2.txt", ((FileStream) fileStream).FileName);
            Assert.AreEqual(true, ((FileStream) fileStream).Append);
            Assert.AreEqual(ELockingModel.Minimal, ((FileStream) fileStream).LockingModel);
        }

        [Test]
        public void ReadWithArguments() {
            // Arrange
            string logCfgFn = Path.Combine(TestContext.CurrentContext.TestDirectory, "_utlog3.cfg");
            
            // Act
            LogConfiguration lgCfg = ConfigurationReader.ReadFile(logCfgFn, false);
            
            // Assert
            Assert.IsNotNull(lgCfg);
            Assert.AreEqual(ELogLevel.Debug, lgCfg.Level);
            Assert.AreEqual("%date(dd.MM.yyyy HH:mm:ss:fff) %level %text%nl", lgCfg.Pattern);
            
            Assert.AreEqual(3, lgCfg.Streams.Length);
            
            ILogStream debugStream = lgCfg.Streams[0];
            Assert.AreEqual(typeof(DebugStream), debugStream.GetType());
            
            ILogStream consoleStream = lgCfg.Streams[1];
            Assert.AreEqual(typeof(ConsoleStream), consoleStream.GetType());
            
            ILogStream fileStream = lgCfg.Streams[2];
            Assert.AreEqual(typeof(FileStream), fileStream.GetType());
        }
        
        [Test]
        public void ReadWithCustomStream() {
            // Arrange
            string logCfgFn = Path.Combine(TestContext.CurrentContext.TestDirectory, "_utlog4.cfg");

            // Act
            LogConfiguration lgCfg = ConfigurationReader.ReadFile(logCfgFn, false);
            
            // Assert
            Assert.IsNotNull(lgCfg);
            Assert.AreEqual(ELogLevel.Debug, lgCfg.Level);
            Assert.AreEqual("%date %level %text%nl", lgCfg.Pattern);
            
            Assert.AreEqual(2, lgCfg.Streams.Length);
            
            ILogStream customStream = lgCfg.Streams[0];
            Assert.AreEqual(typeof(ConsoleStream), customStream.GetType());

            ILogStream hiddenStream = lgCfg.Streams[1];
            Assert.AreEqual(typeof(HiddenStream), hiddenStream.GetType());
        }
        
        [Test]
        public void ReadWithTypes() {
            // Arrange
            string logCfgFn = Path.Combine(TestContext.CurrentContext.TestDirectory, "_utlog5.cfg");

            // Act
            LogConfiguration lgCfg = ConfigurationReader.ReadFile(logCfgFn, false);
            
            // Assert
            Assert.IsNotNull(lgCfg);
            Assert.AreEqual(ELogLevel.Debug, lgCfg.Level);
            Assert.AreEqual("%date %level %text%nl", lgCfg.Pattern);
            
            Assert.AreEqual(1, lgCfg.Streams.Length);
            
            ILogStream stream = lgCfg.Streams[0];
            
            Assert.IsTrue(stream is AbstractStream);
            CollectionAssert.AreEqual(new[] { "foo", "bar" }, ((AbstractStream)stream).Types);
        }

        [Test]
        public void SetLogManagerConfiguration() {
            // Arrange
            string logCfgFn = Path.Combine(TestContext.CurrentContext.TestDirectory, "_utlog.cfg");
            
            // Act
            LogManager.Configuration =  ConfigurationReader.ReadFile(logCfgFn, false);
            
            // Assert
            Assert.AreEqual(ELogLevel.Debug, LogManager.Configuration.Level);
            Assert.AreEqual("%date %level %text%nl", LogManager.Configuration.Pattern);
            Assert.AreEqual(2, LogManager.Configuration.Streams.Length);
        }

        [Test]
        public void ApplyConfigurationFileTwice() {
            // Arrange
            string logCfgFn = Path.Combine(TestContext.CurrentContext.TestDirectory, "_utlog.cfg");
            LogConfiguration lgCfg = ConfigurationReader.ReadFile(logCfgFn, false);
            LogManager.Configuration = lgCfg;
            
            // Act
            ILog log1 = LogManager.GetLog("UTUnit");
            log1.Info("Initialized");

            LogConfiguration lgCfg2 = ConfigurationReader.ReadFile(logCfgFn, false);
            LogManager.Configuration = lgCfg2;

            ILog log2 = LogManager.GetLog("UTUnit2");
            log2.Info("Re-initialized");
            
            // Assert
        }

        class HiddenStream : AbstractStream {

            protected override void OnDispose(bool disposing) {
                
            }

            protected override void Write(string value) {
                
            }
            
        }
    }
}