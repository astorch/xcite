using System;
using System.Text;
using System.Threading;
using NUnit.Framework;
using xcite.logging.streams;

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

        [Test]
        public void DoNotLogIfWrongType() {
            // Arrange
            FilteredStream fooStream = new FilteredStream {
                Types = new[] { "FooType" }
            };
            FilteredStream barStream = new FilteredStream {
                Types = new[] { "BarType" }
            };
            LogManager.Configuration
                .AddStream(fooStream)
                .AddStream(barStream)
                .SetLevel(ELogLevel.Debug);

            // Act
            ILog fooLog = LogManager.GetLog("FooType");
            ILog barLog = LogManager.GetLog("BarType");
            fooLog.Trace("foo");
            barLog.Trace("bar");
            string timestamp = DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss");
            string tid = Thread.CurrentThread.ManagedThreadId.ToString();

            // Assert
            string expFooVal = $"TRACE {timestamp} [{tid}] FooType - foo\r\n";
            string expBarVal = $"TRACE {timestamp} [{tid}] BarType - bar\r\n";
            string fooStreamData = fooStream.GetStreamData();
            string barStreamData = barStream.GetStreamData();
            Assert.AreEqual(expFooVal, fooStreamData);
            Assert.AreEqual(expBarVal, barStreamData);
        }

        private class StringBuilderLogStream : AbstractStream {
            private readonly StringBuilder _stringBuilder = new StringBuilder(1000);

            /// <inheritdoc />
            protected override void OnDispose(bool disposing) {
                // Nothing to do here
            }

            protected override void Write(string value) {
                _stringBuilder.Append(value);
            }

            public string GetStreamData() {
                return _stringBuilder.ToString();
            }
        }

        private class FilteredStream : AbstractStream {
            private readonly StringBuilder _stringBuilder = new StringBuilder(1000);

            /// <inheritdoc />
            protected override void OnDispose(bool disposing) {
                // Nothing to do here
            }

            protected override void Write(string value) {
                _stringBuilder.Append(value);
            }

            public string GetStreamData() {
                return _stringBuilder.ToString();
            }
        }
    }
}