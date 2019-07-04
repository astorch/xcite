using System;

namespace xcite.logging {
    /// <summary> Log level enumeration </summary>
    public class ELogLevel {
        /// <summary> Flag to set that no message is logged. </summary>
        public static readonly ELogLevel None  = new ELogLevel("NONE", -1);
        
        /// <summary> Flag to set that only messages with fatal severity are logged. </summary>
        public static readonly ELogLevel Fatal = new ELogLevel("FATAL", 0);
        
        /// <summary> Flag to set that only messages with error severity are logged. </summary>
        public static readonly ELogLevel Error = new ELogLevel("ERROR", 0);
        
        /// <summary> Flag to set that only messages with warning severity are logged. </summary>
        public static readonly ELogLevel Warn  = new ELogLevel("WARN", 0);
        
        /// <summary> Flag to set that only messages with info severity are logged. </summary>
        public static readonly ELogLevel Info  = new ELogLevel("INFO", 4);
        
        /// <summary> Flag to set that only messages with trace severity are logged. </summary>
        public static readonly ELogLevel Trace = new ELogLevel("TRACE", 5);
        
        /// <summary> Flag to set that only messages with debug severity are logged. </summary>
        public static readonly ELogLevel Debug = new ELogLevel("DEBUG", 6);

        /// <summary> Set of defined enum values. </summary>
        public static readonly ELogLevel[] Values  = {None, Fatal, Error, Warn, Info, Trace, Debug};
        
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

        /// <summary>
        /// Parses the given <paramref name="value"/> and returns the corresponding
        /// <see cref="ELogLevel"/>. If the value cannot be parsed, an <see cref="FormatException"/>
        /// is thrown.
        /// </summary>
        /// <exception cref="FormatException">If the <paramref name="value"/> cannot be parsed.</exception>
        public static ELogLevel Parse(string value) {
            if (TryParse(value, out ELogLevel level)) return level;
            throw new FormatException("Unknown value");
        }

        /// <summary>
        /// Returns TRUE if the given <paramref name="value"/> could be parsed and
        /// the resulting <paramref name="level"/> is being provided. 
        /// </summary>
        public static bool TryParse(string value, out ELogLevel level) {
            level = None;
            string lcval = value.ToLower();
            for (int i = -1; ++i != Values.Length;) {
                ELogLevel lgLvl = Values[i];
                string lgLvlName = lgLvl._name.ToLower();
                if (lgLvlName != lcval) continue;
                
                level = lgLvl;
                return true;
            }

            return false;
        }
    }
}