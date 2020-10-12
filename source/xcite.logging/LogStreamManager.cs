namespace xcite.logging {
    /// <summary> Provides methods to write records to log streams. </summary>
    public class LogStreamManager {
        private readonly object _accessToken = new object();
        
        /// <summary>
        /// Writes the given <paramref name="value"/> into each stream of the given <paramref name="logStreams"/>
        /// that match the specified <paramref name="logData"/>.
        /// </summary>
        public void Write(ILogStream[] logStreams, LogData logData, string value) {
            lock (_accessToken) {
                for (int i = -1, ilen = logStreams.Length; ++i != ilen;)
                    logStreams[i].Write(value, logData);
            }
        }
    }
}