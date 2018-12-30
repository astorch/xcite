namespace xcite.logging {
    /// <summary> Log level enumeration </summary>
    public class ELogLevel {
        public static readonly ELogLevel None  = new ELogLevel("NONE", -1);
        public static readonly ELogLevel Fatal = new ELogLevel("FATAL", 0);
        public static readonly ELogLevel Error = new ELogLevel("ERROR", 0);
        public static readonly ELogLevel Warn  = new ELogLevel("WARN", 0);
        public static readonly ELogLevel Info  = new ELogLevel("INFO", 4);
        public static readonly ELogLevel Trace = new ELogLevel("TRACE", 5);
        public static readonly ELogLevel Debug = new ELogLevel("DEBUG", 6);

        private readonly string _name;
        private readonly int _severity;

        /// <inheritdoc />
        private ELogLevel(string name, int severity) {
            _name = name;
            _severity = severity;
        }

        /// <inheritdoc />
        public override string ToString() {
            return _name;
        }

        /// <summary>
        /// Returns TRUE if the given <paramref name="level"/> is greater or equal
        /// as the current.
        /// </summary>
        public bool IsGreaterOrEqual(ELogLevel level) {
            return level._severity >= _severity;
        }
    }
}