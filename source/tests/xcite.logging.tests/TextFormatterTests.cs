using System;
using System.Diagnostics;
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
    }
}