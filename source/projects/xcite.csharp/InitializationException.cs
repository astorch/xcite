using System;

namespace xcite.csharp {
    /// <summary>
    /// Defines an exception that is thrown when an error during the initialization of 
    /// an object occurred.
    /// </summary>
    public class InitializationException : Exception {
        /// <inheritdoc />
        public InitializationException(string message, Exception innerException) : base(message, innerException) {
            // Nothing to do here
        }

        /// <inheritdoc />
        public InitializationException(string message) : base(message) {
            // Nothing to do here
        }
    }
}
