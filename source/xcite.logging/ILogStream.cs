using System;

namespace xcite.logging {
    /// <summary> Describes a log stream. </summary>
    public interface ILogStream : IDisposable {
        /// <summary>
        /// Writes the specified <paramref name="value"/> into the stream.
        /// Depending on the implementation, the <paramref name="logName"/> may be used to decide whether to write
        /// into the stream or not. 
        /// </summary>
        void Write(string value, string logName);
    }
}