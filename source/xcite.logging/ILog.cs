using System;

namespace xcite.logging {
    /// <summary> Provides methods to write log entries. </summary>
    public interface ILog {
        /// <summary> Writes a record with the specified <paramref name="value"/> as `fatal` entry. </summary>
        void Fatal(string value);
        
        /// <summary>
        /// Writes a record with the specified <paramref name="value"/> as `fatal` entry. Additionally,
        /// the given <paramref name="exception"/> is appended, if it's not NULL.
        /// </summary>
        void Fatal(string value, Exception exception);

        /// <summary> Writes a record with the specified <paramref name="value"/> as `error` entry. </summary>
        void Error(string value);
        
        /// <summary>
        /// Writes a record with the specified <paramref name="value"/> as `error` entry. Additionally,
        /// the given <paramref name="exception"/> is appended, if it's not NULL.
        /// </summary>
        void Error(string value, Exception exception);

        /// <summary> Writes a record with the specified <paramref name="value"/> as `warning` entry. </summary>
        void Warning(string value);
        
        /// <summary>
        /// Writes a record with the specified <paramref name="value"/> as `warning` entry. Additionally,
        /// the given <paramref name="exception"/> is appended, if it's not NULL.
        /// </summary>
        void Warning(string value, Exception exception);

        /// <summary> Writes a record with the specified <paramref name="value"/> as `info` entry. </summary>
        void Info(string value);
        
        /// <summary>
        /// Writes a record with the specified <paramref name="value"/> as `info` entry. Additionally,
        /// the given <paramref name="exception"/> is appended, if it's not NULL.
        /// </summary>
        void Info(string value, Exception exception);

        /// <summary> Writes a record with the specified <paramref name="value"/> as `trace` entry. </summary>
        void Trace(string value);

        /// <summary>
        /// Writes a record with the specified <paramref name="value"/> as `trace` entry. Additionally,
        /// the given <paramref name="exception"/> is appended, if it's not NULL.
        /// </summary>
        void Trace(string value, Exception exception);

        /// <summary> Writes a record with the specified <paramref name="value"/> as `debug` entry. </summary>
        void Debug(string value);

        /// <summary>
        /// Writes a record with the specified <paramref name="value"/> as `debug` entry. Additionally,
        /// the given <paramref name="exception"/> is appended, if it's not NULL.
        /// </summary>
        void Debug(string value, Exception exception);
    }
}