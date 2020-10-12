using System;

namespace xcite.logging {
    /// <summary> Describes a log stream. </summary>
    public interface ILogStream : IDisposable {
        
        /// <summary>
        /// Writes the specified <paramref name="value"/> into the stream.
        /// The argument <paramref name="logData"/> provides additional information
        /// about the value to write.
        /// </summary>
        void Write(string value, LogData logData);
        
    }
}