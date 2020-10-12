using System;
using System.Diagnostics;
using System.Globalization;
using System.Threading;
using NUnit.Framework;

namespace xcite.logging.tests {
    [TestFixture]
    public class TextFormatterTests {
        [Test]
        public void FormatTextValue() {
            // Arrange
            string pattern = LogPatterns.Standard;
            LogData logData = new LogData {
                name = "xcite.logging.tests.Almoner",
                level = ELogLevel.Info,
                value = "A default print-out."
            };
            
            // Act
            TextFormatter tf = new TextFormatter();
            string value = tf.FormatValue(pattern, logData);
            string timestamp = DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss");
            string tid = Thread.CurrentThread.ManagedThreadId.ToString();

            // Assert
            string expVal = $"INFO {timestamp} [{tid}] Almoner - A default print-out.\r\n";
            Assert.AreEqual(expVal, value);
        }

        [Test]
        public void FormatExceptionValue() {
            // Arrange
            string pattern = LogPatterns.Standard;
            LogData logData = new LogData {
                name = "xcite.logging.tests.Almoner",
                level = ELogLevel.Info,
                value = new InvalidOperationException("This is not valid.",
                            new InvalidCastException("Cannot cast value to this",
                                new NullReferenceException("Better do that not")))
                        .ToString()
            };
            
            // Act
            TextFormatter tf = new TextFormatter();
            string value = tf.FormatValue(pattern, logData);
            string timestamp = DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss");
            string tid = Thread.CurrentThread.ManagedThreadId.ToString();
            
            // Assert
            string expVal = $"INFO {timestamp} [{tid}] Almoner - System.InvalidOperationException: This is not valid. ---> " +
                            $"System.InvalidCastException: Cannot cast value to this ---> " +
                            $"System.NullReferenceException: Better do that not\r\n" +
                            $"   --- Ende der internen Ausnahmestapelüberwachung ---\r\n" +
                            $"   --- Ende der internen Ausnahmestapelüberwachung ---" +
                            $"\r\n";
            Assert.AreEqual(expVal, value);
        }
        
        [Test]
        public void LoadTest() {
            // Arrange
            int itrCount = 1000;
            string pattern = LogPatterns.Standard;
            LogData logData = new LogData {
                name = "xcite.logging.tests.Almoner",
                level = ELogLevel.Info,
                value = "A default print-out."
            };
            
            // Act
            TextFormatter tf = new TextFormatter();
            Stopwatch sw = Stopwatch.StartNew();
            for (int i = -1; ++i != itrCount;)
                tf.FormatValue(pattern, logData);
            sw.Stop();

            // Assert
            Console.WriteLine($"#Itr {itrCount.ToString()}, " +
                              $"time {sw.ElapsedMilliseconds.ToString()}, " +
                              $"avg {(sw.ElapsedMilliseconds / (double) itrCount).ToString()}");
            
            Assert.IsTrue(sw.ElapsedMilliseconds < 10);
        }

        [Test]
        public void CultureTest() {
            // Arrange
            string pattern = LogPatterns.Standard;
            LogData logData = new LogData {
                name = "xcite.logging.tests.Almoner",
                level = ELogLevel.Trace,
                value = "This is a message"
            };
            
            // Act
            TextFormatter tf = new TextFormatter();
            string value1 = tf.FormatValue(pattern, logData);
            string timestamp = DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss");
            
            Thread.CurrentThread.CurrentCulture = new CultureInfo("en-us");
            Thread.CurrentThread.CurrentUICulture = new CultureInfo("en-us");

            string value2 = tf.FormatValue(pattern, logData);
            
            // Assert
            int threadId = Thread.CurrentThread.ManagedThreadId;
            Assert.AreEqual($"TRACE {timestamp} [{threadId}] Almoner - This is a message\r\n", value1);
            Assert.AreEqual(value1, value2);
        }
    }
}