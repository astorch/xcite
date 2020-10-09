using System.Collections.Generic;
using System.Linq;

namespace xcite.logging.streams {
    /// <summary>
    /// The <see cref="AbstractStream"/> implements functionality used by all other streams. 
    /// </summary>
    public abstract class AbstractStream : ILogStream {
        private int _typesCount;
        private readonly HashSet<string> _servedTypes = new HashSet<string>();
        
        /// <inheritdoc/>
        public abstract void Dispose();

        /// <summary> Writes the specified <paramref name="value"/> into the stream. </summary>
        public abstract void Write(string value);
        
        /// <summary>
        /// Writes the <paramref name="value"/> if the passed <paramref name="logName"/> is a member of
        /// <see cref="Types"/>, or if <see cref="Types"/> is empty.  
        /// </summary>
        public void Write(string value, string logName) {
            if (_typesCount == 0) Write(value);
            if (_servedTypes.Contains(logName)) Write(value);
        }

        /// <summary>
        /// The types this streams accepts as source of a logging call.
        /// Calls from all other types will be ignored.
        /// </summary>
        public string[] Types {
            get => _servedTypes.ToArray();
            set {
                _servedTypes.Clear();
                _typesCount = value.Length;
                foreach (string type in value) {
                    _servedTypes.Add(type);
                }
            }
        }
    }
}