using System;
using System.Collections.Concurrent;
using System.Diagnostics;

namespace xcite.logging;

/// <summary> Provides methods to create instance of <see cref="ILog"/> and to manages the logging behavior. </summary>
public static class LogManager {
    private static readonly ConcurrentDictionary<string, Log> _logRepository = new();
    private static readonly LogConfiguration _config = new();
    private static readonly LogOperator _logOperator = new(_config, new TextFormatter());

    /// <summary> Log configuration </summary>
    public static LogConfiguration Configuration {
        get => _config;
        set => SetConfiguration(value);
    }
        
    /// <summary> Creates an instance of <see cref="ILog"/> for the specified <paramref name="type"/>. </summary>
    /// <exception cref="ArgumentNullException">When <paramref name="type"/> is NULL.</exception>
    public static ILog GetLog(Type type) {
        ArgumentNullException.ThrowIfNull(type);
        return GetLog(type.FullName);
    }
        
    /// <summary> Creates an instance of <see cref="ILog"/> for the specified <paramref name="name"/>. </summary>
    /// <exception cref="ArgumentNullException">When <paramref name="name"/> is NULL or empty.</exception>
    public static ILog GetLog(string name) {
        ArgumentException.ThrowIfNullOrEmpty(name);
        return _logRepository.GetOrAdd(name, static nm => new Log(nm));
    }

    /// <summary> Applies the specified log <paramref name="config"/>. </summary>
    private static void SetConfiguration(LogConfiguration config) {
        if (config is null) return;

        _config.Reset()
            .SetLevel(config.Level)
            .SetPattern(config.Pattern);

        ILogStream[] logStreams = config.Streams;
        for (int i = -1, ilen = logStreams.Length; ++i != ilen;) {
            _config.AddStream(logStreams[i]);
        }
    }
        
    /// <summary> Anonymous implementation of <see cref="ILog"/>. </summary>
    [DebuggerDisplay("Log '{_logName}'")]
    sealed class Log(string logName) : ILog {
        private static readonly Exception _nullException = null;
        private readonly string _logName = logName;

        /// <inheritdoc />
        public void Fatal(string value) {
            LogData logData = CreateLogData(in ELogLevel.Fatal, in value, in _nullException);
            _logOperator.Write(in logData);
        }

        /// <inheritdoc />
        public void Fatal(string value, Exception exception) {
            LogData logData = CreateLogData(in ELogLevel.Fatal, in value, in exception);
            _logOperator.Write(in logData);
        }

        /// <inheritdoc />
        public void Error(string value) {
            LogData logData = CreateLogData(in ELogLevel.Error, in value, in _nullException);
            _logOperator.Write(in logData);
        }

        /// <inheritdoc />
        public void Error(string value, Exception exception) {
            LogData logData = CreateLogData(in ELogLevel.Error, in value, in exception);
            _logOperator.Write(in logData);
        }

        /// <inheritdoc />
        public void Warning(string value) {
            LogData logData = CreateLogData(in ELogLevel.Warn, in value, in _nullException);
            _logOperator.Write(in logData);
        }

        /// <inheritdoc />
        public void Warning(string value, Exception exception) {
            LogData logData = CreateLogData(in ELogLevel.Warn, in value, in exception);
            _logOperator.Write(in logData);
        }

        /// <inheritdoc />
        public void Info(string value) {
            LogData logData = CreateLogData(in ELogLevel.Info, in value, in _nullException);
            _logOperator.Write(in logData);
        }

        /// <inheritdoc />
        public void Info(string value, Exception exception) {
            LogData logData = CreateLogData(in ELogLevel.Info, in value, in exception);
            _logOperator.Write(in logData);
        }

        /// <inheritdoc />
        public void Trace(string value) {
            LogData logData = CreateLogData(in ELogLevel.Trace, in value, in _nullException);
            _logOperator.Write(in logData);
        }

        /// <inheritdoc />
        public void Trace(string value, Exception exception) {
            LogData logData = CreateLogData(in ELogLevel.Trace, in value, in exception);
            _logOperator.Write(in logData);
        }

        /// <inheritdoc />
        public void Debug(string value) {
            LogData logData = CreateLogData(in ELogLevel.Debug, in value, in _nullException);
            _logOperator.Write(in logData);
        }

        /// <inheritdoc />
        public void Debug(string value, Exception exception) {
            LogData logData = CreateLogData(in ELogLevel.Debug, in value, in exception);
            _logOperator.Write(in logData);
        }

        /// <summary> Creates a new instance of <see cref="LogData"/> with the specified arguments. </summary>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        private LogData CreateLogData(in ELogLevel logLevel, in string value, in Exception exception)
            => new(in _logName, in logLevel, in value, in exception);

    }
}