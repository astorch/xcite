using System;
using System.Collections.Generic;

namespace xcite.logging {
    /// <summary> Provides methods to create instance of <see cref="ILog"/> and to manages the logging behavior. </summary>
    public static class LogManager {
        private static readonly IDictionary<string, LogImpl> _logRepository = new Dictionary<string, LogImpl>(200);
        private static readonly LogConfiguration _config = new LogConfiguration();
        private static readonly LogOperator _logOperator = new LogOperator(_config, new TextFormatter(), new LogStreamManager());

        /// <summary> Log configuration </summary>
        public static LogConfiguration Configuration {
            get => _config;
            set => SetConfiguration(value);
        }
        
        /// <summary> Creates an instance of <see cref="ILog"/> for the specified <paramref name="type"/>. </summary>
        /// <exception cref="ArgumentNullException">When <paramref name="type"/> is NULL.</exception>
        public static ILog GetLog(Type type) {
            if (type == null) throw new ArgumentNullException(nameof(type));
            return GetLog(type.FullName);
        }
        
        /// <summary> Creates an instance of <see cref="ILog"/> for the specified <paramref name="name"/>. </summary>
        /// <exception cref="ArgumentNullException">When <paramref name="name"/> is NULL or empty.</exception>
        public static ILog GetLog(string name) {
            if (string.IsNullOrEmpty(name)) throw new ArgumentNullException(nameof(name));

            lock (_logRepository) {
                if (_logRepository.TryGetValue(name, out LogImpl log)) return log;
                _logRepository.Add(name, log = new LogImpl(name, _logOperator));
                return log;
            }
        }

        /// <summary> Applies the specified log <paramref name="config"/>. </summary>
        private static void SetConfiguration(LogConfiguration config) {
            if (config == null) return;

            _config.Reset()
                .SetLevel(config.Level)
                .SetPattern(config.Pattern);

            ILogStream[] logStreams = config.Streams;
            for (int i = -1; ++i != logStreams.Length;) {
                _config.AddStream(logStreams[i]);
            }
        }
        
        /// <summary> Anonymous implementation of <see cref="ILog"/>. </summary>
        class LogImpl : ILog {
            private readonly string _logName;
            private readonly LogOperator _lgOp;

            public LogImpl(string logName, LogOperator logOperator) {
                _logName = logName;
                _lgOp = logOperator;
            }

            /// <inheritdoc />
            public void Fatal(string value) {
                _lgOp.Write(CreateLogData(ELogLevel.Fatal, value, null));
            }

            /// <inheritdoc />
            public void Fatal(string value, Exception exception) {
                _lgOp.Write(CreateLogData(ELogLevel.Fatal, value, exception));
            }

            /// <inheritdoc />
            public void Error(string value) {
                _lgOp.Write(CreateLogData(ELogLevel.Error, value, null));
            }

            /// <inheritdoc />
            public void Error(string value, Exception exception) {
                _lgOp.Write(CreateLogData(ELogLevel.Error, value, exception));
            }

            /// <inheritdoc />
            public void Warning(string value) {
                _lgOp.Write(CreateLogData(ELogLevel.Warn, value, null));
            }

            /// <inheritdoc />
            public void Warning(string value, Exception exception) {
                _lgOp.Write(CreateLogData(ELogLevel.Warn, value, exception));
            }

            /// <inheritdoc />
            public void Info(string value) {
                _lgOp.Write(CreateLogData(ELogLevel.Info, value, null));
            }

            /// <inheritdoc />
            public void Info(string value, Exception exception) {
                _lgOp.Write(CreateLogData(ELogLevel.Info, value, exception));
            }

            /// <inheritdoc />
            public void Trace(string value) {
                _lgOp.Write(CreateLogData(ELogLevel.Trace, value, null));
            }

            /// <inheritdoc />
            public void Trace(string value, Exception exception) {
                _lgOp.Write(CreateLogData(ELogLevel.Trace, value, exception));
            }

            /// <inheritdoc />
            public void Debug(string value) {
                _lgOp.Write(CreateLogData(ELogLevel.Debug, value, null));
            }

            /// <inheritdoc />
            public void Debug(string value, Exception exception) {
                _lgOp.Write(CreateLogData(ELogLevel.Debug, value, exception));
            }

            /// <summary> Creates a new instance of <see cref="LogData"/> with the specified arguments. </summary>
            [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
            private LogData CreateLogData(ELogLevel logLevel, string value, Exception exception)
                => new LogData {name = _logName, level = logLevel, value = value, exception = exception};

        }
    }
}