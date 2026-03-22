using System;
using System.Diagnostics;
using System.Globalization;
using System.Threading;
using CommunityToolkit.HighPerformance.Buffers;
using NUnit.Framework;

namespace xcite.logging.tests;

[TestFixture]
public class TextFormatterTests {
    
    [Test]
    public void FormatTextValue() {
        // Arrange
        string pattern = LogPatterns.Standard;
        LogData logData = new(
            name:"xcite.logging.tests.Almoner",
            level:ELogLevel.Info,
            value:"A default print-out.",
            exception:null
        );
            
        // Act
        TextFormatter textFormatter = new();
        ArrayPoolBufferWriter<char> bufferWriter = new();
        
        textFormatter.FormatValue(pattern, logData, bufferWriter);

        string value = new(bufferWriter.WrittenSpan);
        string timestamp = DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss");
        string tid = Thread.CurrentThread.ManagedThreadId.ToString();

        // Assert
        string expVal = $"INFO  {timestamp} [{tid}] Almoner - A default print-out.\n";
        Assert.That(value, Is.EqualTo(expVal));
    }

    [Test]
    public void FormatExceptionValue() {
        // Arrange
        string pattern = LogPatterns.Standard;
        LogData logData = new(
            name:"xcite.logging.tests.Almoner",
            level:ELogLevel.Info,
            value:new InvalidOperationException("This is not valid.",
                                                  new InvalidCastException("Cannot cast value to this",
                                                                           new NullReferenceException("Better do that not")))
                .ToString(),
            exception:null
        );
            
        // Act
        TextFormatter textFormatter = new();
        ArrayPoolBufferWriter<char> bufferWriter = new(); 
        
        textFormatter.FormatValue(pattern, logData, bufferWriter);

        string value = new(bufferWriter.WrittenSpan);
        string timestamp = DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss");
        string tid = Thread.CurrentThread.ManagedThreadId.ToString();
            
        // Assert
        string expVal = $"INFO  {timestamp} [{tid}] Almoner - System.InvalidOperationException: This is not valid.\r\n" +
                        $" ---> System.InvalidCastException: Cannot cast value to this\r\n" +
                        $" ---> System.NullReferenceException: Better do that not\r\n" +
                        $"   --- End of inner exception stack trace ---\r\n" +
                        $"   --- End of inner exception stack trace ---\n";
        Assert.That(value, Is.EqualTo(expVal));
    }
        
    [Test]
    public void LoadTest() {
        // Arrange
        int itrCount = 1000;
        string pattern = LogPatterns.Standard;
        LogData logData = new(
            name: "xcite.logging.tests.Almoner",
            level: ELogLevel.Info,
            value: "A default print-out.",
            exception:null
        );
            
        // Act
        TextFormatter textFormatter = new();
        ArrayPoolBufferWriter<char> bufferWriter = new();
        
        Stopwatch sw = Stopwatch.StartNew();
        for (int i = -1; ++i != itrCount;) {
            bufferWriter.Clear();
            textFormatter.FormatValue(pattern, logData, bufferWriter);
        }
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
        LogData logData = new(
            name: "xcite.logging.tests.Almoner",
            level: ELogLevel.Trace,
            value: "This is a message",
            exception:null
        );
            
        // Act
        TextFormatter textFormatter = new();
        ArrayPoolBufferWriter<char> bufferWriter = new();
        
        textFormatter.FormatValue(pattern, logData, bufferWriter);

        string value1 = new(bufferWriter.WrittenSpan);
        string timestamp = DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss");
            
        Thread.CurrentThread.CurrentCulture = new CultureInfo("en-us");
        Thread.CurrentThread.CurrentUICulture = new CultureInfo("en-us");

        bufferWriter.Clear();
        textFormatter.FormatValue(pattern, logData, bufferWriter);

        string value2 = new(bufferWriter.WrittenSpan);
            
        // Assert
        int threadId = Thread.CurrentThread.ManagedThreadId;
        Assert.AreEqual($"TRACE {timestamp} [{threadId}] Almoner - This is a message\n", value1);
        Assert.AreEqual(value1, value2);
    }
    
}