namespace xcite.logging {
    /// <summary> Provides methods to write records to log streams. </summary>
    public class LogStreamManager {
        /// <summary>
        /// Writes the given <paramref name="value"/> into each stream of the given <paramref name="logStreams"/>
        /// that match the specified <paramref name="logName"/> and <paramref name="logLevel"/>.
        /// </summary>
        public void Write(ILogStream[] logStreams, string logName, ELogLevel logLevel, string value) {
            for (int i = -1; ++i != logStreams.Length;)
                logStreams[i].Write(value);
        }
    }
}