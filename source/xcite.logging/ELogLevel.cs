namespace xcite.logging {
    /// <summary> Log level enumeration </summary>
    public class ELogLevel {
        public static readonly ELogLevel None = new ELogLevel("NONE");
        public static readonly ELogLevel Fatal = new ELogLevel("FATAL");
        public static readonly ELogLevel Error = new ELogLevel("ERROR");
        public static readonly ELogLevel Warn  = new ELogLevel("WARN");
        public static readonly ELogLevel Info  = new ELogLevel("INFO");
        public static readonly ELogLevel Trace = new ELogLevel("TRACE");
        public static readonly ELogLevel Debug = new ELogLevel("DEBUG");

        private readonly string _name;

        /// <inheritdoc />
        private ELogLevel(string name) {
            _name = name;
        }

        /// <inheritdoc />
        public override string ToString() {
            return _name;
        }
    }
}