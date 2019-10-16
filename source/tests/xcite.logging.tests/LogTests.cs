using System;
using System.Text;
using System.Threading;
using NUnit.Framework;

namespace xcite.logging.tests {
    [TestFixture]
    public class LogTests {
        [OneTimeSetUp]
        public void OneTimeSetUp() {
            LogManager.Configuration.Reset();
        }
        
        [Test]
        public void LogInfo() {
            // Arrange
            StringBuilderLogStream sbls = new StringBuilderLogStream();
            LogManager.Configuration.AddStream(sbls);

            // Act
            ILog log = LogManager.GetLog(typeof(LogTests));
            log.Info("A simple log output.");
            string timestamp = DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss");
            string tid = Thread.CurrentThread.ManagedThreadId.ToString();

            // Assert
            string expVal = $"INFO {timestamp} [{tid}] LogTests - A simple log output.\r\n";
            
            string streamData = sbls.GetStreamData();
            Assert.AreEqual(expVal, streamData);
        }

        [Test]
        public void LogErrorWithException() {
            // Arrange
            StringBuilderLogStream sbls = new StringBuilderLogStream();
            LogManager.Configuration.AddStream(sbls);
            
            // Act
            ILog log = LogManager.GetLog(typeof(LogTests));
            log.Error("Something went wrong.", new InvalidOperationException("This should not have happen."));
            string timestamp = DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss");
            string tid = Thread.CurrentThread.ManagedThreadId.ToString();
            
            // Assert
            string streamData = sbls.GetStreamData();
            string expLine1 = $"ERROR {timestamp} [{tid}] LogTests - Something went wrong.\r\n";
            string expLine2 = $"ERROR {timestamp} [{tid}] LogTests - System.InvalidOperationException: This should not have happen.\r\n";
            
            Assert.IsTrue(streamData.Contains(expLine1));
            Assert.IsTrue(streamData.Contains(expLine2));
        }

        [Test]
        public void DoNotLogDebugOnInfoLevel() {
            // Arrange
            StringBuilderLogStream sbls = new StringBuilderLogStream();
            LogManager.Configuration
                .AddStream(sbls)
                .SetLevel(ELogLevel.Info);

            // Act
            ILog log = LogManager.GetLog(typeof(LogTests));
            log.Debug("An invisible entry.");
            
            // Assert
            string streamData = sbls.GetStreamData();
            Assert.IsEmpty(streamData);
        }

        [Test]
        public void DoLogTraceOnDebugLevel() {
            // Arrange
            StringBuilderLogStream sbls = new StringBuilderLogStream();
            LogManager.Configuration
                .AddStream(sbls)
                .SetLevel(ELogLevel.Debug);

            // Act
            ILog log = LogManager.GetLog(typeof(LogTests));
            log.Trace("A trace record.");
            string timestamp = DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss");
            string tid = Thread.CurrentThread.ManagedThreadId.ToString();

            // Assert
            string expVal = $"TRACE {timestamp} [{tid}] LogTests - A trace record.\r\n";
            string streamData = sbls.GetStreamData();
            Assert.AreEqual(expVal, streamData);
        }
        

        class StringBuilderLogStream : ILogStream {
            private readonly StringBuilder _stringBuilder = new StringBuilder(1000);

            public void Dispose() {
                // Nothing to do here
            }

            public void Write(string value) {
                _stringBuilder.Append(value);
            }

            public string GetStreamData() {
                return _stringBuilder.ToString();
            }
        }
    }
}