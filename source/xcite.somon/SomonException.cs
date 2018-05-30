using System;

namespace xcite.somon {
    /// <summary> Announces an error that occurred during the SOMON parsing. </summary>
    public class SomonException : Exception {
        /// <inheritdoc />
        public SomonException(string message) : base(message) {
            // Accomplish base initializer
        }

        /// <inheritdoc />
        public SomonException(string message, Exception innerException) : base(message, innerException) {
            // Accomplish base initializer
        }
    }
}