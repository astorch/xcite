using System;
using System.Collections.Generic;

namespace xcite.logging {
    /// <summary> Provides methods to create instance of <see cref="ILog"/> and to manages the logging behavior. </summary>
    public static class LogManager {
        private static readonly IDictionary<string, LogImpl> _logRepository = new Dictionary<string, LogImpl>(200);
        private static readonly LogConfiguration _config = new LogConfiguration();
        private static readonly LogOperator _logOperator = new LogOperator(_config, new TextFormatter(), new LogStreamManager());

        /// <summary> Log configuration </summary>
        public static LogConfiguration Configuration 
            => _config;
        
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
                _lgOp.Write(new LogData {name = _logName, level = ELogLevel.Fatal, value = value});
            }

            /// <inheritdoc />
            public void Fatal(string value, Exception exception) {
                _lgOp.Write(new LogData {name = _logName, level = ELogLevel.Fatal, value = value, exception = exception});
            }

            /// <inheritdoc />
            public void Error(string value) {
                _lgOp.Write(new LogData {name = _logName, level = ELogLevel.Error, value = value});
            }

            /// <inheritdoc />
            public void Error(string value, Exception exception) {
                _lgOp.Write(new LogData {name = _logName, level = ELogLevel.Error, value = value, exception = exception});
            }

            /// <inheritdoc />
            public void Warning(string value) {
                _lgOp.Write(new LogData {name = _logName, level = ELogLevel.Warn, value = value});
            }

            /// <inheritdoc />
            public void Warning(string value, Exception exception) {
                _lgOp.Write(new LogData {name = _logName, level = ELogLevel.Warn, value = value, exception = exception});
            }

            /// <inheritdoc />
            public void Info(string value) {
                _lgOp.Write(new LogData {name = _logName, level = ELogLevel.Info, value = value});
            }

            /// <inheritdoc />
            public void Info(string value, Exception exception) {
                _lgOp.Write(new LogData {name = _logName, level = ELogLevel.Info, value = value, exception = exception});
            }

            /// <inheritdoc />
            public void Trace(string value) {
                _lgOp.Write(new LogData {name = _logName, level = ELogLevel.Trace, value = value});
            }

            /// <inheritdoc />
            public void Trace(string value, Exception exception) {
                _lgOp.Write(new LogData {name = _logName, level = ELogLevel.Trace, value = value, exception = exception});
            }

            /// <inheritdoc />
            public void Debug(string value) {
                _lgOp.Write(new LogData {name = _logName, level = ELogLevel.Debug, value = value});
            }

            /// <inheritdoc />
            public void Debug(string value, Exception exception) {
                _lgOp.Write(new LogData {name = _logName, level = ELogLevel.Debug, value = value, exception = exception});
            }
        }
    }
}