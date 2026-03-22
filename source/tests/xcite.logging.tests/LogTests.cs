using System;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
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

    [Test]
    public void ParallelWithContext() {
        // Arrange
        const int maxWait = 100;
        
        StringBuilderLogStream sbls = new();
        LogManager.Configuration
            .AddStream(sbls)
            .SetPattern("%level %tenantId::%caseId %name - %text%nl")
            ;

        async Task doSomeWorkAndLog(string tenantId, string caseId) {
            ILog log = LogManager.GetLog("WORKER");
            await Task.Delay(Random.Shared.Next(maxWait));
            log.Info("Starting ...");
            await using (LogContext.Add("tenantId", tenantId))
            await using (LogContext.Add("caseId", caseId)) {
                for (int i = -1; ++i != 5;) {
                    await Task.Delay(Random.Shared.Next(maxWait));
                    log.Info($"Running loop {i}");
                    await Task.Delay(Random.Shared.Next(maxWait));
                }
                
                log.Info("Finished loop");
            }
            log.Info("... Finished");
        }
        
        // Act
        int numTasks = 5;
        string[] caseIds = Enumerable.Repeat(0, numTasks).Select(_ => Random.Shared.Next(10000).ToString("#####0")).ToArray();
        Task[] tasks = new Task[numTasks];
        for (int i = -1; ++i != numTasks;) {
            string tenantId = i.ToString();
            string caseId = caseIds[i];
            tasks[i] = doSomeWorkAndLog(tenantId, caseId);
        }

        Task.WaitAll(tasks);

        // Assert
        string streamData = sbls.GetStreamData();
        
        string selTenantId = 0.ToString();
        string selCaseId = caseIds[0];
        Assert.That(streamData, Contains.Substring($"INFO  :: WORKER - Starting ...\n"));
        Assert.That(streamData, Contains.Substring($"INFO  {selTenantId}::{selCaseId} WORKER - Running loop 0\n"));
        Assert.That(streamData, Contains.Substring($"INFO  {selTenantId}::{selCaseId} WORKER - Running loop 1\n"));
        Assert.That(streamData, Contains.Substring($"INFO  {selTenantId}::{selCaseId} WORKER - Running loop 2\n"));
        Assert.That(streamData, Contains.Substring($"INFO  {selTenantId}::{selCaseId} WORKER - Running loop 3\n"));
        Assert.That(streamData, Contains.Substring($"INFO  {selTenantId}::{selCaseId} WORKER - Running loop 4\n"));
        Assert.That(streamData, Contains.Substring($"INFO  {selTenantId}::{selCaseId} WORKER - Finished loop\n"));
        Assert.That(streamData, Contains.Substring($"INFO  :: WORKER - ... Finished\n"));
    }

    private sealed class StringBuilderLogStream : AbstractStream {
        private readonly StringBuilder _stringBuilder = new(1000);
        private readonly Lock _writeLock = new();

        /// <inheritdoc />
        protected override void OnDispose(bool disposing) {
            // Nothing to do here
        }

        protected override void OnWrite(in ReadOnlySpan<char> value, in LogData logData) {
            lock (_writeLock) {
                _stringBuilder.Append(value);
            }
        }

        public string GetStreamData() {
            lock (_writeLock) {
                return _stringBuilder.ToString();
            }
        }
        
    }

    private sealed class FilteredStream : AbstractStream {
        private readonly StringBuilder _stringBuilder = new(1000);
        private readonly Lock _writeLock = new();

        /// <inheritdoc />
        protected override void OnDispose(bool disposing) {
            // Nothing to do here
        }

        protected override void OnWrite(in ReadOnlySpan<char> value, in LogData logData) {
            lock (_writeLock) {
                _stringBuilder.Append(value);
            }
        }

        public string GetStreamData() {
            lock (_writeLock) {
                return _stringBuilder.ToString();
            }
        }

    }
    
}