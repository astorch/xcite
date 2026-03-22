using System;
using System.Text;
using System.Threading;
using NUnit.Framework;
using xcite.logging.streams;

namespace xcite.logging.tests;

[TestFixture]
public class LogTests {
    
    [OneTimeSetUp]
    public void OneTimeSetUp() {
        LogManager.Configuration.Reset();
    }
        
    [Test]
    public void LogInfo() {
        // Arrange
        StringBuilderLogStream sbls = new();
        LogManager.Configuration.AddStream(sbls);

        // Act
        ILog log = LogManager.GetLog(typeof(LogTests));
        log.Info("A simple log output.");
        string timestamp = DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss");
        string tid = Thread.CurrentThread.ManagedThreadId.ToString();

        // Assert
        string expVal = $"INFO  {timestamp} [{tid}] LogTests - A simple log output.\n";
            
        string streamData = sbls.GetStreamData();
        Assert.That(streamData, Is.EqualTo(expVal));
    }

    [Test]
    public void LogErrorWithException() {
        // Arrange
        StringBuilderLogStream sbls = new();
        LogManager.Configuration.AddStream(sbls);
            
        // Act
        ILog log = LogManager.GetLog(typeof(LogTests));
        log.Error("Something went wrong.", new InvalidOperationException("This should not have happen."));
        string timestamp = DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss");
        string tid = Thread.CurrentThread.ManagedThreadId.ToString();
            
        // Assert
        string streamData = sbls.GetStreamData();
        string expLine1 = $"ERROR {timestamp} [{tid}] LogTests - Something went wrong.\n";
        string expLine2 = $"ERROR {timestamp} [{tid}] LogTests - System.InvalidOperationException: This should not have happen.\n";
            
        Assert.That(streamData, Contains.Substring(expLine1));
        Assert.That(streamData, Contains.Substring(expLine2));
    }

    [Test]
    public void DoNotLogDebugOnInfoLevel() {
        // Arrange
        StringBuilderLogStream sbls = new();
        LogManager.Configuration
            .AddStream(sbls)
            .SetLevel(ELogLevel.Info);

        // Act
        ILog log = LogManager.GetLog(typeof(LogTests));
        log.Debug("An invisible entry.");
            
        // Assert
        string streamData = sbls.GetStreamData();
        Assert.That(streamData, Is.Empty);
    }

    [Test]
    public void DoLogTraceOnDebugLevel() {
        // Arrange
        StringBuilderLogStream sbls = new();
        LogManager.Configuration
            .AddStream(sbls)
            .SetLevel(ELogLevel.Debug);

        // Act
        ILog log = LogManager.GetLog(typeof(LogTests));
        log.Trace("A trace record.");
        string timestamp = DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss");
        string tid = Thread.CurrentThread.ManagedThreadId.ToString();

        // Assert
        string expVal = $"TRACE {timestamp} [{tid}] LogTests - A trace record.\n";
        string streamData = sbls.GetStreamData();
        Assert.That(streamData, Is.EqualTo(expVal));
    }

    [Test]
    public void DoNotLogIfWrongType() {
        // Arrange
        FilteredStream fooStream = new() {
            Types = ["FooType"]
        };
        FilteredStream barStream = new() {
            Types = ["BarType"]
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
        string expFooVal = $"TRACE {timestamp} [{tid}] FooType - foo\n";
        string expBarVal = $"TRACE {timestamp} [{tid}] BarType - bar\n";
        string fooStreamData = fooStream.GetStreamData();
        string barStreamData = barStream.GetStreamData();
        Assert.That(fooStreamData, Is.EqualTo(expFooVal));
        Assert.That(barStreamData, Is.EqualTo(expBarVal));
    }

    private sealed class StringBuilderLogStream : AbstractStream {
        private readonly StringBuilder _stringBuilder = new(1000);

        /// <inheritdoc />
        protected override void OnDispose(bool disposing) {
            // Nothing to do here
        }

        protected override void OnWrite(in ReadOnlySpan<char> value, in LogData logData) {
            _stringBuilder.Append(value);
        }

        public string GetStreamData() {
            return _stringBuilder.ToString();
        }
        
    }

    private sealed class FilteredStream : AbstractStream {
        private readonly StringBuilder _stringBuilder = new(1000);

        /// <inheritdoc />
        protected override void OnDispose(bool disposing) {
            // Nothing to do here
        }

        protected override void OnWrite(in ReadOnlySpan<char> value, in LogData logData) {
            _stringBuilder.Append(value);
        }

        public string GetStreamData() {
            return _stringBuilder.ToString();
        }
        
    }
    
}