using System;

namespace xcite.logging {
    /// <summary> Describes the data to log. </summary>
    public class LogData {
        public string name;
        public ELogLevel level;
        public string value;
        public Exception exception;
    }
}