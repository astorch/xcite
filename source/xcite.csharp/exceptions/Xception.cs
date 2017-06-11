using System;

namespace xcite.csharp.exceptions {
    /// <summary>
    /// Provides a common exception base class that supports the Exception Error Reason pattern 
    /// to simplify exception handling.
    /// </summary>
    /// <typeparam name="TErrorReason">Type of associated error reason</typeparam>
    public abstract class Xception<TErrorReason> : Exception where TErrorReason : EErrorReason {
        /// <summary>
        /// Creates a new instance with the given <paramref name="errorReason"/>.
        /// </summary>
        /// <param name="errorReason">Associated error reason</param>
        protected Xception(TErrorReason errorReason) : this(errorReason, errorReason.Hint) {
            // Nothing to do here
        }

        /// <summary>
        /// Creates a new instance with the given <paramref name="errorReason"/> and optional error message. 
        /// Note, if an error message is given, the error hint provided by the error reason is ignored.
        /// </summary>
        /// <param name="errorReason">Associated error reason</param>
        /// <param name="message">Optional error message. Overrides the error hint</param>
        protected Xception(TErrorReason errorReason, string message) 
            : this(errorReason, message ?? errorReason.Hint, null) {
            // Nothing to do here
        }

        /// <summary>
        /// Creates a new instance with the given <paramref name="errorReason"/> and optional error message and 
        /// preceeding exception. Note, if an error message is given, the error hint provided by the error reason is ignored.
        /// </summary>
        /// <param name="errorReason">Associated error reason</param>
        /// <param name="message">Optional error message. Overrides the error hint</param>
        /// <param name="innerException">Preceeding exception</param>
        protected Xception(TErrorReason errorReason, string message, Exception innerException)
            : base(FormatExceptionMessage(errorReason, message ?? errorReason.Hint), innerException) {
            HResult = (int) (EErrorReason) errorReason;
            ErrorReason = errorReason;
        }

        /// <summary> Returns the associated error reason. </summary>
        public EErrorReason ErrorReason { get; }

        /// <summary>
        /// Creates an exception message based on the given <paramref name="errorReason"/> and message.
        /// </summary>
        /// <param name="errorReason">Error reason</param>
        /// <param name="message">Message</param>
        /// <returns>Exception message</returns>
        private static string FormatExceptionMessage(TErrorReason errorReason, string message = null) {
            string tlc = errorReason.TLC;
            string errorMessage = string.Format("{3}-{0} [{1}]: {2}", 
                (int) (EErrorReason) errorReason, errorReason, message, tlc);
            return errorMessage;
        }
    }
}