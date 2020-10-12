using System;

namespace xcite.logging {
    /// <summary> Describes the data to log. </summary>
    public struct LogData {
        
        /// <summary> Logger name </summary>
        public string name;
        
        /// <summary> Log level </summary>
        public ELogLevel level;
        
        /// <summary> Log value </summary>
        public string value;
        
        /// <summary> Log excecption </summary>
        public Exception exception;
        
    }
}