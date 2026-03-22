using System;
using System.IO;
using NUnit.Framework;
using xcite.logging.streams;
using FileStream = xcite.logging.streams.FileStream;

namespace xcite.logging.tests;

[TestFixture]
public class ConfigurationReaderTests {
    
    [Test]
    public void ReadFile() {
        // Arrange
        string logCfgFn = Path.Combine(TestContext.CurrentContext.TestDirectory, "samples", "_utlog.cfg");
            
        // Act
        LogConfiguration lgCfg = ConfigurationReader.ReadFile(logCfgFn, false);
            
        // Assert
        Assert.That(lgCfg, Is.Not.Null);
        Assert.That(lgCfg.Level, Is.EqualTo(ELogLevel.Debug));
        Assert.That(lgCfg.Pattern, Is.EqualTo("%date %level %text%nl"));
        
        Assert.That(lgCfg.Streams, Has.Length.EqualTo(2));    
        
        ILogStream consoleStream = lgCfg.Streams[0];
        Assert.That(consoleStream, Is.TypeOf<ConsoleStream>());
        
        ILogStream fileStream = lgCfg.Streams[1];
        Assert.That(fileStream, Is.TypeOf<FileStream>());
        Assert.That(((FileStream) fileStream).FileName, Is.EqualTo("_lfc.txt"));
    }

    [Test]
    public void ReadFileWithFlags() {
        // Arrange
        string logCfgFn = Path.Combine(TestContext.CurrentContext.TestDirectory, "samples", "_utlog2.cfg");
            
        // Act
        LogConfiguration lgCfg = ConfigurationReader.ReadFile(logCfgFn, false);
            
        // Assert
        Assert.That(lgCfg, Is.Not.Null);
        Assert.That(lgCfg.Level, Is.EqualTo(ELogLevel.Debug));
        Assert.That(lgCfg.Pattern, Is.EqualTo("%date %level %text%nl"));
        
        Assert.That(lgCfg.Streams, Has.Length.EqualTo(2));    
        
        ILogStream consoleStream = lgCfg.Streams[0];
        Assert.That(consoleStream, Is.TypeOf<ConsoleStream>());
        
        ILogStream fileStream = lgCfg.Streams[1];
        Assert.That(fileStream, Is.TypeOf<FileStream>());
        Assert.That(((FileStream) fileStream).FileName, Is.EqualTo("_lfc2.txt"));
        
        Assert.That(((FileStream) fileStream).Append, Is.True);
        Assert.That(((FileStream) fileStream).LockingModel, Is.EqualTo(ELockingModel.Minimal));
    }

    [Test]
    public void ReadWithArguments() {
        // Arrange
        string logCfgFn = Path.Combine(TestContext.CurrentContext.TestDirectory, "samples", "_utlog3.cfg");
            
        // Act
        LogConfiguration lgCfg = ConfigurationReader.ReadFile(logCfgFn, false);
            
        // Assert
        Assert.That(lgCfg, Is.Not.Null);
        Assert.That(lgCfg.Level, Is.EqualTo(ELogLevel.Debug));
        Assert.That(lgCfg.Pattern, Is.EqualTo("%date(dd.MM.yyyy HH:mm:ss:fff) %level %text%nl"));
        
        Assert.That(lgCfg.Streams, Has.Length.EqualTo(3));
        
        ILogStream debugStream = lgCfg.Streams[0];
        Assert.That(debugStream, Is.TypeOf<DebugStream>());
        
        ILogStream consoleStream = lgCfg.Streams[1];
        Assert.That(consoleStream, Is.TypeOf<ConsoleStream>());
        
        ILogStream fileStream = lgCfg.Streams[2];
        Assert.That(fileStream, Is.TypeOf<FileStream>());
    }
        
    [Test]
    public void ReadWithCustomStream() {
        // Arrange
        string logCfgFn = Path.Combine(TestContext.CurrentContext.TestDirectory, "samples", "_utlog4.cfg");

        // Act
        LogConfiguration lgCfg = ConfigurationReader.ReadFile(logCfgFn, false);
            
        // Assert
        Assert.That(lgCfg, Is.Not.Null);
        Assert.That(lgCfg.Level, Is.EqualTo(ELogLevel.Debug));
        Assert.That(lgCfg.Pattern, Is.EqualTo("%date %level %text%nl"));
        
        Assert.That(lgCfg.Streams, Has.Length.EqualTo(2));    
        
        ILogStream customStream = lgCfg.Streams[0];
        Assert.That(customStream, Is.TypeOf<ConsoleStream>());
        
        ILogStream hiddenStream = lgCfg.Streams[1];
        Assert.That(hiddenStream, Is.TypeOf<HiddenStream>());
    }
        
    [Test]
    public void ReadWithTypes() {
        // Arrange
        string logCfgFn = Path.Combine(TestContext.CurrentContext.TestDirectory, "samples", "_utlog5.cfg");

        // Act
        LogConfiguration lgCfg = ConfigurationReader.ReadFile(logCfgFn, false);
            
        // Assert
        Assert.That(lgCfg, Is.Not.Null);
        Assert.That(lgCfg.Level, Is.EqualTo(ELogLevel.Debug));
        Assert.That(lgCfg.Pattern, Is.EqualTo("%date %level %text%nl"));
        
        Assert.That(lgCfg.Streams, Has.Length.EqualTo(1));    
        
        ILogStream stream = lgCfg.Streams[0];
            
        Assert.That(stream is AbstractStream);
        Assert.That(((AbstractStream)stream).Types, Is.EqualTo(["foo", "bar"]).AsCollection);
    }

    [Test]
    public void SetLogManagerConfiguration() {
        // Arrange
        string logCfgFn = Path.Combine(TestContext.CurrentContext.TestDirectory, "samples", "_utlog.cfg");
            
        // Act
        LogManager.Configuration =  ConfigurationReader.ReadFile(logCfgFn, false);
            
        // Assert
        Assert.That(LogManager.Configuration.Level, Is.EqualTo(ELogLevel.Debug));
        Assert.That(LogManager.Configuration.Pattern, Is.EqualTo("%date %level %text%nl"));
        Assert.That(LogManager.Configuration.Streams, Has.Length.EqualTo(2));
    }

    [Test]
    public void ApplyConfigurationFileTwice() {
        // Arrange
        string logCfgFn = Path.Combine(TestContext.CurrentContext.TestDirectory, "samples", "_utlog.cfg");
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

    sealed class HiddenStream : AbstractStream {

        /// <inheritdoc />
        protected override void OnDispose(bool disposing) {
            // Nothing to add here
        }

        /// <inheritdoc />
        protected override void OnWrite(in ReadOnlySpan<char> value, in LogData logData) {
            // Nothing to add here
        }
            
    }
}